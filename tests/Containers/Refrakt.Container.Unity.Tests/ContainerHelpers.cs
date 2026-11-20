using Unity;

namespace Refrakt.Ioc.Tests
{
    public static class ContainerHelpers
    {
        public static IUnityContainer GetContainer(this IContainerExtension container) =>
            Refrakt.Unity.RefraktIocExtensions.GetContainer((IContainerProvider)container);
    }
}
