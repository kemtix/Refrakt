using System;
using Refrakt.Ioc;
using Refrakt.Unity;
using Unity;

namespace Refrakt.Ioc.Tests
{
    partial class ContainerSetup
    {
        IContainerExtension CreateContainerInternal() => new UnityContainerExtension();

        public Type NativeContainerType => typeof(IUnityContainer);
    }
}
