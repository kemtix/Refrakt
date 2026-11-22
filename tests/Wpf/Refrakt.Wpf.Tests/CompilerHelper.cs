

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Refrakt.Modularity;
using Xunit;

namespace Refrakt.Wpf.Tests
{
    public class CompilerHelper
    {
        private static string moduleTemplate =
            @"using System;
            using Refrakt.Ioc;
            using Refrakt.Modularity;

            namespace TestModules
            {
                #module#
	            public class #className#Class : IModule
	            {
                    public void OnInitialized(IContainerProvider containerProvider)
                    {
                        Console.WriteLine(""#className#.Start"");
                    }

                    public void RegisterTypes(IContainerRegistry containerRegistry)
                    {

                    }
                }
            }";

        public static Assembly CompileFileAndLoadAssembly(string input, string output, params string[] references)
        {
            CompileFile(input, output, references);
            return Assembly.LoadFrom(Path.GetFullPath(output));
        }

        public static EmitResult CompileFile(string input, string output, params string[] references)
        {
            CreateOutput(output);

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(input))
            {
                if (stream == null)
                {
                    throw new ArgumentException("input");
                }

                StreamReader reader = new StreamReader(stream);
                string source = reader.ReadToEnd();
                return Compile(source, output, references);
            }
        }

        public static void CreateOutput(string output)
        {
            string dir = Path.GetDirectoryName(output);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            else
            {
                //Delete the file if exists
                if (File.Exists(output))
                {
                    try
                    {
                        File.Delete(output);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //The file might be locked by Visual Studio, so rename it
                        if (File.Exists(output + ".locked"))
                            File.Delete(output + ".locked");
                        File.Move(output, output + ".locked");
                    }
                }
            }
        }

        public static EmitResult CompileCode(string code, string output)
        {
            CreateOutput(output);

            return Compile(code, output);
        }

        public static string GenerateDynamicModule(string assemblyName, string moduleName, string outpath, params string[] dependencies)
        {
            CreateOutput(outpath);

            // Create temporary module.
            string moduleCode = moduleTemplate.Replace("#className#", assemblyName);
            if (!string.IsNullOrEmpty(moduleName))
            {
                moduleCode = moduleCode.Replace("#module#", String.Format("[Module(ModuleName = \"{0}\") #dependencies#]", moduleName));
            }
            else
            {
                moduleCode = moduleCode.Replace("#module#", "");
            }

            string depString = string.Empty;

            foreach (string module in dependencies)
            {
                depString += String.Format(", ModuleDependency(\"{0}\")", module);
            }

            moduleCode = moduleCode.Replace("#dependencies#", depString);

            CompileCode(moduleCode, outpath);

            return outpath;
        }

        public static string GenerateDynamicModule(string assemblyName, string moduleName, params string[] dependencies)
        {
            string assemblyFile = assemblyName + ".dll";
            string outpath = Path.Combine(assemblyName, assemblyFile);

            return GenerateDynamicModule(assemblyName, moduleName, outpath, dependencies);
        }

        public static void ThrowIfCompilerError(EmitResult result)
        {
            if (!result.Success)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Compilation failed.");
                foreach (Diagnostic error in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                {
                    sb.AppendLine(error.ToString());
                }
                Assert.Fail(sb.ToString());
            }
        }

        public static void CleanUpDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //The file might be locked by Visual Studio, so rename it
                        if (File.Exists(file + ".locked"))
                            File.Delete(file + ".locked");
                        File.Move(file, file + ".locked");
                    }
                }
            }
        }

        private static EmitResult Compile(string source, string output, params string[] references)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                Path.GetFileNameWithoutExtension(output),
                new[] { CSharpSyntaxTree.ParseText(source) },
                GetMetadataReferences(references),
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (MemoryStream peStream = new MemoryStream())
            {
                EmitResult result = compilation.Emit(peStream);
                ThrowIfCompilerError(result);
                File.WriteAllBytes(output, peStream.ToArray());
                return result;
            }
        }

        private static IEnumerable<MetadataReference> GetMetadataReferences(string[] references)
        {
            string[] trustedAssemblies = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") ?? string.Empty)
                .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);

            List<string> paths = new List<string>(trustedAssemblies);
            paths.Add(typeof(IModule).Assembly.Location);
            paths.Add(typeof(ModuleAttribute).Assembly.Location);

            foreach (string reference in references)
            {
                if (Path.GetFileName(reference) == reference)
                {
                    // A bare file name, such as System.Transactions.dll, is taken from the trusted platform assemblies.
                    string trustedAssembly = trustedAssemblies.FirstOrDefault(
                        path => string.Equals(Path.GetFileName(path), reference, StringComparison.OrdinalIgnoreCase));
                    if (trustedAssembly == null)
                    {
                        throw new ArgumentException($"'{reference}' is not a trusted platform assembly.", nameof(references));
                    }

                    paths.Add(trustedAssembly);
                }
                else
                {
                    paths.Add(reference);
                }
            }

            // This test assembly also compiles Mocks\Modules\*.cs: referencing it gives CS0433 when a
            // module source uses a type of another compiled module (for example DummyClass).
            string testAssembly = Path.GetFullPath(typeof(CompilerHelper).Assembly.Location);

            // Refrakt.dll and Refrakt.Wpf.dll are already trusted platform assemblies: a duplicate gives CS1703.
            return paths
                .Select(Path.GetFullPath)
                .Where(path => !string.Equals(path, testAssembly, StringComparison.OrdinalIgnoreCase))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(path => MetadataReference.CreateFromFile(path))
                .ToList();
        }
    }
}
