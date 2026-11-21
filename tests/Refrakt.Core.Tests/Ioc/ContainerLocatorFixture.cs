using NSubstitute;
using Refrakt.Ioc;
using Xunit;

namespace Refrakt.Tests.Ioc
{
    public class ContainerLocator { }

    [CollectionDefinition(nameof(ContainerLocator), DisableParallelization = true)]
    public class ContainerLocatorCollection : ICollectionFixture<ContainerLocator>
    {
    }

    [Collection(nameof(ContainerLocator))]
    public class ContainerLocatorFixture
    {
        [Fact]
        public void FactoryCreatesContainerExtension()
        {
            Refrakt.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Refrakt.Ioc.ContainerLocator.Current);
            Refrakt.Ioc.ContainerLocator.SetContainerExtension(() => Substitute.For<IContainerExtension>());
            Assert.NotNull(Refrakt.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void ResetNullsCurrentContainer()
        {
            Refrakt.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Refrakt.Ioc.ContainerLocator.Current);
            Refrakt.Ioc.ContainerLocator.SetContainerExtension(() => Substitute.For<IContainerExtension>());
            Assert.NotNull(Refrakt.Ioc.ContainerLocator.Current);
            Refrakt.Ioc.ContainerLocator.ResetContainer();
            Assert.Null(Refrakt.Ioc.ContainerLocator.Current);
        }

        [Fact]
        public void FactoryOnlySetsContainerOnce()
        {
            Refrakt.Ioc.ContainerLocator.ResetContainer();
            var container = Substitute.For<IContainerExtension>();
            var container2 = Substitute.For<IContainerExtension>();

            Refrakt.Ioc.ContainerLocator.SetContainerExtension(() => container);
            Assert.Same(container, Refrakt.Ioc.ContainerLocator.Container);

            Refrakt.Ioc.ContainerLocator.SetContainerExtension(() => container2);
            Assert.NotSame(container2, Refrakt.Ioc.ContainerLocator.Container);
            Assert.Same(container, Refrakt.Ioc.ContainerLocator.Container);
        }
    }
}
