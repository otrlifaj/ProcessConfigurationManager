using ProcessConfigurationManager.UPMM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using Northwoods.GoXam.Model;
using System.Collections.ObjectModel;

namespace ProcessConfigurationManager.WPF.UML
{
    public class UmlPage<NodeData, LinkData> : Page
    {
        protected List<SoftwareProcessElement> SoftwareProcessProfile { get; set; }
        protected UML4UPMM Uml4Upmm { get; set; }
        protected List<NodeData> PaletteModel { get; set; }
        protected GraphLinksModel<NodeData, String, String, LinkData> DiagramModel { get; set; }

        public Boolean IsValidatingWithModel { get; set; } = false;
        public Boolean AllowDuplicateNodes { get; set; } = false;

        public UmlPage()
        {
            Application.Current.MainWindow.Width = 1000;
            Application.Current.MainWindow.Height = 600;
            DiagramModel = new GraphLinksModel<NodeData, string, string, LinkData>();
            DiagramModel.NodesSource = new ObservableCollection<NodeData>();
            DiagramModel.LinksSource = new ObservableCollection<LinkData>();
            DiagramModel.Modifiable = false;
            DiagramModel.HasUndoManager = false;
            DiagramModel.NodeKeyPath = "Key";
            DiagramModel.LinkFromPath = "From";
            DiagramModel.LinkToPath = "To";
            DiagramModel.NodeCategoryPath = "Category";
            DiagramModel.NodeIsGroupPath = "IsSubGraph";
            DiagramModel.GroupNodePath = "SubGraphKey";
        }

        public UmlPage(List<SoftwareProcessElement> softwareProcessProfile) : this()
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
    }
}
