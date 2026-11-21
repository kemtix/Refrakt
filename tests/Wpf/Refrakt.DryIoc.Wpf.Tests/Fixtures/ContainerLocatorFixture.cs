using System.Linq;
using DryIoc;
using NSubstitute;
using Refrakt.Ioc;
using Xunit;

namespace Refrakt.DryIoc.Wpf.Tests
{
    public class ContainerLocatorFixture
    {
        [Fact]
        public void ShouldForwardResolveToInnerContainer()
        {
            var mockContainer = Substitute.For<IContainer>();

            Assert.Empty(mockContainer.ReceivedCalls());
            var containerExtension = new DryIocContainerExtension(mockContainer);

            // We register the IContainerExtension and IContainerProvider directly with the container in the ctor.
            Assert.Equal(2, mockContainer.ReceivedCalls().Count());
            ContainerLocator.ResetContainer();
            ContainerLocator.SetContainerExtension(() => containerExtension);

            var resolved = ContainerLocator.Container.Resolve(typeof(object));
            Assert.Equal(3, mockContainer.ReceivedCalls().Count());
        }
    }
}
