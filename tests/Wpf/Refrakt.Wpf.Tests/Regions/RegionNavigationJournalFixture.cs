

using System;
using NSubstitute;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class RegionNavigationJournalFixture
    {
        [Fact]
        public void ConstructingJournalInitializesValues()
        {
            // Act
            RegionNavigationJournal target = new RegionNavigationJournal();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Null(target.CurrentEntry);
            Assert.Null(target.NavigationTarget);
        }

        [Fact]
        public void SettingNavigationServiceUpdatesValue()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockINavigate = Substitute.For<INavigateAsync>();

            // Act
            target.NavigationTarget = mockINavigate;

            // Verify
            Assert.Same(mockINavigate, target.NavigationTarget);
        }

        [Fact]
        public void RecordingNavigationUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri = new Uri("Uri", UriKind.Relative);
            RegionNavigationJournalEntry entry = new RegionNavigationJournalEntry() { Uri = uri };

            // Act
            target.RecordNavigation(entry, true);

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry, target.CurrentEntry);
        }

        [Fact]
        public void RecordingNavigationMultipleTimesUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            // Act
            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);
        }

        [Fact]
        public void ClearUpdatesNavigationState()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            // Act
            target.Clear();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Null(target.CurrentEntry);
        }

        [Fact]
        public void GoBackNavigatesBack()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.DidNotReceive().RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void GoBackDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, false)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            // Act
            target.GoBack();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);

            mockNavigationTarget.DidNotReceive().RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void GoBackMultipleTimesNavigatesBack()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);


            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            // Act
            target.GoBack();
            target.GoBack();

            // Verify
            Assert.False(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry1, target.CurrentEntry);

            mockNavigationTarget.Received(1).RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void GoForwardNavigatesForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.Received(1).RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(2).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void GoForwardDoesNotChangeStateWhenNavigationFails()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, false)));

            target.GoBack();

            // Act
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.DidNotReceive().RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void GoForwardMultipleTimesNavigatesForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, true);

            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            target.GoBack();
            target.GoBack();

            // Act
            target.GoForward();
            target.GoForward();

            // Verify
            Assert.True(target.CanGoBack);
            Assert.False(target.CanGoForward);
            Assert.Same(entry3, target.CurrentEntry);

            mockNavigationTarget.Received(1).RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(2).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
        }

        [Fact]
        public void WhenNavigationToNewUri_ThenCanNoLongerNavigateForward()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            Uri uri4 = new Uri("Uri4", UriKind.Relative);
            RegionNavigationJournalEntry entry4 = new RegionNavigationJournalEntry() { Uri = uri4 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);

            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            target.GoBack();

            Assert.True(target.CanGoForward);

            // Act
            target.RecordNavigation(entry3, true);

            // Verify
            Assert.False(target.CanGoForward);
            Assert.Equal(entry3, target.CurrentEntry);
        }

        [Fact]
        public void WhenSavePreviousFalseDoNotRecordEntry()
        {
            // Prepare
            RegionNavigationJournal target = new RegionNavigationJournal();

            INavigateAsync mockNavigationTarget = Substitute.For<INavigateAsync>();
            target.NavigationTarget = mockNavigationTarget;

            Uri uri1 = new Uri("Uri1", UriKind.Relative);
            RegionNavigationJournalEntry entry1 = new RegionNavigationJournalEntry() { Uri = uri1 };

            Uri uri2 = new Uri("Uri2", UriKind.Relative);
            RegionNavigationJournalEntry entry2 = new RegionNavigationJournalEntry() { Uri = uri2 };

            Uri uri3 = new Uri("Uri3", UriKind.Relative);
            RegionNavigationJournalEntry entry3 = new RegionNavigationJournalEntry() { Uri = uri3 };

            Uri uri4 = new Uri("Uri4", UriKind.Relative);
            RegionNavigationJournalEntry entry4 = new RegionNavigationJournalEntry() { Uri = uri4 };

            target.RecordNavigation(entry1, true);
            target.RecordNavigation(entry2, true);
            target.RecordNavigation(entry3, false);
            target.RecordNavigation(entry4, true);

            mockNavigationTarget
                .When(x => x.RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));
            mockNavigationTarget
                .When(x => x.RequestNavigate(uri4, Arg.Any<Action<NavigationResult>>(), null))
                .Do(ci => ci.ArgAt<Action<NavigationResult>>(1)(new NavigationResult(null, true)));

            Assert.Equal(entry4, target.CurrentEntry);

            target.GoBack();

            Assert.True(target.CanGoBack);
            Assert.True(target.CanGoForward);
            Assert.Same(entry2, target.CurrentEntry);

            mockNavigationTarget.DidNotReceive().RequestNavigate(uri1, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.Received(1).RequestNavigate(uri2, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri3, Arg.Any<Action<NavigationResult>>(), null);
            mockNavigationTarget.DidNotReceive().RequestNavigate(uri4, Arg.Any<Action<NavigationResult>>(), null);
        }
    }
}
