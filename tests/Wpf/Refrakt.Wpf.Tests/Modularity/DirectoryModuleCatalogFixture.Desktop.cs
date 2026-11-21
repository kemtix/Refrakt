#if DEBUG

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Refrakt.Modularity;
using Xunit;

namespace Refrakt.Wpf.Tests.Modularity
{
    public class DirectoryModuleCatalogFixture : IDisposable
    {
        private const string ModulesDirectory1 = @".\DynamicModules\MocksModules1";
        private const string ModulesDirectory2 = @".\DynamicModules\AttributedModules";
        private const string ModulesDirectory3 = @".\DynamicModules\DependantModules";
        private const string ModulesDirectory4 = @".\DynamicModules\MocksModules2";
        private const string ModulesDirectory5 = @".\DynamicModules\ModulesMainDomain\";
        private const string ModulesDirectory6 = @".\DynamicModules\Special char #";
        private const string InvalidModulesDirectory = @".\Modularity";

        public DirectoryModuleCatalogFixture()
        {
            CleanUpDirectories();
        }

        private void CleanUpDirectories()
        {
            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
            CompilerHelper.CleanUpDirectory(ModulesDirectory2);
            CompilerHelper.CleanUpDirectory(ModulesDirectory3);
            CompilerHelper.CleanUpDirectory(ModulesDirectory4);
            CompilerHelper.CleanUpDirectory(ModulesDirectory5);
            CompilerHelper.CleanUpDirectory(InvalidModulesDirectory);
        }

        [Fact]
        public void NullPathThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                DirectoryModuleCatalog catalog = new DirectoryModuleCatalog();
                catalog.Load();
            });
        }

        [Fact]
        public void EmptyPathThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
                {
                    ModulePath = string.Empty
                };
                catalog.Load();
            });

        }

        [Fact]
        public void NonExistentPathThrows()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
            {
                DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
                {
                    ModulePath = "NonExistentPath"
                };
                catalog.Load();
            });
        }

        [Fact]
        public void ShouldReturnAListOfModuleInfo()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");


            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.NotNull(modules);
            Assert.Single(modules);
            Assert.NotNull(modules[0].Ref);
            Assert.StartsWith("file://", modules[0].Ref);
            Assert.Contains(@"MockModuleA.dll", modules[0].Ref);
            Assert.NotNull(modules[0].ModuleType);
            Assert.Contains("Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA", modules[0].ModuleType);
        }

        [Fact]
        public void ShouldCorrectlyEscapeRef()
        {
            string assemblyPath = ModulesDirectory6 + @"\Mock Module #.dll";
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs", assemblyPath);
            string fullAssemblyPath = Path.GetFullPath(assemblyPath);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory6
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.NotNull(modules);
            Assert.Single(modules);
            Assert.NotNull(modules[0].Ref);

            string moduleRef = modules[0].Ref;
            // = new Uri(moduleRef);
            Assert.True(Uri.TryCreate(moduleRef, UriKind.Absolute, out Uri moduleUri));

            Assert.Equal(fullAssemblyPath, moduleUri.LocalPath);
        }

        //TODO: figure out how ot translat ehtese tests to Xunit
        //[Fact]
        //[DeploymentItem(@"Modularity\NotAValidDotNetDll.txt.dll", @".\Modularity")]
        //public void ShouldNotThrowWithNonValidDotNetAssembly()
        //{
        //    DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
        //    {
        //        ModulePath = InvalidModulesDirectory
        //    };
        //    try
        //    {
        //        catalog.Load();
        //    }
        //    catch (Exception)
        //    {
        //        //Assert.Fail("Should not have thrown.");
        //    }

        //    var modules = catalog.Modules.ToArray();
        //    Assert.NotNull(modules);
        //    Assert.Equal(0, modules.Length);
        //}

        //[Fact]
        //[DeploymentItem(@"Modularity\NotAValidDotNetDll.txt.dll", InvalidModulesDirectory)]
        //public void LoadsValidAssembliesWhenInvalidDllsArePresent()
        //{
        //    CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
        //                               InvalidModulesDirectory + @"\MockModuleA.dll");

        //    DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
        //    {
        //        ModulePath = InvalidModulesDirectory
        //    };
        //    try
        //    {
        //        catalog.Load();
        //    }
        //    catch (Exception)
        //    {
        //        //Assert.Fail("Should not have thrown.");
        //    }

        //    var modules = catalog.Modules.ToArray();

        //    Assert.NotNull(modules);
        //    Assert.Equal(1, modules.Length);
        //    Assert.NotNull(modules[0].Ref);
        //    Assert.StartsWith(modules[0].Ref, "file://");
        //    Assert.True(modules[0].Ref.Contains(@"MockModuleA.dll"));
        //    Assert.NotNull(modules[0].ModuleType);
        //    Assert.Contains(modules[0].ModuleType, "Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA");
        //}

        [Fact]
        public void ShouldGetModuleNameFromAttribute()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       ModulesDirectory2 + @"\MockAttributedModule.dll");


            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory2
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Single(modules);
            Assert.Equal("TestModule", modules[0].ModuleName);
        }

        [Fact]
        public void ShouldGetDependantModulesFromAttribute()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockDependencyModule.cs",
                                       ModulesDirectory3 + @"\DependencyModule.dll");

            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockDependantModule.cs",
                                       ModulesDirectory3 + @"\DependantModule.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory3
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Equal(2, modules.Length);
            var dependantModule = modules.First(module => module.ModuleName == "DependantModule");
            var dependencyModule = modules.First(module => module.ModuleName == "DependencyModule");
            Assert.NotNull(dependantModule);
            Assert.NotNull(dependencyModule);
            Assert.NotNull(dependantModule.DependsOn);
            Assert.Single(dependantModule.DependsOn);
            Assert.Equal(dependencyModule.ModuleName, dependantModule.DependsOn[0]);
        }

        [Fact]
        public void UseClassNameAsModuleNameWhenNotSpecifiedInAttribute()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.NotNull(modules);
            Assert.Equal("MockModuleA", modules[0].ModuleName);
        }

        [Fact]
        public void ShouldDefaultInitializationModeToWhenAvailable()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.NotNull(modules);
            Assert.Equal(InitializationMode.WhenAvailable, modules[0].InitializationMode);
        }

        [Fact]
        public void ShouldGetOnDemandFromAttribute()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockAttributedModule.cs",
                                       ModulesDirectory3 + @"\MockAttributedModule.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory3
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Single(modules);
            Assert.Equal(InitializationMode.OnDemand, modules[0].InitializationMode);

        }

        [Fact]
        public void ShouldNotLoadAssembliesInCurrentAppDomain()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory4 + @"\MockModuleA.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            // filtering out dynamic assemblies due to using a dynamic mocking framework.
            Assembly loadedAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.IsDynamic)
                .Where(assembly => assembly.Location.Equals(modules[0].Ref, StringComparison.InvariantCultureIgnoreCase))
                .FirstOrDefault();
            Assert.Null(loadedAssembly);
        }

        [Fact]
        public void ShouldNotGetModuleInfoForAnAssemblyAlreadyLoadedInTheMainDomain()
        {
            var assemblyPath = Assembly.GetCallingAssembly().Location;
            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory5
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Empty(modules);
        }

        [Fact]
        public void ShouldLoadAssemblyEvenIfTheyAreReferencingEachOther()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory4 + @"\MockModuleZZZ.dll");

            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleReferencingOtherModule.cs",
                                       ModulesDirectory4 + @"\MockModuleReferencingOtherModule.dll", ModulesDirectory4 + @"\MockModuleZZZ.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Equal(2, modules.Count());
        }

        [Fact]
        public void ShouldLoadAssemblyEvenIfIsExposingTypesFromAnAssemblyInTheGac()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockExposingTypeFromGacAssemblyModule.cs",
                                       ModulesDirectory4 + @"\MockExposingTypeFromGacAssemblyModule.dll", @"System.Transactions.dll");

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory4
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();

            Assert.Single(modules);
        }

        [Fact]
        public void ShouldNotFailWhenAlreadyLoadedAssembliesAreAlsoFoundOnTargetDirectory()
        {
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockModuleA.cs",
                                       ModulesDirectory1 + @"\MockModuleA.dll");

            string filename = typeof(DirectoryModuleCatalog).Assembly.Location;
            string destinationFileName = Path.Combine(ModulesDirectory1, Path.GetFileName(filename));
            File.Copy(filename, destinationFileName);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();
            Assert.Single(modules);
        }

        [Fact]
        public void ShouldIgnoreAbstractClassesThatImplementIModule()
        {
            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
            CompilerHelper.CompileFile(@"Refrakt.Wpf.Tests.Mocks.Modules.MockAbstractModule.cs",
                                     ModulesDirectory1 + @"\MockAbstractModule.dll");

            string filename = typeof(DirectoryModuleCatalog).Assembly.Location;
            string destinationFileName = Path.Combine(ModulesDirectory1, Path.GetFileName(filename));
            File.Copy(filename, destinationFileName);

            DirectoryModuleCatalog catalog = new DirectoryModuleCatalog
            {
                ModulePath = ModulesDirectory1
            };
            catalog.Load();

            var modules = catalog.Modules.ToArray();
            Assert.Single(modules);
            Assert.Equal("MockInheritingModule", modules[0].ModuleName);

            CompilerHelper.CleanUpDirectory(ModulesDirectory1);
        }

        public void Dispose()
        {
            CleanUpDirectories();
        }
    }
}
#endif
