﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : VTOC.cs
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
using System.Collections.Generic;
using System.IO;
using DiscImageChef.CommonTypes;
using DiscImageChef.DiscImages;
using DiscImageChef.Filesystems;
using DiscImageChef.Filters;
using DiscImageChef.ImagePlugins;
using DiscImageChef.PartPlugins;
using NUnit.Framework;

namespace DiscImageChef.Tests.Partitions
{
    [TestFixture]
    public class VTOC
    {
        readonly string[] testfiles = {
            "att_unix_vtoc.vdi.lz",
        };

        readonly Partition[][] wanted = {
            // AT&T UNIX System V Release 4 Version 2.1 for 386
            new []{
                new Partition{ Description = null, Name = null, Type = "UNIX: Boot", Length = 34, Sequence = 0, Start = 2 },
                new Partition{ Description = null, Name = null, Type = "UNIX: Whole disk", Length = 1023119, Sequence = 1, Start = 2 },
                new Partition{ Description = null, Name = null, Type = "UNIX: Stand", Length = 253, Sequence = 2, Start = 64 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 3, Start = 379 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 4, Start = 79003 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 5, Start = 157627 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 6, Start = 236251 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 7, Start = 314875 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 8, Start = 393499 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 9, Start = 472123 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 10, Start = 550747 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 78624, Sequence = 11, Start = 629371 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 76608, Sequence = 12, Start = 707995 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 77616, Sequence = 13, Start = 784603 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 75600, Sequence = 14, Start = 862219 },
                new Partition{ Description = null, Name = null, Type = "UNIX: /usr", Length = 84672, Sequence = 15, Start = 937819 },
            },
        };  

        [Test]
        public void Test()
        {
            for(int i = 0; i < testfiles.Length; i++)
            {
                string location = Path.Combine(Consts.TestFilesRoot, "partitions", "vtoc", testfiles[i]);
                Filter filter = new LZip();
                filter.Open(location);
                ImagePlugin image = new VDI();
                Assert.AreEqual(true, image.OpenImage(filter), testfiles[i]);
                List<Partition> partitions = Core.Partitions.GetAll(image);
                Assert.AreEqual(wanted[i].Length, partitions.Count, testfiles[i]);
                for(int j = 0; j < partitions.Count; j++)
                {
                    // Too chatty
                    //Assert.AreEqual(wanted[i][j].PartitionDescription, partitions[j].PartitionDescription, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Length * 512, partitions[j].Size, testfiles[i]);
//                    Assert.AreEqual(wanted[i][j].Name, partitions[j].Name, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Type, partitions[j].Type, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Start * 512, partitions[j].Offset, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Length, partitions[j].Length, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Sequence, partitions[j].Sequence, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].Start, partitions[j].Start, testfiles[i]);
                }
            }
        }
    }
}
