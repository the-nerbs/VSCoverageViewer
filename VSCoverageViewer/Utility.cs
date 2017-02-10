using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// General utilities.
    /// </summary>
    internal static class Utility
    {
        /// <summary>
        /// Used to indicate unreachable sections of code.
        /// </summary>
        /// <param name="message">A message describing why the code is unreachable.</param>
        /// <returns>An exception object.</returns>
        /// <example>
        /// This method is intended to be used like so.
        /// <code lang="C#">
        /// public bool ExampleMethod(int value)
        /// {
        ///     Debug.Assert(0 &lt;= value && value &lt;= 3, "value must be between 0 and 3.");
        ///     switch (value)
        ///     {
        ///     case 0:
        ///     case 1:
        ///         return true;
        ///
        ///     case 2:
        ///     case 3:
        ///         return false;
        ///
        ///     default:
        ///         throw Utilities.UnreachableCode("unexpected value");
        ///     }
        /// }
        /// </code>
        /// </example>
        [ExcludeFromCodeCoverage]   // should _never_ be hit.
        internal static Exception UnreachableCode([Localizable(false)] string message)
        {
            Debug.Fail(message);
            return new InternalFailureException(message);
        }


        /// <summary>
        /// Private exception type thrown for internal failures.
        /// </summary>
        [Serializable]
        private sealed class InternalFailureException : Exception
        {
            public InternalFailureException()
            { }

            public InternalFailureException(string message)
                : base(message)
            { }

            public InternalFailureException(string message, Exception inner)
                : base(message, inner)
            { }

            private InternalFailureException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            { }
        }
    }
}
