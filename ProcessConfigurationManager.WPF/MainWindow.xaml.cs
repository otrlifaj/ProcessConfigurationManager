using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ProcessConfigurationManager.UPMM;
using ProcessConfigurationManager.OWLParser;
using SWF = System.Windows.Forms;
using ProcessConfigurationManager.WPF.UML;

namespace ProcessConfigurationManager.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<SoftwareProcessElement> softwareProcessProfile = null;
        private StartPage startPage = null;
        private About aboutPage = null;
        private ActivityDiagramPage activityDiagramPage = null;
        private ClassDiagramPage classDiagramPage = null;
        public MainWindow()
        {
            InitializeComponent();
            startPage = new StartPage();
            profileInfoLabel.Text = "Process profile file not loaded";
            profileInfoLabel.Foreground = Brushes.Red;
            ContentFrame.Navigate(startPage);
            statusLabel.Text = "Start page";

            //this.MaxWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            //this.MaxHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        }


        private void LoadProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SWF.OpenFileDialog();
            fileDialog.Filter = "OWL Files (*.owl)|*.owl";
            fileDialog.Title = "Select file with UPMM profile";
            fileDialog.FilterIndex = 0;

            statusLabel.Text = "Loading process profile...";
            var result = fileDialog.ShowDialog();

            if (result == SWF.DialogResult.OK)
            {
                try
                {
                    OWLAPI.Initialize(fileDialog.FileName);
                    softwareProcessProfile = OWLAPI.GetSoftwareProcess();
                    ActivityDiagram.IsEnabled = true;
                    ClassDiagram.IsEnabled = true;
                    profileInfoLabel.Text = "Process profile file loaded";
                    profileInfoLabel.Foreground = Brushes.Green;
                    LoadProfile.IsEnabled = false;
                    UnloadProfile.IsEnabled = true;                 
                }
                catch (ApplicationException ex)
                {
                    MessageBox.Show(ex.Message + "\n" + ex.InnerException.Message + "\n" + ex.InnerException.InnerException.Message);
                }
            }
            statusLabel.Text = null;

        }

        private void ActivityDiagramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (activityDiagramPage == null)
                {
                    activityDiagramPage = new ActivityDiagramPage(softwareProcessProfile);
                }
                ContentFrame.Navigate(activityDiagramPage);
                statusLabel.Text = "Activity Diagram";
            }
            catch (Exception ex)
            {
                new Utils().ShowExceptionMessageBox(ex);
            }
        }

        private void ClassDiagramMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (classDiagramPage == null)
                {
                    classDiagramPage = new ClassDiagramPage(softwareProcessProfile);
                }
                ContentFrame.Navigate(classDiagramPage);
                statusLabel.Text = "Class Diagram";
            }
            catch (Exception ex)
            {
                new Utils().ShowExceptionMessageBox(ex);
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (aboutPage == null)
                aboutPage = new About();
            ContentFrame.Navigate(aboutPage);
            statusLabel.Text = "About";
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void StartPageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(startPage);
            statusLabel.Text = "Start page";
        }
        private void UnloadProfileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(startPage);
            statusLabel.Text = "Start page";
            softwareProcessProfile = null;
            activityDiagramPage = null;
            classDiagramPage = null;
            ActivityDiagram.IsEnabled = false;
            LoadProfile.IsEnabled = true;
            UnloadProfile.IsEnabled = false;
            statusLabel.Text = null;
            profileInfoLabel.Text = "Process profile file not loaded";
            profileInfoLabel.Foreground = Brushes.Red;

        }

    }
}
