// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : AppleDouble.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Component
//
// --[ Description ] ----------------------------------------------------------
//
//     Provides a filter to open AppleDouble files.
//
// --[ License ] --------------------------------------------------------------
//
//     This library is free software; you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as
//     published by the Free Software Foundation; either version 2.1 of the
//     License, or (at your option) any later version.
//
//     This library is distributed in the hope that it will be useful, but
//     WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//     Lesser General Public License for more details.
//
//     You should have received a copy of the GNU Lesser General Public
//     License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2017 Natalia Portillo
// ****************************************************************************/
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

namespace DiscImageChef.Filters
{
    public class AppleDouble : Filter
    {
        enum AppleDoubleEntryID : uint
        {
            Invalid = 0,
            DataFork = 1,
            ResourceFork = 2,
            RealName = 3,
            Comment = 4,
            Icon = 5,
            ColorIcon = 6,
            FileInfo = 7,
            FileDates = 8,
            FinderInfo = 9,
            MacFileInfo = 10,
            ProDOSFileInfo = 11,
            DOSFileInfo = 12,
            ShortName = 13,
            AFPFileInfo = 14,
            DirectoryID = 15
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleHeader
        {
            public uint magic;
            public uint version;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] homeFilesystem;
            public ushort entries;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleEntry
        {
            public uint id;
            public uint offset;
            public uint length;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleFileDates
        {
            public uint creationDate;
            public uint modificationDate;
            public uint backupDate;
            public uint accessDate;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleMacFileInfo
        {
            public uint creationDate;
            public uint modificationDate;
            public uint backupDate;
            public uint accessDate;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleUNIXFileInfo
        {
            public uint creationDate;
            public uint accessDate;
            public uint modificationDate;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleDOSFileInfo
        {
            public ushort modificationDate;
            public ushort modificationTime;
            public ushort attributes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AppleDoubleProDOSFileInfo
        {
            public uint creationDate;
            public uint modificationDate;
            public uint backupDate;
            public ushort access;
            public ushort fileType;
            public uint auxType;
        }

        const uint AppleDoubleMagic = 0x00051607;
        const uint AppleDoubleVersion = 0x00010000;
        const uint AppleDoubleVersion2 = 0x00020000;

        readonly byte[] MacintoshHome = { 0x4D, 0x61, 0x63, 0x69, 0x6E, 0x74, 0x6F, 0x73, 0x68, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        readonly byte[] ProDOSHome = { 0x50, 0x72, 0x6F, 0x44, 0x4F, 0x53, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        readonly byte[] DOSHome = { 0x4D, 0x53, 0x2D, 0x44, 0x4F, 0x53, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        readonly byte[] UNIXHome = { 0x55, 0x6E, 0x69, 0x78, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        readonly byte[] VMXHome = { 0x56, 0x41, 0x58, 0x20, 0x56, 0x4D, 0x53, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        readonly byte[] OSXHome = { 0x4D, 0x61, 0x63, 0x20, 0x4F, 0x53, 0x20, 0x58, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };

        AppleDoubleEntry dataFork;
        AppleDoubleEntry rsrcFork;
        bool opened;
        string basePath;
        string headerPath;
        DateTime lastWriteTime;
        DateTime creationTime;
        AppleDoubleHeader header;

        public AppleDouble()
        {
            Name = "AppleDouble";
            UUID = new Guid("1B2165EE-C9DF-4B21-BBBB-9E5892B2DF4D");
        }

        public override void Close()
        {
            opened = false;
        }

        public override string GetBasePath()
        {
            return basePath;
        }

        public override DateTime GetCreationTime()
        {
            return creationTime;
        }

        public override long GetDataForkLength()
        {
            return dataFork.length;
        }

        public override Stream GetDataForkStream()
        {
            return new FileStream(basePath, FileMode.Open, FileAccess.Read);
        }

        public override string GetFilename()
        {
            return basePath != null ? Path.GetFileName(basePath) : null;
        }

        public override DateTime GetLastWriteTime()
        {
            return lastWriteTime;
        }

        public override long GetLength()
        {
            return dataFork.length + rsrcFork.length;
        }

        public override string GetParentFolder()
        {
            return Path.GetDirectoryName(basePath);
        }

        public override string GetPath()
        {
            return basePath;
        }

        public override long GetResourceForkLength()
        {
            return rsrcFork.length;
        }

        public override Stream GetResourceForkStream()
        {
            if(rsrcFork.length == 0)
                return null;

            return new OffsetStream(headerPath, FileMode.Open, FileAccess.Read, rsrcFork.offset, rsrcFork.offset + rsrcFork.length - 1);
        }

        public override bool HasResourceFork()
        {
            return rsrcFork.length > 0;
        }

        public override bool Identify(byte[] buffer)
        {
            // Now way to have two files in a single byte array
            return false;
        }

        public override bool Identify(Stream stream)
        {
            // Now way to have two files in a single stream
            return false;
        }

        public override bool Identify(string path)
        {
            // Prepend data fork name with "R."
            string ProDosAppleDouble;
            // Prepend data fork name with '%'
            string UNIXAppleDouble;
            // Change file extension to ADF
            string DOSAppleDouble;
            // Change file extension to adf
            string DOSAppleDoubleLower;
            // Store AppleDouble header file in ".AppleDouble" folder with same name
            string NetatalkAppleDouble;
            // Store AppleDouble header file in "resource.frk" folder with same name
            string DAVEAppleDouble;
            // Prepend data fork name with "._"
            string OSXAppleDouble;
            // Adds ".rsrc" extension
            string UnArAppleDouble;

            string filename = Path.GetFileName(path);
            string filenameNoExt = Path.GetFileNameWithoutExtension(path);
            string parentFolder = Path.GetDirectoryName(path);

            ProDosAppleDouble = Path.Combine(parentFolder, "R." + filename);
            UNIXAppleDouble = Path.Combine(parentFolder, "%" + filename);
            DOSAppleDouble = Path.Combine(parentFolder, filenameNoExt + ".ADF");
            DOSAppleDoubleLower = Path.Combine(parentFolder, filenameNoExt + ".adf");
            NetatalkAppleDouble = Path.Combine(parentFolder, ".AppleDouble", filename);
            DAVEAppleDouble = Path.Combine(parentFolder, "resource.frk", filename);
            OSXAppleDouble = Path.Combine(parentFolder, "._" + filename);
            UnArAppleDouble = Path.Combine(parentFolder, filename + ".rsrc");

            // Check AppleDouble created by A/UX in ProDOS filesystem
            if(File.Exists(ProDosAppleDouble))
            {
                FileStream prodosStream = new FileStream(ProDosAppleDouble, FileMode.Open, FileAccess.Read);
                if(prodosStream != null && prodosStream.Length > 26)
                {
                    byte[] prodos_b = new byte[26];
                    prodosStream.Read(prodos_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(prodos_b);
                    prodosStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by A/UX in UFS filesystem
            if(File.Exists(UNIXAppleDouble))
            {
                FileStream unixStream = new FileStream(UNIXAppleDouble, FileMode.Open, FileAccess.Read);
                if(unixStream != null && unixStream.Length > 26)
                {
                    byte[] unix_b = new byte[26];
                    unixStream.Read(unix_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(unix_b);
                    unixStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by A/UX in FAT filesystem
            if(File.Exists(DOSAppleDouble))
            {
                FileStream dosStream = new FileStream(DOSAppleDouble, FileMode.Open, FileAccess.Read);
                if(dosStream != null && dosStream.Length > 26)
                {
                    byte[] dos_b = new byte[26];
                    dosStream.Read(dos_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dos_b);
                    dosStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by A/UX in case preserving FAT filesystem
            if(File.Exists(DOSAppleDoubleLower))
            {
                FileStream doslStream = new FileStream(DOSAppleDoubleLower, FileMode.Open, FileAccess.Read);
                if(doslStream != null && doslStream.Length > 26)
                {
                    byte[] dosl_b = new byte[26];
                    doslStream.Read(dosl_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dosl_b);
                    doslStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by Netatalk
            if(File.Exists(NetatalkAppleDouble))
            {
                FileStream netatalkStream = new FileStream(NetatalkAppleDouble, FileMode.Open, FileAccess.Read);
                if(netatalkStream != null && netatalkStream.Length > 26)
                {
                    byte[] netatalk_b = new byte[26];
                    netatalkStream.Read(netatalk_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(netatalk_b);
                    netatalkStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by DAVE
            if(File.Exists(DAVEAppleDouble))
            {
                FileStream daveStream = new FileStream(DAVEAppleDouble, FileMode.Open, FileAccess.Read);
                if(daveStream != null && daveStream.Length > 26)
                {
                    byte[] dave_b = new byte[26];
                    daveStream.Read(dave_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dave_b);
                    daveStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by Mac OS X
            if(File.Exists(OSXAppleDouble))
            {
                FileStream osxStream = new FileStream(OSXAppleDouble, FileMode.Open, FileAccess.Read);
                if(osxStream != null && osxStream.Length > 26)
                {
                    byte[] osx_b = new byte[26];
                    osxStream.Read(osx_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(osx_b);
                    osxStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            // Check AppleDouble created by UnAr (from The Unarchiver)
            if(File.Exists(UnArAppleDouble))
            {
                FileStream unarStream = new FileStream(UnArAppleDouble, FileMode.Open, FileAccess.Read);
                if(unarStream != null && unarStream.Length > 26)
                {
                    byte[] unar_b = new byte[26];
                    unarStream.Read(unar_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(unar_b);
                    unarStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        return true;
                }
            }

            return false;
        }

        public override bool IsOpened()
        {
            return opened;
        }

        public override void Open(byte[] buffer)
        {
            // Now way to have two files in a single byte array
            throw new NotSupportedException();
        }

        public override void Open(Stream stream)
        {
            // Now way to have two files in a single stream
            throw new NotSupportedException();
        }

        public override void Open(string path)
        {
            // Prepend data fork name with "R."
            string ProDosAppleDouble;
            // Prepend data fork name with '%'
            string UNIXAppleDouble;
            // Change file extension to ADF
            string DOSAppleDouble;
            // Change file extension to adf
            string DOSAppleDoubleLower;
            // Store AppleDouble header file in ".AppleDouble" folder with same name
            string NetatalkAppleDouble;
            // Store AppleDouble header file in "resource.frk" folder with same name
            string DAVEAppleDouble;
            // Prepend data fork name with "._"
            string OSXAppleDouble;
            // Adds ".rsrc" extension
            string UnArAppleDouble;

            string filename = Path.GetFileName(path);
            string filenameNoExt = Path.GetFileNameWithoutExtension(path);
            string parentFolder = Path.GetDirectoryName(path);

            ProDosAppleDouble = Path.Combine(parentFolder, "R." + filename);
            UNIXAppleDouble = Path.Combine(parentFolder, "%" + filename);
            DOSAppleDouble = Path.Combine(parentFolder, filenameNoExt + ".ADF");
            DOSAppleDoubleLower = Path.Combine(parentFolder, filenameNoExt + ".adf");
            NetatalkAppleDouble = Path.Combine(parentFolder, ".AppleDouble", filename);
            DAVEAppleDouble = Path.Combine(parentFolder, "resource.frk", filename);
            OSXAppleDouble = Path.Combine(parentFolder, "._" + filename);
            UnArAppleDouble = Path.Combine(parentFolder, filename + ".rsrc");

            // Check AppleDouble created by A/UX in ProDOS filesystem
            if(File.Exists(ProDosAppleDouble))
            {
                FileStream prodosStream = new FileStream(ProDosAppleDouble, FileMode.Open, FileAccess.Read);
                if(prodosStream != null && prodosStream.Length > 26)
                {
                    byte[] prodos_b = new byte[26];
                    prodosStream.Read(prodos_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(prodos_b);
                    prodosStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = ProDosAppleDouble;
                }
            }

            // Check AppleDouble created by A/UX in UFS filesystem
            if(File.Exists(UNIXAppleDouble))
            {
                FileStream unixStream = new FileStream(UNIXAppleDouble, FileMode.Open, FileAccess.Read);
                if(unixStream != null && unixStream.Length > 26)
                {
                    byte[] unix_b = new byte[26];
                    unixStream.Read(unix_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(unix_b);
                    unixStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = UNIXAppleDouble;
                }
            }

            // Check AppleDouble created by A/UX in FAT filesystem
            if(File.Exists(DOSAppleDouble))
            {
                FileStream dosStream = new FileStream(DOSAppleDouble, FileMode.Open, FileAccess.Read);
                if(dosStream != null && dosStream.Length > 26)
                {
                    byte[] dos_b = new byte[26];
                    dosStream.Read(dos_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dos_b);
                    dosStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = DOSAppleDouble;
                }
            }

            // Check AppleDouble created by A/UX in case preserving FAT filesystem
            if(File.Exists(DOSAppleDoubleLower))
            {
                FileStream doslStream = new FileStream(DOSAppleDoubleLower, FileMode.Open, FileAccess.Read);
                if(doslStream != null && doslStream.Length > 26)
                {
                    byte[] dosl_b = new byte[26];
                    doslStream.Read(dosl_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dosl_b);
                    doslStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = DOSAppleDoubleLower;
                }
            }

            // Check AppleDouble created by Netatalk
            if(File.Exists(NetatalkAppleDouble))
            {
                FileStream netatalkStream = new FileStream(NetatalkAppleDouble, FileMode.Open, FileAccess.Read);
                if(netatalkStream != null && netatalkStream.Length > 26)
                {
                    byte[] netatalk_b = new byte[26];
                    netatalkStream.Read(netatalk_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(netatalk_b);
                    netatalkStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = NetatalkAppleDouble;
                }
            }

            // Check AppleDouble created by DAVE
            if(File.Exists(DAVEAppleDouble))
            {
                FileStream daveStream = new FileStream(DAVEAppleDouble, FileMode.Open, FileAccess.Read);
                if(daveStream != null && daveStream.Length > 26)
                {
                    byte[] dave_b = new byte[26];
                    daveStream.Read(dave_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(dave_b);
                    daveStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = DAVEAppleDouble;
                }
            }

            // Check AppleDouble created by Mac OS X
            if(File.Exists(OSXAppleDouble))
            {
                FileStream osxStream = new FileStream(OSXAppleDouble, FileMode.Open, FileAccess.Read);
                if(osxStream != null && osxStream.Length > 26)
                {
                    byte[] osx_b = new byte[26];
                    osxStream.Read(osx_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(osx_b);
                    osxStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = OSXAppleDouble;
                }
            }

            // Check AppleDouble created by UnAr (from The Unarchiver)
            if(File.Exists(UnArAppleDouble))
            {
                FileStream unarStream = new FileStream(UnArAppleDouble, FileMode.Open, FileAccess.Read);
                if(unarStream != null && unarStream.Length > 26)
                {
                    byte[] unar_b = new byte[26];
                    unarStream.Read(unar_b, 0, 26);
                    header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(unar_b);
                    unarStream.Close();
                    if(header.magic == AppleDoubleMagic && (header.version == AppleDoubleVersion || header.version == AppleDoubleVersion2))
                        headerPath = UnArAppleDouble;
                }
            }

            FileStream fs = new FileStream(headerPath, FileMode.Open, FileAccess.Read);
            fs.Seek(0, SeekOrigin.Begin);

            byte[] hdr_b = new byte[26];
            fs.Read(hdr_b, 0, 26);
            header = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleHeader>(hdr_b);

            AppleDoubleEntry[] entries = new AppleDoubleEntry[header.entries];
            for(int i = 0; i < header.entries; i++)
            {
                byte[] entry = new byte[12];
                fs.Read(entry, 0, 12);
                entries[i] = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleEntry>(entry);
            }

            creationTime = DateTime.UtcNow;
            lastWriteTime = creationTime;
            foreach(AppleDoubleEntry entry in entries)
            {
                switch((AppleDoubleEntryID)entry.id)
                {
                    case AppleDoubleEntryID.DataFork:
                        // AppleDouble have datafork in separated file
                        break;
                    case AppleDoubleEntryID.FileDates:
                        fs.Seek(entry.offset, SeekOrigin.Begin);
                        byte[] dates_b = new byte[16];
                        fs.Read(dates_b, 0, 16);
                        AppleDoubleFileDates dates = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleFileDates>(dates_b);
                        creationTime = DateHandlers.AppleDoubleToDateTime(dates.creationDate);
                        lastWriteTime = DateHandlers.AppleDoubleToDateTime(dates.modificationDate);
                        break;
                    case AppleDoubleEntryID.FileInfo:
                        fs.Seek(entry.offset, SeekOrigin.Begin);
                        byte[] finfo = new byte[entry.length];
                        fs.Read(finfo, 0, finfo.Length);
                        if(MacintoshHome.SequenceEqual(header.homeFilesystem))
                        {
                            AppleDoubleMacFileInfo macinfo = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleMacFileInfo>(finfo);
                            creationTime = DateHandlers.MacToDateTime(macinfo.creationDate);
                            lastWriteTime = DateHandlers.MacToDateTime(macinfo.modificationDate);
                        }
                        else if(ProDOSHome.SequenceEqual(header.homeFilesystem))
                        {
                            AppleDoubleProDOSFileInfo prodosinfo = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleProDOSFileInfo>(finfo);
                            creationTime = DateHandlers.MacToDateTime(prodosinfo.creationDate);
                            lastWriteTime = DateHandlers.MacToDateTime(prodosinfo.modificationDate);
                        }
                        else if(UNIXHome.SequenceEqual(header.homeFilesystem))
                        {
                            AppleDoubleUNIXFileInfo unixinfo = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleUNIXFileInfo>(finfo);
                            creationTime = DateHandlers.UNIXUnsignedToDateTime(unixinfo.creationDate);
                            lastWriteTime = DateHandlers.UNIXUnsignedToDateTime(unixinfo.modificationDate);
                        }
                        else if(DOSHome.SequenceEqual(header.homeFilesystem))
                        {
                            AppleDoubleDOSFileInfo dosinfo = BigEndianMarshal.ByteArrayToStructureBigEndian<AppleDoubleDOSFileInfo>(finfo);
                            lastWriteTime = DateHandlers.DOSToDateTime(dosinfo.modificationDate, dosinfo.modificationTime);
                        }
                        break;
                    case AppleDoubleEntryID.ResourceFork:
                        rsrcFork = entry;
                        break;
                }
            }

            dataFork = new AppleDoubleEntry();
            dataFork.id = (uint)AppleDoubleEntryID.DataFork;
            if(File.Exists(path))
            {
                FileStream dataFs = new FileStream(path, FileMode.Open, FileAccess.Read);
                dataFork.length = (uint)dataFs.Length;
                dataFs.Close();
            }

            fs.Close();
            opened = true;
            basePath = path;
        }
    }
}
