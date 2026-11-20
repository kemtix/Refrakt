

using System.Windows;
using Refrakt.Regions;
using Refrakt.Regions.Behaviors;

namespace Refrakt.Wpf.Tests.Mocks
{
    public class MockHostAwareRegionBehavior : IHostAwareRegionBehavior
    {
        public IRegion Region { get; set; }

        public void Attach()
        {

        }

        public DependencyObject HostControl { get; set; }
    }
}
