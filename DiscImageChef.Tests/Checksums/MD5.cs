﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : MD5.cs
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
    public class MD5
    {
        static readonly byte[] ExpectedEmpty = { 0xb6, 0xd8, 0x1b, 0x36, 0x0a, 0x56, 0x72, 0xd8, 0x0c, 0x27, 0x43, 0x0f, 0x39, 0x15, 0x3e, 0x2c };
        static readonly byte[] ExpectedRandom = { 0xd7, 0x8f, 0x0e, 0xec, 0x41, 0x7b, 0xe3, 0x86, 0x21, 0x9b, 0x21, 0xb7, 0x00, 0x04, 0x4b, 0x95 };

        [Test]
        public void MD5EmptyFile()
        {
            MD5Context ctx = new MD5Context();
            ctx.Init();
            byte[] result = ctx.File(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"));
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void MD5EmptyData()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            MD5Context ctx = new MD5Context();
            ctx.Init();
            ctx.Data(data, out byte[] result);
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void MD5EmptyInstance()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "empty"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            MD5Context ctx = new MD5Context();
            ctx.Init();
            ctx.Update(data);
            byte[] result = ctx.Final();
            Assert.AreEqual(ExpectedEmpty, result);
        }

        [Test]
        public void MD5RandomFile()
        {
            MD5Context ctx = new MD5Context();
            ctx.Init();
            byte[] result = ctx.File(Path.Combine(Consts.TestFilesRoot, "checksums", "random"));
            Assert.AreEqual(ExpectedRandom, result);
        }

        [Test]
        public void MD5RandomData()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "random"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            MD5Context ctx = new MD5Context();
            ctx.Init();
            ctx.Data(data, out byte[] result);
            Assert.AreEqual(ExpectedRandom, result);
        }

        [Test]
        public void MD5RandomInstance()
        {
            byte[] data = new byte[1048576];
            FileStream fs = new FileStream(Path.Combine(Consts.TestFilesRoot, "checksums", "random"), FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, 1048576);
            fs.Close();
            fs.Dispose();
            MD5Context ctx = new MD5Context();
            ctx.Init();
            ctx.Update(data);
            byte[] result = ctx.Final();
            Assert.AreEqual(ExpectedRandom, result);
        }
    }
}
