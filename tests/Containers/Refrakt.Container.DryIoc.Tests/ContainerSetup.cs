using System;
using DryIoc;
using Refrakt.DryIoc;

namespace Refrakt.Ioc.Tests
{
    partial class ContainerSetup
    {
        protected virtual IContainerExtension CreateContainerInternal() => new DryIocContainerExtension();

        public Type NativeContainerType => typeof(IContainer);
    }
}
