

using System;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using Refrakt.Regions;
using Refrakt.Regions.Behaviors;
using Xunit;

namespace Refrakt.Wpf.Tests.Regions
{

    public class SelectorRegionAdapterFixture
    {
        [StaFact]
        public void AdapterAddsSelectorItemsSourceSyncBehavior()
        {
            var control = new ListBox();
            IRegionAdapter adapter = new TestableSelectorRegionAdapter();

            IRegion region = adapter.Initialize(control, "Region1");
            Assert.NotNull(region);

            Assert.IsType<SelectorItemsSourceSyncBehavior>(region.Behaviors["SelectorItemsSourceSyncBehavior"]);

        }


        [StaFact]
        public void AdapterDoesNotPreventRegionFromBeingGarbageCollected()
        {
            object model = new object();
            IRegionAdapter adapter = new SelectorRegionAdapter(null);

            (WeakReference regionWeakReference, WeakReference controlWeakReference) = CreateRegionWithModel(adapter, model);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.KeepAlive(adapter);
            GC.KeepAlive(model);

            Assert.False(regionWeakReference.IsAlive);
            Assert.False(controlWeakReference.IsAlive);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static (WeakReference Region, WeakReference Control) CreateRegionWithModel(IRegionAdapter adapter, object model)
        {
            var selector = new ListBox();
            var region = adapter.Initialize(selector, "Region1");
            region.Add(model);

            WeakReference regionWeakReference = new WeakReference(region);
            WeakReference controlWeakReference = new WeakReference(selector);
            Assert.True(regionWeakReference.IsAlive);
            Assert.True(controlWeakReference.IsAlive);
            return (regionWeakReference, controlWeakReference);
        }

        [StaFact]
        public void ActivatingTheViewShouldUpdateTheSelectedItem()
        {
            var selector = new ListBox();
            var view1 = new object();
            var view2 = new object();

            IRegionAdapter adapter = new SelectorRegionAdapter(null);

            var region = adapter.Initialize(selector, "Region1");
            region.Add(view1);
            region.Add(view2);

            Assert.NotEqual(view1, selector.SelectedItem);

            region.Activate(view1);

            Assert.Equal(view1, selector.SelectedItem);

            region.Activate(view2);

            Assert.Equal(view2, selector.SelectedItem);
        }

        [StaFact]
        public void DeactivatingTheSelectedViewShouldUpdateTheSelectedItem()
        {
            var selector = new ListBox();
            var view1 = new object();
            IRegionAdapter adapter = new SelectorRegionAdapter(null);
            var region = adapter.Initialize(selector, "Region1");
            region.Add(view1);

            region.Activate(view1);

            Assert.Equal(view1, selector.SelectedItem);

            region.Deactivate(view1);

            Assert.NotEqual(view1, selector.SelectedItem);
        }


        private class TestableSelectorRegionAdapter : SelectorRegionAdapter
        {
            public TestableSelectorRegionAdapter()
                : base(null)
            {
            }


            protected override IRegion CreateRegion()
            {
                return new Region();
            }
        }
    }
}
