using NSubstitute;
using Refrakt.Modularity;
using Refrakt.Wpf.Tests.Mocks.Modules;
using Xunit;

namespace Refrakt.Wpf.Tests.Modularity
{
    public class ModuleManagerExtensionsFixture
    {
        [Fact]
        public void ModuleManagerExposesIModuleCatalogModules()
        {
            var modules = new[]
            {
                new ModuleInfo(typeof(MockModuleA))
            };
            var moduleCatalogMock = Substitute.For<IModuleCatalog>();
            moduleCatalogMock.Modules.Returns(modules);
            IModuleManager manager = new ModuleManager(Substitute.For<IModuleInitializer>(), moduleCatalogMock);

            Assert.Same(modules, manager.Modules);
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithGeneric()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = Substitute.For<IModuleCatalog>();
            moduleCatalogMock.Modules.Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Substitute.For<IModuleInitializer>(), moduleCatalogMock);

            Assert.Equal(moduleInfo.State, manager.GetModuleState<MockModuleA>());
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState<MockModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectModuleStateWithName()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = Substitute.For<IModuleCatalog>();
            moduleCatalogMock.Modules.Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Substitute.For<IModuleInitializer>(), moduleCatalogMock);

            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.LoadingTypes;
            Assert.Equal(moduleInfo.State, manager.GetModuleState(nameof(MockModuleA)));
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithGeneric()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = Substitute.For<IModuleCatalog>();
            moduleCatalogMock.Modules.Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Substitute.For<IModuleInitializer>(), moduleCatalogMock);

            Assert.False(manager.IsModuleInitialized<MockModuleA>());
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized<MockModuleA>());
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized<MockModuleA>());
        }

        [Fact]
        public void ModuleManagerReturnsCorrectInitializationStateWithName()
        {
            var moduleInfo = new ModuleInfo(typeof(MockModuleA));
            var moduleCatalogMock = Substitute.For<IModuleCatalog>();
            moduleCatalogMock.Modules.Returns(new[] { moduleInfo });
            IModuleManager manager = new ModuleManager(Substitute.For<IModuleInitializer>(), moduleCatalogMock);

            Assert.False(manager.IsModuleInitialized(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.Initializing;
            Assert.False(manager.IsModuleInitialized(nameof(MockModuleA)));
            moduleInfo.State = ModuleState.Initialized;
            Assert.True(manager.IsModuleInitialized(nameof(MockModuleA)));
        }

        [Fact]
        public void ModuleManagerLoadModuleGeneric_CallsLoadModuleWithName()
        {
            var managerMock = Substitute.For<IModuleManager>();
            managerMock.LoadModule<MockModuleA>();
            managerMock.Received().LoadModule(nameof(MockModuleA));
        }
    }
}
