using System;
using Refrakt.Ioc;
using Unity;

namespace Refrakt.Unity
{
    /// <summary>
    /// Base application class that uses <see cref="UnityContainerExtension"/> as it's container.
    /// </summary>
    public abstract class RefraktApplication : RefraktApplicationBase
    {
        /// <summary>
        /// Create a new <see cref="UnityContainerExtension"/> used by Refrakt.
        /// </summary>
        /// <returns>A new <see cref="UnityContainerExtension"/>.</returns>
        protected override IContainerExtension CreateContainerExtension()
        {
            return new UnityContainerExtension();
        }

        /// <summary>
        /// Registers the <see cref="Type"/>s of the Exceptions that are not considered 
        /// root exceptions by the <see cref="ExceptionExtensions"/>.
        /// </summary>
        protected override void RegisterFrameworkExceptionTypes()
        {
            ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ResolutionFailedException));
        }
    }
}
