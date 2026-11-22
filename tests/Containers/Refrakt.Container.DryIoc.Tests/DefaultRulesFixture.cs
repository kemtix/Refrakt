using System;
using System.Collections.Generic;
using DryIoc;
using Refrakt.Ioc.Mocks.Services;
using Refrakt.Ioc.Tests;
using Xunit;

namespace Refrakt.Ioc.DryIoc.Tests
{
    public class DefaultRulesFixture : TestBase
    {
        public DefaultRulesFixture(ContainerSetup setup)
            : base(setup)
        {
        }

        [Fact]
        public void DisposableTransientIsDisposedWithItsScope()
        {
            var container = Setup.CreateContainer();
            Setup.Registry.Register<DisposableService>();
            var resolved = container.Resolve<DisposableService>();
            Assert.NotNull(resolved);
            Assert.False(resolved.IsDisposed);

            container.CurrentScope.Dispose();

            Assert.True(resolved.IsDisposed);
        }

        [Fact]
        public void FuncOfUnregisteredServiceResolvesAndThrowsContainerExceptionWhenInvoked()
        {
            var container = Setup.CreateContainer();
            Func<IServiceA> func = null;
            var ex = Record.Exception(() => func = container.Resolve<Func<IServiceA>>());
            Assert.Null(ex);
            Assert.NotNull(func);

            Assert.Throws<ContainerException>(() => func());
        }

        [Fact]
        public void CollectionOfUnregisteredConcreteTypeIsEmpty()
        {
            var container = Setup.CreateContainer();
            IEnumerable<ServiceA> enumerable = null;
            var enumerableEx = Record.Exception(() => enumerable = container.Resolve<IEnumerable<ServiceA>>());
            Assert.Null(enumerableEx);
            Assert.NotNull(enumerable);
            Assert.Empty(enumerable);

            ServiceA[] array = null;
            var arrayEx = Record.Exception(() => array = container.Resolve<ServiceA[]>());
            Assert.Null(arrayEx);
            Assert.NotNull(array);
            Assert.Empty(array);
        }

        public class DisposableService : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
