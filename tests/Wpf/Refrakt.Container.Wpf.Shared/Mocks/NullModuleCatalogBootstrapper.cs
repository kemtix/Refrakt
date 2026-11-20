using System.Windows;
using Refrakt.Ioc;
using Refrakt.Modularity;

namespace Refrakt.Container.Wpf.Mocks
{
    internal partial class NullModuleCatalogBootstrapper
    {
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return null;
        }

        protected override DependencyObject CreateShell()
        {
            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
