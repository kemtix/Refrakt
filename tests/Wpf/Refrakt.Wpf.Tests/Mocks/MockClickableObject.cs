

using System.Windows.Controls.Primitives;

namespace Refrakt.Wpf.Tests.Mocks
{
    internal class MockClickableObject : ButtonBase
    {
        public void RaiseClick()
        {
            OnClick();
        }
    }
}
