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
using NUnit.Framework;
using MOIParser;

namespace MOITests
{
    [TestFixture]
    public class MOIPathParserTest
    {
        [Test]
        public void TestFilePass()
        {
            string relativePath = "..\\..\\TestFiles\\MOV045.MOI";
            MOIPathParser pathParser = new MOIPathParser(relativePath);
            pathParser.Parse();

            Assert.IsEmpty(pathParser.ParseErrors.ToList());
            Assert.IsNotEmpty(pathParser.ParsedMOIFiles.ToList());
        }

        [Test]
        public void TestDirectoryPass()
        {
            string relativePath = "..\\..\\TestFiles\\";
            MOIPathParser pathParser = new MOIPathParser(relativePath);
            pathParser.Parse();

            Assert.IsEmpty(pathParser.ParseErrors.ToList());
            Assert.IsNotEmpty(pathParser.ParsedMOIFiles.ToList());
        }

        [Test]
        [ExpectedException(typeof(ApplicationException))]
        public void TestNoFile()
        {
            string relativePath = "..\\..\\TestFiles\\NOTHERE.MOI";
            MOIPathParser pathParser = new MOIPathParser(relativePath);
            pathParser.Parse();
        }
    }
}
