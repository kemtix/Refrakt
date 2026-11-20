using System.Windows;
using Refrakt.Mvvm;

namespace Refrakt.Wpf.Tests.Mocks.Views
{
    public class MockOptOut : FrameworkElement
    {
        public MockOptOut()
        {
            ViewModelLocator.SetAutoWireViewModel(this, false);
        }
    }
}
