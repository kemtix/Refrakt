using Refrakt.Common;

namespace Refrakt.Tests.Common.Mocks
{
    internal class MockParameters : ParametersBase
    {
        public MockParameters() : base() { }
        public MockParameters(string query) : base(query) { }
    }
}
