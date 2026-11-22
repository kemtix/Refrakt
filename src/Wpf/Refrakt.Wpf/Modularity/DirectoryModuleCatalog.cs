using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Refrakt.Properties;

namespace Refrakt.Modularity
{
    /// <summary>
    /// Represents a catalog created from a directory on disk.
    /// </summary>
    /// <remarks>
    /// The directory catalog will scan the contents of a directory, locating classes that implement
    /// <see cref="IModule"/> and add them to the catalog based on contents in their associated <see cref="ModuleAttribute"/>.
    /// The assemblies are inspected with a <see cref="MetadataLoadContext"/> without being loaded into the application:
    /// only their metadata is read, and the context is disposed once the modules have been discovered.
    /// Applications published as a single file are not supported.
    ///
    /// The directory catalog does not continue to monitor the directory after it has created the initialize catalog.
    /// </remarks>
    public class DirectoryModuleCatalog : ModuleCatalog
    {
        /// <summary>
        /// Directory containing modules to search for.
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// Inspects the assemblies of <see cref="ModulePath"/> with a <see cref="MetadataLoadContext"/>, without loading them,
        /// and adds a <see cref="ModuleInfo"/> for each module found. Applications published as a single file are not supported,
        /// and the directory is not monitored after the scan.
        /// </summary>
        protected override void InnerLoad()
        {
            if (string.IsNullOrEmpty(this.ModulePath))
                throw new InvalidOperationException(Resources.ModulePathCannotBeNullOrEmpty);

            if (!Directory.Exists(this.ModulePath))
                throw new InvalidOperationException(
                    string.Format(CultureInfo.CurrentCulture, Resources.DirectoryNotFound, this.ModulePath));

            try
            {
                DirectoryInfo directory = new DirectoryInfo(this.ModulePath);
                Assembly[] alreadyLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(p => !p.IsDynamic).ToArray();
                ModuleInfo[] moduleInfos;

                using (MetadataLoadContext context = new MetadataLoadContext(CreateAssemblyResolver(directory, alreadyLoadedAssemblies)))
                {
                    // Types of the context are invalid once it is disposed: every ModuleInfo is built before.
                    moduleInfos = GetNotAlreadyLoadedModuleInfos(context, directory, alreadyLoadedAssemblies).ToArray();
                }

                this.Items.AddRange(moduleInfos);
            }
            catch (Exception ex)
            {
                throw new Exception("There was an error loading assemblies.", ex);
            }
        }

        private static PathAssemblyResolver CreateAssemblyResolver(DirectoryInfo directory, Assembly[] alreadyLoadedAssemblies)
        {
            List<string> paths = new List<string>();

            paths.AddRange(directory.GetFiles("*.dll").Select(file => file.FullName));
            paths.AddRange(Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll"));
            paths.AddRange(Directory.GetFiles(Path.GetDirectoryName(typeof(System.Windows.Application).Assembly.Location), "*.dll"));
            paths.AddRange(alreadyLoadedAssemblies
                .Where(assembly => !string.IsNullOrEmpty(assembly.Location))
                .Select(assembly => assembly.Location));

            string trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (!string.IsNullOrEmpty(trustedPlatformAssemblies))
            {
                paths.AddRange(trustedPlatformAssemblies.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries));
            }

            return new PathAssemblyResolver(paths
                .Select(Path.GetFullPath)
                .Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static IEnumerable<ModuleInfo> GetNotAlreadyLoadedModuleInfos(MetadataLoadContext context, DirectoryInfo directory, Assembly[] alreadyLoadedAssemblies)
        {
            // typeof(IModule) never matches a type of the context: IModule is resolved inside the context.
            Assembly moduleAssembly = context.LoadFromAssemblyName(typeof(IModule).Assembly.GetName());
            Type IModuleType = moduleAssembly.GetType(typeof(IModule).FullName, throwOnError: true);

            List<KeyValuePair<Assembly, FileInfo>> validAssemblies = new List<KeyValuePair<Assembly, FileInfo>>();

            var fileInfos = directory.GetFiles("*.dll")
                .Where(file => alreadyLoadedAssemblies.FirstOrDefault(
                    assembly => String.Compare(Path.GetFileName(assembly.Location),
                    file.Name, StringComparison.OrdinalIgnoreCase) == 0) == null).ToList();

            foreach (FileInfo fileInfo in fileInfos)
            {
                try
                {
                    validAssemblies.Add(new KeyValuePair<Assembly, FileInfo>(context.LoadFromAssemblyPath(fileInfo.FullName), fileInfo));
                }
                catch (BadImageFormatException)
                {
                    // skip non-.NET Dlls
                }
            }

            return validAssemblies.SelectMany(assembly => assembly.Key
                        .GetExportedTypes()
                        .Where(IModuleType.IsAssignableFrom)
                        .Where(t => t != IModuleType)
                        .Where(t => !t.IsAbstract)
                        .Select(type => CreateModuleInfo(type, assembly.Value)));
        }

        private static ModuleInfo CreateModuleInfo(Type type, FileInfo assemblyFile)
        {
            string moduleName = type.Name;
            List<string> dependsOn = new List<string>();
            bool onDemand = false;
            var moduleAttribute =
                CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                    cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);

            if (moduleAttribute != null)
            {
                foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                {
                    string argumentName = argument.MemberInfo.Name;
                    switch (argumentName)
                    {
                        case "ModuleName":
                            moduleName = (string)argument.TypedValue.Value;
                            break;

                        case "OnDemand":
                            onDemand = (bool)argument.TypedValue.Value;
                            break;

                        case "StartupLoaded":
                            onDemand = !((bool)argument.TypedValue.Value);
                            break;
                    }
                }
            }

            var moduleDependencyAttributes =
                CustomAttributeData.GetCustomAttributes(type).Where(
                    cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);

            foreach (CustomAttributeData cad in moduleDependencyAttributes)
            {
                dependsOn.Add((string)cad.ConstructorArguments[0].Value);
            }

            ModuleInfo moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
            {
                InitializationMode = onDemand ? InitializationMode.OnDemand : InitializationMode.WhenAvailable,
                Ref = new Uri(assemblyFile.FullName).AbsoluteUri,
            };
            moduleInfo.DependsOn.AddRange(dependsOn);
            return moduleInfo;
        }
    }
}
