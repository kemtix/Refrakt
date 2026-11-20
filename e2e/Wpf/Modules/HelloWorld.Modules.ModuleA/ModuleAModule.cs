using HelloWorld.Modules.ModuleA.Views;
using Refrakt.Ioc;
using Refrakt.Modularity;

namespace HelloWorld.Modules.ModuleA
{
    public class ModuleAModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}
