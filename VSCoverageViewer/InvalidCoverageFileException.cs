using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Exception thrown when a coverage file is found to be invalid.
    /// </summary>
    [Serializable]
    internal class InvalidCoverageFileException : Exception
    {
        public InvalidCoverageFileException()
        { }

        public InvalidCoverageFileException(string message)
            : base(message)
        { }

        public InvalidCoverageFileException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected InvalidCoverageFileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
