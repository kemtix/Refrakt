using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

[assembly: CLSCompliant(true)]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//In order to begin building localizable applications, set 
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page, 
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page, 
                                              // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsDefinition("http://refraktlib.com/", "Refrakt.Unity")]
[assembly: InternalsVisibleTo("Refrakt.Unity.Wpf.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100edb04e820c45bb7b020592f9cfc6dc10e903ff3c55a44d324ce6a148c96fdeb320637ee501be63948bbbdad13b001f9adfe4c5fba95202c7658efba8cfab00780686fe2e659e169f35cff033842a1281d8a135e7aa9a288f994830a64d83bfaa78386ce976da3c96cab0c985549ca00a3e9583e31ebc966d858132511475d0b8")]
