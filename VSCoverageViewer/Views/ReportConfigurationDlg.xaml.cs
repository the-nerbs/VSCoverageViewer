using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VSCoverageViewer.Models;
using VSCoverageViewer.ViewModels;

namespace VSCoverageViewer.Views
{
    /// <summary>
    /// Interaction logic for ReportConfigurationDlg.xaml
    /// </summary>
    public partial class ReportConfigurationDlg : Window
    {
        internal ReportConfigurationViewModel ViewModel
        {
            get
            {
                Debug.Assert(DataContext is ReportConfigurationViewModel);
                return (ReportConfigurationViewModel)DataContext;
            }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationDlg"/>.
        /// </summary>
        public ReportConfigurationDlg()
            : this(new ReportConfigurationModel())
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReportConfigurationDlg"/>.
        /// </summary>
        /// <param name="configuration">The model to bind to.</param>
        internal ReportConfigurationDlg(ReportConfigurationModel configuration)
        {
            InitializeComponent();

            SourceInitialized += HandleSourceInitialized;

            ViewModel.Owner = this;
            ViewModel.Model = configuration;
        }


        /// <summary>
        /// Performs source initialization.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void HandleSourceInitialized(object sender, EventArgs e)
        {
            // probably doesn't matter, but unsubscribe just to be safe.
            SourceInitialized -= HandleSourceInitialized;

            NativeMethods.ClearWindowStyles(this, WindowStyles.SysMenu);
        }

        /// <summary>
        /// Handles the "Create Report" button being clicked.
        /// </summary>
        /// <param name="sender">unused.</param>
        /// <param name="e">unused.</param>
        private void CreateReportClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
