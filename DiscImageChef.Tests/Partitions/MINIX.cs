﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : MINIX.cs
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
using DiscImageChef.Filters;
using DiscImageChef.ImagePlugins;
using DiscImageChef.PartPlugins;
using NUnit.Framework;

namespace DiscImageChef.Tests.Partitions
{
    [TestFixture]
    public class MINIX
    {
        readonly string[] testfiles = {
            "minix_3.1.2a.vdi.lz",
        };

        readonly Partition[][] wanted = {
            // Parted
            new []{
                new Partition{ PartitionDescription = null, PartitionLength = 268369408, PartitionName = null, PartitionType = "MINIX", PartitionStart = 2064896, PartitionSectors = 524159,
                    PartitionSequence = 0, PartitionStartSector = 4033 },
                new Partition{ PartitionDescription = null, PartitionLength = 270434304, PartitionName = null, PartitionType = "MINIX", PartitionStart = 270434304, PartitionSectors = 528192,
                    PartitionSequence = 1, PartitionStartSector = 528192 },
                new Partition{ PartitionDescription = null, PartitionLength = 270434304, PartitionName = null, PartitionType = "MINIX", PartitionStart = 540868608, PartitionSectors = 528192,
                    PartitionSequence = 2, PartitionStartSector = 1056384 },
                new Partition{ PartitionDescription = null, PartitionLength = 262176768, PartitionName = null, PartitionType = "MINIX", PartitionStart = 811302912, PartitionSectors = 512064,
                    PartitionSequence = 2, PartitionStartSector = 1584576 },
            },
        };

        [Test]
        public void Test()
        {
            throw new System.NotImplementedException("Partition schemes inside partitions are not yet implemented, and should be tested here.");
            /*
            for(int i = 0; i < testfiles.Length; i++)
            {
                string location = Path.Combine(Consts.TestFilesRoot, "partitions", "minix", testfiles[i]);
                Filter filter = new LZip();
                filter.Open(location);
                ImagePlugin image = new VDI();
                Assert.AreEqual(true, image.OpenImage(filter), testfiles[i]);
                PartPlugin parts = new DiscImageChef.PartPlugins.BSD();
                Assert.AreEqual(true, parts.GetInformation(image, out List<Partition> partitions), testfiles[i]);
                Assert.AreEqual(wanted[i].Length, partitions.Count, testfiles[i]);
                for(int j = 0; j < partitions.Count; j++)
                {
                    // Too chatty
                    //Assert.AreEqual(wanted[i][j].PartitionDescription, partitions[j].PartitionDescription, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionLength, partitions[j].PartitionLength, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionName, partitions[j].PartitionName, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionType, partitions[j].PartitionType, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionStart, partitions[j].PartitionStart, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionSectors, partitions[j].PartitionSectors, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionSequence, partitions[j].PartitionSequence, testfiles[i]);
                    Assert.AreEqual(wanted[i][j].PartitionStartSector, partitions[j].PartitionStartSector, testfiles[i]);
                }
            }*/
        }
    }
}