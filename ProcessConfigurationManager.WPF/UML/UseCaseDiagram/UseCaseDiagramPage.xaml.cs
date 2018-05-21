using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;
using ProcessConfigurationManager.UPMM;
using ProcessConfigurationManager.WPF.UML.Common;
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

namespace ProcessConfigurationManager.WPF.UML
{
    /// <summary>
    /// Interaction logic for UseCaseDiagramPage.xaml
    /// </summary>
    public partial class UseCaseDiagramPage : Page
    {
        protected List<SoftwareProcessElement> SoftwareProcessProfile { get; set; }
        protected UML4UPMM Uml4Upmm { get; set; }
        protected List<UseCaseDiagramNodeData> PaletteModel { get; set; }
        protected GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData> DiagramModel { get; set; }
        private List<LinkTypeComboBoxItem> LinkTypes { get; set; }
        private string SelectedLinkType { get; set; }
        private UseCaseDiagramNodeData NodeToEdit { get; set; }

        public static Boolean IsValidatingWithModel { get; set; } = false;
        public static Boolean AllowDuplicateNodes { get; set; } = false;


        public UseCaseDiagramPage()
        {
            InitializeComponent();
            LinkTypes = new List<LinkTypeComboBoxItem>()
            {
                new LinkTypeComboBoxItem(1, Constants.UML_UCD_ASSOCIATION),
                new LinkTypeComboBoxItem(2, Constants.UML_UCD_ANCHOR),
                new LinkTypeComboBoxItem(3, Constants.UML_UCD_INCLUDE),
                new LinkTypeComboBoxItem(4, Constants.UML_UCD_EXTEND)
            };
            SelectedLinkType = LinkTypes.First().Name;
            linkTypeComboBox.ItemsSource = LinkTypes.OrderBy(item => item.Id);
            linkTypeComboBox.DisplayMemberPath = "Name";
            linkTypeComboBox.SelectedValuePath = "Name";
            linkTypeComboBox.SelectedValue = Constants.UML_UCD_ASSOCIATION;

            Application.Current.MainWindow.Width = 1000;
            Application.Current.MainWindow.Height = 600;
            DiagramModel = new GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData>();
            DiagramModel.NodesSource = new ObservableCollection<UseCaseDiagramNodeData>();
            DiagramModel.LinksSource = new ObservableCollection<UseCaseDiagramLinkData>();
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

            var labelTool = new SimpleLabelDraggingTool();
            labelTool.Diagram = diagram;
            diagram.MouseMoveTools.Insert(0, labelTool);

            palette.Model = new GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData>();

            notePalette.Model = new GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData>();
            notePalette.Model.NodesSource = new List<UseCaseDiagramNodeData>()
            {
                new UseCaseDiagramNodeData() { Key = Constants.UML_UCD_EDITABLE_NOTE, Category=Constants.UML_UCD_EDITABLE_NOTE, Name="Note" }
            };
        }

        public UseCaseDiagramPage(List<SoftwareProcessElement> softwareProcessProfile) : this()
        {
            try
            {
                SoftwareProcessProfile = softwareProcessProfile;
                Uml4Upmm = new UML4UPMM(softwareProcessProfile);
                PaletteModel = Uml4Upmm.MapUPMMToUseCaseDiagramNodeData();
                UCDElementsListbox.SelectedIndex = 0;

                diagram.SelectionChanged += diagram_SelectionChanged;
                palette.SelectionChanged += palette_SelectionChanged;
            }
            catch (Exception ex)
            {
                throw new ProcessManagerException("Diagram initialization failed. Missing OWL file?", ex);
            }

        }

        // event handler pro filtraci palety podle vybrané kategorie
        private void UCDElementsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette == null) return;
            String category = ((sender as ListBox).SelectedItem as ListBoxItem).Content as String;
            palette.Model.NodesSource = PaletteModel.Where(x => x.Category == category).OrderBy(x => x.Stereotype).ThenBy(x => x.Name).ToList();
        }

        // metoda pro serializaci datového modelu grafu do kotr xml
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if ((diagram.NodesSource as ObservableCollection<UseCaseDiagramNodeData>).Count == 0)
                return;

            var model = diagram.Model as GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData>;
            if (model == null) return;
            XElement root = model.Save<UseCaseDiagramNodeData, UseCaseDiagramLinkData>(Constants.UML_UCD_XML_ROOT_STRING, Constants.UML_UCD_XML_NODE_STRING, Constants.UML_UCD_XML_LINK_STRING);
            root.SetAttributeValue(Constants.UML_UCD_XML_VALIDATION_ATTRIBUTE_STRING, ValidationComboBox.SelectedIndex);

            DiagramUtils diagramUtils = new DiagramUtils();
            diagramUtils.SaveDiagramDialog(root, "UseCaseDiagram.kotr");
        }

        // metoda pro deserializaci datového modelu grafu z kotr xml
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var diagramModel = diagram.Model as GraphLinksModel<UseCaseDiagramNodeData, String, String, UseCaseDiagramLinkData>;
            if (diagramModel == null) return;

            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramXml = diagramUtils.LoadDiagramDialog();

            try
            {
                if (diagramXml == null)
                {
                    return;
                }
                if (diagramXml.Name != Constants.UML_UCD_XML_ROOT_STRING)
                {
                    throw new ProcessManagerException("This file cannot be imported, because it contains different diagram type data.");
                }
                //vypnu validaci z UPMM(protože diagram může být uložený jako nevalidovaný)
                var validationAttribute = diagramXml.Attribute(Constants.UML_UCD_XML_VALIDATION_ATTRIBUTE_STRING);
                if (validationAttribute != null)
                {
                    ValidationComboBox.SelectedIndex = Int32.Parse(validationAttribute.Value);
                }
                else
                {
                    ValidationComboBox.SelectedIndex = 0;
                }

                //zkontroluju, jestli všechny uzly, které mají IRI mají svůj elemnet v načteném profilu procesu
                var loadedModel = new GraphLinksModel<UseCaseDiagramNodeData, string, string, UseCaseDiagramLinkData>();
                loadedModel.Load<UseCaseDiagramNodeData, UseCaseDiagramLinkData>(diagramXml, Constants.UML_UCD_XML_NODE_STRING, Constants.UML_UCD_XML_LINK_STRING);

                string[] categories = { Constants.UML_UCD_ACTOR, Constants.UML_UCD_SYSTEM, Constants.UML_UCD_USE_CASE, Constants.UML_UCD_NOTE };
                int countOfMissing = 0;
                foreach (string IRI in (loadedModel.NodesSource as ObservableCollection<UseCaseDiagramNodeData>).Where(x => categories.Contains(x.Category)).Select(x => x.IRI))
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
                    diagramModel.Load<UseCaseDiagramNodeData, UseCaseDiagramLinkData>(diagramXml, Constants.UML_UCD_XML_NODE_STRING, Constants.UML_UCD_XML_LINK_STRING);
                }

            }
            catch (Exception ex)
            {
                new Utils().ShowExceptionMessageBox(ex);
            }
        }

        // metoda pro uložení diagramu do obrázku png
        private void PNGButton_Click(object sender, RoutedEventArgs e)
        {
            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramBmp = diagramUtils.MakeBitmap(diagram.Panel);
            diagramUtils.SavePngDialog(diagramBmp, defaultFilename: "UseCaseDiagram.png");
        }

        // metoda reagující na zapnutí, vypnutí validace - kontrola existujícího modelu diagramu
        private void ValidationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ValidationComboBox.SelectedIndex == 1)
            {
                UseCaseDiagramPage.IsValidatingWithModel = true;
                Validate.IsEnabled = true;

            }
            else
            {
                UseCaseDiagramPage.IsValidatingWithModel = false;

                if (Validate != null)
                    Validate.IsEnabled = false;

                if (diagram == null || diagram.Model == null || diagram.Model.NodesSource == null)
                    return;
                var originalNodesSource = (diagram.Model.NodesSource as ObservableCollection<UseCaseDiagramNodeData>);

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
                UseCaseDiagramPage.AllowDuplicateNodes = false;
            }
            else
            {
                UseCaseDiagramPage.AllowDuplicateNodes = true;
            }
        }

        // vynucení validace diagramu
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            var originalLinksSource = new ObservableCollection<UseCaseDiagramLinkData>(diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>);
            diagram.LinksSource = new ObservableCollection<UseCaseDiagramLinkData>();
            foreach (var linkData in originalLinksSource)
            {
                var fromData = (diagram.Model.NodesSource as ObservableCollection<UseCaseDiagramNodeData>).First(x => x.Key == linkData.From);
                var toData = (diagram.Model.NodesSource as ObservableCollection<UseCaseDiagramNodeData>).First(x => x.Key == linkData.To);
                var isValid = CheckLink(fromData, toData, linkData, category: linkData.Category);
                if (isValid)
                {
                    (diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>).Add(linkData);
                }
            }

            // kontrola všech elementů, které jsou ve swimlane
            var originalNodesSource = (diagram.Model.NodesSource as ObservableCollection<UseCaseDiagramNodeData>);

            foreach (var nodeData in originalNodesSource)
            {
                if (nodeData.Category == Constants.UML_UCD_ACTOR || nodeData.SubGraphKey == null || nodeData.SubGraphKey == "" || nodeData.Category == Constants.UML_UCD_SYSTEM)
                {
                    nodeData.BorderColor = Constants.VALID_COLOR;
                    nodeData.Text = null;
                    continue;
                }

                string groupIRI = (diagram.Model.NodesSource as ObservableCollection<UseCaseDiagramNodeData>).Where(x => x.Key == nodeData.SubGraphKey).Select(x => x.IRI).FirstOrDefault();
                if (groupIRI == "" || groupIRI == null)
                    continue;


                if (nodeData != null)
                {

                    if (nodeData.Category == Constants.UML_UCD_EDITABLE_NOTE || nodeData.Category == Constants.UML_UCD_SYSTEM || nodeData.Category == Constants.UML_UCD_ACTOR)
                    {
                        continue;
                    }
                    else
                    {
                        string elementIRI = nodeData.IRI;

                        if (elementIRI != null && groupIRI != null)
                        {
                            string relationship = Uml4Upmm.CheckUCDSystemRelationship(groupIRI, elementIRI, UseCaseDiagramPage.IsValidatingWithModel, out string color);
                            if (relationship != null)
                            {
                                nodeData.BorderColor = color;
                                nodeData.Text = null;
                            }
                            else
                            {
                                nodeData.BorderColor = Constants.INVALID_COLOR;
                                nodeData.Text = Constants.UML_UCD_UNSUPPORTED;
                            }
                        }
                    }
                }
            }
        }

        private void linkTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            String selectedValue = (sender as ComboBox).SelectedValue as String;
            SelectedLinkType = selectedValue ?? SelectedLinkType;
        }

        // event handler pro vkládání uzlu
        private void diagram_ExternalObjectsDropped(object sender, Northwoods.GoXam.DiagramEventArgs e)
        {
            UseCaseDiagramNodeData data = null;
            if ((sender as Diagram).SelectedNode == null)
            {
                data = (sender as Diagram).SelectedGroup.Data as UseCaseDiagramNodeData;
            }
            else
            {
                data = (sender as Diagram).SelectedNode.Data as UseCaseDiagramNodeData;

            }
            string[] categories = { Constants.UML_UCD_ACTOR, Constants.UML_UCD_SYSTEM, Constants.UML_UCD_NOTE, Constants.UML_UCD_USE_CASE, Constants.UML_UCD_EDITABLE_NOTE };

            if (categories.Contains(data.Category) && !AllowDuplicateNodes)
            {
                if ((sender as Diagram).Nodes.Count(x => (x.Data as UseCaseDiagramNodeData).IRI == data.IRI && (x.Data as UseCaseDiagramNodeData).Category == data.Category) > 1)
                {
                    diagram.Model.RemoveNode(data);
                }
            }
        }

        private void diagram_LinkDrawn(object sender, Northwoods.GoXam.DiagramEventArgs e)
        {
            Link link = (e.Part as Link);
            UseCaseDiagramLinkData linkData = link.Data as UseCaseDiagramLinkData;
            UseCaseDiagramNodeData fromData = link.FromData as UseCaseDiagramNodeData;
            UseCaseDiagramNodeData toData = link.ToData as UseCaseDiagramNodeData;

            var isValid = CheckLink(fromData, toData, linkData, category: SelectedLinkType);
            if (!isValid)
            {
                (diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>).Remove(linkData);
            }
        }

        private void diagram_LinkRelinked(object sender, Northwoods.GoXam.DiagramEventArgs e)
        {
            Link link = (e.Part as Link);
            UseCaseDiagramLinkData linkData = link.Data as UseCaseDiagramLinkData;
            UseCaseDiagramNodeData fromData = link.FromData as UseCaseDiagramNodeData;
            UseCaseDiagramNodeData toData = link.ToData as UseCaseDiagramNodeData;

            var isValid = CheckLink(fromData, toData, linkData, linkData.Category);
            if (!isValid)
            {
                (diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>).Remove(linkData);
            }
        }

        private bool CheckLink(UseCaseDiagramNodeData fromData, UseCaseDiagramNodeData toData, UseCaseDiagramLinkData linkData, string category)
        {
            if (
                (category == Constants.UML_UCD_ANCHOR)
                &&
                (fromData.Category == Constants.UML_UCD_EDITABLE_NOTE || toData.Category == Constants.UML_UCD_EDITABLE_NOTE)
                &&
                (fromData.Category != toData.Category)
                &&
                (fromData.Category != Constants.UML_UCD_NOTE)
                &&
                (toData.Category != Constants.UML_UCD_NOTE)
                )
            {
                linkData.Category = category;
                linkData.Color = Constants.VALID_COLOR;
                return IsLinkUniqueBothWays(linkData, category);
            }
            else
            {
                string relationship = Uml4Upmm.CheckUCDRelationship(fromData.IRI, toData.IRI, UseCaseDiagramPage.IsValidatingWithModel, out string color, category);

                if (relationship == null)
                {
                    return false;
                }
                else
                {
                    linkData.Category = category;
                    linkData.Text = relationship == "" ? relationship : "<<" + relationship + ">>";
                    linkData.Color = color;
                }
            }
            return IsLinkUnique(linkData, category);
        }

        private bool IsLinkUnique(UseCaseDiagramLinkData linkData, string category)
        {
            if ((diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>)
                .Count(x => x.From == linkData.From && x.To == linkData.To && x.Category == category) > 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsLinkUniqueBothWays(UseCaseDiagramLinkData linkData, string category)
        {
            int count1 = (diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>)
                .Count(x => x.From == linkData.From && x.To == linkData.To && x.Category == category);
            int count2 = (diagram.LinksSource as ObservableCollection<UseCaseDiagramLinkData>)
                .Count(x => x.From == linkData.To && x.To == linkData.From && x.Category == category);
            return count1 + count2 < 2;
        }

        // event handlery pro zobrazování popisu vybraného uzlu
        private void diagram_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (diagram.SelectedNode != null && diagram.SelectedGroup == null)
            {
                descriptionTextBox.Text = (diagram.SelectedNode.Data as UseCaseDiagramNodeData).Description;

            }
            else if (diagram.SelectedGroup != null && diagram.SelectedNode == null)
            {
                descriptionTextBox.Text = (diagram.SelectedGroup.Data as UseCaseDiagramNodeData).Description;
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

        private void palette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette.SelectedNode != null && palette.SelectedGroup == null)
            {
                descriptionTextBox.Text = (palette.SelectedNode.Data as UseCaseDiagramNodeData).Description;

            }
            else if (palette.SelectedGroup != null && palette.SelectedNode == null)
            {
                descriptionTextBox.Text = (palette.SelectedGroup.Data as UseCaseDiagramNodeData).Description;
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

        private void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            EditNoteInputBox.Visibility = Visibility.Collapsed;
            String noteName = NoteNameInputTextBox.Text;
            String noteText = NoteTextInputTextBox.Text;
            NodeToEdit.Name = noteName;
            NodeToEdit.Text = noteText;
            NoteNameInputTextBox.Text = String.Empty;
            NoteTextInputTextBox.Text = String.Empty;
        }

        private void CancelSaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            EditNoteInputBox.Visibility = Visibility.Collapsed;
            NoteNameInputTextBox.Text = String.Empty;
            NoteTextInputTextBox.Text = String.Empty;
        }

        private void editNoteButton_Click(object sender, RoutedEventArgs e)
        {
            Node node = Part.FindAncestor<Node>(e.OriginalSource as UIElement);
            if (node == null) return;

            NodeToEdit = node.Data as UseCaseDiagramNodeData;
            NoteNameInputTextBox.Text = NodeToEdit.Name;
            NoteTextInputTextBox.Text = NodeToEdit.Text;
            EditNoteInputBox.Visibility = Visibility.Visible;
        }
    }

    // tooly pro zajištění korektnosti hrany - není možná reflexivní hrana
    internal class UseCaseDiagramLinkingTool : LinkingTool
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

    internal class UseCaseDiagramRelinkingTool : RelinkingTool
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

    // tool pro vkládání uzlů do systému -- obsahuje validaci pro drag-and-drop do swimlane
    internal class SystemDraggingTool : DraggingTool
    {
        UML4UPMM Uml4Upmm { get; set; }
        public SystemDraggingTool()
        {
            Uml4Upmm = new UML4UPMM(OWLParser.OWLAPI.GetSoftwareProcess());
        }
        public override bool IsValidMember(Northwoods.GoXam.Group group, Node node)
        {
            try
            {
                if (group == null)
                {
                    (node.Data as UseCaseDiagramNodeData).BorderColor = Constants.VALID_COLOR;
                    (node.Data as UseCaseDiagramNodeData).Text = null;
                    return true;
                }

                if (node != null)
                {

                    if (node.Category == Constants.UML_UCD_SYSTEM || node.Category == Constants.UML_UCD_ACTOR)
                    {
                        return false;
                    }

                    if (node.Category == Constants.UML_UCD_EDITABLE_NOTE) return true;
                    string systemIRI = (group.Data as UseCaseDiagramNodeData).IRI;
                    string elementIRI = (node.Data as UseCaseDiagramNodeData).IRI;

                    if (elementIRI != null && systemIRI != null)
                    {
                        string relationship = Uml4Upmm.CheckUCDSystemRelationship(systemIRI, elementIRI, UseCaseDiagramPage.IsValidatingWithModel, out string color);
                        if (relationship != null)
                        {
                            (node.Data as UseCaseDiagramNodeData).BorderColor = color;
                            (node.Data as UseCaseDiagramNodeData).Text = null;
                        }
                        else
                        {
                            (node.Data as UseCaseDiagramNodeData).BorderColor = Constants.INVALID_COLOR;
                            (node.Data as UseCaseDiagramNodeData).Text = Constants.UML_UCD_UNSUPPORTED;

                        }
                    }
                }
                return base.IsValidMember(group, node);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void DoDragLeave(DragEventArgs e)
        {
            base.DoDragLeave(e);
        }
    }
}
