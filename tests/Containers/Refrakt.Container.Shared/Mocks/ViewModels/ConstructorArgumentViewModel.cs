using Refrakt.Ioc.Mocks.Services;
using Refrakt.Mvvm;

namespace Refrakt.Ioc.Mocks.ViewModels
{
    public class ConstructorArgumentViewModel : BindableBase
    {
        public IServiceA Service { get; }

        public ConstructorArgumentViewModel(IServiceA service)
        {
            Service = service;
        }
    }
}
