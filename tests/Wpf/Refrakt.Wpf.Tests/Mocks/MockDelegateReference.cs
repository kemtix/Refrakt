

using System;
using Refrakt.Events;

namespace Refrakt.Wpf.Tests.Mocks
{
    class MockDelegateReference : IDelegateReference
    {
        public Delegate Target { get; set; }

        public MockDelegateReference()
        {

        }

        public MockDelegateReference(Delegate target)
        {
            Target = target;
        }
    }
}
