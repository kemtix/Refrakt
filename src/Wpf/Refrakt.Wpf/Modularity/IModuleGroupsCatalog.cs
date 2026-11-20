using System.Collections.ObjectModel;

namespace Refrakt.Modularity
{
    public interface IModuleGroupsCatalog
    {
        Collection<IModuleCatalogItem> Items { get; }
    }
}
