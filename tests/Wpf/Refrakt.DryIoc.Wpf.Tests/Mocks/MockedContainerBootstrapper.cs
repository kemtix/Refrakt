using System.Windows;
using System.Windows.Controls;
using DryIoc;
using Refrakt.DryIoc;
using Refrakt.Ioc;

namespace Refrakt.Container.Wpf.Mocks
{
    internal class MockedContainerBootstrapper : RefraktBootstrapper
    {
        private readonly IContainer _container;

        public MockedContainerBootstrapper(IContainer container)
        {
            ContainerLocator.ResetContainer();
            this._container = container;
        }

        bool _useDefaultConfiguration = true;

        public void Run(bool useDefaultConfiguration)
        {
            _useDefaultConfiguration = useDefaultConfiguration;

            base.Run();
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            return new DryIocContainerExtension(_container);
        }

        protected override DependencyObject CreateShell()
        {
            return new UserControl();
        }

        protected override void InitializeShell(DependencyObject shell)
        {

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void RegisterRequiredTypes(IContainerRegistry containerRegistry)
        {
            if (_useDefaultConfiguration)
                base.RegisterRequiredTypes(containerRegistry);
        }
    }
}
