using System;
using NSubstitute;
using Refrakt.Ioc;
using Xunit;

namespace Refrakt.Tests.Ioc
{
    public class ContainerRegistryExtensionsFixture
    {
        private interface ITestService { }

        private interface ITest2Service { }

        private class TestService : ITestService, ITest2Service { }

        [Fact]
        public void RegisterInstanceOfT()
        {
            var testService = new TestService();
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterInstance(typeof(TestService), testService)
                .Returns(mock);

            var cr = mock.RegisterInstance(testService);
            Assert.Same(mock, cr);

            mock.Received().RegisterInstance(typeof(TestService), testService);
        }

        [Fact]
        public void RegisterNamedInstanceOfT()
        {
            var testService = new TestService();
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterInstance(typeof(TestService), testService, "Test")
                .Returns(mock);

            var cr = mock.RegisterInstance(testService, "Test");
            Assert.Same(mock, cr);
            mock.Received().RegisterInstance(typeof(TestService), testService, "Test");
        }

        [Fact]
        public void RegisterSingletonFromConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterSingleton(typeof(TestService));
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterSingletonFromGenericConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterSingleton<TestService>();
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterSingletonWithService()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(ITestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterSingleton<ITestService, TestService>();
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(ITestService), typeof(TestService));
        }

        [Fact]
        public void RegisterSingletonNamedWithService()
        {

            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(ITestService), typeof(TestService), "Test")
                .Returns(mock);

            var cr = mock.RegisterSingleton<ITestService, TestService>("Test");
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(ITestService), typeof(TestService), "Test");
        }

        [Fact]
        public void RegisterSingletonFromDelegateFunction()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.RegisterSingleton<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(ITestService), TestDelegate);
        }

        [Fact]
        public void RegisterSingletonFromDelegateFunctionWithContainerProvider()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterSingleton(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.RegisterSingleton<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().RegisterSingleton(typeof(ITestService), TestDelegate);
        }

        [Fact]
        public void RegisterSingletonWithManyInterfaces()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterManySingleton(Arg.Any<Type>(), Arg.Any<Type[]>())
                .Returns(mock);
            var services = new[] { typeof(ITestService), typeof(ITest2Service) };
            var cr = mock.RegisterManySingleton<TestService>(services);
            Assert.Same(mock, cr);
            mock.Received().RegisterManySingleton(typeof(TestService), services);
        }

        [Fact]
        public void RegisterFromConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.Register(typeof(TestService));
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterFromGenericConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.Register<TestService>();
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterWithService()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(ITestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.Register<ITestService, TestService>();
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(ITestService), typeof(TestService));
        }

        [Fact]
        public void RegisterNamedWithService()
        {

            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(ITestService), typeof(TestService), "Test")
                .Returns(mock);

            var cr = mock.Register<ITestService, TestService>("Test");
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(ITestService), typeof(TestService), "Test");
        }

        [Fact]
        public void RegisterFromDelegateFunction()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.Register<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(ITestService), TestDelegate);
        }

        [Fact]
        public void RegisterFromDelegateFunctionWithContainerProvider()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.Register(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.Register<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().Register(typeof(ITestService), TestDelegate);
        }

        [Fact]
        public void RegisterWithManyInterfaces()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterMany(Arg.Any<Type>(), Arg.Any<Type[]>())
                .Returns(mock);
            var services = new[] { typeof(ITestService), typeof(ITest2Service) };
            var cr = mock.RegisterMany<TestService>(services);
            Assert.Same(mock, cr);
            mock.Received().RegisterMany(typeof(TestService), services);
        }

        [Fact]
        public void RegisterScopedFromConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterScoped(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterScoped(typeof(TestService));
            Assert.Same(mock, cr);
            mock.Received().RegisterScoped(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterScopedFromGenericConcreteType()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterScoped(typeof(TestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterScoped<TestService>();
            Assert.Same(mock, cr);
            mock.Received().RegisterScoped(typeof(TestService), typeof(TestService));
        }

        [Fact]
        public void RegisterScopedWithService()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterScoped(typeof(ITestService), typeof(TestService))
                .Returns(mock);

            var cr = mock.RegisterScoped<ITestService, TestService>();
            Assert.Same(mock, cr);
            mock.Received().RegisterScoped(typeof(ITestService), typeof(TestService));
        }

        [Fact]
        public void RegisterScopedFromDelegateFunction()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterScoped(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.RegisterScoped<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().RegisterScoped(typeof(ITestService), TestDelegate);
        }

        [Fact]
        public void RegisterScopedFromDelegateFunctionWithContainerProvider()
        {
            var mock = Substitute.For<IContainerRegistry>();
            mock.RegisterScoped(typeof(ITestService), TestDelegate)
                .Returns(mock);

            var cr = mock.RegisterScoped<ITestService>(TestDelegate);
            Assert.Same(mock, cr);
            mock.Received().RegisterScoped(typeof(ITestService), TestDelegate);
        }




        private static ITestService TestDelegate() =>
            new TestService();

        private static ITestService TestDelegateFactory(IContainerProvider containerProvider) =>
            containerProvider.Resolve<ITestService>("Named");
    }
}
