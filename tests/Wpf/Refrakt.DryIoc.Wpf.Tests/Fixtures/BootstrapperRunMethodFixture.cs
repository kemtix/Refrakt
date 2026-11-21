using System;
using DryIoc;
using NSubstitute;
using Refrakt.Container.Wpf.Mocks;
using Refrakt.DryIoc;
using Refrakt.Events;
using Refrakt.Ioc;
using Refrakt.Modularity;
using Refrakt.Regions;
using Xunit;
using Arg = NSubstitute.Arg;

namespace Refrakt.Container.Wpf.Tests.Bootstrapper
{
    public partial class BootstrapperRunMethodFixture
    {
        [StaFact]
        public void RunAddsCompositionContainerToContainer()
        {
            var bootstrapper = new MockBootstrapper();

            var createdContainer = bootstrapper.CallCreateContainer();
            var returnedContainer = createdContainer.Resolve<IContainer>();
            Assert.NotNull(returnedContainer);
            Assert.Equal(typeof(global::DryIoc.Container), returnedContainer.GetType());
        }

        [StaFact]
        public void RunRegistersInstanceOfIModuleCatalog()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received().Register(Arg.Any<Factory>(), Arg.Is(typeof(IModuleCatalog)), Arg.Any<object>(), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForIModuleInitializer()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(IModuleInitializer)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionManager()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(IRegionManager)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForRegionAdapterMappings()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(RegionAdapterMappings)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionViewRegistry()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(IRegionViewRegistry)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForIRegionBehaviorFactory()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(IRegionBehaviorFactory)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunRegistersTypeForIEventAggregator()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);

            bootstrapper.Run();

            mockedContainer.Received(1).Register(Arg.Any<Factory>(), Arg.Is(typeof(IEventAggregator)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        [StaFact]
        public void RunFalseShouldNotRegisterDefaultServicesAndTypes()
        {
            var mockedContainer = Substitute.For<IContainer, IResolver>();
            SetupMockedContainerForVerificationTests(mockedContainer);

            var bootstrapper = new MockedContainerBootstrapper(mockedContainer);
            bootstrapper.Run(false);

            mockedContainer.DidNotReceive().Register(Arg.Any<Factory>(), Arg.Is(typeof(IEventAggregator)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
            mockedContainer.DidNotReceive().Register(Arg.Any<Factory>(), Arg.Is(typeof(IRegionManager)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
            mockedContainer.DidNotReceive().Register(Arg.Any<Factory>(), Arg.Is(typeof(RegionAdapterMappings)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
            mockedContainer.DidNotReceive().Register(Arg.Any<Factory>(), Arg.Is(typeof(IModuleInitializer)), Arg.Is((object)null), Arg.Any<IfAlreadyRegistered?>(), Arg.Any<bool>());
        }

        private static void SetupMockedContainerForVerificationTests(IContainer mockedContainer)
        {
            var mockedModuleInitializer = Substitute.For<IModuleInitializer>();
            var mockedModuleManager = Substitute.For<IModuleManager>();
            var regionAdapterMappings = new RegionAdapterMappings();

            var containerExtension = new DryIocContainerExtension(mockedContainer);
            var regionBehaviorFactory = new RegionBehaviorFactory(containerExtension);

            // Values are created before the Returns calls: creating them inside Returns(...) would call the substitute first.
            var moduleCatalog = new ModuleCatalog();
            var selectorRegionAdapter = new SelectorRegionAdapter(regionBehaviorFactory);
            var itemsControlRegionAdapter = new ItemsControlRegionAdapter(regionBehaviorFactory);
            var contentControlRegionAdapter = new ContentControlRegionAdapter(regionBehaviorFactory);

            // NOTE: The actual method called by Refrakt's DryIocContainerExtension is off over the IResolver not IContainer
            var mockedResolver = (IResolver)mockedContainer;

            mockedResolver.Resolve(Arg.Is(typeof(IModuleCatalog)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                moduleCatalog);

            mockedResolver.Resolve(Arg.Is(typeof(IModuleInitializer)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                mockedModuleInitializer);

            mockedResolver.Resolve(Arg.Is(typeof(IModuleManager)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                mockedModuleManager);

            mockedResolver.Resolve(Arg.Is(typeof(RegionAdapterMappings)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                regionAdapterMappings);

            mockedResolver.Resolve(Arg.Is(typeof(SelectorRegionAdapter)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                selectorRegionAdapter);

            mockedResolver.Resolve(Arg.Is(typeof(ItemsControlRegionAdapter)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                itemsControlRegionAdapter);

            mockedResolver.Resolve(Arg.Is(typeof(ContentControlRegionAdapter)), Arg.Any<object>(), Arg.Is(IfUnresolved.Throw), Arg.Any<Type>(), Arg.Any<Request>(), Arg.Any<object[]>()).Returns(
                contentControlRegionAdapter);
        }
    }
}
