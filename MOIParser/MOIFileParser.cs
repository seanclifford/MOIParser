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
using System.IO;

namespace MOIParser
{
    /// <summary>
    /// Parses a single .MOI file into a MOIFile object.
    /// </summary>
    public class MOIFileParser
    {
        /* From Wikipedia article on MOI file format (I couldn't find any official documentation)
         * 00-01 	Version 				56 36 (V6)
         * 02-05 	MOI filesize (bytes) 	00 00 01 C3 (451 bytes)
         * 06-07 	Year 					07 D8 (2008)
         * 08  		Month 					07 (July)
         * 09 		Day 					04 (4th)
         * 0A 		Hour 					0B (11)
         * 0B 		Minutes 				16 (22)
         * 0E-11 	Video length (ms) 		00 08 9D 00 (564480 ms, 9 mn 24 s 12 frames)
         * 80 		Video format 			Low nibble: 0 and 1 for 4:3, 4 and 5 for 16:9. High nibble: 4 for NTSC, 5 for PAL.
         */
        const int VERSION_POS = 0x00;
        const int FILE_SIZE_POS = 0x02;
        const int YEAR_POS = 0x06;
        const int MONTH_POS = 0x08;
        const int DAY_POS = 0x09;
        const int HOUR_POS = 0x0A;
        const int MIN_POS = 0x0B;
        const int VIDEO_LENGTH_POS = 0x0E;
        const int VIDEO_FMT_POS = 0x80;
        
        private byte[] moiData;
        private MOIFile moiFile;

        /// <summary>
        /// Constructor which takes a byte array loaded from an MOI file.
        /// </summary>
        /// <param name="moiFileData">The raw binary file data.</param>
        public MOIFileParser(byte[] moiFileData)
        {
            this.moiData = moiFileData;
        }

        /// <summary>
        /// The parsed data.
        /// </summary>
        /// <exception cref="ApplicationException">Throws if parsing if has been run.</exception>
        public MOIFile MOIFile
        {
            get { return moiFile;}
        }

        /// <summary>
        /// This will store an exception if the Parse method fails.
        /// </summary>
        private MOIParserError parserError;
        public MOIParserError ParseError
        {
            get { return parserError; }
            private set
            {
                moiFile = null; //MOI File should removed when we hit an error.
                parserError = value;
            }
        }

        /// <summary>
        /// Parses the file data into an MOIFile object which is stored in the MOIFile property.
        /// </summary>
        /// <returns>If parsing passed</returns>
        public bool Parse()
        {
            try
            {
                //Pull data out from the byte array into variables
                string version = GetString(VERSION_POS, 2);
                uint moiFileSize = GetUInt32(FILE_SIZE_POS);
                ushort year = GetUInt16(YEAR_POS);
                byte month = GetByte(MONTH_POS);
                byte day = GetByte(DAY_POS);
                byte hour = GetByte(HOUR_POS);
                byte minute = GetByte(MIN_POS);
                uint videoLengthMs = GetUInt32(VIDEO_LENGTH_POS);
                byte videoFmt = GetByte(VIDEO_FMT_POS);

                //Create a MOI file object to store the parsed values.
                moiFile = new MOIFile { Version = version, FileSize = moiFileSize };

                if (!SetCreationDate(year, month, day, hour, minute))
                    return false;

                if (!SetVideoLength(videoLengthMs))
                    return false;

                moiFile.AspectRatio = ParseAspectRatio(videoFmt);
                moiFile.TVSystem = ParseTVSystem(videoFmt);

                return true;
            }
            catch (Exception e)
            {
                ParseError = new MOIParserError(e);
            }

            return false;
        }

        /// <summary>
        /// Sets the CreationDate field on the moiFile.
        /// </summary>
        /// <returns>If setting the video length was successful.</returns>
        /// <remarks>This handles the possible errors resulting from the instantiation of a DateTime.</remarks>
        private bool SetCreationDate(ushort year, byte month, byte day, byte hour, byte minute)
        {
            try
            {
                moiFile.CreationDate = new DateTime(year, month, day, hour, minute, 0);
            }
            catch (ArgumentException)
            {
                string errorMessage = String.Format("Could not parse the creation date. {2}\\{1}\\{0} {3}:{4} is not a valid date and time.",
                    year, month, day, hour, minute);
                ParseError = new MOIParserError("CreationDate", errorMessage);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the VideoLength field on the moiFile.
        /// </summary>
        /// <param name="videoLengthMs">video length in milliseconds.</param>
        /// <returns>If setting the video length was successful.</returns>
        /// <remarks>This handles the possible errors resulting from the instantiation of a TimeSpan.</remarks>
        private bool SetVideoLength(uint videoLengthMs)
        {
            try
            {
                moiFile.VideoLength = TimeSpan.FromMilliseconds(videoLengthMs);
            }
            catch (SystemException e)
            {
                //Handle the docuemented possible exceptions.
                if (e is ArgumentException || e is OverflowException)
                {
                    string errorMessage = String.Format("Could not parse the video length. {0} could not be converted to a length of time.",
                        videoLengthMs);
                    ParseError = new MOIParserError("VideoLength", errorMessage);
                    return false;
                }
                else //Unexpected exception
                {
                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Parse the Aspect Ratio out of the video format byte.
        /// </summary>
        /// <param name="videoFmt">The byte that holds the aspect ratio and TV system</param>
        /// <remarks>The Video Format byte contains 2 sets of data: the aspect ratio in the low nibble and the TV system in the high nibble.
        /// Low nibble: 0 and 1 for 4:3, 4 and 5 for 16:9.</remarks>
        private AspectRatio ParseAspectRatio(byte videoFmt)
        {
            int lowNibble = videoFmt & 15; //mask the high nibble

            switch (lowNibble)
            {
                case 0:
                case 1:
                    return AspectRatio._4_3;
                case 4:
                case 5:
                    return AspectRatio._16_9;
                default:
                    return AspectRatio.Unknown;
            }
        }

        /// <summary>
        /// Parse the TV System out of the video format byte.
        /// </summary>
        /// <param name="videoFmt">The byte that holds the aspect ratio and TV system</param>
        /// <remarks>The Video Format byte contains 2 sets of data: the aspect ratio in the low nibble and the TV system in the high nibble.
        /// High nibble: 4 for NTSC, 5 for PAL.</remarks>
        private TVSystem ParseTVSystem(byte videoFmt)
        {
            int highNibble = videoFmt >> 4; //shift high nibble to low

            switch (highNibble)
            {
                case 4:
                   return TVSystem.NTSC;
                case 5:
                    return TVSystem.PAL;
                default:
                   return TVSystem.Unknown;
            }
        }

        /// <summary>
        /// Validate that there is a value in the moiData array at the position.
        /// </summary>
        /// <param name="pos"></param>
        private void ValidateDataIsAtPosition(int pos)
        {
            if (moiData.Length <= pos)
                throw new ApplicationException("File does not contain enough data.");
        }

        /// <summary>
        /// Returns an ASCII string at the specified byte postion for the specified length.
        /// </summary>
        /// <param name="pos">byte position.</param>
        /// <returns>the string at the byte position.</returns>
        private string GetString(int pos, int length)
        {
            ValidateDataIsAtPosition(pos);
            return Encoding.ASCII.GetString(moiData, pos, length);
        }

        /// <summary>
        /// Returns the byte at the specified byte postion.
        /// </summary>
        /// <param name="pos">byte position.</param>
        /// <returns>the byte at the byte position.</returns>
        private byte GetByte(int pos)
        {
            ValidateDataIsAtPosition(pos);
            return moiData[pos];
        }

        /// <summary>
        /// Returns the 2 bytes at and after the specified byte postion.
        /// </summary>
        /// <param name="pos">byte position.</param>
        /// <returns>An unsigned 16 bit int</returns>
        /// <remarks>This handles the endianess of the MOI files (BigEndian) and converts to little endian if needed.</remarks>
        private ushort GetUInt16(int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (ushort)ConvertToIntFromBigEndian(pos, 16 / 8);
            }
            else
            {
                ValidateDataIsAtPosition(pos + 1);
                return BitConverter.ToUInt16(moiData, pos);
            }
        }

        /// <summary>
        /// Returns the 4 bytes at and after the specified byte postion.
        /// </summary>
        /// <param name="pos">byte position.</param>
        /// <returns>An unsigned 32 bit int</returns>
        /// <remarks>This handles the endianess of the MOI files (big-endian) and converts to little-endian if needed.</remarks>
        private uint GetUInt32(int pos)
        {
            if (BitConverter.IsLittleEndian)
            {
                return (uint)ConvertToIntFromBigEndian(pos, 32 / 8);
            }
            else
            {
                ValidateDataIsAtPosition(pos + 3);
                return BitConverter.ToUInt32(moiData, pos);
            }
        }

        /// <summary>
        /// Gets the value at the defined position after flipping the bytes around to little-endian.
        /// </summary>
        /// <param name="pos">Position in the data array</param>
        /// <param name="numBytes">Number of bytes to process (should be more than 1)</param>
        /// <remarks>As JVC use the big-endian format, and x86 / x86-64 architectures use the little-endian format, 
        /// we need to flip the bytes around when retrieving values more than 1 byte in length.</remarks>
        /// <returns></returns>
        private int ConvertToIntFromBigEndian(int pos, int numBytes)
        {
            const int bitMultiplier = 8;
            int result = 0;
            int bitShift = bitMultiplier * numBytes;

            for (int i = 0; i < numBytes; i++)
            {
                //reduce bitShift before using it so on the last loop it will be 0 (do nothing)
                bitShift -= bitMultiplier;

                //Shift the byte left
                int shiftedByte = GetByte(pos + i) << bitShift;

                //combine the shifted bits to the result
                result = result | shiftedByte;             
            }

            return result;
        }
    }
}
