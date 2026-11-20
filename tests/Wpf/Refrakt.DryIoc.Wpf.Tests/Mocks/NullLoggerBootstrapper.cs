using System.Windows;
using Refrakt.DryIoc;
using Refrakt.Ioc;

namespace Refrakt.Container.Wpf.Mocks
{
    internal partial class NullLoggerBootstrapper : RefraktBootstrapper
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            throw new System.NotImplementedException();
        }

        protected override DependencyObject CreateShell()
        {
            throw new System.NotImplementedException();
        }
    }
}
