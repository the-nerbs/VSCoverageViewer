using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using VSCoverageViewer.Models;
using VSCoverageViewer.Properties;
using VSCoverageViewer.Serialization.Export;

namespace VSCoverageViewer.Report
{
    /// <summary>
    /// Writes a coverage report in the format of a single web page.
    /// </summary>
    /// <remarks>
    /// The reports created by this writer may require an internet connection to obtain jQuery.
    /// It is possible that Windows (I think?) will have it's own cached copy, and if so,
    /// the reports might open without the need to download it.
    /// </remarks>
    internal class HtmlSingleFileReportWriter : ReportWriter
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlSingleFileReportWriter"/>.
        /// </summary>
        public HtmlSingleFileReportWriter()
            : base(ReportFormat.HtmlSingleFile)
        { }


        /// <inheritdoc />
        public override void WriteReport(CoverageExport dataset, ReportConfigurationModel configuration)
        {
            using (var tempFile = WriteToTempFile(dataset))
            {
                var transform = new XslCompiledTransform();
                using (var xslReader = XmlReader.Create(new StringReader(Resources.HTMLTransform)))
                {
                    transform.Load(xslReader);
                }

                var args = new XsltArgumentList();

                args.AddParam(ReportConstants.HtmlGenDate, "",
                    DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"));

                args.AddParam(ReportConstants.HtmlTotalLines, "",
                    dataset.LinesCovered + dataset.LinesPartiallyCovered + dataset.LinesNotCovered);

                args.AddParam(ReportConstants.HtmlTotalBlocks, "",
                    dataset.BlocksCovered + dataset.BlocksNotCovered);

                args.AddParam(ReportConstants.HtmlExpansionDepth, "",
                    (int)configuration.DefaultExpansion);

                args.AddParam("jQuerySource", "",
                    ReportConstants.JQueryCDNLocation);


                using (var outputFile = new FileStream(configuration.DestinationPath, FileMode.Create, FileAccess.Write))
                {
                    using (var tempReader = XmlReader.Create(tempFile.Stream))
                    {
                        transform.Transform(tempReader, args, outputFile);
                    }
                }
            }
        }
    }
}
