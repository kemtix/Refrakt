using HelloWorld.Views;
using Refrakt.Ioc;
using System.Windows;
using Refrakt.Modularity;

namespace HelloWorld
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// <remarks>
    /// HelloWorld with one difference: ModuleA is not referenced at compile time,
    /// it is found at run time in the Modules folder next to the executable.
    /// </remarks>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSharedSamples();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = "Modules" };
        }
    }
}
