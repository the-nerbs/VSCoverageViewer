using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
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
    /// Writes a coverage report in the format of a web page and a supporting files folder.
    /// </summary>
    /// <remarks>
    /// The reports created by this writer do not require an internet connection to obtain jQuery.
    /// However, the writer itself may require a connection to get jQuery if it is not already cached.
    /// </remarks>
    internal class HtmlMultiFileReportWriter : ReportWriter
    {
        /// <summary>
        /// The path to the cached jQuery file.
        /// </summary>
        private static string JQueryCachedPath
        {
            get { return Path.Combine(Constants.DataDirectory, ReportConstants.JQueryFileName); }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="HtmlMultiFileReportWriter"/>.
        /// </summary>
        public HtmlMultiFileReportWriter()
            : base(ReportFormat.HtmlMultiFile)
        { }


        /// <inheritdoc />
        /// <devdoc>
        /// This will delete a pre-existing _files folder. Be sure the user has at least seen an
        /// overwrite prompt for the web page by now!
        /// </devdoc>
        public override void WriteReport(CoverageExport dataset, ReportConfigurationModel configuration)
        {
            EnsureJQueryIsCached();

            // Note (Windows-specific):
            // when a web page ("Page.html") is accompanied by a folder named like "Page_files",
            // windows will try to keep the folder with the html page when moved, and warn if one of
            // these are renamed (since the other will not be, and any links in the page will not work).
            string filesDirName = Path.GetFileNameWithoutExtension(configuration.DestinationPath) + "_files";
            string filesDirPath = Path.Combine(Path.GetDirectoryName(configuration.DestinationPath), filesDirName);


            // make sure we're working with a fresh files folder.
            var filesDir = new DirectoryInfo(filesDirPath);
            if (filesDir.Exists)
            {
                filesDir.Delete(recursive: true);
            }
            filesDir.Create();


            string jQueryFullPath = Path.Combine(filesDirPath, ReportConstants.JQueryFileName);
            File.Copy(JQueryCachedPath, jQueryFullPath);


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
                    Path.Combine(filesDirName, ReportConstants.JQueryFileName));


                using (var outputFile = new FileStream(configuration.DestinationPath, FileMode.Create, FileAccess.Write))
                {
                    using (var tempReader = XmlReader.Create(tempFile.Stream))
                    {
                        transform.Transform(tempReader, args, outputFile);
                    }
                }
            }
        }


        /// <summary>
        /// Ensures that jQuery is in the file cache, and downloads a copy if it is not.
        /// </summary>
        private static void EnsureJQueryIsCached()
        {
            string dataDir = Constants.DataDirectory;
            Directory.CreateDirectory(dataDir);

            string jQueryPath = JQueryCachedPath;

            var jQueryFileInfo = new FileInfo(jQueryPath);

            if (!jQueryFileInfo.Exists)
            {
                using (var webClient = new WebClient())
                {
                    // try to get the file from the web cache if it's already there.
                    webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.CacheIfAvailable);
                    webClient.DownloadFile(ReportConstants.JQueryCDNLocation, jQueryPath);
                }
            }

            // FUTURE: verify the integrity of the cached jQuery file?
            // Is jQuery's 3.1.1 slim min always *exactly* the same?
            // Are there unversioned revisions?
            // (if yes and no, then a SHA-256 on the recognized one would do well.)
        }
    }
}
