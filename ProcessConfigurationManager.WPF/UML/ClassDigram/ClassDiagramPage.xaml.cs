using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Northwoods.GoXam.Model;
using ProcessConfigurationManager.UPMM;

namespace ProcessConfigurationManager.WPF.UML
{
    /// <summary>
    /// Interaction logic for ClassDiagramPage.xaml
    /// </summary>
    public partial class ClassDiagramPage : Page
    {
        protected List<SoftwareProcessElement> SoftwareProcessProfile { get; set; }
        protected UML4UPMM Uml4Upmm { get; set; }
        protected List<ClassDiagramNodeData> PaletteModel { get; set; }
        protected GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData> DiagramModel { get; set; }

        public Boolean IsValidatingWithModel { get; set; } = false;
        public Boolean AllowDuplicateNodes { get; set; } = false;


        public ClassDiagramPage()
        {
            InitializeComponent();
            Application.Current.MainWindow.Width = 1000;
            Application.Current.MainWindow.Height = 600;
            DiagramModel = new GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>();
            DiagramModel.NodesSource = new ObservableCollection<ClassDiagramNodeData>();
            DiagramModel.LinksSource = new ObservableCollection<ClassDiagramLinkData>();
            DiagramModel.Modifiable = false;
            DiagramModel.HasUndoManager = false;
            DiagramModel.NodeKeyPath = "Key";
            DiagramModel.LinkFromPath = "From";
            DiagramModel.LinkToPath = "To";
            DiagramModel.NodeCategoryPath = "Category";
            DiagramModel.NodeIsGroupPath = "IsSubGraph";
            DiagramModel.GroupNodePath = "SubGraphKey";
        }

        public ClassDiagramPage(List<SoftwareProcessElement> softwareProcessProfile) : this()
        {
            try
            {
                SoftwareProcessProfile = softwareProcessProfile;
                Uml4Upmm = new UML4UPMM(softwareProcessProfile);
            }

            catch (Exception ex)
            {
                throw new ProcessManagerException("Diagram initialization failed. Missing OWL file?", ex);
            }
        }

        private void CDElementsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SavePNG_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ValidationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DuplicatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
