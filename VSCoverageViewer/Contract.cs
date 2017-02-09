using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Helper class for FxCop.  Indicates that a parameter is verified as being non-null.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    { }


    /// <summary>
    /// Mimics the Code Contracts pre-conditions API, but does not require the code rewrite tool.
    /// Since this does not use the rewrite tool, the post-conditions API is not available.
    /// </summary>
    static class Contract
    {
        /// <summary>
        /// Indicates a precondition on a parameter being non-null. If the parameter
        /// is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type to validate.</typeparam>
        /// <param name="value">The value to validate as being non-null.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        public static void RequiresNotNull<T>([ValidatedNotNull] T value, string paramName)
            where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Indicates a precondition on a parameter being non-null. If the parameter
        /// is null, an <see cref="ArgumentNullException"/> is thrown.
        /// </summary>
        /// <typeparam name="T">The type to validate.</typeparam>
        /// <param name="value">The value to validate as being non-null.</param>
        /// <param name="paramName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">The value is null.</exception>
        public static void RequiresNotNull<T>([ValidatedNotNull] T? value, string paramName)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }


        /// <summary>
        /// Indicates a precondition. If the condition fails, an
        /// <see cref="ArgumentException"/> is thrown.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <param name="message">The exception message.</param>
        /// <param name="paramName">The name of the parameter.</param>
        /// <exception cref="ArgumentException">If <paramref name="condition"/> is false.</exception>
        public static void Requires(bool condition, string message, string paramName)
        {
            if (!condition)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// Indicates a precondition. If the condition fails, an exception of type TException is thrown.
        /// </summary>
        /// <typeparam name="TException">The exception to throw if the condition fails.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <param name="args">The arguments to the constructor of TException to call.</param>
        public static void Requires<TException>(bool condition, params object[] args)
            where TException : Exception, new()
        {
            if (!condition)
            {
                Throw<TException>(args);
            }
        }


        /// <summary>
        /// Throws an exception of type TException.
        /// </summary>
        /// <typeparam name="TException">The exception type to throw.</typeparam>
        /// <param name="args">The arguments to the constructor of TException to call.</param>
        private static void Throw<TException>(params object[] args)
            where TException : Exception, new()
        {
            var ctor = typeof(TException).GetConstructor(Type.GetTypeArray(args));

            if (ctor != null)
            {
                throw (TException)ctor.Invoke(args);
            }

            // if not, invoke the default constructor.
            throw new TException();
        }
    }
}
