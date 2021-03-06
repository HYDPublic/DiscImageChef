﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : EFS.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Component
//
// --[ Description ] ----------------------------------------------------------
//
//     Identifies the EFS filesystem and shows information.
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
using DiscImageChef.Console;

namespace DiscImageChef.Filesystems
{
    public class EFS : Filesystem
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct EFS_Superblock
        {
            /* 0:   fs size incl. bb 0 (in bb) */
            public int sb_size;
            /* 4:   first cg offset (in bb) */
            public int sb_firstcg;
            /* 8:   cg size (in bb) */
            public int sb_cgfsize;
            /* 12:  inodes/cg (in bb) */
            public short sb_cgisize;
            /* 14:  geom: sectors/track */
            public short sb_sectors;
            /* 16:  geom: heads/cylinder (unused) */
            public short sb_heads;
            /* 18:  num of cg's in the filesystem */
            public short sb_ncg;
            /* 20:  non-0 indicates fsck required */
            public short sb_dirty;
            /* 22:  */
            public short sb_pad0;
            /* 24:  superblock ctime */
            public int sb_time;
            /* 28:  magic [0] */
            public uint sb_magic;
            /* 32:  name of filesystem */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] sb_fname;
            /* 38:  name of filesystem pack */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] sb_fpack;
            /* 44:  bitmap size (in bytes) */
            public int sb_bmsize;
            /* 48:  total free data blocks */
            public int sb_tfree;
            /* 52:  total free inodes */
            public int sb_tinode;
            /* 56:  bitmap offset (grown fs) */
            public int sb_bmblock;
            /* 62:  repl. superblock offset */
            public int sb_replsb;
            /* 64:  last allocated inode */
            public int sb_lastinode;
            /* 68:  unused */
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] sb_spare;
            /* 88:  checksum (all above) */
            public uint sb_checksum;
        }

        const uint EFS_Magic = 0x00072959;
        const uint EFS_Magic_New = 0x0007295A;

        public EFS()
        {
            Name = "Extent File System Plugin";
            PluginUUID = new Guid("52A43F90-9AF3-4391-ADFE-65598DEEABAB");
            CurrentEncoding = Encoding.GetEncoding("iso-8859-15");
        }

        public EFS(ImagePlugins.ImagePlugin imagePlugin, Partition partition, Encoding encoding)
        {
            Name = "Extent File System Plugin";
            PluginUUID = new Guid("52A43F90-9AF3-4391-ADFE-65598DEEABAB");
            if(encoding == null)
                CurrentEncoding = Encoding.GetEncoding("iso-8859-15");
            else
                CurrentEncoding = encoding;
        }

        public override bool Identify(ImagePlugins.ImagePlugin imagePlugin, Partition partition)
        {
            if(imagePlugin.GetSectorSize() < 512)
                return false;

            // Misaligned
            if(imagePlugin.ImageInfo.xmlMediaType == ImagePlugins.XmlMediaType.OpticalDisc)
            {
                EFS_Superblock efs_sb = new EFS_Superblock();

                uint sbSize = (uint)((Marshal.SizeOf(efs_sb) + 0x200) / imagePlugin.GetSectorSize());
                if((Marshal.SizeOf(efs_sb) + 0x200) % imagePlugin.GetSectorSize() != 0)
                    sbSize++;

                byte[] sector = imagePlugin.ReadSectors(partition.Start, sbSize);
                if(sector.Length < Marshal.SizeOf(efs_sb))
                    return false;

                byte[] sbpiece = new byte[Marshal.SizeOf(efs_sb)];

                Array.Copy(sector, 0x200, sbpiece, 0, Marshal.SizeOf(efs_sb));

                efs_sb = BigEndianMarshal.ByteArrayToStructureBigEndian<EFS_Superblock>(sbpiece);

                DicConsole.DebugWriteLine("EFS plugin", "magic at 0x{0:X3} = 0x{1:X8} (expected 0x{2:X8} or 0x{3:X8})", 0x200, efs_sb.sb_magic, EFS_Magic, EFS_Magic_New);

                if(efs_sb.sb_magic == EFS_Magic || efs_sb.sb_magic == EFS_Magic_New)
                    return true;
            }
            else
            {
                EFS_Superblock efsSb = new EFS_Superblock();

                uint sbSize = (uint)(Marshal.SizeOf(efsSb) / imagePlugin.GetSectorSize());
                if(Marshal.SizeOf(efsSb) % imagePlugin.GetSectorSize() != 0)
                    sbSize++;

                byte[] sector = imagePlugin.ReadSectors(partition.Start + 1, sbSize);
                if(sector.Length < Marshal.SizeOf(efsSb))
                    return false;

                efsSb = BigEndianMarshal.ByteArrayToStructureBigEndian<EFS_Superblock>(sector);

                DicConsole.DebugWriteLine("EFS plugin", "magic at {0} = 0x{1:X8} (expected 0x{2:X8} or 0x{3:X8})", 1, efsSb.sb_magic, EFS_Magic, EFS_Magic_New);

                if(efsSb.sb_magic == EFS_Magic || efsSb.sb_magic == EFS_Magic_New)
                    return true;
            }

            return false;
        }

        public override void GetInformation(ImagePlugins.ImagePlugin imagePlugin, Partition partition, out string information)
        {
            information = "";
            if(imagePlugin.GetSectorSize() < 512)
                return;

            EFS_Superblock efsSb = new EFS_Superblock();

            // Misaligned
            if(imagePlugin.ImageInfo.xmlMediaType == ImagePlugins.XmlMediaType.OpticalDisc)
            {
                uint sbSize = (uint)((Marshal.SizeOf(efsSb) + 0x400) / imagePlugin.GetSectorSize());
                if((Marshal.SizeOf(efsSb) + 0x400) % imagePlugin.GetSectorSize() != 0)
                    sbSize++;

                byte[] sector = imagePlugin.ReadSectors(partition.Start, sbSize);
                if(sector.Length < Marshal.SizeOf(efsSb))
                    return;

                byte[] sbpiece = new byte[Marshal.SizeOf(efsSb)];

                Array.Copy(sector, 0x200, sbpiece, 0, Marshal.SizeOf(efsSb));

                efsSb = BigEndianMarshal.ByteArrayToStructureBigEndian<EFS_Superblock>(sbpiece);

                DicConsole.DebugWriteLine("EFS plugin", "magic at 0x{0:X3} = 0x{1:X8} (expected 0x{2:X8} or 0x{3:X8})", 0x200, efsSb.sb_magic, EFS_Magic, EFS_Magic_New);
            }
            else
            {
                uint sbSize = (uint)(Marshal.SizeOf(efsSb) / imagePlugin.GetSectorSize());
                if(Marshal.SizeOf(efsSb) % imagePlugin.GetSectorSize() != 0)
                    sbSize++;

                byte[] sector = imagePlugin.ReadSectors(partition.Start + 1, sbSize);
                if(sector.Length < Marshal.SizeOf(efsSb))
                    return;

                efsSb = BigEndianMarshal.ByteArrayToStructureBigEndian<EFS_Superblock>(sector);

                DicConsole.DebugWriteLine("EFS plugin", "magic at {0} = 0x{1:X8} (expected 0x{2:X8} or 0x{3:X8})", 1, efsSb.sb_magic, EFS_Magic, EFS_Magic_New);
            }

            if(efsSb.sb_magic != EFS_Magic && efsSb.sb_magic != EFS_Magic_New)
                return;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("SGI extent filesystem");
            if(efsSb.sb_magic == EFS_Magic_New)
                sb.AppendLine("New version");
            sb.AppendFormat("Filesystem size: {0} basic blocks", efsSb.sb_size).AppendLine();
            sb.AppendFormat("First cylinder group starts at block {0}", efsSb.sb_firstcg).AppendLine();
            sb.AppendFormat("Cylinder group size: {0} basic blocks", efsSb.sb_cgfsize).AppendLine();
            sb.AppendFormat("{0} inodes per cylinder group", efsSb.sb_cgisize).AppendLine();
            sb.AppendFormat("{0} sectors per track", efsSb.sb_sectors).AppendLine();
            sb.AppendFormat("{0} heads per cylinder", efsSb.sb_heads).AppendLine();
            sb.AppendFormat("{0} cylinder groups", efsSb.sb_ncg).AppendLine();
            sb.AppendFormat("Volume created on {0}", DateHandlers.UNIXToDateTime(efsSb.sb_time)).AppendLine();
            sb.AppendFormat("{0} bytes on bitmap", efsSb.sb_bmsize).AppendLine();
            sb.AppendFormat("{0} free blocks", efsSb.sb_tfree).AppendLine();
            sb.AppendFormat("{0} free inodes", efsSb.sb_tinode).AppendLine();
            if(efsSb.sb_bmblock > 0)
                sb.AppendFormat("Bitmap resides at block {0}", efsSb.sb_bmblock).AppendLine();
            if(efsSb.sb_replsb > 0)
                sb.AppendFormat("Replacement superblock resides at block {0}", efsSb.sb_replsb).AppendLine();
            if(efsSb.sb_lastinode > 0)
                sb.AppendFormat("Last inode allocated: {0}", efsSb.sb_lastinode).AppendLine();
            if(efsSb.sb_dirty > 0)
                sb.AppendLine("Volume is dirty");
            sb.AppendFormat("Checksum: 0x{0:X8}", efsSb.sb_checksum).AppendLine();
            sb.AppendFormat("Volume name: {0}", StringHandlers.CToString(efsSb.sb_fname, CurrentEncoding)).AppendLine();
            sb.AppendFormat("Volume pack: {0}", StringHandlers.CToString(efsSb.sb_fpack, CurrentEncoding)).AppendLine();

            information = sb.ToString();

            xmlFSType = new Schemas.FileSystemType
            {
                Type = "Extent File System",
                ClusterSize = 512,
                Clusters = efsSb.sb_size,
                FreeClusters = efsSb.sb_tfree,
                FreeClustersSpecified = true,
                Dirty = efsSb.sb_dirty > 0,
                VolumeName = StringHandlers.CToString(efsSb.sb_fname, CurrentEncoding),
                VolumeSerial = string.Format("{0:X8}", efsSb.sb_checksum),
                CreationDate = DateHandlers.UNIXToDateTime(efsSb.sb_time),
                CreationDateSpecified = true
            };
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