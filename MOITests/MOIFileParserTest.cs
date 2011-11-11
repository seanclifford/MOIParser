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
    public class MOIFileParserTest
    {
        byte[] testBytes = new byte[]{
            0x56, 0x36, 0x00, 0x00, 0x01, 0x16, 0x07, 0xd9, 0x07, 0x11, 0x0e, 0x12, 0xc3, 0x50, 0x00, 0x00, 
            0x05, 0xa0, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 
            0x55, 0x00, 0x01, 0x00, 0x00, 0xc1, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private MOIFile ParseMOIExpectSuccess(byte[] moiData)
        {
            MOIFileParser fileParser = new MOIFileParser(moiData);
            bool result = fileParser.Parse();

            Assert.IsTrue(result, "Parsing failed due to the following error: {0}", fileParser.ParseError);

            Assert.NotNull(fileParser.MOIFile, "No MOIFile generated.");
            Assert.Null(fileParser.ParseError, "ParseError should not have been set if parsing succeeded.");

            return fileParser.MOIFile;
        }

        private MOIParserError ParseMOIExpectFail(byte[] moiData)
        {
            MOIFileParser fileParser = new MOIFileParser(moiData);
            bool result = fileParser.Parse();

            Assert.IsFalse(result, "Parsing should have failed");

            Assert.NotNull(fileParser.ParseError, "No ParseError generated.");
            Assert.Null(fileParser.MOIFile, "MOIFile should not have been set if parsing failed.");

            return fileParser.ParseError;
        }

        [Test]
        public void FullSuccess()
        {
            MOIFile moiFile = ParseMOIExpectSuccess(testBytes);

            Assert.AreEqual("V6", moiFile.Version);
            Assert.AreEqual(AspectRatio._16_9, moiFile.AspectRatio);
            Assert.AreEqual(TVSystem.PAL, moiFile.TVSystem);
            Assert.AreEqual(new TimeSpan(14400000), moiFile.VideoLength);
            Assert.AreEqual(new DateTime(2009, 7, 17, 14, 18, 0), moiFile.CreationDate);
            Assert.AreEqual(278, moiFile.FileSize);
            Assert.AreEqual(null, moiFile.FileName);
        }

        [Test]
        public void FailNoData()
        {
            MOIParserError parserError = ParseMOIExpectFail(new byte[0]);

            Assert.AreEqual("UNKNOWN", parserError.ErrorId);
            Assert.NotNull(parserError.InnerException);
            Assert.IsInstanceOf<ApplicationException>(parserError.InnerException);
        }

        [TestCase("V6")]
        [TestCase("  ")]
        [TestCase("~+")]
        public void VersionTest(string version)
        {
            byte[] copyTestBytes = (byte[])testBytes.Clone();
            copyTestBytes[0x00] = (byte)version[0];
            copyTestBytes[0x01] = (byte)version[1];

            MOIFile moiFile = ParseMOIExpectSuccess(copyTestBytes);

            Assert.AreEqual(version, moiFile.Version);
        }

        [TestCase(0)]
        [TestCase(0xFFFFFFF)]
        [TestCase(0x1234567)]
        public void FileSizeTest(int fileSize)
        {
            byte[] copyTestBytes = (byte[])testBytes.Clone();
            copyTestBytes[0x02] = (byte)(fileSize >> 24);
            copyTestBytes[0x03] = (byte)((fileSize >> 16) & 0xFF);
            copyTestBytes[0x04] = (byte)((fileSize >> 8) & 0xFF);
            copyTestBytes[0x05] = (byte)(fileSize & 0xFF);

            MOIFile moiFile = ParseMOIExpectSuccess(copyTestBytes);

            Assert.AreEqual(fileSize, moiFile.FileSize);
        }

        private byte[] GetCreationDateData(int year, int month, int day, int hour, int minute)
        {
            byte[] copyTestBytes = (byte[])testBytes.Clone();
            copyTestBytes[0x06] = (byte)(year >> 8);
            copyTestBytes[0x07] = (byte)(year & 0xFF);
            copyTestBytes[0x08] = (byte)month;
            copyTestBytes[0x09] = (byte)day;
            copyTestBytes[0x0A] = (byte)hour;
            copyTestBytes[0x0B] = (byte)minute;

            return copyTestBytes;
        }

        [TestCase(1900, 1, 2, 3, 4)]
        [TestCase(2011, 11, 11, 13, 59)]
        public void CreationDateTest(int year, int month, int day, int hour, int minute)
        {
            byte[] copyTestBytes = GetCreationDateData(year, month, day, hour, minute);

            MOIFile moiFile = ParseMOIExpectSuccess(copyTestBytes);

            Assert.AreEqual(new DateTime(year, month, day, hour, minute, 0), moiFile.CreationDate);
        }

        [TestCase(0, 0, 0, 0, 0, "CreationDate")]
        public void CreationDateTestFail(int year, int month, int day, int hour, int minute, string expectedErrorId)
        {
            byte[] copyTestBytes = GetCreationDateData(year, month, day, hour, minute);

            MOIParserError parseError = ParseMOIExpectFail(copyTestBytes);

            Assert.AreEqual(expectedErrorId, parseError.ErrorId);
        }

        [TestCase(0)]
        [TestCase(0xFFFFFFF)]
        [TestCase(0x1234567)]
        public void VideoLengthTest(int videoLength)
        {
            byte[] copyTestBytes = (byte[])testBytes.Clone();
            copyTestBytes[0x0E] = (byte)(videoLength >> 24);
            copyTestBytes[0x0F] = (byte)((videoLength >> 16) & 0xFF);
            copyTestBytes[0x10] = (byte)((videoLength >> 8) & 0xFF);
            copyTestBytes[0x11] = (byte)(videoLength & 0xFF);

            MOIFile moiFile = ParseMOIExpectSuccess(copyTestBytes);

            Assert.AreEqual(videoLength, moiFile.VideoLength.TotalMilliseconds);
        }

        [TestCase(0xFF, AspectRatio.Unknown, TVSystem.Unknown)]
        [TestCase(0x40, AspectRatio._4_3, TVSystem.NTSC)]
        [TestCase(0x45, AspectRatio._16_9, TVSystem.NTSC)]
        [TestCase(0x50, AspectRatio._4_3, TVSystem.PAL)]
        [TestCase(0x55, AspectRatio._16_9, TVSystem.PAL)]
        public void VideoFormatTest(byte videoFmtByte, AspectRatio expectedAspectRatio, TVSystem expectedTVSystem)
        {
            byte[] copyTestBytes = (byte[])testBytes.Clone();
            copyTestBytes[0x80] = videoFmtByte;

            MOIFile moiFile = ParseMOIExpectSuccess(copyTestBytes);

            Assert.AreEqual(expectedAspectRatio, moiFile.AspectRatio);
            Assert.AreEqual(expectedTVSystem, moiFile.TVSystem);
        }

    }
}
