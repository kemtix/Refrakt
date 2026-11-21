using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Refrakt.Mvvm;

namespace Refrakt.Tests.Mvvm
{
    /// <summary>
    /// Puts <see cref="ViewModelLocationProvider"/> back in its process start state by reflection.
    /// Compiled into Refrakt.Core.Tests and linked into Refrakt.Wpf.Tests.
    /// </summary>
    internal static class ViewModelLocationProviderReset
    {
        private const BindingFlags StaticNonPublic = BindingFlags.NonPublic | BindingFlags.Static;

        private static readonly string[] DictionaryFieldNames =
        {
            "_factories",
            "_typeFactories",
        };

        private static readonly string[] DelegateFieldNames =
        {
            "_defaultViewModelFactory",
            "_defaultViewModelFactoryWithViewParameter",
            "_defaultViewTypeToViewModelTypeResolver",
        };

        private static object[] _startupDelegates;
        private static string _missingFieldName;

#pragma warning disable CA2255 // Test assembly, not a library: the snapshot must be taken before any test runs.
        [ModuleInitializer]
        internal static void SnapshotStartupState()
#pragma warning restore CA2255
        {
            foreach (var name in DictionaryFieldNames)
            {
                if (FindStaticField(name) == null)
                {
                    _missingFieldName = name;
                    return;
                }
            }

            var startupDelegates = new object[DelegateFieldNames.Length];
            for (var i = 0; i < DelegateFieldNames.Length; i++)
            {
                var field = FindStaticField(DelegateFieldNames[i]);
                if (field == null)
                {
                    _missingFieldName = DelegateFieldNames[i];
                    return;
                }

                // Reading a static field runs the class's static initializer first, which gives the startup values.
                startupDelegates[i] = field.GetValue(null);
            }

            _startupDelegates = startupDelegates;
        }

        internal static void Reset()
        {
            if (_missingFieldName != null)
                throw MissingField(_missingFieldName);

            foreach (var name in DictionaryFieldNames)
                ((IDictionary)GetStaticField(name).GetValue(null)).Clear();

            for (var i = 0; i < DelegateFieldNames.Length; i++)
                GetStaticField(DelegateFieldNames[i]).SetValue(null, _startupDelegates[i]);
        }

        internal static FieldInfo GetStaticField(string name)
        {
            return FindStaticField(name) ?? throw MissingField(name);
        }

        private static FieldInfo FindStaticField(string name)
        {
            return typeof(ViewModelLocationProvider).GetField(name, StaticNonPublic);
        }

        private static InvalidOperationException MissingField(string name)
        {
            return new InvalidOperationException($"ViewModelLocationProvider has no static field '{name}'; update ViewModelLocationProviderReset.");
        }
    }
}
