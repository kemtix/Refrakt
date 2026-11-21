using System;
using System.Linq;
using NSubstitute;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class NavigationContextFixture
    {
        [Fact]
        public void WhenCreatingANewContextForAUriWithAQuery_ThenNewContextInitializesPropertiesAndExtractsTheQuery()
        {
            var uri = new Uri("test?name=value", UriKind.Relative);

            var navigationJournalMock = Substitute.For<IRegionNavigationJournal>();
            var navigationServiceMock = Substitute.For<IRegionNavigationService>();

            IRegion region = new Region();
            navigationServiceMock.Region.Returns(region);
            navigationServiceMock.Journal.Returns(navigationJournalMock);

            var context = new NavigationContext(navigationServiceMock, uri);

            Assert.Same(navigationServiceMock, context.NavigationService);
            Assert.Equal(uri, context.Uri);
            Assert.Single(context.Parameters);
            Assert.Equal("value", context.Parameters["name"]);
        }

        [Fact]
        public void WhenCreatingANewContextForAUriWithNoQuery_ThenNewContextInitializesPropertiesGetsEmptyQuery()
        {
            var uri = new Uri("test", UriKind.Relative);

            var navigationJournalMock = Substitute.For<IRegionNavigationJournal>();

            var navigationServiceMock = Substitute.For<IRegionNavigationService>();
            navigationServiceMock.Journal.Returns(navigationJournalMock);

            var context = new NavigationContext(navigationServiceMock, uri);

            Assert.Same(navigationServiceMock, context.NavigationService);
            Assert.Equal(uri, context.Uri);
            Assert.Empty(context.Parameters);
        }
    }
}
