using HelloWorld.Modules.ModuleA;
using HelloWorld.Views;
using Refrakt.Ioc;
using Refrakt.Modularity;
using Refrakt.Unity;
using System.Windows;
using Unity;

namespace HelloWorld
{
    class Bootstrapper : RefraktBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSharedSamples();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<ModuleAModule>();
        }
    }
}