using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Refrakt.Ioc;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class RegionNavigationServiceFixture
    {
        [Fact]
        public void WhenNavigating_ViewIsActivated()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            var container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingWithQueryString_ViewIsActivated()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name + "?MyQuery=true", UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.True(isViewActive);
        }

        [Fact]
        public void WhenNavigatingAndViewCannotBeAcquired_ThenNavigationResultHasError()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string otherType = "OtherType";

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            IRegionNavigationContentLoader targetHandler = Substitute.For<IRegionNavigationContentLoader>();
            targetHandler.LoadContent(Arg.Any<IRegion>(), Arg.Any<NavigationContext>()).Throws<ArgumentException>();

            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, targetHandler, journal)
            {
                Region = region
            };

            // Act
            Exception error = null;
            target.RequestNavigate(
                new Uri(otherType.GetType().Name, UriKind.Relative),
                nr =>
                {
                    error = nr.Error;
                });

            // Verify
            bool isViewActive = region.ActiveViews.Contains(view);
            Assert.False(isViewActive);
            Assert.IsType<ArgumentException>(error);
        }

        [Fact]
        public void WhenNavigatingWithNullUri_Throws()
        {
            // Prepare
            IRegion region = new Region();

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            NavigationResult navigationResult = null;
            target.RequestNavigate((Uri)null, nr => navigationResult = nr);

            // Verify
            Assert.False(navigationResult.Result.Value);
            Assert.NotNull(navigationResult.Error);
            Assert.IsType<ArgumentNullException>(navigationResult.Error);
        }

        [Fact]
        public void WhenNavigatingAndViewImplementsINavigationAware_ThenNavigatedIsInvokedOnNavigation()
        {
            // Prepare
            var region = new Region();

            var view = Substitute.For<INavigationAware>();
            view.IsNavigationTarget(Arg.Any<NavigationContext>()).Returns(true);
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view.Received().OnNavigatedTo(Arg.Is<NavigationContext>(nc => nc.Uri == navigationUri && nc.NavigationService == target));
        }

        [StaFact]
        public void WhenNavigatingAndDataContextImplementsINavigationAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            FrameworkElement frameworkElement = Substitute.For<FrameworkElement>();
            INavigationAware navigationAwareDataContext = Substitute.For<INavigationAware>();
            navigationAwareDataContext.IsNavigationTarget(Arg.Any<NavigationContext>()).Returns(true);
            frameworkElement.DataContext = navigationAwareDataContext;

            var view = frameworkElement;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            navigationAwareDataContext.Received().OnNavigatedTo(Arg.Is<NavigationContext>(nc => nc.Uri == navigationUri));
        }

        [StaFact]
        public void WhenNavigatingAndBothViewAndDataContextImplementINavigationAware_ThenNavigatedIsInvokesOnNavigation()
        {
            // Prepare
            var region = new Region();

            FrameworkElement frameworkElement = Substitute.For<FrameworkElement, INavigationAware>();
            INavigationAware navigationAwareView = (INavigationAware)frameworkElement;
            navigationAwareView.IsNavigationTarget(Arg.Any<NavigationContext>()).Returns(true);

            INavigationAware navigationAwareDataContext = Substitute.For<INavigationAware>();
            navigationAwareDataContext.IsNavigationTarget(Arg.Any<NavigationContext>()).Returns(true);
            frameworkElement.DataContext = navigationAwareDataContext;

            var view = frameworkElement;
            region.Add(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            navigationAwareView.Received().OnNavigatedTo(Arg.Is<NavigationContext>(nc => nc.Uri == navigationUri));
            navigationAwareDataContext.Received().OnNavigatedTo(Arg.Is<NavigationContext>(nc => nc.Uri == navigationUri));
        }

        [Fact]
        public void WhenNavigating_NavigationIsRecordedInJournal()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IRegionNavigationJournalEntry journalEntry = new RegionNavigationJournalEntry();

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(journalEntry);

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);

            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();


            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(viewUri, nr => { });

            // Verify
            Assert.NotNull(journalEntry);
            Assert.Equal(viewUri, journalEntry.Uri);
            journal.Received().RecordNavigation(journalEntry, true);
        }

        [Fact]
        public void WhenNavigatingAndCurrentlyActiveViewImplementsINavigateWithVeto_ThenNavigationRequestQueriesForVeto()
        {
            // Prepare
            var region = new Region();

            var view = Substitute.For<IConfirmNavigationRequest>();

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
        }

        [Fact]
        public void WhenNavigating_ThenNavigationRequestQueriesForVetoOnAllActiveViewsIfAllSucceed()
        {
            // Prepare
            var region = new Region();

            var view1 = Substitute.For<IConfirmNavigationRequest>();
            view1
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(true));

            region.Add(view1);
            region.Activate(view1);

            var view2 = Substitute.For<IConfirmNavigationRequest>();

            region.Add(view2);

            var view3 = Substitute.For<INavigationAware>();

            region.Add(view3);
            region.Activate(view3);

            var view4 = Substitute.For<IConfirmNavigationRequest>();
            view4
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(true));

            region.Add(view4);
            region.Activate(view4);

            var navigationUri = new Uri(view1.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view1.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            view2.DidNotReceive().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            // view3 has no configured call, so there is nothing to check on it.
            view4.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
        }

        [Fact]
        public void WhenRequestNavigateAwayAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1 = Substitute.For<IConfirmNavigationRequest>();
            view1
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(true));

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Result == true; });

            // Verify
            view1.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.True(navigationSucceeded);
            Assert.Equal(new object[] { view1, view2 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenRequestNavigateAwayRejectsThroughCallback_ThenNavigationDoesNotProceed()
        {
            // Prepare
            var region = new Region();

            var view1 = Substitute.For<IConfirmNavigationRequest>();
            view1
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(false));

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Result == false; });

            // Verify
            view1.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [StaFact]
        public void WhenNavigatingAndDataContextOnCurrentlyActiveViewImplementsINavigateWithVeto_ThenNavigationRequestQueriesForVeto()
        {
            // Prepare
            var region = new Region();

            var viewModel = Substitute.For<IConfirmNavigationRequest>();

            var view = Substitute.For<FrameworkElement>();
            view.DataContext = viewModel;

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri(view.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            viewModel.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
        }

        [StaFact]
        public void WhenRequestNavigateAwayOnDataContextAcceptsThroughCallback_ThenNavigationProceeds()
        {
            // Prepare
            var region = new Region();

            var view1DataContext = Substitute.For<IConfirmNavigationRequest>();
            view1DataContext
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(true));

            var view1 = Substitute.For<FrameworkElement>();
            view1.DataContext = view1DataContext;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationSucceeded = false;
            target.RequestNavigate(navigationUri, nr => { navigationSucceeded = nr.Result == true; });

            // Verify
            view1DataContext.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.True(navigationSucceeded);
            Assert.Equal(new object[] { view1, view2 }, region.ActiveViews.ToArray());
        }

        [StaFact]
        public void WhenRequestNavigateAwayOnDataContextRejectsThroughCallback_ThenNavigationDoesNotProceed()
        {
            // Prepare
            var region = new Region();

            var view1DataContext = Substitute.For<IConfirmNavigationRequest>();
            view1DataContext
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(false));

            var view1 = Substitute.For<FrameworkElement>();
            view1.DataContext = view1DataContext;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            var navigationFailed = false;
            target.RequestNavigate(navigationUri, nr => { navigationFailed = nr.Result == false; });

            // Verify
            view1DataContext.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.True(navigationFailed);
            Assert.Equal(new object[] { view1 }, region.ActiveViews.ToArray());
        }

        [Fact]
        public void WhenViewAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var view = Substitute.For<IConfirmNavigationRequest>();

            var confirmationRequests = new List<Action<bool>>();

            view
                .When(icnr => icnr.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => { confirmationRequests.Add(ci.ArgAt<Action<bool>>(1)); });

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri("", UriKind.Relative);

            var container = Substitute.For<IContainerExtension>();
            container
                .Resolve(typeof(IRegionNavigationJournalEntry))
                .Returns(new RegionNavigationJournalEntry());

            var contentLoader = Substitute.For<IRegionNavigationContentLoader>();
            contentLoader
                .LoadContent(region, Arg.Any<NavigationContext>())
                .Returns(view);

            var journal = Substitute.For<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool firstNavigation = false;
            bool secondNavigation = false;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr.Result.Value);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr.Result.Value);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation);
            Assert.True(secondNavigation);
        }

        [StaFact]
        public void WhenViewModelAcceptsNavigationOutAfterNewIncomingRequestIsReceived_ThenOriginalRequestIsIgnored()
        {
            var region = new Region();

            var viewModel = Substitute.For<IConfirmNavigationRequest>();

            var view = Substitute.For<FrameworkElement>();
            view.DataContext = viewModel;

            var confirmationRequests = new List<Action<bool>>();

            viewModel
                .When(icnr => icnr.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => { confirmationRequests.Add(ci.ArgAt<Action<bool>>(1)); });

            region.Add(view);
            region.Activate(view);

            var navigationUri = new Uri("", UriKind.Relative);

            var container = Substitute.For<IContainerExtension>();
            container
                .Resolve(typeof(IRegionNavigationJournalEntry))
                .Returns(new RegionNavigationJournalEntry());

            var contentLoader = Substitute.For<IRegionNavigationContentLoader>();
            contentLoader
                .LoadContent(region, Arg.Any<NavigationContext>())
                .Returns(view);

            var journal = Substitute.For<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool firstNavigation = false;
            bool secondNavigation = false;
            target.RequestNavigate(navigationUri, nr => firstNavigation = nr.Result.Value);
            target.RequestNavigate(navigationUri, nr => secondNavigation = nr.Result.Value);

            Assert.Equal(2, confirmationRequests.Count);

            confirmationRequests[0](true);
            confirmationRequests[1](true);

            Assert.False(firstNavigation);
            Assert.True(secondNavigation);
        }

        [Fact]
        public void BeforeNavigating_NavigatingEventIsRaised()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatingRaised = false;
            target.Navigating += delegate (object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatingRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            Assert.True(isNavigatingRaised);
        }

        [Fact]
        public void WhenNavigationSucceeds_NavigatedIsRaised()
        {
            // Prepare
            object view = new object();
            Uri viewUri = new Uri(view.GetType().Name, UriKind.Relative);

            IRegion region = new Region();
            region.Add(view);

            string regionName = "RegionName";
            RegionManager regionManager = new RegionManager();
            regionManager.Regions.Add(regionName, region);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            RegionNavigationContentLoader contentLoader = new RegionNavigationContentLoader(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            bool isNavigatedRaised = false;
            target.Navigated += delegate (object sender, RegionNavigationEventArgs e)
            {
                if (sender == target)
                {
                    isNavigatedRaised = true;
                }
            };

            // Act
            bool isNavigationSuccessful = false;
            target.RequestNavigate(viewUri, nr => isNavigationSuccessful = nr.Result == true);

            // Verify
            Assert.True(isNavigationSuccessful);
            Assert.True(isNavigatedRaised);
        }

        [Fact]
        public void WhenTargetViewCreationThrowsWithAsyncConfirmation_ThenExceptionIsProvidedToNavigationCallback()
        {
            var container = Substitute.For<IContainerExtension>();

            var targetException = new Exception();
            var targetHandler = Substitute.For<IRegionNavigationContentLoader>();
            targetHandler
                .LoadContent(Arg.Any<IRegion>(), Arg.Any<NavigationContext>())
                .Throws(targetException);

            var journal = Substitute.For<IRegionNavigationJournal>();

            Action<bool> navigationCallback = null;
            var view = Substitute.For<IConfirmNavigationRequest>();
            view
                .When(v => v.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => { navigationCallback = ci.ArgAt<Action<bool>>(1); });

            var region = new Region();
            region.Add(view);
            region.Activate(view);

            var target = new RegionNavigationService(container, targetHandler, journal)
            {
                Region = region
            };

            NavigationResult result = null;
            target.RequestNavigate(new Uri("", UriKind.Relative), nr => result = nr);
            navigationCallback(true);

            Assert.NotNull(result);
            Assert.Same(targetException, result.Error);
        }

        [Fact]
        public void WhenNavigatingFromViewThatIsNavigationAware_ThenNotifiesActiveViewNavigatingFrom()
        {
            // Arrange
            var region = new Region();
            var view = Substitute.For<INavigationAware>();
            region.Add(view);

            var view2 = new object();
            region.Add(view2);

            region.Activate(view);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            view.Received().OnNavigatedFrom(Arg.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0));
        }

        [Fact]
        public void WhenNavigationFromViewThatIsNavigationAware_OnlyNotifiesOnNavigateFromForActiveViews()
        {
            // Arrange

            bool navigationFromInvoked = false;

            var region = new Region();

            var view = Substitute.For<INavigationAware>();
            view
                .When(x => x.OnNavigatedFrom(Arg.Any<NavigationContext>())).Do(_ => navigationFromInvoked = true);
            region.Add(view);

            var targetView = Substitute.For<INavigationAware>();
            region.Add(targetView);

            var activeView = Substitute.For<INavigationAware>();
            region.Add(activeView);

            region.Activate(activeView);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(targetView.GetType().Name, UriKind.Relative);
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            Assert.False(navigationFromInvoked);
        }

        [StaFact]
        public void WhenNavigatingFromActiveViewWithNavigatinAwareDataConext_NotifiesContextOfNavigatingFrom()
        {
            // Arrange
            var region = new Region();

            var dataContext = Substitute.For<INavigationAware>();

            var view1 = Substitute.For<FrameworkElement>();
            view1.DataContext = dataContext;

            region.Add(view1);

            var view2 = new object();
            region.Add(view2);

            region.Activate(view1);

            IContainerExtension container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            // Act
            target.RequestNavigate(navigationUri, nr => { });

            // Verify
            dataContext.Received().OnNavigatedFrom(Arg.Is<NavigationContext>(ctx => ctx.Uri == navigationUri && ctx.Parameters.Count() == 0));
        }

        [Fact]
        public void WhenNavigatingWithNullCallback_ThenThrows()
        {
            var region = new Region();

            var navigationUri = new Uri("/", UriKind.Relative);
            IContainerExtension container = Substitute.For<IContainerExtension>();
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    target.RequestNavigate(navigationUri, null);
                });
        }

        [Fact]
        public void WhenNavigatingWithNoRegionSet_ThenMarshallExceptionToCallback()
        {
            var navigationUri = new Uri("/", UriKind.Relative);
            IContainerExtension container = Substitute.For<IContainerExtension>();
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal);

            Exception error = null;
            target.RequestNavigate(navigationUri, nr => error = nr.Error);

            Assert.NotNull(error);
            Assert.IsType<InvalidOperationException>(error);
        }

        [Fact]
        public void WhenNavigatingWithNullUri_ThenMarshallExceptionToCallback()
        {
            IContainerExtension container = Substitute.For<IContainerExtension>();
            RegionNavigationContentLoader contentLoader = Substitute.For<RegionNavigationContentLoader>(container);
            IRegionNavigationJournal journal = Substitute.For<IRegionNavigationJournal>();

            RegionNavigationService target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = new Region()
            };

            Exception error = null;
            target.RequestNavigate(null, nr => error = nr.Error);

            Assert.NotNull(error);
            Assert.IsType<ArgumentNullException>(error);
        }


        [Fact]
        public void WhenNavigationFailsBecauseTheContentViewCannotBeRetrieved_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var contentLoader = Substitute.For<IRegionNavigationContentLoader>();
            contentLoader
                .LoadContent(region, Arg.Any<NavigationContext>())
                .Throws<InvalidOperationException>();

            var journal = Substitute.For<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(new Uri("invalid", UriKind.Relative), nr => isNavigationSuccessful = nr.Result);

            // Verify
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.NotNull(eventArgs.Error);
        }

        [Fact]
        public void WhenNavigationFailsBecauseActiveViewRejectsIt_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var view1 = Substitute.For<IConfirmNavigationRequest>();
            view1
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(false));

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var contentLoader = Substitute.For<IRegionNavigationContentLoader>();
            contentLoader
                .LoadContent(region, Arg.Any<NavigationContext>())
                .Returns(view2);

            var journal = Substitute.For<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Result);

            // Verify
            view1.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }

        [StaFact]
        public void WhenNavigationFailsBecauseDataContextForActiveViewRejectsIt_ThenNavigationFailedIsRaised()
        {
            // Prepare
            var region = new Region { Name = "RegionName" };

            var viewModel1 = Substitute.For<IConfirmNavigationRequest>();
            viewModel1
                .When(ina => ina.ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>()))
                .Do(ci => ci.ArgAt<Action<bool>>(1)(false));

            var view1 = Substitute.For<FrameworkElement>();
            view1.DataContext = viewModel1;

            var view2 = new object();

            region.Add(view1);
            region.Add(view2);

            region.Activate(view1);

            var navigationUri = new Uri(view2.GetType().Name, UriKind.Relative);

            var container = Substitute.For<IContainerExtension>();
            container.Resolve(typeof(IRegionNavigationJournalEntry)).Returns(new RegionNavigationJournalEntry());

            var contentLoader = Substitute.For<IRegionNavigationContentLoader>();
            contentLoader
                .LoadContent(region, Arg.Any<NavigationContext>())
                .Returns(view2);

            var journal = Substitute.For<IRegionNavigationJournal>();

            var target = new RegionNavigationService(container, contentLoader, journal)
            {
                Region = region
            };

            RegionNavigationFailedEventArgs eventArgs = null;
            target.NavigationFailed += delegate (object sender, RegionNavigationFailedEventArgs e)
            {
                if (sender == target)
                {
                    eventArgs = e;
                }
            };

            // Act
            bool? isNavigationSuccessful = null;
            target.RequestNavigate(navigationUri, nr => isNavigationSuccessful = nr.Result);

            // Verify
            viewModel1.Received().ConfirmNavigationRequest(Arg.Any<NavigationContext>(), Arg.Any<Action<bool>>());
            Assert.False(isNavigationSuccessful.Value);
            Assert.NotNull(eventArgs);
            Assert.Null(eventArgs.Error);
        }
    }
}
