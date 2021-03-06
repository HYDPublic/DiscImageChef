// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : NeXT.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Partitioning scheme plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Manages NeXTStep and OpenStep disklabels.
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
using DiscImageChef.Console;

// Information learnt from XNU source and testing against real disks
namespace DiscImageChef.PartPlugins
{
    public class NeXTDisklabel : PartPlugin
    {
        const uint NEXT_MAGIC1 = 0x4E655854;
        // "NeXT"
        const uint NEXT_MAGIC2 = 0x646C5632;
        // "dlV2"
        const uint NEXT_MAGIC3 = 0x646C5633;
        // "dlV3"
        const ushort disktabStart = 0xB4;
        // 180
        const ushort disktabEntrySize = 0x2C;
        // 44
        public NeXTDisklabel()
        {
            Name = "NeXT Disklabel";
            PluginUUID = new Guid("246A6D93-4F1A-1F8A-344D-50187A5513A9");
        }

        public override bool GetInformation(ImagePlugins.ImagePlugin imagePlugin, out List<CommonTypes.Partition> partitions, ulong sectorOffset)
        {
            bool magic_found = false;
            byte[] label_sector;

            uint magic;
            uint sector_size;

            if(imagePlugin.GetSectorSize() == 2352 || imagePlugin.GetSectorSize() == 2448)
                sector_size = 2048;
            else
                sector_size = imagePlugin.GetSectorSize();

            partitions = new List<CommonTypes.Partition>();

            BigEndianBitConverter.IsLittleEndian = BitConverter.IsLittleEndian;

            ulong label_position = 0;

            foreach(ulong i in new ulong[]{0, 4, 15, 16})
            {
                if(i + sectorOffset >= imagePlugin.GetSectors())
                    break;
                
                label_sector = imagePlugin.ReadSector(i + sectorOffset);
                magic = BigEndianBitConverter.ToUInt32(label_sector, 0x00);
                if(magic == NEXT_MAGIC1 || magic == NEXT_MAGIC2 || magic == NEXT_MAGIC3)
                {
                    magic_found = true;
                    label_position = i + sectorOffset;
                    break;
                }
            }

            if(!magic_found)
                return false;

            uint sectors_to_read = 7680 / imagePlugin.ImageInfo.sectorSize;
            if(7680 % imagePlugin.ImageInfo.sectorSize > 0)
                sectors_to_read++;

            label_sector = imagePlugin.ReadSectors(label_position, sectors_to_read);

            NeXTLabel label = BigEndianMarshal.ByteArrayToStructureBigEndian<NeXTLabel>(label_sector);
            byte[] disktab_b = new byte[498];
            Array.Copy(label_sector, 44, disktab_b, 0, 498);
            label.dl_dt = BigEndianMarshal.ByteArrayToStructureBigEndian<NeXTDiskTab>(disktab_b);
            label.dl_dt.d_partitions = new NeXTEntry[8];

            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_version = 0x{0:X8}", label.dl_version);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_label_blkno = {0}", label.dl_label_blkno);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_size = {0}", label.dl_size);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_label = \"{0}\"", StringHandlers.CToString(label.dl_label));
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_flags = {0}", label.dl_flags);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_tag = 0x{0:X8}", label.dl_tag);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_name = \"{0}\"", StringHandlers.CToString(label.dl_dt.d_name));
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_type = \"{0}\"", StringHandlers.CToString(label.dl_dt.d_type));
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_secsize = {0}", label.dl_dt.d_secsize);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ntracks = {0}", label.dl_dt.d_ntracks);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_nsectors = {0}", label.dl_dt.d_nsectors);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ncylinders = {0}", label.dl_dt.d_ncylinders);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_rpm = {0}", label.dl_dt.d_rpm);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_front = {0}", label.dl_dt.d_front);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_back = {0}", label.dl_dt.d_back);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ngroups = {0}", label.dl_dt.d_ngroups);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ag_size = {0}", label.dl_dt.d_ag_size);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ag_alts = {0}", label.dl_dt.d_ag_alts);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_ag_off = {0}", label.dl_dt.d_ag_off);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_boot0_blkno[0] = {0}", label.dl_dt.d_boot0_blkno[0]);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_boot0_blkno[1] = {0}", label.dl_dt.d_boot0_blkno[1]);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_bootfile = \"{0}\"", StringHandlers.CToString(label.dl_dt.d_bootfile));
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_hostname = \"{0}\"", StringHandlers.CToString(label.dl_dt.d_hostname));
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_rootpartition = {0}", label.dl_dt.d_rootpartition);
            DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_rwpartition = {0}", label.dl_dt.d_rwpartition);

            for(int i = 0; i < 8; i++)
            {
                byte[] part_b = new byte[44];
                Array.Copy(label_sector, 44 + 146 + 44 * i, part_b, 0, 44);
                label.dl_dt.d_partitions[i] = BigEndianMarshal.ByteArrayToStructureBigEndian<NeXTEntry>(part_b);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_base = {1}", i, label.dl_dt.d_partitions[i].p_base);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_size = {1}", i, label.dl_dt.d_partitions[i].p_size);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_bsize = {1}", i, label.dl_dt.d_partitions[i].p_bsize);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_fsize = {1}", i, label.dl_dt.d_partitions[i].p_fsize);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_opt = {1}", i, label.dl_dt.d_partitions[i].p_opt);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_cpg = {1}", i, label.dl_dt.d_partitions[i].p_cpg);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_density = {1}", i, label.dl_dt.d_partitions[i].p_density);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_minfree = {1}", i, label.dl_dt.d_partitions[i].p_minfree);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_newfs = {1}", i, label.dl_dt.d_partitions[i].p_newfs);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_mountpt = \"{1}\"", i, StringHandlers.CToString(label.dl_dt.d_partitions[i].p_mountpt));
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_automnt = {1}", i, label.dl_dt.d_partitions[i].p_automnt);
                DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_type = \"{1}\"", i, StringHandlers.CToString(label.dl_dt.d_partitions[i].p_type));

                if(label.dl_dt.d_partitions[i].p_size > 0 && label.dl_dt.d_partitions[i].p_base >= 0 && label.dl_dt.d_partitions[i].p_bsize >= 0)
                {
                    StringBuilder sb = new StringBuilder();

                    CommonTypes.Partition part = new CommonTypes.Partition()
                    {
                        Size = (ulong)(label.dl_dt.d_partitions[i].p_size * label.dl_dt.d_secsize),
                        Offset = (ulong)((label.dl_dt.d_partitions[i].p_base + label.dl_dt.d_front) * label.dl_dt.d_secsize),
                        Type = StringHandlers.CToString(label.dl_dt.d_partitions[i].p_type),
                        Sequence = (ulong)i,
                        Name = StringHandlers.CToString(label.dl_dt.d_partitions[i].p_mountpt),
                        Length = (ulong)((label.dl_dt.d_partitions[i].p_size * label.dl_dt.d_secsize) / sector_size),
                        Start = (ulong)(((label.dl_dt.d_partitions[i].p_base + label.dl_dt.d_front) * label.dl_dt.d_secsize) / sector_size),
                        Scheme = Name
                    };

                    if(part.Start + part.Length > imagePlugin.ImageInfo.sectors)
                    {
                        DicConsole.DebugWriteLine("NeXT Plugin", "Partition bigger than device, reducing...");
                        part.Length = imagePlugin.ImageInfo.sectors - part.Start;
                        part.Size = part.Length * sector_size;
                        DicConsole.DebugWriteLine("NeXT Plugin", "label.dl_dt.d_partitions[{0}].p_size = {1}", i, part.Length);
                    }

                    sb.AppendFormat("{0} bytes per block", label.dl_dt.d_partitions[i].p_bsize).AppendLine();
                    sb.AppendFormat("{0} bytes per fragment", label.dl_dt.d_partitions[i].p_fsize).AppendLine();
                    if(label.dl_dt.d_partitions[i].p_opt == 's')
                        sb.AppendLine("Space optimized");
                    else if(label.dl_dt.d_partitions[i].p_opt == 't')
                        sb.AppendLine("Time optimized");
                    else
                        sb.AppendFormat("Unknown optimization {0:X2}", label.dl_dt.d_partitions[i].p_opt).AppendLine();
                    sb.AppendFormat("{0} cylinders per group", label.dl_dt.d_partitions[i].p_cpg).AppendLine();
                    sb.AppendFormat("{0} bytes per inode", label.dl_dt.d_partitions[i].p_density).AppendLine();
                    sb.AppendFormat("{0}% of space must be free at minimum", label.dl_dt.d_partitions[i].p_minfree).AppendLine();
                    if(label.dl_dt.d_partitions[i].p_newfs != 1)
                        sb.AppendLine("Filesystem should be formatted at start");
                    if(label.dl_dt.d_partitions[i].p_automnt == 1)
                        sb.AppendLine("Filesystem should be automatically mounted");

                    part.Description = sb.ToString();

                    partitions.Add(part);
                }
            }

            return true;
        }

        /// <summary>
        /// NeXT v3 disklabel, 544 bytes
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NeXTLabel
        {
            /// <summary>Signature</summary>
            public uint dl_version;
            /// <summary>Block on which this label resides</summary>
            public int dl_label_blkno;
            /// <summary>Device size in blocks</summary>
            public int dl_size;
            /// <summary>Device name</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] dl_label;
            /// <summary>Device flags</summary>
            public uint dl_flags;
            /// <summary>Device tag</summary>
            public uint dl_tag;
            /// <summary>Device info and partitions</summary>
            public NeXTDiskTab dl_dt;
            /// <summary>Checksum</summary>
            public ushort dl_v3_checksum;
        }

        /// <summary>
        /// NeXT v1 and v2 disklabel, 7224 bytes
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NeXTLabelOld
        {
            /// <summary>Signature</summary>
            public uint dl_version;
            /// <summary>Block on which this label resides</summary>
            public int dl_label_blkno;
            /// <summary>Device size in blocks</summary>
            public int dl_size;
            /// <summary>Device name</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] dl_label;
            /// <summary>Device flags</summary>
            public uint dl_flags;
            /// <summary>Device tag</summary>
            public uint dl_tag;
            /// <summary>Device info and partitions</summary>
            public NeXTDiskTab dl_dt;
            /// <summary>Bad sector table</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1670)]
            public int[] dl_bad;
            /// <summary>Checksum</summary>
            public ushort dl_checksum;
        }

        /// <summary>
        /// NeXT disktab and partitions, 498 bytes
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct NeXTDiskTab
        {
            /// <summary>Drive name</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] d_name;
            /// <summary>Drive type</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] d_type;
            /// <summary>Sector size</summary>
            public int d_secsize;
            /// <summary>tracks/cylinder</summary>
            public int d_ntracks;
            /// <summary>sectors/track</summary>
            public int d_nsectors;
            /// <summary>cylinders</summary>
            public int d_ncylinders;
            /// <summary>revolutions/minute</summary>
            public int d_rpm;
            /// <summary>size of front porch in sectors</summary>
            public short d_front;
            /// <summary>size of back porch in sectors</summary>
            public short d_back;
            /// <summary>number of alt groups</summary>
            public short d_ngroups;
            /// <summary>alt group size in sectors</summary>
            public short d_ag_size;
            /// <summary>alternate sectors per alt group</summary>
            public short d_ag_alts;
            /// <summary>sector offset to first alternate</summary>
            public short d_ag_off;
            /// <summary>"blk 0" boot locations</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public int[] d_boot0_blkno;
            /// <summary>default bootfile</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] d_bootfile;
            /// <summary>host name</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] d_hostname;
            /// <summary>root partition</summary>
            public byte d_rootpartition;
            /// <summary>r/w partition</summary>
            public byte d_rwpartition;
            /// <summary>partitions</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public NeXTEntry[] d_partitions;
        }

        /// <summary>
        /// Partition entries, 44 bytes each
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct NeXTEntry
        {
            /// <summary>Sector of start, counting from front porch</summary>
            public int p_base;
            /// <summary>Length in sectors</summary>
            public int p_size;
            /// <summary>Filesystem's block size</summary>
            public short p_bsize;
            /// <summary>Filesystem's fragment size</summary>
            public short p_fsize;
            /// <summary>'s'pace or 't'ime</summary>
            public byte p_opt;
            /// <summary>Cylinders per group</summary>
            public short p_cpg;
            /// <summary>Bytes per inode</summary>
            public short p_density;
            /// <summary>% of minimum free space</summary>
            public byte p_minfree;
            /// <summary>Should newfs be run on first start?</summary>
            public byte p_newfs;
            /// <summary>Mount point or empty if mount where you want</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] p_mountpt;
            /// <summary>Should automount</summary>
            public byte p_automnt;
            /// <summary>Filesystem type, always "4.3BSD"?</summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] p_type;
        }
    }
}