using Northwoods.GoXam;
using Northwoods.GoXam.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using SWF = System.Windows.Forms;

namespace ProcessConfigurationManager.WPF.UML
{
    public class DiagramUtils
    {
        public DiagramUtils()
        {
        }

        public BitmapSource MakeBitmap(DiagramPanel diagramPanel)
        {
            Rect bounds = diagramPanel.DiagramBounds;
            double width = bounds.Width;
            double height = bounds.Height;
            double scale = 1.0;

            if (width > 2000)
                scale = 2000 / width;
            if (height > 2000)
                scale = Math.Min(scale, 2000 / height);

            width = Math.Ceiling(width * scale);
            height = Math.Ceiling(height * scale);

            return diagramPanel.MakeBitmap(new Size(width, height), 96, new Point(bounds.X, bounds.Y), scale);
        }

        public void SavePngDialog(BitmapSource diagramBmp, string defaultFilename = "Diagram.png")
        {
            var fileDialog = new SWF.SaveFileDialog();
            fileDialog.Filter = "PNG Files (*.png)|*.png";
            fileDialog.Title = "Save diagram as PNG";
            fileDialog.FilterIndex = 0;
            fileDialog.FileName = defaultFilename;

            var result = fileDialog.ShowDialog();

            if (result == SWF.DialogResult.OK)
            {
                try
                {
                    PngBitmapEncoder png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(diagramBmp));
                    using (System.IO.Stream stream = System.IO.File.Create(fileDialog.FileName))
                    {
                        png.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    new Utils().ShowExceptionMessageBox(ex);
                }
            }

        }

        public void SaveDiagramDialog(XElement root, String defaultFilename)
        {
            var fileDialog = new SWF.SaveFileDialog();
            fileDialog.Filter = "KOTR Files (*.kotr)|*.kotr";
            fileDialog.Title = "Save diagram as KOTR XML";
            fileDialog.FilterIndex = 0;
            fileDialog.FileName = defaultFilename;

            var result = fileDialog.ShowDialog();

            if (result == SWF.DialogResult.OK)
            {
                try
                {
                    if (fileDialog.CheckFileExists)
                    {
                        MessageBox.Show("File with this name already exists.");
                    }
                    else
                    {
                        root.Save(fileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    new Utils().ShowExceptionMessageBox(ex);
                }
            }
        }

        public XElement LoadDiagramDialog()
        {
            XElement diagramXml = null;
            var fileDialog = new SWF.OpenFileDialog();
            fileDialog.Filter = "KOTR Files (*.kotr)|*.kotr";
            fileDialog.Title = "Load diagram from KOTR XML";
            fileDialog.FilterIndex = 0;

            var result = fileDialog.ShowDialog();

            if (result == SWF.DialogResult.OK)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(fileDialog.FileName, FileMode.Open))
                    {
                        diagramXml = XElement.Load(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    new Utils().ShowExceptionMessageBox(ex);
                }

            }
            return diagramXml;
        }
    }
}
