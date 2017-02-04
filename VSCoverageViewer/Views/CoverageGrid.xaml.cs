using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using VSCoverageViewer.Interfaces;
using VSCoverageViewer.Messaging;

namespace VSCoverageViewer.Views
{
    /// <summary>
    /// Interaction logic for CoverageGrid.xaml
    /// </summary>
    public partial class CoverageGrid : UserControl
    {
        private readonly IMessenger _messenger;


        /// <summary>
        /// Gets the view model.
        /// </summary>
        private ICoverageGridViewModel ViewModel
        {
            get { return DataContext as ICoverageGridViewModel; }
        }


        /// <summary>
        /// Initializes a new instance of <see cref="CoverageGrid"/>.
        /// </summary>
        public CoverageGrid()
        {
            InitializeComponent();

            _messenger = Messenger.Default;
            _messenger.Register<ColumnVisibilityChangedMessage>(this, HandleColumnVisibilityChanged);
        }


        /// <summary>
        /// Handles a column visibility changed application message.
        /// </summary>
        /// <param name="msg">The message data.</param>
        private void HandleColumnVisibilityChanged(ColumnVisibilityChangedMessage msg)
        {
            foreach (var column in _grid.Columns)
            {
                Column tag = column.GetTag();
                column.Visibility = ViewModel.IsColumnVisible(tag) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
