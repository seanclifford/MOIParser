﻿/* Copyright © 2011, Sean Clifford
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

namespace MOIParser
{
    /// <summary>
    /// Holds parsed information from a single .MOI file
    /// </summary>
    /// <remarks>An MOI file is a binary metadata file generated by JVC camcorders. 
    /// They are associated with MOD or TOD video files whose content they represent.</remarks>
    public class MOIFile
    {
        public string FileName { get; set; }

        public string Version { get; set; }

        public uint FileSize { get; set; }

        public DateTime CreationDate { get; set; }

        public TimeSpan VideoLength { get; set; }

        public AspectRatio AspectRatio { get; set; }

        public TVSystem TVSystem { get; set; }
    }

    public enum AspectRatio
    {
        /// <summary>Unrecognised aspect ratio</summary>
        Unknown,
        /// <summary>4:3 aspect ratio</summary>
        _4_3,
        /// <summary>16:9 aspect ratio</summary>
        _16_9
    }

    public enum TVSystem
    {
        /// <summary>Unrecognised TV system</summary>
        Unknown,
        /// <summary>NTSC TV system</summary>
        NTSC,
        /// <summary>PAL TV system</summary>
        PAL
    }

}
