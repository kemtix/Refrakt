using System;
using Refrakt.Container.Wpf.Mocks;
using Refrakt.IocContainer.Wpf.Tests.Support;
using Xunit;

namespace Refrakt.Container.Wpf.Tests.Bootstrapper
{
    public class BootstrapperNullModuleCatalogFixture : BootstrapperFixtureBase
    {
        [Fact]
        public void NullModuleCatalogThrowsOnDefaultModuleInitialization()
        {
            var bootstrapper = new NullModuleCatalogBootstrapper();

            AssertExceptionThrownOnRun(bootstrapper, typeof(InvalidOperationException), "IModuleCatalog");
        }
    }
}
