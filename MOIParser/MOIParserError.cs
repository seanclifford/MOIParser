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

namespace MOIParser
{
    /// <summary>
    /// Represents an error encountered during parsing.
    /// </summary>
    public class MOIParserError
    {
        /// <summary>
        /// Constructor for an expected error.
        /// </summary>
        /// <param name="errorId">A unique string to identify the error from others.</param>
        /// <param name="errorMessage">The error message.</param>
        public MOIParserError(string errorId, string errorMessage)
        {
            ErrorId = errorId;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Constructor for an unexpected error.
        /// </summary>
        /// <param name="exception">The unexpected exception</param>
        public MOIParserError(Exception exception)
        {
            ErrorId = "UNKNOWN";
            ErrorMessage = "Unexpected error occcured during parsing.";
            InnerException = exception;
        }

        /// <summary>
        /// A unique string to identify the error from others.
        /// </summary>
        public string ErrorId { get; private set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// If this is an unexpected error, this will hold the exception that was encountered.
        /// </summary>
        public Exception InnerException { get; private set; }

        /// <summary>
        /// The path of the file that failed during parsing.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Returns the Error Message and the InnerException if its set.
        /// </summary>
        public override string ToString()
        {
            string str = ErrorMessage;
            if (InnerException != null)
                str += Environment.NewLine + InnerException;

            return str;
        }
    }
}
