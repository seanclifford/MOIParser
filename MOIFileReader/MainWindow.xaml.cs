/* Copyright © 2011, Sean Clifford
 * This file is part of MOIParser.
 *
 *  MOIParser is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 *  MOIParser is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with MOIParser.  If not, see <http://www.gnu.org/licenses/>.
 */
using MOIParser;
using System;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace MOIFileReader
{
    /// <summary>
    /// File / Folder selection window.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Browse for a File
        /// </summary>
        private void btnFileBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "MOI files (*.MOI)|*.moi";
            if (fileDialog.ShowDialog() ?? false)
            {
                txtFile.Text = fileDialog.FileName;
            }
        }

        /// <summary>
        /// Browse for a Folder
        /// </summary>
        private void btnFolderBrowse_Click(object sender, RoutedEventArgs e)
        {
            //TODO: either remove or make this work with .NET Core

            //FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            //folderBrowserDialog.Description = "Select a folder";

            //if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            //{
            //    txtFolder.Text = folderBrowserDialog.SelectedPath;
            //}
        }

        /// <summary>
        /// File or Folder selection changed
        /// </summary>
        private void rb_Checked(object sender, RoutedEventArgs e)
        {
            if (this.IsInitialized)
            {
                txtFile.IsEnabled = rbFile.IsChecked ?? false;
                btnFileBrowse.IsEnabled = txtFile.IsEnabled;

                txtFolder.IsEnabled = rbFolder.IsChecked ?? false;
                btnFolderBrowse.IsEnabled = txtFolder.IsEnabled;
            }
        }

        /// <summary>
        /// Get the selected file or folder path
        /// </summary>
        private string GetMoiPath()
        {
            return (rbFolder.IsChecked ?? false) ? txtFolder.Text : txtFile.Text;
        }

        /// <summary>
        /// Button event that does the parsing of the files and opens a form to display the results.
        /// </summary>
        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string moiPath = GetMoiPath();
                MOIPathParser moiParser = new MOIPathParser(moiPath);
                moiParser.Parse();

                DisplayParserResults(moiParser);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message + Environment.NewLine + ex.StackTrace, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Displays the results if there are any
        /// </summary>
        private void DisplayParserResults(MOIPathParser moiParser)
        {
            if (moiParser.ParsedMOIFiles.Count() == 0 && moiParser.ParseErrors.Count() == 0)
            {
                MessageBox.Show(this, "No MOI Files Found", this.Title, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                OpenMOIFileViewerForm(moiParser);
            }
        }

        /// <summary>
        /// Opens the MOI file viewer form to show the parse results.
        /// </summary>
        /// <param name="moiParser">The parser which should have already run</param>
        private void OpenMOIFileViewerForm(MOIPathParser moiParser)
        {
            MOIFileViewer moiFileViewer = new MOIFileViewer();
            moiFileViewer.PopulateFileGrid(moiParser.ParsedMOIFiles);
            moiFileViewer.PopulateErrorGrid(moiParser.ParseErrors);

            moiFileViewer.Show();
        }
    }
}
