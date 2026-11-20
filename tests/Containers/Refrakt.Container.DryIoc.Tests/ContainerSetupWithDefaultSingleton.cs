using DryIoc;
using Refrakt.DryIoc;
using Refrakt.Ioc.Tests;

namespace Refrakt.Ioc.DryIoc.Tests
{
    public class ContainerSetupWithDefaultSingleton : ContainerSetup
    {
        public static Rules RulesWithDefaultSingleton => DryIocContainerExtension.DefaultRules.WithDefaultReuse(Reuse.Singleton);

        protected override IContainerExtension CreateContainerInternal() => new DryIocContainerExtension(new Container(RulesWithDefaultSingleton));
    }
}
