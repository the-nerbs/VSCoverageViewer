using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Report
{
    /// <summary>
    /// A simple disposable wrapper around a temporary file.
    /// </summary>
    internal class TempFile : IDisposable
    {
        private readonly string _path;
        private readonly Stream _stream;
        private bool _isDisposed = false;


        /// <summary>
        /// Gets a read/write stream to the temp file.
        /// </summary>
        public Stream Stream
        {
            get { return _stream; }
        }


        /// <summary>
        /// Creates a new temporary file.
        /// </summary>
        public TempFile()
        {
            _path = Path.GetTempFileName();
            _stream = new FileStream(_path, FileMode.Create, FileAccess.ReadWrite);
        }


        /// <summary>
        /// Resets the stream back to the beginning of the file.
        /// </summary>
        public void ResetPosition()
        {
            Debug.Assert(!_isDisposed);
            Stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// Disposes the stream and attempts to delete the temp file.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed && _stream != null)
            {
                _stream.Dispose();

                try
                {
                    File.Delete(_path);
                }
                catch
                {
                    // ignore - there's nothing that can really be done here.
                }

                _isDisposed = true;
            }
        }
    }
}
