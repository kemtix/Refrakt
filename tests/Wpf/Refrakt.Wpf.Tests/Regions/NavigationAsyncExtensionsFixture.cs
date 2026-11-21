

using System;
using NSubstitute;
using Refrakt.Regions;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class NavigationAsyncExtensionsFixture
    {
        [Fact]
        public void WhenNavigatingWithANullThis_ThenThrows()
        {
            INavigateAsync navigate = null;
            string target = "";

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [Fact]
        public void WhenNavigatingWithANullStringTarget_ThenThrows()
        {
            INavigateAsync navigate = Substitute.For<INavigateAsync>();
            string target = null;

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [Fact]
        public void WhenNavigatingWithARelativeStringTarget_ThenNavigatesToRelativeUri()
        {
            var navigateMock = Substitute.For<INavigateAsync>();

            string target = "relative";

            navigateMock.RequestNavigate(target);

            navigateMock.Received().RequestNavigate(
                Arg.Is<Uri>(u => !u.IsAbsoluteUri && u.OriginalString == "relative"),
                Arg.Is<Action<NavigationResult>>(c => c != null));
        }

        [Fact]
        public void WhenNavigatingWithAnAbsoluteStringTarget_ThenNavigatesToAbsoluteUri()
        {
            var navigateMock = Substitute.For<INavigateAsync>();

            string target = "http://test/path";

            navigateMock.RequestNavigate(target);

            navigateMock.Received().RequestNavigate(
                Arg.Is<Uri>(u => u.IsAbsoluteUri && u.Host == "test" && u.AbsolutePath == "/path"),
                Arg.Is<Action<NavigationResult>>(c => c != null));
        }

        [Fact]
        public void WhenNavigatingWithANullThisAndAUri_ThenThrows()
        {
            INavigateAsync navigate = null;
            Uri target = new Uri("test", UriKind.Relative);

            ExceptionAssert.Throws<ArgumentNullException>(
                () =>
                {
                    navigate.RequestNavigate(target);
                });
        }

        [Fact]
        public void WhenNavigatingWithAUri_ThenNavigatesToUriWithCallback()
        {
            Uri target = new Uri("relative", UriKind.Relative);

            var navigateMock = Substitute.For<INavigateAsync>();

            navigateMock.RequestNavigate(target);

            navigateMock.Received().RequestNavigate(
                target,
                Arg.Is<Action<NavigationResult>>(c => c != null));
        }
    }
}
