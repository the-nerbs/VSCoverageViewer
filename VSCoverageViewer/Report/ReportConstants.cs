using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Report
{
    internal static class ReportConstants
    {
        public const string JQueryFileName = "jquery-3.1.1.slim.min.js";
        public const string JQueryCDNLocation = @"https://code.jquery.com/" + JQueryFileName;


        // HTML Transform arguments:
        public const string HtmlGenDate = "genDate";
        public const string HtmlTotalLines = "totalLines";
        public const string HtmlTotalBlocks = "totalBlocks";
        public const string HtmlExpansionDepth = "depth";
        public const string HtmlJQuerySource = "jQuerySource";
    }
}
