using System;
using Refrakt.Container.Wpf.Mocks;
using Refrakt.Ioc;
using Refrakt.Modularity;
using Refrakt.Regions;
using Xunit;
using static Refrakt.Container.Wpf.Tests.ContainerHelper;

namespace Refrakt.Container.Wpf.Tests.Bootstrapper
{
    [Collection(nameof(ContainerExtension))]
    public class BootstrapperFixture
    {
        [StaFact]
        public void ContainerDefaultsToNull()
        {
            var bootstrapper = new MockBootstrapper();
            var container = bootstrapper.ContainerExtension;

            Assert.Null(container);
        }

        [StaFact]
        public void CanCreateConcreteBootstrapper()
        {
            new MockBootstrapper();
        }

        [StaFact]
        public void CreateContainerShouldInitializeContainer()
        {
            var bootstrapper = new MockBootstrapper();

            var container = bootstrapper.CallCreateContainer();

            Assert.NotNull(container);
            Assert.IsAssignableFrom(BaseContainerInterfaceType, container);
        }

        [StaFact]
        public void ConfigureContainerAddsModuleCatalogToContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var returnedCatalog = bootstrapper.ContainerExtension.Resolve<IModuleCatalog>();
            Assert.NotNull(returnedCatalog);
            Assert.IsType<ModuleCatalog>(returnedCatalog);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalEntryToContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationJournalEntry>();
            var actual2 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationJournalEntry>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationJournalToContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationJournal>();
            var actual2 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationJournal>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsRegionNavigationServiceToContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationService>();
            var actual2 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationService>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.NotSame(actual1, actual2);
        }

        [StaFact]
        public void ConfigureContainerAddsNavigationTargetHandlerToContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            var actual1 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationContentLoader>();
            var actual2 = bootstrapper.ContainerExtension.Resolve<IRegionNavigationContentLoader>();

            Assert.NotNull(actual1);
            Assert.NotNull(actual2);
            Assert.Same(actual1, actual2);
        }

        [StaFact]
        public void RegisterFrameworkExceptionTypesShouldRegisterResolutionFailedException()
        {
            var bootstrapper = new MockBootstrapper();

            bootstrapper.CallRegisterFrameworkExceptionTypes();

            Assert.True(ExceptionExtensions.IsFrameworkExceptionRegistered(RegisteredFrameworkException));
        }
    }
}
