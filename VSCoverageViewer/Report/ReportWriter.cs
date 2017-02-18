using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VSCoverageViewer.Models;
using VSCoverageViewer.Serialization;
using VSCoverageViewer.Serialization.Export;

namespace VSCoverageViewer.Report
{
    /// <summary>
    /// Base class for classes which create reports of coverage data. 
    /// </summary>
    internal abstract class ReportWriter
    {
        /// <summary>
        /// Gets the format this instance's reports are created in.
        /// </summary>
        public ReportFormat Format { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="ReportWriter"/>.
        /// </summary>
        /// <param name="format">The format of reports created by this instance.</param>
        protected ReportWriter(ReportFormat format)
        {
            Format = format;
        }


        /// <summary>
        /// Writes a report containing the given coverage data.
        /// </summary>
        /// <param name="dataset">The coverage dataset to write to the report.</param>
        /// <param name="configuration">The report configuration.</param>
        public abstract void WriteReport(CoverageExport dataset, ReportConfigurationModel configuration);


        /// <summary>
        /// Creates a temporary file containing the given coverage data.
        /// </summary>
        /// <param name="dataset">The data to write to the temporary file.</param>
        /// <returns>A disposable wrapper for the temporary file.</returns>
        protected static TempFile WriteToTempFile(CoverageExport dataset)
        {
            var tempFile = new TempFile();

            var ser = new XmlSerializer(typeof(CoverageExport));

            ser.Serialize(tempFile.Stream, dataset);

            tempFile.ResetPosition();

            return tempFile;
        }
    }
}
