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
using System.Xml.Linq;
using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;
using ProcessConfigurationManager.UPMM;

namespace ProcessConfigurationManager.WPF.UML
{
    /// <summary>
    /// Interaction logic for ClassDiagramPage.xaml
    /// </summary>
    public partial class ClassDiagramPage : Page
    {
        private const string XML_LINK_STRING = "Link";
        private const string XML_NODE_STRING = "Node";
        private const string XML_ROOT_STRING = "KOTRClassDiagram";
        private const string XML_VALIDATION_ATRIBUTE_STRING = "validation";
        private const string UML_CD_ASSOCIATION = "Association";
        private const string UML_CD_AGGREGATION = "Aggregation";
        private const string UML_CD_GENERALIZATION = "Generalization";
        private const string UML_CD_ANCHOR = "Anchor";

        protected List<SoftwareProcessElement> SoftwareProcessProfile { get; set; }
        protected UML4UPMM Uml4Upmm { get; set; }
        protected List<ClassDiagramNodeData> PaletteModel { get; set; }
        protected GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData> DiagramModel { get; set; }
        private List<LinkTypeComboBoxItem> LinkTypes { get; set; }
        private string SelectedLinkType { get; set; }

        public static Boolean IsValidatingWithModel { get; set; } = false;
        public static Boolean AllowDuplicateNodes { get; set; } = false;

        public ClassDiagramPage()
        {
            InitializeComponent();
            LinkTypes = new List<LinkTypeComboBoxItem>()
            {
                new LinkTypeComboBoxItem(1, UML_CD_ASSOCIATION),
                new LinkTypeComboBoxItem(2, UML_CD_AGGREGATION),
                new LinkTypeComboBoxItem(3, UML_CD_GENERALIZATION),
                new LinkTypeComboBoxItem(4, UML_CD_ANCHOR)
            };
            SelectedLinkType = LinkTypes.First().Name;

            linkTypeComboBox.ItemsSource = LinkTypes.OrderBy(item => item.Id);
            linkTypeComboBox.DisplayMemberPath = "Name";
            linkTypeComboBox.SelectedValuePath = "Name";
            linkTypeComboBox.SelectedValue = UML_CD_ASSOCIATION;

            Application.Current.MainWindow.Width = 1000;
            Application.Current.MainWindow.Height = 600;
            DiagramModel = new GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>();
            DiagramModel.NodesSource = new ObservableCollection<ClassDiagramNodeData>();
            DiagramModel.LinksSource = new ObservableCollection<ClassDiagramLinkData>();
            DiagramModel.Modifiable = true;
            DiagramModel.HasUndoManager = false;
            DiagramModel.NodeKeyPath = "Key";
            DiagramModel.LinkFromPath = "From";
            DiagramModel.LinkToPath = "To";
            DiagramModel.NodeCategoryPath = "Category";
            DiagramModel.NodeIsGroupPath = "IsSubGraph";
            DiagramModel.GroupNodePath = "SubGraphKey";

            diagram.Model = DiagramModel;
            diagram.AllowDrop = true;

            palette.Model = new GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>();

            notePalette.Model = new GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>();
            notePalette.Model.NodesSource = new List<ClassDiagramNodeData>()
            {
                new ClassDiagramNodeData() { Key = "Note", Category="Note", Name="Note"}
            };
        }

        public ClassDiagramPage(List<SoftwareProcessElement> softwareProcessProfile) : this()
        {
            try
            {
                SoftwareProcessProfile = softwareProcessProfile;
                Uml4Upmm = new UML4UPMM(softwareProcessProfile);
                PaletteModel = Uml4Upmm.MapUPMMToClassDiagramNodeData();
                CDElementsListbox.SelectedIndex = 0;

                diagram.SelectionChanged += diagram_SelectionChanged;
                palette.SelectionChanged += palette_SelectionChanged;

            }

            catch (Exception ex)
            {
                throw new ProcessManagerException("Diagram initialization failed. Missing OWL file?", ex);
            }
        }
        // event handlery pro zobrazování popisu vybraného uzlu
        private void palette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette.SelectedNode != null && palette.SelectedGroup == null)
            {
                descriptionTextBox.Text = (palette.SelectedNode.Data as ClassDiagramNodeData).Description;

            }
            else if (palette.SelectedGroup != null && palette.SelectedNode == null)
            {
                descriptionTextBox.Text = (palette.SelectedGroup.Data as ClassDiagramNodeData).Description;
            }

            if (descriptionTextBox.Text == "")
            {
                this.descriptionTextBox.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.descriptionTextBox.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void diagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (diagram.SelectedNode != null && diagram.SelectedGroup == null)
            {
                descriptionTextBox.Text = (diagram.SelectedNode.Data as ClassDiagramNodeData).Description;

            }
            else if (diagram.SelectedGroup != null && diagram.SelectedNode == null)
            {
                descriptionTextBox.Text = (diagram.SelectedGroup.Data as ClassDiagramNodeData).Description;
            }

            if (descriptionTextBox.Text == "")
            {
                this.descriptionTextBox.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                this.descriptionTextBox.Visibility = System.Windows.Visibility.Visible;
            }
        }

        // event handler pro filtraci palety podle vybrané kategorie
        private void CDElementsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette == null) return;
            String category = ((sender as ListBox).SelectedItem as ListBoxItem).Content as String;
            palette.Model.NodesSource = PaletteModel.Where(x => x.Category == category).OrderBy(x => x.Stereotype).ThenBy(x => x.Name).ToList();
        }

        // event handler pro vkládání uzlu
        private void diagram_ExternalObjectsDropped(object sender, Northwoods.GoXam.DiagramEventArgs e)
        {
            ClassDiagramNodeData data = (sender as Diagram).SelectedNode.Data as ClassDiagramNodeData;

            string[] categories = { "Class" };

            if (categories.Contains(data.Category) && !AllowDuplicateNodes)
            {
                if ((sender as Diagram).Nodes.Count(x => (x.Data as ClassDiagramNodeData).IRI == data.IRI && (x.Data as ClassDiagramNodeData).Category == data.Category) > 1)
                {
                    diagram.Model.RemoveNode(data);
                }
            }
        }

        // metoda pro uložení diagramu do obrázku png
        private void SavePNG_Click(object sender, RoutedEventArgs e)
        {
            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramBmp = diagramUtils.MakeBitmap(diagram.Panel);
            diagramUtils.SavePngDialog(diagramBmp, defaultFilename: "ClassDiagram.png");
        }

        // metoda pro serializaci datového modelu grafu do kotr xml
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if ((diagram.NodesSource as ObservableCollection<ClassDiagramNodeData>).Count == 0)
                return;

            var model = diagram.Model as GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>;
            if (model == null) return;
            XElement root = model.Save<ClassDiagramNodeData, ClassDiagramLinkData>(XML_ROOT_STRING, XML_NODE_STRING, XML_LINK_STRING);
            root.SetAttributeValue(XML_VALIDATION_ATRIBUTE_STRING, ValidationComboBox.SelectedIndex);

            DiagramUtils diagramUtils = new DiagramUtils();
            diagramUtils.SaveDiagramDialog(root, "ClassDiagram.kotr");
        }

        // metoda pro deserializaci datového modelu grafu z kotr xml
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var diagramModel = diagram.Model as GraphLinksModel<ClassDiagramNodeData, String, String, ClassDiagramLinkData>;
            if (diagramModel == null) return;

            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramXml = diagramUtils.LoadDiagramDialog();

            try
            {
                if (diagramXml.Name != XML_ROOT_STRING)
                {
                    throw new ProcessManagerException("This file cannot be imported, because it contains different diagram type data.");
                }
                //vypnu validaci z UPMM(protože diagram může být uložený jako nevalidovaný)
                var validationAttribute = diagramXml.Attribute(XML_VALIDATION_ATRIBUTE_STRING);
                if (validationAttribute != null)
                {
                    ValidationComboBox.SelectedIndex = Int32.Parse(validationAttribute.Value);
                }
                else
                {
                    ValidationComboBox.SelectedIndex = 0;
                }

                //zkontroluju, jestli všechny uzly, které mají IRI mají svůj elemnet v načteném profilu procesu
                var loadedModel = new GraphLinksModel<ClassDiagramNodeData, string, string, ClassDiagramLinkData>();
                loadedModel.Load<ClassDiagramNodeData, ClassDiagramLinkData>(diagramXml, XML_NODE_STRING, XML_LINK_STRING);

                string[] categories = { "Class", "Note" };
                int countOfMissing = 0;
                foreach (string IRI in (loadedModel.NodesSource as ObservableCollection<ClassDiagramNodeData>).Where(x => categories.Contains(x.Category)).Select(x => x.IRI))
                {
                    if (!SoftwareProcessProfile.Any(x => x.IRI == IRI))
                    {
                        countOfMissing++;
                    }
                }

                if (countOfMissing > 0)
                {
                    throw new ProcessManagerException("Diagram can't be loaded, because it does not match your OWL profile.");
                }
                else
                {
                    //pokud všechny uzly s IRI mají své IRI v profilu, přidám diagram, 
                    diagramModel.Load<ClassDiagramNodeData, ClassDiagramLinkData>(diagramXml, XML_NODE_STRING, XML_LINK_STRING);
                }

            }
            catch (Exception ex)
            {
                new Utils().ShowExceptionMessageBox(ex);
            }
        }

        // metoda reagujícína zapnutí, vypnutí validace - kontrola existujícího modelu diagramu
        private void ValidationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValidationComboBox.SelectedIndex == 1)
            {
                ClassDiagramPage.IsValidatingWithModel = true;
                Validate.IsEnabled = true;

            }
            else
            {
                ClassDiagramPage.IsValidatingWithModel = false;

                if (Validate != null)
                    Validate.IsEnabled = false;

                if (diagram == null || diagram.Model == null || diagram.Model.NodesSource == null)
                    return;
                var originalNodesSource = (diagram.Model.NodesSource as ObservableCollection<ClassDiagramNodeData>);

                foreach (var nodeData in originalNodesSource)
                {
                    nodeData.Text = null;
                }
            }
        }

        // povoluje nebo zakazuje duplikátní uzly
        private void DuplicatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DuplicatesComboBox.SelectedIndex == 1)
            {
                ClassDiagramPage.AllowDuplicateNodes = false;
            }
            else
            {
                ClassDiagramPage.AllowDuplicateNodes = true;
            }
        }

        // vynucení validace diagramu
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void LinkTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = (sender as ComboBox).SelectedValue as string;
            SelectedLinkType = selectedValue ?? SelectedLinkType;
        }

        // event handler pro validaci myší vytvořených linků jakožto povolených vztahů z UPMM
        private void diagram_LinkDrawn(object sender, Northwoods.GoXam.DiagramEventArgs e)
        {
            Link link = (e.Part as Link);
            (link.Data as ClassDiagramLinkData).Category = SelectedLinkType;
            var linkData = link.Data as ClassDiagramLinkData;
            var fromData = link.FromData as ClassDiagramNodeData;
            var toData = link.ToData as ClassDiagramNodeData;

            CheckLink(fromData, toData, linkData);
        }

        private void diagram_LinkRelinked(object sender, DiagramEventArgs e)
        {
            Link link = (e.Part as Link);
            var linkData = link.Data as ClassDiagramLinkData;
            var fromData = link.FromData as ClassDiagramNodeData;
            var toData = link.ToData as ClassDiagramNodeData;

            CheckLink(fromData, toData, linkData);
        }

        private void CheckLink(ClassDiagramNodeData fromData, ClassDiagramNodeData toData, ClassDiagramLinkData linkData)
        {
            // TODO
        }

    }

    internal class LinkTypeComboBoxItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public LinkTypeComboBoxItem()
        {

        }

        public LinkTypeComboBoxItem(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    // tooly pro zajištění korektnosti hrany - není možná reflexivní hrana
    internal class ClassDiagramLinkingTool : LinkingTool
    {
        public override bool IsValidLink(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport)
        {
            if (fromnode == tonode)
            {
                return false;
            }

            return base.IsValidLink(fromnode, fromport, tonode, toport);
        }
    }

    internal class ClassDiagramRelinkingTool : RelinkingTool
    {
        public override bool IsValidLink(Node fromnode, FrameworkElement fromport, Node tonode, FrameworkElement toport)
        {
            if (fromnode == tonode)
            {
                return false;
            }

            return base.IsValidLink(fromnode, fromport, tonode, toport);
        }
    }

}
