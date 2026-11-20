using System;
using Refrakt.Ioc;
using Refrakt.Modularity;

namespace Refrakt.Wpf.Tests.Mocks.Modules
{
    public class MockModuleReferencingAssembly : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            MockReferencedModule instance = new MockReferencedModule();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new NotImplementedException();
        }
    }
}
