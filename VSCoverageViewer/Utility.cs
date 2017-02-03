using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    internal static class Utility
    {
        [ExcludeFromCodeCoverage]   // should _never_ be hit.
        internal static Exception UnreachableCode(string message)
        {
            Debug.Fail(message);
            return new InternalFailureException(message);
        }


        [Serializable]
        private class InternalFailureException : Exception
        {
            public InternalFailureException()
            { }

            public InternalFailureException(string message)
                : base(message)
            { }

            public InternalFailureException(string message, Exception inner)
                : base(message, inner)
            { }

            protected InternalFailureException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            { }
        }
    }
}
