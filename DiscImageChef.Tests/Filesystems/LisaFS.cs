﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : LisaFS.cs
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
using System.IO;
using DiscImageChef.CommonTypes;
using DiscImageChef.Filesystems;
using DiscImageChef.Filters;
using DiscImageChef.ImagePlugins;
using NUnit.Framework;

namespace DiscImageChef.Tests.Filesystems
{
    [TestFixture]
    public class LisaFS
    {
        readonly string[] testfiles = {
            "166files.dc42.lz", "222files.dc42.lz", "blank2.0.dc42.lz", "blank-disk.dc42.lz", "file-with-a-password.dc42.lz",
            "tfwdndrc-has-been-erased.dc42.lz", "tfwdndrc-has-been-restored.dc42.lz", "three-empty-folders.dc42.lz",
            "three-folders-with-differently-named-docs.dc42.lz",
            "three-folders-with-differently-named-docs-root-alphabetical.dc42.lz",
            "three-folders-with-differently-named-docs-root-chronological.dc42.lz",
            "three-folders-with-identically-named-docs.dc42.lz",
        };

        readonly MediaType[] mediatypes = {
            MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,
            MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,
            MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,MediaType.AppleSonySS,
        };

        readonly ulong[] sectors = {
            800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800, 800,
        };

        readonly uint[] sectorsize = {
            512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 
        };

        readonly long[] clusters = {
            800, 800, 792, 800, 800, 800, 800, 800, 800, 800, 800, 800, 
        };

        readonly int[] clustersize = {
            512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 512, 
        };

        readonly string[] volumename = {
            "166Files", "222Files", "AOS  4:59 pm 10/02/87", "AOS 3.0",
            "AOS 3.0", "AOS 3.0", "AOS 3.0", "AOS 3.0",
            "AOS 3.0", "AOS 3.0", "AOS 3.0", "AOS 3.0",
        };

        readonly string[] volumeserial = {
            "A23703A202010663", "A23703A201010663", "A32D261301010663", "A22CB48D01010663",
            "A22CC3A702010663", "A22CB48D14010663", "A22CB48D14010663", "A22CB48D01010663",
            "A22CB48D01010663", "A22CB48D01010663", "A22CB48D01010663", "A22CB48D01010663",
        };

        readonly string[] oemid = {
            null, null, null, null, null, null, null, null, null, null, null, null, 
        };

        [Test]
        public void Test()
        {
            for(int i = 0; i < testfiles.Length; i++)
            {
                string location = Path.Combine(Consts.TestFilesRoot, "filesystems", "lisafs", testfiles[i]);
                Filter filter = new LZip();
                filter.Open(location);
                ImagePlugin image = new DiskCopy42();
                Assert.AreEqual(true, image.OpenImage(filter), testfiles[i]);
                Assert.AreEqual(mediatypes[i], image.ImageInfo.mediaType, testfiles[i]);
                Assert.AreEqual(sectors[i], image.ImageInfo.sectors, testfiles[i]);
                Assert.AreEqual(sectorsize[i], image.ImageInfo.sectorSize, testfiles[i]);
                Filesystem fs = new DiscImageChef.Filesystems.LisaFS.LisaFS();
                Partition wholePart = new Partition
                {
                    Name = "Whole device",
                    Length = image.ImageInfo.sectors,
                    Size = image.ImageInfo.sectors * image.ImageInfo.sectorSize
                };
                Assert.AreEqual(true, fs.Identify(image, wholePart), testfiles[i]);
                fs.GetInformation(image, wholePart, out string information);
                Assert.AreEqual(clusters[i], fs.XmlFSType.Clusters, testfiles[i]);
                Assert.AreEqual(clustersize[i], fs.XmlFSType.ClusterSize, testfiles[i]);
                Assert.AreEqual("LisaFS", fs.XmlFSType.Type, testfiles[i]);
                Assert.AreEqual(volumename[i], fs.XmlFSType.VolumeName, testfiles[i]);
                Assert.AreEqual(volumeserial[i], fs.XmlFSType.VolumeSerial, testfiles[i]);
                Assert.AreEqual(oemid[i], fs.XmlFSType.SystemIdentifier, testfiles[i]);
            }
        }
    }
}
