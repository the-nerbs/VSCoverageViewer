using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSCoverageViewer.Models
{
    internal enum ViewLevel
    {
        // note: changes here require changes in the transform!
        Totals = 1,
        Modules = 2,
        Namespaces = 3,
        Classes = 4,
        Members = 5,
    }

    internal enum ReportFormat
    {
        [Description("HTML single file (requires internet connection)")]
        HtmlSingleFile,

        [Description("HTML file + folder (no internet connection)")]
        HtmlMultiFile,
    }


    /// <summary>
    /// Model for the report configuration.
    /// </summary>
    internal class ReportConfigurationModel : ObservableObject
    {
        private string _destinationPath;
        private string _projectName;
        private ViewLevel _defaultExpansion = ViewLevel.Classes;
        private ReportFormat _reportType = ReportFormat.HtmlSingleFile;
        private bool _openWhenDone = true;


        /// <summary>
        /// Gets or sets the path to which the report will be saved.
        /// </summary>
        public string DestinationPath
        {
            get { return _destinationPath; }
            set { SetIfChanged(ref _destinationPath, value); }
        }

        /// <summary>
        /// Gets or sets the name of the project to use in the report title.
        /// </summary>
        public string ProjectName
        {
            get { return _projectName; }
            set { SetIfChanged(ref _projectName, value); }
        }

        /// <summary>
        /// Gets or sets the default level of expansion for HTML reports.
        /// </summary>
        public ViewLevel DefaultExpansion
        {
            get { return _defaultExpansion; }
            set { SetIfChanged(ref _defaultExpansion, value); }
        }

        /// <summary>
        /// Gets or sets the type of report to create.
        /// </summary>
        public ReportFormat ReportFormat
        {
            get { return _reportType; }
            set { SetIfChanged(ref _reportType, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating if the report should be opened immediately after generation.
        /// </summary>
        public bool OpenWhenDone
        {
            get { return _openWhenDone; }
            set { SetIfChanged(ref _openWhenDone, value); }
        }
    }
}
