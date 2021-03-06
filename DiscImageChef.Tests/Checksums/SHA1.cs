﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : SHA1.cs
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
using NUnit.Framework;
using DiscImageChef.Checksums;
using System.IO;

namespace DiscImageChef.Tests.Checksums
{
    [TestFixture]
    public class SHA1
    {
        static readonly byte[] ExpectedEmpty = { 0x3b, 0x71, 0xf4, 0x3f, 0xf3, 0x0f, 0x4b, 0x15, 0xb5, 0xcd, 0x85, 0xdd, 0x9e, 0x95, 0xeb, 0xc7, 0xe8, 0x4e, 0xb5, 0xa3 };
        static readonly byte[] ExpectedRandom = { 0x72, 0x0d, 0x3b, 0x71, 0x7d, 0xe0, 0xc7, 0x4c, 0x77, 0xdd, 0x9c, 0xaa, 0x9e, 0xba, 0x50, 0x60, 0xdc, 0xbd, 0x28, 0x8d };

        [Test]
        public void SHA1EmptyFile()
        {
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            byte[] result = ctx.File(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"));
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void SHA1EmptyData()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            ctx.Data(data, out byte[] result);
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void SHA1EmptyInstance()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            ctx.Update(data);
            byte[] result = ctx.Final();
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void SHA1RandomFile()
        {
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            byte[] result = ctx.File(Path.Combine(Consts.TestFilesRoot, "checksums", "random"));
            Assert.AreEqual(ExpectedRandom, result);
        }

        [Test]
        public void SHA1RandomData()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "random"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            ctx.Data(data, out byte[] result);
            Assert.AreEqual(ExpectedRandom, result);
        }

        [Test]
        public void SHA1RandomInstance()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "random"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            SHA1Context ctx = new SHA1Context();
            ctx.Init();
            ctx.Update(data);
            byte[] result = ctx.Final();
            Assert.AreEqual(ExpectedRandom, result);
        }
    }
}
