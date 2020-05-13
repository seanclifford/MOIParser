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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MOIParser
{
    /// <summary>
    /// Populates a grid with MOI Files and another with parsing errors
    /// </summary>
    public partial class MOIFileViewer : Window
    {
        public MOIFileViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Populate the MOI file grid
        /// </summary>
        public void PopulateFileGrid(IEnumerable<MOIFile> moiFiles)
        {
            PopulateDataGrid(moiFileGrid, moiFiles);
        }

        /// <summary>
        /// Populate the errors grid
        /// </summary>
        public void PopulateErrorGrid(IEnumerable<MOIParserError> moiErrors)
        {
            PopulateDataGrid(moiErrorGrid, moiErrors);
        }

        /// <summary>
        /// Method used to the data grids. It will hide the grid if there is nothing to display.
        /// </summary>
        private void PopulateDataGrid<T>(DataGrid dataGrid, IEnumerable<T> items)
        {
            dataGrid.ItemsSource = items;

            //if there are no items, hide the grid and the splitter bar
            if (items.Count() == 0)
            {
                int mainGridRowIndex = Grid.GetRow(dataGrid);
                mainGrid.RowDefinitions[mainGridRowIndex].Height = new GridLength(0);

                splitterRow.Height = new GridLength(0);
            }
        }
    }
}
