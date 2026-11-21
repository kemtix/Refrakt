

using System;
using System.Windows;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Refrakt.Ioc;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class LocatorNavigationTargetHandlerFixture
    {
        [Fact]
        public void WhenViewExistsAndDoesNotImplementINavigationAware_ThenReturnsView()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var view = new TestView();

            region.Add(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingTypeNameAreConsidered()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var view1 = new TestView();
            var view2 = "view";

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenRegionHasMultipleViews_ThenViewsWithMatchingFullTypeNameAreConsidered()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var view1 = new TestView();
            var view2 = "view";

            region.Add(view1);
            region.Add(view2);

            var navigationContext = new NavigationContext(null, new Uri(view2.GetType().FullName, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view2, returnedView);
        }

        [Fact]
        public void WhenViewExistsAndImplementsINavigationAware_ThenViewIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var viewMock = Substitute.For<INavigationAware>();
            viewMock
                .IsNavigationTarget(Arg.Any<NavigationContext>())
                .Returns(true);

            region.Add(viewMock);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock, returnedView);
            viewMock.Received().IsNavigationTarget(Arg.Any<NavigationContext>());
        }

        [StaFact]
        public void WhenViewExistsAndHasDataContextThatImplementsINavigationAware_ThenDataContextIsQueriedForNavigationAndIsReturnedIfAcceptsIt()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var dataContextMock = Substitute.For<INavigationAware>();
            dataContextMock
                .IsNavigationTarget(Arg.Any<NavigationContext>())
                .Returns(true);
            var viewMock = Substitute.For<FrameworkElement>();
            viewMock.DataContext = dataContextMock;

            region.Add(viewMock);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(viewMock, returnedView);
            dataContextMock.Received().IsNavigationTarget(Arg.Any<NavigationContext>());
        }

        [Fact]
        public void WhenNoCurrentMatchingViewExists_ThenReturnsNewlyCreatedInstanceWithServiceLocatorAddedToTheRegion()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var view = new TestView();

            containerMock.Resolve(typeof(object), view.GetType().Name).Returns(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(view, returnedView);
            Assert.True(region.Views.Contains(view));
        }

        [Fact]
        public void WhenViewExistsAndImplementsINavigationAware_ThenViewIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange

            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var viewMock = Substitute.For<INavigationAware>();
            viewMock
                .IsNavigationTarget(Arg.Any<NavigationContext>())
                .Returns(false);

            region.Add(viewMock);

            var newView = new TestView();

            containerMock.Resolve(typeof(object), viewMock.GetType().Name).Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            // Act

            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);


            // Assert

            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            viewMock.Received().IsNavigationTarget(Arg.Any<NavigationContext>());
        }

        [StaFact]
        public void WhenViewExistsAndHasDataContextThatImplementsINavigationAware_ThenDataContextIsQueriedForNavigationAndNewInstanceIsCreatedIfItRejectsIt()
        {
            // Arrange
            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var dataContextMock = Substitute.For<INavigationAware>();
            dataContextMock
                .IsNavigationTarget(Arg.Any<NavigationContext>())
                .Returns(false);
            var viewMock = Substitute.For<FrameworkElement>();
            viewMock.DataContext = dataContextMock;

            region.Add(viewMock);

            var newView = new TestView();

            containerMock.Resolve(typeof(object), viewMock.GetType().Name).Returns(newView);

            var navigationContext = new NavigationContext(null, new Uri(viewMock.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);

            // Act
            var returnedView = navigationTargetHandler.LoadContent(region, navigationContext);

            // Assert
            Assert.Same(newView, returnedView);
            Assert.True(region.Views.Contains(newView));
            dataContextMock.Received().IsNavigationTarget(Arg.Any<NavigationContext>());
        }

        [Fact]
        public void WhenViewCannotBeCreated_ThenThrowsAnException()
        {
            var containerMock = Substitute.For<IContainerExtension>();
            containerMock.Resolve(typeof(object), typeof(TestView).Name).Throws<ActivationException>();

            var region = new Region();

            var navigationContext = new NavigationContext(null, new Uri(typeof(TestView).Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);

            // Act

            ExceptionAssert.Throws<InvalidOperationException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(region, navigationContext);

                });
        }

        [Fact]
        public void WhenViewAddedByHandlerDoesNotImplementINavigationAware_ThenReturnsView()
        {
            // Arrange
            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var view = new TestView();

            containerMock.Resolve(typeof(object), view.GetType().Name).Returns(view);

            var navigationContext = new NavigationContext(null, new Uri(view.GetType().Name, UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);

            // Act
            var firstReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);
            var secondReturnedView = navigationTargetHandler.LoadContent(region, navigationContext);

            // Assert
            Assert.Same(view, firstReturnedView);
            Assert.Same(view, secondReturnedView);
            containerMock.Received(1).Resolve(typeof(object), view.GetType().Name);
        }

        [Fact]
        public void WhenRequestingContentForNullRegion_ThenThrows()
        {
            var containerMock = Substitute.For<IContainerExtension>();

            var navigationContext = new NavigationContext(null, new Uri("/", UriKind.Relative));

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(null, navigationContext);

                });
        }

        [Fact]
        public void WhenRequestingContentForNullContext_ThenThrows()
        {
            var containerMock = Substitute.For<IContainerExtension>();

            var region = new Region();

            var navigationTargetHandler = new TestRegionNavigationContentLoader(containerMock);


            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigationTargetHandler.LoadContent(region, null);

                });
        }

        public class TestRegionNavigationContentLoader : RegionNavigationContentLoader
        {
            public TestRegionNavigationContentLoader(IContainerExtension container)
                : base(container)
            { }
        }

        public class TestView { }
    }

    public class ActivationException : Exception
    {
        public ActivationException()
        {
        }
    }
}
