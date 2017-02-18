using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using VSCoverageViewer.Models;

namespace VSCoverageViewer.ViewModels
{
    /// <summary>
    /// View model for the report configuration dialog.
    /// </summary>
    internal class ReportConfigurationViewModel : BaseViewModel<ReportConfigurationModel>
    {
        private bool _canCreateReport;


        /// <summary>
        /// Gets or sets the owner window.
        /// </summary>
        [Bindable(false)]
        public Window Owner { get; set; }


        /// <summary>
        /// Gets a value indicating if a report can be created with the current configuration.
        /// </summary>
        public bool CanCreateReport
        {
            get { return _canCreateReport; }
            private set { Set(ref _canCreateReport, value); }
        }


        /// <summary>
        /// Gets a command for browsing for the destination path.
        /// </summary>
        public RelayCommand BrowseForDestinationCmd { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationViewModel"/>.
        /// </summary>
        public ReportConfigurationViewModel()
            : this(new ReportConfigurationModel())
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationViewModel"/>.
        /// </summary>
        /// <param name="model">The model to bind to.</param>
        public ReportConfigurationViewModel(ReportConfigurationModel model)
            : base(model)
        {
            BrowseForDestinationCmd = new RelayCommand(BrowseForDestination);
        }


        /// <summary>
        /// Prompts the user to browse for a destination path.
        /// </summary>
        public void BrowseForDestination()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Coverage Report File (*.html;*.htm)|*.html;*.htm";

            if (!string.IsNullOrEmpty(Model.DestinationPath))
            {
                sfd.InitialDirectory = Path.GetDirectoryName(Model.DestinationPath);
                sfd.FileName = Path.GetFileName(Model.DestinationPath);
            }

            // note: nullable bools...
            if (sfd.ShowDialog(Owner) == true)
            {
                Model.DestinationPath = sfd.FileName;
            }
        }


        /// <summary>
        /// Validates that the current configuration can be used to create a report.
        /// </summary>
        private void ValidateConfiguration()
        {
            bool isValid = !string.IsNullOrWhiteSpace(Model.DestinationPath) &&
                           Model.DestinationPath.IndexOfAny(Path.GetInvalidPathChars()) == -1;

            CanCreateReport = isValid;
        }


        /// <summary>
        /// Handles a model's property changing.
        /// </summary>
        /// <param name="sender">The model.</param>
        /// <param name="e">The event details.</param>
        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ValidateConfiguration();
        }


        /// <inheritdoc />
        protected override void AttachModel(ReportConfigurationModel newModel)
        {
            newModel.PropertyChanged += ModelPropertyChanged;
        }

        /// <inheritdoc />
        protected override void DetachModel(ReportConfigurationModel oldModel)
        {
            oldModel.PropertyChanged -= ModelPropertyChanged;
        }
    }
}
