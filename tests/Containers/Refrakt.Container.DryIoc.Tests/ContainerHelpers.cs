using DryIoc;

namespace Refrakt.Ioc.Tests
{
    public static class ContainerHelpers
    {
        public static IContainer GetContainer(this IContainerExtension container) =>
            Refrakt.DryIoc.RefraktIocExtensions.GetContainer((IContainerProvider)container);
    }
}
