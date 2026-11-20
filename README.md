# Refrakt

Refrakt is a framework for building loosely coupled, maintainable, and testable XAML applications in WPF. It provides an implementation of a collection of design patterns that are helpful in writing well-structured and maintainable XAML applications, including MVVM, dependency injection, commands, EventAggregator, and others. Refrakt's core functionality is a shared code base supported in .NET Standard 2.0, .NET Framework 4.6.1 / 4.7 and .NET 5. Those things that need to be platform specific are implemented in the respective libraries for the target platform.

Refrakt is maintained by Kemtix. It is a fork of [Prism](https://github.com/PrismLibrary/Prism) 8.1 and keeps its MIT license and original copyright notice.

## Getting started

Derive your application from `RefraktApplication` (or use `RefraktBootstrapper`) from the container package of your choice, and declare the Refrakt XAML namespace in your views:

```xml
<refrakt:RefraktApplication x:Class="MyApp.App"
                            xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                            xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                            xmlns:refrakt="http://refraktlib.com/">
</refrakt:RefraktApplication>
```

Assemblies are strong-named with the Refrakt key (`refrakt.snk`, PublicKeyToken `d5a2fdbf971f0578`).

## Packages

### Core Packages

| Platform | Package |
| -------- | ------- |
| Cross Platform | Refrakt.Core |
| WPF | Refrakt.Wpf |

### Container-specific packages

Each supported IoC container has its own package assisting in the setup and usage of that container together with Refrakt. The assembly is named using this convention: Refrakt.*Container.Platform*.dll, e.g. **Refrakt.DryIoc.Wpf.dll**. Be sure to install the package for the Container and the Platform of your choice.

| Platform | DryIoc |
| -------- | ------ |
| WPF | Refrakt.DryIoc |

## Building

The solutions are at the root of the repository:

- `Refrakt.sln`: the full library
- `Refrakt_Core.sln`: the core library only
- `Refrakt_Wpf.sln`: the core and WPF libraries

## Contributing

Issues and pull requests are welcome on [kemtix/Refrakt](https://github.com/kemtix/Refrakt). See [CONTRIBUTING](.github/CONTRIBUTING.md).

## License

Refrakt is distributed under the [MIT License](LICENSE).
