﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Helpers.cs
// Version        : 1.0
// Author(s)      : Natalia Portillo
//
// Component      : Component
//
// Revision       : $Revision$
// Last change by : $Author$
// Date           : $Date$
//
// --[ Description ] ----------------------------------------------------------
//
// Description
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright (C) 2011-2015 Claunia.com
// ****************************************************************************/
// //$Id$
using System;
namespace DiscImageChef.Core
{
    public static partial class Sidecar
    {
        static string LbaToMsf(long lba)
        {
            long m, s, f;
            if(lba >= -150)
            {
                m = (lba + 150) / (75 * 60);
                lba -= m * (75 * 60);
                s = (lba + 150) / 75;
                lba -= s * 75;
                f = lba + 150;
            }
            else
            {
                m = (lba + 450150) / (75 * 60);
                lba -= m * (75 * 60);
                s = (lba + 450150) / 75;
                lba -= s * 75;
                f = lba + 450150;
            }

            return string.Format("{0}:{1:D2}:{2:D2}", m, s, f);
        }

        static string DdcdLbaToMsf(long lba)
        {
            long h, m, s, f;
            if(lba >= -150)
            {
                h = (lba + 150) / (75 * 60 * 60);
                lba -= h * (75 * 60 * 60);
                m = (lba + 150) / (75 * 60);
                lba -= m * (75 * 60);
                s = (lba + 150) / 75;
                lba -= s * 75;
                f = lba + 150;
            }
            else
            {
                h = (lba + 450150 * 2) / (75 * 60 * 60);
                lba -= h * (75 * 60 * 60);
                m = (lba + 450150 * 2) / (75 * 60);
                lba -= m * (75 * 60);
                s = (lba + 450150 * 2) / 75;
                lba -= s * 75;
                f = lba + 450150 * 2;
            }

            return string.Format("{3}:{0:D2}:{1:D2}:{2:D2}", m, s, f, h);
        }
    }
}
