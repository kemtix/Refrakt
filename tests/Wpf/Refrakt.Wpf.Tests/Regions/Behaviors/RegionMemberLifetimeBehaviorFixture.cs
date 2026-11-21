

using NSubstitute;
using Refrakt.Regions;
using Refrakt.Regions.Behaviors;
using Refrakt.Wpf.Tests.Mocks;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions.Behaviors
{

    public class RegionMemberLifetimeBehaviorFixture
    {
        protected Region Region { get; set; }
        protected RegionMemberLifetimeBehavior Behavior { get; set; }

        public RegionMemberLifetimeBehaviorFixture()
        {
            Arrange();
        }

        protected virtual void Arrange()
        {
            this.Region = new Region();
            this.Behavior = new RegionMemberLifetimeBehavior();
            this.Behavior.Region = this.Region;
            this.Behavior.Attach();
        }

        [Fact]
        public void WhenBehaviorAttachedThenReportsIsAttached()
        {
            Assert.True(Behavior.IsAttached);
        }

        [Fact]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveFalseRemovesWhenInactive()
        {
            // Arrange
            var regionItemMock = Substitute.For<IRegionMemberLifetime>();
            regionItemMock.KeepAlive.Returns(false);

            Region.Add(regionItemMock);
            Region.Activate(regionItemMock);

            // Act
            Region.Deactivate(regionItemMock);

            // Assert
            Assert.False(Region.Views.Contains(regionItemMock));
        }

        [Fact]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveTrueDoesNotRemoveOnDeactivation()
        {
            // Arrange
            var regionItemMock = Substitute.For<IRegionMemberLifetime>();
            regionItemMock.KeepAlive.Returns(true);

            Region.Add(regionItemMock);
            Region.Activate(regionItemMock);

            // Act
            Region.Deactivate(regionItemMock);

            // Assert
            Assert.True(Region.Views.Contains(regionItemMock));

        }

        [Fact]
        public void WhenIRegionMemberLifetimeItemReturnsKeepAliveFalseCanRemoveFromRegion()
        {
            // Arrange
            var regionItemMock = Substitute.For<IRegionMemberLifetime>();
            regionItemMock.KeepAlive.Returns(false);

            var view = regionItemMock;

            Region.Add(view);
            Region.Activate(view);

            // The presence of the following two lines is essential for the test:
            // we want to access both ActiveView and Views in that order
            Assert.True(Region.ActiveViews.Contains(view));
            Assert.True(Region.Views.Contains(view));

            // Act
            // This may throw
            Region.Remove(view);

            // Assert
            Assert.False(Region.Views.Contains(view));
            Assert.False(Region.ActiveViews.Contains(view));
        }

        [Fact]
        public void WhenRegionContainsMultipleMembers_OnlyRemovesThoseDeactivated()
        {
            // Arrange
            var firstMockItem = Substitute.For<IRegionMemberLifetime>();
            firstMockItem.KeepAlive.Returns(true);

            var secondMockItem = Substitute.For<IRegionMemberLifetime>();
            secondMockItem.KeepAlive.Returns(false);

            Region.Add(firstMockItem);
            Region.Activate(firstMockItem);

            Region.Add(secondMockItem);
            Region.Activate(secondMockItem);

            // Act
            Region.Deactivate(secondMockItem);

            // Assert
            Assert.True(Region.Views.Contains(firstMockItem));
            Assert.False(Region.Views.Contains(secondMockItem));
        }

        [Fact]
        public void WhenMemberNeverActivatedThenIsNotRemovedOnAnothersDeactivation()
        {
            // Arrange
            var firstMockItem = Substitute.For<IRegionMemberLifetime>();
            firstMockItem.KeepAlive.Returns(false);

            var secondMockItem = Substitute.For<IRegionMemberLifetime>();
            secondMockItem.KeepAlive.Returns(false);

            Region.Add(firstMockItem);  // Never activated

            Region.Add(secondMockItem);
            Region.Activate(secondMockItem);

            // Act
            Region.Deactivate(secondMockItem);

            // Assert
            Assert.True(Region.Views.Contains(firstMockItem));
            Assert.False(Region.Views.Contains(secondMockItem));
        }

        [StaFact]
        public virtual void RemovesRegionItemIfDataContextReturnsKeepAliveFalse()
        {
            // Arrange
            var regionItemMock = Substitute.For<IRegionMemberLifetime>();
            regionItemMock.KeepAlive.Returns(false);

            var regionItem = new MockFrameworkElement();
            regionItem.DataContext = regionItemMock;

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.False(Region.Views.Contains(regionItem));
        }

        [StaFact]
        public virtual void RemovesOnlyDeactivatedItemsInRegionBasedOnDataContextKeepAlive()
        {
            // Arrange
            var retionItemDataContextToKeepAlive = Substitute.For<IRegionMemberLifetime>();
            retionItemDataContextToKeepAlive.KeepAlive.Returns(true);

            var regionItemToKeepAlive = new MockFrameworkElement();
            regionItemToKeepAlive.DataContext = retionItemDataContextToKeepAlive;
            Region.Add(regionItemToKeepAlive);
            Region.Activate(regionItemToKeepAlive);

            var regionItemMock = Substitute.For<IRegionMemberLifetime>();
            regionItemMock.KeepAlive.Returns(false);

            var regionItem = new MockFrameworkElement();
            regionItem.DataContext = regionItemMock;

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.False(Region.Views.Contains(regionItem));
            Assert.True(Region.Views.Contains(regionItemToKeepAlive));
        }

        [Fact]
        public virtual void WillRemoveDeactivatedItemIfKeepAliveAttributeFalse()
        {
            // Arrange
            var regionItem = new RegionMemberNotKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.False(Region.Views.Contains((object)regionItem));
        }

        [Fact]
        public virtual void WillNotRemoveDeactivatedItemIfKeepAliveAttributeTrue()
        {
            // Arrange
            var regionItem = new RegionMemberKeptAlive();

            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.True(Region.Views.Contains((object)regionItem));
        }

        [StaFact]
        public virtual void WillRemoveDeactivatedItemIfDataContextKeepAliveAttributeFalse()
        {
            // Arrange
            var regionItemDataContext = new RegionMemberNotKeptAlive();
            var regionItem = new MockFrameworkElement() { DataContext = regionItemDataContext };
            Region.Add(regionItem);
            Region.Activate(regionItem);

            // Act
            Region.Deactivate(regionItem);

            // Assert
            Assert.False(Region.Views.Contains(regionItem));
        }

        [RegionMemberLifetime(KeepAlive = false)]
        public class RegionMemberNotKeptAlive
        {
        }

        [RegionMemberLifetime(KeepAlive = true)]
        public class RegionMemberKeptAlive
        {
        }


    }


    public class RegionMemberLifetimeBehaviorAgainstSingleActiveRegionFixture
                : RegionMemberLifetimeBehaviorFixture
    {
        protected override void Arrange()
        {
            this.Region = new SingleActiveRegion();
            this.Behavior = new RegionMemberLifetimeBehavior();
            this.Behavior.Region = this.Region;
            this.Behavior.Attach();
        }
    }
}
