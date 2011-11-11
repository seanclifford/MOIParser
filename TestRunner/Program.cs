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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.ConsoleRunner;

namespace TestRunner
{
    /// <summary>
    /// Workaround to debug unit tests in Visual Studio Express 2010 (no attach to process, no running Class Libraries).
    /// Technique found at: http://www.blackwasp.co.uk/NUnitCSharpExpress.aspx
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            Runner.Main(args);
        }
    }
}
