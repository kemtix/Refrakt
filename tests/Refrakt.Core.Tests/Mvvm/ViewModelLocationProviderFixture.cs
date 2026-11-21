using System;
using Refrakt.Mvvm;
using Refrakt.Tests.Mocks.ViewModels;
using Refrakt.Tests.Mocks.Views;
using Xunit;

namespace Refrakt.Tests.Mvvm
{
    public class ViewModelLocationProviderFixture
    {
        [Fact]
        public void ShouldLocateViewModelWithDefaultSettings()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<MockViewModel>(vm);
            });
        }

        [Fact]
        public void ShouldLocateViewModelWithDefaultSettingsForViewsThatEndWithView()
        {
            ResetViewModelLocationProvider();

            var view = new MockView();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<MockViewModel>(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomDefaultViewModelFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            object mockObject = new object();
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewType => mockObject);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType(mockObject.GetType(), vm);
            });
        }

        [Fact]
        public void ShouldUseCustomDefaultViewTypeToViewModelTypeResolverWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => typeof(ViewModelLocationProviderFixture));

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        [Fact]
        public void ShouldFailWhenCustomDefaultViewTypeToViewModelTypeResolverIsNull()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => null);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.Null(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomFactoryWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            string viewModel = "Test String";
            ViewModelLocationProvider.Register(view.GetType().ToString(), () => viewModel);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
                {
                    Assert.NotNull(v);
                    Assert.NotNull(vm);
                    Assert.Equal(viewModel, vm);
                });
        }

        [Fact]
        public void ShouldUseCustomFactoryWhenSet_Generic()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            string viewModel = "Test String";
            ViewModelLocationProvider.Register<Mock>(() => viewModel);

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.Equal(viewModel, vm);
            });
        }

        [Fact]
        public void ShouldUseCustomTypeWhenSet()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.Register(view.GetType().ToString(), typeof(ViewModelLocationProviderFixture));

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        [Fact]
        public void ShouldUseCustomTypeWhenSet_Generic()
        {
            ResetViewModelLocationProvider();

            var view = new Mock();

            ViewModelLocationProvider.Register<Mock, ViewModelLocationProviderFixture>();

            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
            {
                Assert.NotNull(v);
                Assert.NotNull(vm);
                Assert.IsType<ViewModelLocationProviderFixture>(vm);
            });
        }

        [Fact]
        public void ResetShouldRemoveRegisteredFactories()
        {
            ViewModelLocationProvider.Register<Mock>(() => "x");

            ResetViewModelLocationProvider();

            Assert.IsType<MockViewModel>(AutoWire(new Mock()));
        }

        [Fact]
        public void ResetShouldRemoveRegisteredTypes()
        {
            ViewModelLocationProvider.Register<Mock, ViewModelLocationProviderFixture>();

            ResetViewModelLocationProvider();

            Assert.IsType<MockViewModel>(AutoWire(new Mock()));
        }

        [Fact]
        public void ResetShouldRestoreDefaultViewModelFactory()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(viewModelType => "x");

            ResetViewModelLocationProvider();

            Assert.IsType<MockViewModel>(AutoWire(new Mock()));
        }

        [Fact]
        public void ResetShouldRestoreDefaultViewModelFactoryWithViewParameter()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, viewModelType) => "x");

            ResetViewModelLocationProvider();

            Assert.IsType<MockViewModel>(AutoWire(new Mock()));
        }

        [Fact]
        public void ResetShouldRestoreDefaultViewTypeToViewModelTypeResolver()
        {
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(viewType => typeof(ViewModelLocationProviderFixture));

            ResetViewModelLocationProvider();

            Assert.IsType<MockViewModel>(AutoWire(new Mock()));
        }

        [Fact]
        public void ResetHelperShouldNameTheMissingField()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => ViewModelLocationProviderReset.GetStaticField("_missingField"));

            Assert.Equal("ViewModelLocationProvider has no static field '_missingField'; update ViewModelLocationProviderReset.", ex.Message);
        }

        private static object AutoWire(object view)
        {
            object viewModel = null;
            ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) => viewModel = vm);
            return viewModel;
        }

        private static void ResetViewModelLocationProvider()
        {
            ViewModelLocationProviderReset.Reset();
        }
    }
}
