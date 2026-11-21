using System;
using System.Collections.Generic;
using System.Text;
using NSubstitute;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class RegionManagerRequestNavigateFixture
    {
        const string region = "Region";
        const string nonExistentRegion = "NonExistentRegion";
        const string source = "Source";

        private static Uri sourceUri = new Uri(source, UriKind.RelativeOrAbsolute);
        private static NavigationParameters parameters = new NavigationParameters();
        private static Action<NavigationResult> callback = (_) => { };

        private static IRegion mockRegion;
        private static RegionManager regionManager;

        public RegionManagerRequestNavigateFixture()
        {
            mockRegion = Substitute.For<IRegion>();
            mockRegion.Name.Returns(region);

            regionManager = new RegionManager();
            regionManager.Regions.Add(mockRegion);
        }

        [Fact]
        public void ThrowsWhenNavigationCallbackIsNull()
        {
            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, source, null, parameters)
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, source, navigationCallback: null)
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, sourceUri, null, parameters)
            );

            ExceptionAssert.Throws<ArgumentNullException>(() =>
                regionManager.RequestNavigate(region, sourceUri, navigationCallback: null)
            );
        }

        [Fact]
        public void WhenNonExistentRegion_ReturnNavigationResultFalse()
        {
            NavigationResult result;

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, source, (r) => result = r, parameters);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, source, (r) => result = r);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, sourceUri, (r) => result = r, parameters);
            Assert.Equal(false, result.Result);

            result = null;
            regionManager.RequestNavigate(nonExistentRegion, sourceUri, (r) => result = r);
            Assert.Equal(false, result.Result);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSource()
        {
            regionManager.RequestNavigate(region, source);
            mockRegion.Received().RequestNavigate(sourceUri, Arg.Any<Action<NavigationResult>>());
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTarget()
        {
            regionManager.RequestNavigate(region, sourceUri);
            mockRegion.Received().RequestNavigate(sourceUri, Arg.Any<Action<NavigationResult>>());
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceParameters()
        {
            regionManager.RequestNavigate(region, source, parameters);
            mockRegion.Received().RequestNavigate(sourceUri, Arg.Any<Action<NavigationResult>>(), parameters);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, parameters);
            mockRegion.Received().RequestNavigate(sourceUri, Arg.Any<Action<NavigationResult>>(), parameters);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallback()
        {
            regionManager.RequestNavigate(region, source, callback);
            mockRegion.Received().RequestNavigate(sourceUri, callback);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionTargetCallback()
        {
            regionManager.RequestNavigate(region, sourceUri, callback);
            mockRegion.Received().RequestNavigate(sourceUri, callback);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceCallbackParameters()
        {
            regionManager.RequestNavigate(region, source, callback, parameters);
            mockRegion.Received().RequestNavigate(sourceUri, callback, parameters);
        }

        [Fact]
        public void DelegatesCallToRegion_RegionSourceUriCallbackParameters()
        {
            regionManager.RequestNavigate(region, sourceUri, callback, parameters);
            mockRegion.Received().RequestNavigate(sourceUri, callback, parameters);
        }
    }
}
