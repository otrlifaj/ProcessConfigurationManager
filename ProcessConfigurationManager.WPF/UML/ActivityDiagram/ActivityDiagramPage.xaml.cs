﻿using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using Northwoods.GoXam.Tool;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using SWF = System.Windows.Forms;

namespace ProcessConfigurationManager.WPF.UML
{
    /// <summary>
    /// Interaction logic for ActivityDiagramPage.xaml
    /// </summary>
    public partial class ActivityDiagramPage : Page
    {
        private List<UPMM.SoftwareProcessElement> softwareProcessProfile = null;
        private List<ActivityDiagramNodeData> paletteModel = null;
        private UML4UPMM uml4upmm = null;
        public static Boolean IsValidatingWithModel = false;
        public static Boolean AllowDuplicateNodes = false;

        public ActivityDiagramPage()
        {
            InitializeComponent();
            Application.Current.MainWindow.Width = 1000;
            Application.Current.MainWindow.Height = 600;
            var model = new GraphLinksModel<ActivityDiagramNodeData, String, String, ActivityDiagramLinkData>();
            model.NodesSource = new ObservableCollection<ActivityDiagramNodeData>();

            model.LinksSource = new ObservableCollection<ActivityDiagramLinkData>();
            model.Modifiable = true;
            model.HasUndoManager = false;
            diagram.Model = model;
            diagram.AllowDrop = true;
            var labelTool = new SimpleLabelDraggingTool();
            labelTool.Diagram = diagram;
            diagram.MouseMoveTools.Insert(0, labelTool);
            palette.Model = new GraphLinksModel<ActivityDiagramNodeData, String, String, ActivityDiagramLinkData>();
            flowPalette.Model = new GraphLinksModel<ActivityDiagramNodeData, String, String, ActivityDiagramLinkData>();
            flowPalette.Model.NodesSource = new List<ActivityDiagramNodeData>()
            {
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_INITIAL_ACTIVITY, Category=Constants.UML_AD_INITIAL_ACTIVITY, Name=Constants.UML_AD_INITIAL_ACTIVITY},
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_FINAL_ACTIVITY, Category=Constants.UML_AD_FINAL_ACTIVITY, Name=Constants.UML_AD_FINAL_ACTIVITY},
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_DECISION, Category=Constants.UML_AD_DECISION, Name=Constants.UML_AD_DECISION},
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_DECISION_MERGE, Category=Constants.UML_AD_DECISION_MERGE, Name=Constants.UML_AD_DECISION_MERGE},
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_FORK, Category=Constants.UML_AD_FORK, Name=Constants.UML_AD_FORK},
                new ActivityDiagramNodeData() { Key=Constants.UML_AD_JOIN, Category=Constants.UML_AD_JOIN, Name=Constants.UML_AD_JOIN},
            };

            model.NodeKeyPath = "Key";
            model.LinkFromPath = "From";
            model.LinkToPath = "To";
            model.NodeCategoryPath = "Category";
            model.NodeIsGroupPath = "IsSubGraph";
            model.GroupNodePath = "SubGraphKey";
        }

        public ActivityDiagramPage(List<UPMM.SoftwareProcessElement> softwareProcessProfile)
            : this()
        {
            try
            {
                this.softwareProcessProfile = softwareProcessProfile;
                uml4upmm = new UML4UPMM(softwareProcessProfile);
                paletteModel = uml4upmm.MapUPMMToActivityDiagramNodeData();
                ADElementsListbox.SelectedIndex = 0;

                diagram.SelectionChanged += diagram_SelectionChanged;
                palette.SelectionChanged += palette_SelectionChanged;
            }
            catch (Exception ex)
            {
                throw new ProcessManagerException("Activity diagram initialization failed. Missing OWL file?", ex);
            }
        }

        // event handlery pro zobrazování popisu vybraného uzlu
        private void palette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette.SelectedNode != null && palette.SelectedGroup == null)
            {
                descriptionTextBox.Text = (palette.SelectedNode.Data as ActivityDiagramNodeData).Description;

            }
            else if (palette.SelectedGroup != null && palette.SelectedNode == null)
            {
                descriptionTextBox.Text = (palette.SelectedGroup.Data as ActivityDiagramNodeData).Description;
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
                descriptionTextBox.Text = (diagram.SelectedNode.Data as ActivityDiagramNodeData).Description;

            }
            else if (diagram.SelectedGroup != null && diagram.SelectedNode == null)
            {
                descriptionTextBox.Text = (diagram.SelectedGroup.Data as ActivityDiagramNodeData).Description;
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
        private void ADElementsListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (palette == null) return;
            String category = ((sender as ListBox).SelectedItem as ListBoxItem).Content as String;
            palette.Model.NodesSource = paletteModel.Where(x => x.Category == category).OrderBy(x => x.Stereotype).ThenBy(x => x.Name).ToList();
        }

        // event handler pro vkládání uzlu
        private void diagram_ExternalObjectsDropped(object sender, DiagramEventArgs e)
        {
            ActivityDiagramNodeData data = null;
            if ((sender as Diagram).SelectedNode == null)
            {
                data = (sender as Diagram).SelectedGroup.Data as ActivityDiagramNodeData;
            }
            else
            {
                data = (sender as Diagram).SelectedNode.Data as ActivityDiagramNodeData;

            }
            string[] categories = { Constants.UML_AD_ACTIVITY, Constants.UML_AD_OBJECT, Constants.UML_AD_SWIMLANE, Constants.UML_AD_NOTE, Constants.UML_AD_SEND_SIGNAL_ACTION, Constants.UML_AD_ACCEPT_EVENT_ACTION };
            if (data.Category == Constants.UML_AD_INITIAL_ACTIVITY /*|| data.Category == Constants.UML_AD_FINAL_ACTIVITY*/)
            {
                if ((sender as Diagram).Nodes.Where(x => (x.Data as ActivityDiagramNodeData).Category == data.Category).Count() > 1)
                {
                    diagram.Model.RemoveNode(data);
                }
            }
            else if (categories.Contains(data.Category) && !AllowDuplicateNodes)
            {
                if ((sender as Diagram).Nodes.Where(x => (x.Data as ActivityDiagramNodeData).IRI == data.IRI && (x.Data as ActivityDiagramNodeData).Category == data.Category).Count() > 1)
                {
                    diagram.Model.RemoveNode(data);
                }
            }
        }
        
        // event handler pro validaci myší vytvořených linků jakožto povolených vztahů z UPMM
        private void diagram_LinkDrawn(object sender, DiagramEventArgs e)
        {
            Link link = (e.Part as Link);
            ActivityDiagramLinkData linkData = link.Data as ActivityDiagramLinkData;
            ActivityDiagramNodeData fromData = link.FromData as ActivityDiagramNodeData;
            ActivityDiagramNodeData toData = link.ToData as ActivityDiagramNodeData;

            ChooseLinkCategory(fromData, toData, linkData);
            switch (linkData.Category)
            {
                case "Control Flow":
                    CheckLink(fromData, toData, linkData);
                    break;
                case "Object Flow":
                    CheckLink(fromData, toData, linkData);
                    break;
                case "Anchor":
                    CheckLink(fromData, toData, linkData);
                    break;
                default:
                    (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Remove(linkData);
                    break;
            }

        }
        // metoda pro validaci hran jakožto povolených vztahů z UPMM - volá mapovací pravidla z UML4UPMM
        private void CheckLink(ActivityDiagramNodeData fromData, ActivityDiagramNodeData toData, ActivityDiagramLinkData linkData)
        {
            //pokud je na některé straně hrany jeden z uzlů těchto typů, tak se hrana automaticky povoluje, jedná se o podporu toku, která není mapovaná z UPMM
            if (fromData.Category == Constants.UML_AD_INITIAL_ACTIVITY || toData.Category == Constants.UML_AD_FINAL_ACTIVITY
                || fromData.Category == Constants.UML_AD_DECISION || toData.Category == Constants.UML_AD_DECISION
                || fromData.Category == Constants.UML_AD_DECISION_MERGE || toData.Category == Constants.UML_AD_DECISION_MERGE
                || fromData.Category == Constants.UML_AD_FORK || toData.Category == Constants.UML_AD_FORK
                || fromData.Category == Constants.UML_AD_JOIN || toData.Category == Constants.UML_AD_JOIN)
            {
                linkData.Color = Constants.VALID_COLOR;
                if (linkData.Guide == null || linkData.Guide == "")
                {
                    linkData.Guide = "[Guide]";
                }
                if ((diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Where(x => x.From == linkData.From && x.To == linkData.To).Count() == 0)
                {
                    (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Add(linkData);
                }
            }
            // jinak zkontroluj vztah z UPMM
            else
            {
                // metoda CheeckADRelationShip může vracet dvě možnosti
                // 1. null - to znamená, že je ZAPNUTÁ validace a hrana není datově obsažena v profilu
                // 2. ekvivalentní namapovaný vztah z UPMM a barvu pro hranu podle toho, jestli je hrana v profilu nebo není
                string color = Constants.VALID_COLOR;
                string relationship = uml4upmm.CheckADRelationship(fromData.IRI, toData.IRI, ActivityDiagramPage.IsValidatingWithModel, out color);

                // možnost 1
                if (relationship == null)
                {
                    (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Remove(linkData);
                    return;
                }
                // možnost 2
                linkData.Guide = "<<" + relationship + ">>";
                linkData.Color = color;
                if ((diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Where(x => x.From == linkData.From && x.To == linkData.To).Count() > 1)
                {
                    (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Remove(linkData);
                }
                else if ((diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Where(x => x.From == linkData.From && x.To == linkData.To).Count() == 0)
                {
                    (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>).Add(linkData);
                }


            }

        }


        // metoda pro výběr typu hrany - anchor, object flow nebo control flow
        private void ChooseLinkCategory(ActivityDiagramNodeData fromData, ActivityDiagramNodeData toData, ActivityDiagramLinkData linkData)
        {
            if ((fromData.Category == Constants.UML_AD_OBJECT || toData.Category == Constants.UML_AD_OBJECT) && (fromData.Category != Constants.UML_AD_NOTE && toData.Category != Constants.UML_AD_NOTE))
            {
                linkData.Category = Constants.UML_AD_OBJECT_FLOW;
            }
            else if (fromData.Category == Constants.UML_AD_NOTE || toData.Category == Constants.UML_AD_NOTE)
            {
                linkData.Category = Constants.UML_AD_ANCHOR;
            }
            else
                linkData.Category = Constants.UML_AD_CONTROL_FLOW;
        }

        // metoda pro uložení diagramu do obrázku png
        private void SavePNG_Click(object sender, RoutedEventArgs e)
        {
            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramBmp = diagramUtils.MakeBitmap(diagram.Panel);
            diagramUtils.SavePngDialog(diagramBmp, defaultFilename: "ActivityDiagram.png");

        }
        // metoda pro serializaci datového modelu grafu do kotr xml

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if ((diagram.NodesSource as ObservableCollection<ActivityDiagramNodeData>).Count == 0)
                return;

            var model = diagram.Model as GraphLinksModel<ActivityDiagramNodeData, String, String, ActivityDiagramLinkData>;
            if (model == null) return;
            XElement root = model.Save<ActivityDiagramNodeData, ActivityDiagramLinkData>(Constants.UML_AD_XML_ROOT_STRING, Constants.UML_AD_XML_NODE_STRING, Constants.UML_AD_XML_LINK_STRING);
            root.SetAttributeValue(Constants.UML_AD_XML_VALIDATION_ATTRIBUTE_STRING, ValidationComboBox.SelectedIndex);

            DiagramUtils diagramUtils = new DiagramUtils();
            diagramUtils.SaveDiagramDialog(root, "ActivityDiagram.kotr");
        }
        // metoda pro deserializaci datového modelu grafu z kotr xml

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var diagramModel = diagram.Model as GraphLinksModel<ActivityDiagramNodeData, String, String, ActivityDiagramLinkData>;
            if (diagramModel == null) return;

            DiagramUtils diagramUtils = new DiagramUtils();
            var diagramXml = diagramUtils.LoadDiagramDialog();
            try
            {
                if (diagramXml == null)
                {
                    return;
                }
                if (diagramXml.Name != Constants.UML_AD_XML_ROOT_STRING)
                {
                    throw new ProcessManagerException("This file cannot be imported, because it contains different diagram type data.");
                }
                //vypnu validaci z UPMM(protože diagram může být uložený jako nevalidovaný)
                var validationAttribute = diagramXml.Attribute(Constants.UML_AD_XML_VALIDATION_ATTRIBUTE_STRING);
                if (validationAttribute != null)
                {
                    ValidationComboBox.SelectedIndex = Int32.Parse(validationAttribute.Value);
                }
                else
                {
                    ValidationComboBox.SelectedIndex = 0;
                }

                //zkontroluju, jestli všechny uzly, které mají IRI mají svůj elemnet v načteném profilu procesu
                var loadedModel = new GraphLinksModel<ActivityDiagramNodeData, string, string, ActivityDiagramLinkData>();
                loadedModel.Load<ActivityDiagramNodeData, ActivityDiagramLinkData>(diagramXml, Constants.UML_AD_XML_NODE_STRING, Constants.UML_AD_XML_LINK_STRING);

                string[] categories = { Constants.UML_AD_ACTIVITY, Constants.UML_AD_OBJECT, Constants.UML_AD_SWIMLANE, Constants.UML_AD_NOTE, Constants.UML_AD_SEND_SIGNAL_ACTION, Constants.UML_AD_ACCEPT_EVENT_ACTION };
                int countOfMissing = 0;
                foreach (string IRI in (loadedModel.NodesSource as ObservableCollection<ActivityDiagramNodeData>).Where(x => categories.Contains(x.Category)).Select(x => x.IRI))
                {
                    if (!softwareProcessProfile.Any(x => x.IRI == IRI))
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
                    diagramModel.Load<ActivityDiagramNodeData, ActivityDiagramLinkData>(diagramXml, Constants.UML_AD_XML_NODE_STRING, Constants.UML_AD_XML_LINK_STRING);
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
                ActivityDiagramPage.IsValidatingWithModel = true;
                Validate.IsEnabled = true;

            }
            else
            {
                ActivityDiagramPage.IsValidatingWithModel = false;

                if (Validate != null)
                    Validate.IsEnabled = false;

                if (diagram == null || diagram.Model == null || diagram.Model.NodesSource == null)
                    return;
                var originalNodesSource = (diagram.Model.NodesSource as ObservableCollection<ActivityDiagramNodeData>);

                foreach (var nodeData in originalNodesSource)
                {
                    nodeData.Text = null;
                }
            }
        }

        // vynucení validace diagramu
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            // kontrola všech hran
            var originalLinksSource = new ObservableCollection<ActivityDiagramLinkData>();

            foreach (ActivityDiagramLinkData link in (diagram.LinksSource as ObservableCollection<ActivityDiagramLinkData>))
            {
                originalLinksSource.Add(link);
            }

            diagram.LinksSource = new ObservableCollection<ActivityDiagramLinkData>();

            foreach (var linkData in originalLinksSource)
            {
                var fromData = (diagram.Model.NodesSource as ObservableCollection<ActivityDiagramNodeData>).Where(x => x.Key == linkData.From).First();
                var toData = (diagram.Model.NodesSource as ObservableCollection<ActivityDiagramNodeData>).Where(x => x.Key == linkData.To).First();

                ChooseLinkCategory(fromData, toData, linkData);
                switch (linkData.Category)
                {
                    case "Control Flow":
                        CheckLink(fromData, toData, linkData);
                        break;
                    case "Object Flow":
                        CheckLink(fromData, toData, linkData);
                        break;
                    case "Anchor":
                        CheckLink(fromData, toData, linkData);
                        break;
                }

            }

            // kontrola všech elementů, které jsou ve swimlane
            var originalNodesSource = (diagram.Model.NodesSource as ObservableCollection<ActivityDiagramNodeData>);

            foreach (var nodeData in originalNodesSource)
            {
                if (nodeData.Category == Constants.UML_AD_SWIMLANE || nodeData.SubGraphKey == null || nodeData.SubGraphKey == "")
                {
                    nodeData.BorderColor = Constants.VALID_COLOR;
                    nodeData.Text = null;
                    continue;
                }

                string groupIRI = (diagram.Model.NodesSource as ObservableCollection<ActivityDiagramNodeData>).Where(x => x.Key == nodeData.SubGraphKey).Select(x => x.IRI).FirstOrDefault();
                if (groupIRI == "" || groupIRI == null)
                    continue;


                string nodeIRI = nodeData.IRI;

                if (nodeIRI != null)
                {

                    var item = softwareProcessProfile.Where(x => x.IRI == groupIRI).Select(x => x as UPMM.Context).FirstOrDefault();
                    int count = item.Executes.Where(x => x.IRI == nodeIRI).Count() + item.Satisfies.Where(x => x.IRI == nodeIRI).Count() + item.Scopes.Where(x => x.IRI == nodeIRI).Count();
                    if (count == 0)
                    {
                        nodeData.BorderColor = Constants.INVALID_COLOR;
                        nodeData.Text = Constants.UML_AD_UNSUPPORTED;
                    }
                    else
                    {
                        nodeData.BorderColor = Constants.VALID_COLOR;
                        nodeData.Text = null;
                    }
                }
            }
        }

        // povoluje nebo zakazuje duplikátní uzly
        private void DuplicatesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DuplicatesComboBox.SelectedIndex == 1)
            {
                ActivityDiagramPage.AllowDuplicateNodes = false;
            }
            else
            {
                ActivityDiagramPage.AllowDuplicateNodes = true;
            }
        }



    }

    // tool pro vkládání uzlů do swimlane -- obsahuje validaci pro drag-and-drop do swimlane
    public class SwimlaneDraggingTool : DraggingTool
    {
        List<UPMM.SoftwareProcessElement> process = OWLParser.OWLAPI.GetSoftwareProcess();
        public override bool IsValidMember(Group group, Node node)
        {
            try
            {
                if (group == null)
                {
                    (node.Data as ActivityDiagramNodeData).BorderColor = Constants.VALID_COLOR;
                    (node.Data as ActivityDiagramNodeData).Text = null;
                    return true;
                }

                if (node != null)
                {

                    if (node.Category == Constants.UML_AD_SWIMLANE) return false;
                    string groupIRI = (group.Data as ActivityDiagramNodeData).IRI;
                    string nodeIRI = (node.Data as ActivityDiagramNodeData).IRI;

                    if (nodeIRI != null)
                    {
                        if (ActivityDiagramPage.IsValidatingWithModel)
                        {
                            var item = process.Where(x => x.IRI == groupIRI).Select(x => x as UPMM.Context).FirstOrDefault();
                            int count = item.Executes.Where(x => x.IRI == nodeIRI).Count() + item.Satisfies.Where(x => x.IRI == nodeIRI).Count() + item.Scopes.Where(x => x.IRI == nodeIRI).Count();
                            if (count == 0)
                            {
                                (node.Data as ActivityDiagramNodeData).BorderColor = Constants.INVALID_COLOR;
                                (node.Data as ActivityDiagramNodeData).Text = Constants.UML_AD_UNSUPPORTED;
                            }
                            else
                            {
                                (node.Data as ActivityDiagramNodeData).BorderColor = Constants.VALID_COLOR;
                                (node.Data as ActivityDiagramNodeData).Text = null;
                            }
                        }
                        else
                        {
                            var item = process.Where(x => x.IRI == groupIRI).Select(x => x as UPMM.Context).FirstOrDefault();
                            int count = item.Executes.Where(x => x.IRI == nodeIRI).Count() + item.Satisfies.Where(x => x.IRI == nodeIRI).Count() + item.Scopes.Where(x => x.IRI == nodeIRI).Count();
                            if (count == 0)
                            {
                                (node.Data as ActivityDiagramNodeData).BorderColor = Constants.INVALID_COLOR;
                                (node.Data as ActivityDiagramNodeData).Text = null;
                            }
                            else
                            {
                                (node.Data as ActivityDiagramNodeData).BorderColor = Constants.VALID_COLOR;
                                (node.Data as ActivityDiagramNodeData).Text = null;
                            }
                        }
                    }

                    return true;

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

    // tooly pro zajištění korektnosti hrany - není možná reflexivní hrana
    public class ActivityDiagramLinkingTool : LinkingTool
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

    public class ActivityDiagramRelinkingTool : RelinkingTool
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

