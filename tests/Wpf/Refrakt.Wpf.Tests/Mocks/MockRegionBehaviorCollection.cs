

using System.Collections;
using System.Collections.Generic;
using Refrakt.Regions;

namespace Refrakt.Wpf.Tests.Mocks
{
    internal class MockRegionBehaviorCollection : Dictionary<string, IRegionBehavior>, IRegionBehaviorCollection
    {
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
