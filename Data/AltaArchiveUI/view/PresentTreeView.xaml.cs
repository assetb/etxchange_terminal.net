using AltaArchiveUI.vm;
using AltaBO.archive;
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

namespace AltaArchiveUI.view {
    /// <summary>
    /// Interaction logic for PresentTreeView.xaml
    /// </summary>
    public partial class PresentTreeView : UserControl {
        public PresentTreeView() {
            InitializeComponent();
            
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            ((PresentTreeVM)DataContext).SelectedNodeVM = (NodeVM)e.NewValue;
        }

        private void PresentTreeView_SelectedNodeChangedEvent(object sender, EventArgs e) {
            throw new NotImplementedException();
        }
    }
}
