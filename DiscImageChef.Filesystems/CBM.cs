﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : CBM.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Commodore file system plugin.
//
// --[ Description ] ----------------------------------------------------------
//
//     Identifies the Commodore file system and shows information.
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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DiscImageChef.CommonTypes;

namespace DiscImageChef.Filesystems
{
    public class CBM : Filesystem
    {
        public CBM()
        {
            Name = "Commodore file system";
            PluginUUID = new Guid("D104744E-A376-450C-BAC0-1347C93F983B");
            CurrentEncoding = new Claunia.Encoding.PETSCII();
        }

        public CBM(ImagePlugins.ImagePlugin imagePlugin, Partition partition, Encoding encoding)
        {
            Name = "Commodore file system";
            PluginUUID = new Guid("D104744E-A376-450C-BAC0-1347C93F983B");
            CurrentEncoding = new Claunia.Encoding.PETSCII();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct CommodoreBAM
        {
            /// <summary>
            /// Track where directory starts
            /// </summary>
            public byte directoryTrack;
            /// <summary>
            /// Sector where directory starts
            /// </summary>
            public byte directorySector;
            /// <summary>
            /// Disk DOS version, 0x41
            /// </summary>
            public byte dosVersion;
            /// <summary>
            /// Set to 0x80 if 1571, 0x00 if not
            /// </summary>
            public byte doubleSided;
            /// <summary>
            /// Block allocation map
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 140)]
            public byte[] bam;
            /// <summary>
            /// Disk name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] name;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public ushort fill1;
            /// <summary>
            /// Disk ID
            /// </summary>
            public ushort diskId;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public byte fill2;
            /// <summary>
            /// DOS type
            /// </summary>
            public ushort dosType;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public uint fill3;
            /// <summary>
            /// Unused
            /// </summary>
            public byte unused1;
            /// <summary>
            /// Block allocation map for Dolphin DOS extended tracks
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] dolphinBam;
            /// <summary>
            /// Block allocation map for Speed DOS extended tracks
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] speedBam;
            /// <summary>
            /// Unused
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public byte[] unused2;
            /// <summary>
            /// Free sector count for second side in 1571
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
            public byte[] freeCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct CommodoreHeader
        {
            /// <summary>
            /// Track where directory starts
            /// </summary>
            public byte directoryTrack;
            /// <summary>
            /// Sector where directory starts
            /// </summary>
            public byte directorySector;
            /// <summary>
            /// Disk DOS version, 0x44
            /// </summary>
            public byte diskDosVersion;
            /// <summary>
            /// Unusued
            /// </summary>
            public byte unused1;
            /// <summary>
            /// Disk name
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] name;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public ushort fill1;
            /// <summary>
            /// Disk ID
            /// </summary>
            public ushort diskId;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public byte fill2;
            /// <summary>
            /// DOS version ('3')
            /// </summary>
            public byte dosVersion;
            /// <summary>
            /// Disk version ('D')
            /// </summary>
            public byte diskVersion;
            /// <summary>
            /// Filled with 0xA0
            /// </summary>
            public short fill3;
        }

        public override bool Identify(ImagePlugins.ImagePlugin imagePlugin, Partition partition)
        {
            if(partition.Start > 0)
                return false;

            if(imagePlugin.ImageInfo.sectorSize != 256)
                return false;

            if(imagePlugin.ImageInfo.sectors != 683 && imagePlugin.ImageInfo.sectors != 768 &&
               imagePlugin.ImageInfo.sectors != 1366 && imagePlugin.ImageInfo.sectors != 3200)
                return false;

            byte[] sector;

            if(imagePlugin.ImageInfo.sectors == 3200)
            {
                sector = imagePlugin.ReadSector(1560);
                CommodoreHeader cbmHdr = new CommodoreHeader();
                IntPtr cbmHdrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cbmHdr));
                Marshal.Copy(sector, 0, cbmHdrPtr, Marshal.SizeOf(cbmHdr));
                cbmHdr = (CommodoreHeader)Marshal.PtrToStructure(cbmHdrPtr, typeof(CommodoreHeader));
                Marshal.FreeHGlobal(cbmHdrPtr);

                if(cbmHdr.diskDosVersion == 0x44 && cbmHdr.dosVersion == 0x33 && cbmHdr.diskVersion == 0x44)
                    return true;
            }
            else
            {
                sector = imagePlugin.ReadSector(357);
                CommodoreBAM cbmBam = new CommodoreBAM();
                IntPtr cbmBamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cbmBam));
                Marshal.Copy(sector, 0, cbmBamPtr, Marshal.SizeOf(cbmBam));
                cbmBam = (CommodoreBAM)Marshal.PtrToStructure(cbmBamPtr, typeof(CommodoreBAM));
                Marshal.FreeHGlobal(cbmBamPtr);

                if(cbmBam.dosVersion == 0x41 && (cbmBam.doubleSided == 0x00 || cbmBam.doubleSided == 0x80) && cbmBam.unused1 == 0x00 && cbmBam.directoryTrack == 0x12)
                    return true;
            }

            return false;
        }

        public override void GetInformation(ImagePlugins.ImagePlugin imagePlugin, Partition partition, out string information)
        {
            byte[] sector;

            StringBuilder sbInformation = new StringBuilder();

            sbInformation.AppendLine("Commodore file system");

            xmlFSType = new Schemas.FileSystemType();
            xmlFSType.Type = "Commodore file system";
            xmlFSType.Clusters = (long)imagePlugin.ImageInfo.sectors;
            xmlFSType.ClusterSize = 256;

            if(imagePlugin.ImageInfo.sectors == 3200)
            {
                sector = imagePlugin.ReadSector(1560);
                CommodoreHeader cbmHdr = new CommodoreHeader();
                IntPtr cbmHdrPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cbmHdr));
                Marshal.Copy(sector, 0, cbmHdrPtr, Marshal.SizeOf(cbmHdr));
                cbmHdr = (CommodoreHeader)Marshal.PtrToStructure(cbmHdrPtr, typeof(CommodoreHeader));
                Marshal.FreeHGlobal(cbmHdrPtr);

                sbInformation.AppendFormat("Directory starts at track {0} sector {1}", cbmHdr.directoryTrack, cbmHdr.directorySector).AppendLine();
                sbInformation.AppendFormat("Disk DOS Version: {0}", Encoding.ASCII.GetString(new byte[] { cbmHdr.diskDosVersion })).AppendLine();
                sbInformation.AppendFormat("DOS Version: {0}", Encoding.ASCII.GetString(new byte[] { cbmHdr.dosVersion })).AppendLine();
                sbInformation.AppendFormat("Disk Version: {0}", Encoding.ASCII.GetString(new byte[] { cbmHdr.diskVersion })).AppendLine();
                sbInformation.AppendFormat("Disk ID: {0}", cbmHdr.diskId).AppendLine();
                sbInformation.AppendFormat("Disk name: {0}", StringHandlers.CToString(cbmHdr.name, CurrentEncoding)).AppendLine();

                xmlFSType.VolumeName = StringHandlers.CToString(cbmHdr.name, CurrentEncoding);
                xmlFSType.VolumeSerial = string.Format("{0}", cbmHdr.diskId);
            }
            else
            {
                sector = imagePlugin.ReadSector(357);
                CommodoreBAM cbmBam = new CommodoreBAM();
                IntPtr cbmBamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(cbmBam));
                Marshal.Copy(sector, 0, cbmBamPtr, Marshal.SizeOf(cbmBam));
                cbmBam = (CommodoreBAM)Marshal.PtrToStructure(cbmBamPtr, typeof(CommodoreBAM));
                Marshal.FreeHGlobal(cbmBamPtr);

                sbInformation.AppendFormat("Directory starts at track {0} sector {1}", cbmBam.directoryTrack, cbmBam.directorySector).AppendLine();
                sbInformation.AppendFormat("Disk DOS type: {0}", Encoding.ASCII.GetString(BitConverter.GetBytes(cbmBam.dosType))).AppendLine();
                sbInformation.AppendFormat("DOS Version: {0}", Encoding.ASCII.GetString(new byte[] { cbmBam.dosVersion })).AppendLine();
                sbInformation.AppendFormat("Disk ID: {0}", cbmBam.diskId).AppendLine();
                sbInformation.AppendFormat("Disk name: {0}", StringHandlers.CToString(cbmBam.name, CurrentEncoding)).AppendLine();

                xmlFSType.VolumeName = StringHandlers.CToString(cbmBam.name, CurrentEncoding);
                xmlFSType.VolumeSerial = string.Format("{0}", cbmBam.diskId);
            }

            information = sbInformation.ToString();
        }

        public override Errno Mount()
        {
            return Errno.NotImplemented;
        }

        public override Errno Mount(bool debug)
        {
            return Errno.NotImplemented;
        }

        public override Errno Unmount()
        {
            return Errno.NotImplemented;
        }

        public override Errno MapBlock(string path, long fileBlock, ref long deviceBlock)
        {
            return Errno.NotImplemented;
        }

        public override Errno GetAttributes(string path, ref FileAttributes attributes)
        {
            return Errno.NotImplemented;
        }

        public override Errno ListXAttr(string path, ref List<string> xattrs)
        {
            return Errno.NotImplemented;
        }

        public override Errno GetXattr(string path, string xattr, ref byte[] buf)
        {
            return Errno.NotImplemented;
        }

        public override Errno Read(string path, long offset, long size, ref byte[] buf)
        {
            return Errno.NotImplemented;
        }

        public override Errno ReadDir(string path, ref List<string> contents)
        {
            return Errno.NotImplemented;
        }

        public override Errno StatFs(ref FileSystemInfo stat)
        {
            return Errno.NotImplemented;
        }

        public override Errno Stat(string path, ref FileEntryInfo stat)
        {
            return Errno.NotImplemented;
        }

        public override Errno ReadLink(string path, ref string dest)
        {
            return Errno.NotImplemented;
        }
    }
}