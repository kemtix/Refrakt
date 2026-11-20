using Refrakt.Container.Wpf.Mocks;
using Refrakt.Ioc;
using Refrakt.IocContainer.Wpf.Tests.Support.Mocks;
using Refrakt.IocContainer.Wpf.Tests.Support.Mocks.ViewModels;
using Refrakt.IocContainer.Wpf.Tests.Support.Mocks.Views;
using Refrakt.Mvvm;
using Xunit;

namespace Refrakt.Container.Wpf.Tests.Mvvm
{
    [Collection(nameof(ContainerExtension))]
    public class ViewModelLocatorFixture
    {
        [StaFact]
        public void ShouldLocateViewModelAndResolveWithContainer()
        {
            var bootstrapper = new MockBootstrapper();
            bootstrapper.Run();

            bootstrapper.ContainerRegistry.Register<IService, MockService>();

            var view = new MockView();
            Assert.Null(view.DataContext);

            ViewModelLocator.SetAutoWireViewModel(view, true);
            Assert.NotNull(view.DataContext);
            Assert.IsType<MockViewModel>(view.DataContext);

            Assert.NotNull(((MockViewModel)view.DataContext).MockService);
        }
    }
}
