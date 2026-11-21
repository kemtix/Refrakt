

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Refrakt.Regions;
using Refrakt.Regions.Behaviors;
using Refrakt.Wpf.Tests.Mocks;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions.Behaviors
{

    public class DelayedRegionCreationBehaviorFixture
    {
        private DelayedRegionCreationBehavior GetBehavior(DependencyObject control, MockRegionManagerAccessor accessor, MockRegionAdapter adapter)
        {
            var mappings = new RegionAdapterMappings();
            mappings.RegisterMapping(control.GetType(), adapter);
            var behavior = new DelayedRegionCreationBehavior(mappings);
            behavior.RegionManagerAccessor = accessor;
            behavior.TargetElement = control;
            return behavior;
        }


        private DelayedRegionCreationBehavior GetBehavior(DependencyObject control, MockRegionManagerAccessor accessor)
        {
            return GetBehavior(control, accessor, new MockRegionAdapter());
        }

        [StaFact]
        public void RegionWillNotGetCreatedTwiceWhenThereAreMoreRegions()
        {
            var control1 = new MockFrameworkElement();
            var control2 = new MockFrameworkElement();

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => d == control1 ? "Region1" : "Region2"
            };

            var adapter = new MockRegionAdapter();
            adapter.Accessor = accessor;

            var behavior1 = this.GetBehavior(control1, accessor, adapter);
            var behavior2 = this.GetBehavior(control2, accessor, adapter);

            behavior1.Attach();
            behavior2.Attach();

            accessor.UpdateRegions();

            Assert.Contains("Region1", adapter.CreatedRegions);
            Assert.Contains("Region2", adapter.CreatedRegions);
            Assert.Equal(1, adapter.CreatedRegions.Count((name) => name == "Region2"));

        }


        [StaFact]
        public void RegionGetsCreatedWhenAccessingRegions()
        {
            var control1 = new MockFrameworkElement();
            var control2 = new MockFrameworkContentElement();

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName"
            };

            var behavior1 = this.GetBehavior(control1, accessor);
            behavior1.Attach();
            var behavior2 = this.GetBehavior(control2, accessor);
            behavior2.Attach();

            accessor.UpdateRegions();

            Assert.NotNull(RegionManager.GetObservableRegion(control1).Value);
            Assert.IsAssignableFrom<IRegion>(RegionManager.GetObservableRegion(control1).Value);
            Assert.NotNull(RegionManager.GetObservableRegion(control2).Value);
            Assert.IsAssignableFrom<IRegion>(RegionManager.GetObservableRegion(control2).Value);
        }

        [StaFact]
        public void RegionDoesNotGetCreatedTwiceWhenUpdatingRegions()
        {
            var control = new MockFrameworkElement();

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName"
            };

            var behavior = this.GetBehavior(control, accessor);
            behavior.Attach();
            accessor.UpdateRegions();
            IRegion region = RegionManager.GetObservableRegion(control).Value;

            accessor.UpdateRegions();

            Assert.Same(region, RegionManager.GetObservableRegion(control).Value);
        }

        [StaFact]
        public void BehaviorDoesNotPreventControlFromBeingGarbageCollected()
        {
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName"
            };

            WeakReference controlWeakReference = this.AttachBehaviorToNewControl(accessor);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.KeepAlive(accessor);

            Assert.False(controlWeakReference.IsAlive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference AttachBehaviorToNewControl(MockRegionManagerAccessor accessor)
        {
            var control = new MockFrameworkElement();
            WeakReference controlWeakReference = new WeakReference(control);

            var behavior = this.GetBehavior(control, accessor);
            behavior.Attach();

            Assert.True(controlWeakReference.IsAlive);
            return controlWeakReference;
        }

        [StaFact]
        public void BehaviorDoesNotPreventControlFromBeingGarbageCollectedWhenRegionWasCreated()
        {
            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName"
            };

            WeakReference controlWeakReference = this.AttachBehaviorToNewControlAndCreateRegion(accessor);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.KeepAlive(accessor);

            Assert.False(controlWeakReference.IsAlive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference AttachBehaviorToNewControlAndCreateRegion(MockRegionManagerAccessor accessor)
        {
            var control = new MockFrameworkElement();
            WeakReference controlWeakReference = new WeakReference(control);

            var behavior = this.GetBehavior(control, accessor);
            behavior.Attach();
            accessor.UpdateRegions();

            Assert.True(controlWeakReference.IsAlive);
            return controlWeakReference;
        }

        [StaFact]
        public void BehaviorShouldUnhookEventWhenDetaching()
        {
            var control = new MockFrameworkElement();

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName",
            };
            var behavior = this.GetBehavior(control, accessor);
            behavior.Attach();

            int startingCount = accessor.GetSubscribersCount();

            behavior.Detach();

            Assert.Equal<int>(startingCount - 1, accessor.GetSubscribersCount());
        }

        [StaFact]
        public void ShouldCleanupBehaviorOnceRegionIsCreated()
        {
            var control = new MockFrameworkElement();
            var control2 = new MockFrameworkContentElement();

            var accessor = new MockRegionManagerAccessor
            {
                GetRegionName = d => "myRegionName"
            };

            WeakReference behaviorWeakReference = this.AttachFirstBehaviorAndCreateRegion(control, accessor);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.False(behaviorWeakReference.IsAlive);

            WeakReference behaviorWeakReference2 = this.AttachSecondBehaviorAndCreateRegion(control2, accessor);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.KeepAlive(control);
            GC.KeepAlive(control2);
            GC.KeepAlive(accessor);

            Assert.False(behaviorWeakReference2.IsAlive);
        }

        // One helper per behavior, so that each keeps its own liveness pre-check, as before.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference AttachFirstBehaviorAndCreateRegion(DependencyObject control, MockRegionManagerAccessor accessor)
        {
            var behavior = this.GetBehavior(control, accessor);
            WeakReference behaviorWeakReference = new WeakReference(behavior);
            behavior.Attach();
            accessor.UpdateRegions();
            Assert.True(behaviorWeakReference.IsAlive);
            return behaviorWeakReference;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private WeakReference AttachSecondBehaviorAndCreateRegion(DependencyObject control2, MockRegionManagerAccessor accessor)
        {
            var behavior2 = this.GetBehavior(control2, accessor);
            WeakReference behaviorWeakReference2 = new WeakReference(behavior2);
            behavior2.Attach();
            accessor.UpdateRegions();
            Assert.True(behaviorWeakReference2.IsAlive);
            return behaviorWeakReference2;
        }
    }
}
