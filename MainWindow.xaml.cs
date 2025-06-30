using CodeCoverageAnalyzer.Model;
using CodeCoverageAnalyzer.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeCoverageAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var vm = (MainViewModel)DataContext;

            if (e.NewValue is CoverageNode node && node.File != null)
            {
                vm.SelectedFile = node.File;

                var converter = new CoverageFileToDocumentConverter();
                CodeViewer.Document = (FlowDocument)converter.Convert(node.File, typeof(FlowDocument), null, null);
            }
            else
            {
                vm.SelectedFile = null;
                CodeViewer.Document = new FlowDocument(new Paragraph(new Run(string.Empty))); // Clear
            }
        }
    }
}