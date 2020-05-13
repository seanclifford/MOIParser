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
using System.IO;

namespace MOIParser
{
    /// <summary>
    /// Parses multiple .MOI files if given a directory path, or a single .MOI file if given a file path.
    /// </summary>
    public class MOIPathParser
    {
        private const string MoiExtension = ".MOI";

        private List<string> moiFilePaths;

        /// <summary>
        /// Constructor for
        /// </summary>
        /// <param name="moiPath">The path to find the MOI file(s) at. This can be a directory or a file path.</param>
        /// <exception cref="ApplicationException">File access exceptions</exception>
        public MOIPathParser(string moiPath)
        {
            LoadFilesToParse(moiPath);
        }

        private List<MOIFile> parsedMoiFiles;
        /// <summary>The MOI file objects generated from the Parse() method.</summary>
        public IEnumerable<MOIFile> ParsedMOIFiles { get { return parsedMoiFiles; } }

        private List<MOIParserError> parseErrors;
        /// <summary>Any exceptions that resulted from parsing.</summary>
        public IEnumerable<MOIParserError> ParseErrors { get { return parseErrors; } }

        /// <summary>
        /// Called by the constructor to populate the list of moi file paths that the Parse method will use.
        /// </summary>
        /// <param name="moiPath">The path to find the MOI file(s) at. This can be a directory or a file path.</param>
        /// <exception cref="ApplicationException">File access exceptions</exception>
        private void LoadFilesToParse(string moiPath)
        {
            moiFilePaths = new List<string>();

            //Check if the path is a directory and if it is, load all the MOI file within into moiFilePaths
            if (Directory.Exists(moiPath))
            {
                string[] moiFilePathArray = Directory.GetFiles(moiPath, "*" + MoiExtension);
                moiFilePaths.AddRange(moiFilePathArray);
            }
            //If the file is a path, load only this path into moiFilePaths
            else if (File.Exists(moiPath))
            {
                string fileExtension = Path.GetExtension(moiPath);

                //Check that the file is actually an MOI file before adding it.
                if (String.Compare(fileExtension, MoiExtension, true) != 0)
                    throw new ApplicationException(String.Format("The file \"{0}\" is does not have an .MOI extension.", moiPath));//TODO: Add error message

                moiFilePaths.Add(moiPath);
            }
            else
            {
                //If the path is not a path or directory, or it doesn't exist, throw it.
                throw new ApplicationException(String.Format("Cannot find file or path \"{0}\"", moiPath));
            }
        }

        /// <summary>
        /// This parses the MOI files and populates ParsedMOIFiles and ParseExceptions.
        /// </summary>
        /// <returns>If there were no parsing errors</returns>
        public bool Parse()
        {
            parsedMoiFiles = new List<MOIFile>();
            parseErrors = new List<MOIParserError>();

            //Parse each path to create an MOIFile or ParserError
            foreach (string moiFilePath in moiFilePaths)
            {
                try
                {
                    byte[] moiFileData = File.ReadAllBytes(moiFilePath);
                    MOIFileParser fileParser = new MOIFileParser(moiFileData);

                    if (fileParser.Parse())
                    {
                        fileParser.MOIFile.FileName = Path.GetFileName(moiFilePath);
                        parsedMoiFiles.Add(fileParser.MOIFile);
                    }
                    else
                    {
                        fileParser.ParseError.FilePath = moiFilePath;
                        parseErrors.Add(fileParser.ParseError);
                    }
                }
                catch (Exception e) //if an unexpected error occured, create a ParserError for it.
                {
                    MOIParserError parseError = new MOIParserError(e);
                    parseError.FilePath = moiFilePath;
                    parseErrors.Add(parseError);
                }
            }

            return parseErrors.Count == 0;
        }

    }
}
