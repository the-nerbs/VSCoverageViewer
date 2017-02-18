using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer
{
    /// <summary>
    /// Application constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Path to a directory used to store data files.
        /// </summary>
        public static readonly string DataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Nerbs",
            "VSCoverageViewer"
        );
    }
}
