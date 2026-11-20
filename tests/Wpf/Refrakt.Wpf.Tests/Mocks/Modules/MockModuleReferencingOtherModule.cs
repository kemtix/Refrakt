using System;
using Refrakt.Ioc;
using Refrakt.Modularity;

namespace Refrakt.Wpf.Tests.Mocks.Modules
{
    public class MockModuleReferencingOtherModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }
    }

    public class MyDummyClass : DummyClass { }
}
