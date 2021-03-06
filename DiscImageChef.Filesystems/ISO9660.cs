﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : ISO9660.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : ISO 9660 filesystem plugin.
//
// --[ Description ] ----------------------------------------------------------
//
//     Identifies the ISO 9660 filesystem and shows information.
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
using System.Globalization;
using System.Text;
using DiscImageChef.CommonTypes;
using DiscImageChef.Console;

namespace DiscImageChef.Filesystems
{
    // This is coded following ECMA-119.
    // TODO: Differentiate ISO Level 1, 2, 3 and ISO 9660:1999
    // TODO: Apple extensiones, requires XA or advance RR interpretation.
    // TODO: Needs a major rewrite
    public class ISO9660Plugin : Filesystem
    {
        //static bool alreadyLaunched;

        public ISO9660Plugin()
        {
            Name = "ISO9660 Filesystem";
            PluginUUID = new Guid("d812f4d3-c357-400d-90fd-3b22ef786aa8");
            CurrentEncoding = Encoding.ASCII;
        }

        public ISO9660Plugin(ImagePlugins.ImagePlugin imagePlugin, Partition partition, Encoding encoding)
        {
            Name = "ISO9660 Filesystem";
            PluginUUID = new Guid("d812f4d3-c357-400d-90fd-3b22ef786aa8");
            if(encoding == null)
                CurrentEncoding = Encoding.ASCII;
        }

        struct DecodedVolumeDescriptor
        {
            public string SystemIdentifier;
            public string VolumeIdentifier;
            public string VolumeSetIdentifier;
            public string PublisherIdentifier;
            public string DataPreparerIdentifier;
            public string ApplicationIdentifier;
            public DateTime CreationTime;
            public bool HasModificationTime;
            public DateTime ModificationTime;
            public bool HasExpirationTime;
            public DateTime ExpirationTime;
            public bool HasEffectiveTime;
            public DateTime EffectiveTime;
        }

        public override bool Identify(ImagePlugins.ImagePlugin imagePlugin, Partition partition)
        {
            /*            if (alreadyLaunched)
                            return false;
                        alreadyLaunched = true;*/

            byte VDType;

            // ISO9660 is designed for 2048 bytes/sector devices
            if(imagePlugin.GetSectorSize() < 2048)
                return false;

            // ISO9660 Primary Volume Descriptor starts at sector 16, so that's minimal size.
            if(partition.End <= (16 + partition.Start))
                return false;

            // Read to Volume Descriptor
            byte[] vd_sector = imagePlugin.ReadSector(16 + partition.Start);

            int xa_off = 0;
            if(vd_sector.Length == 2336)
                xa_off = 8;

            VDType = vd_sector[0 + xa_off];
            byte[] VDMagic = new byte[5];

            // Wrong, VDs can be any order!
            if(VDType == 255) // Supposedly we are in the PVD.
                return false;

            Array.Copy(vd_sector, 0x001 + xa_off, VDMagic, 0, 5);

            DicConsole.DebugWriteLine("ISO9660 plugin", "VDMagic = {0}", CurrentEncoding.GetString(VDMagic));

            return CurrentEncoding.GetString(VDMagic) == "CD001";
        }

        public override void GetInformation(ImagePlugins.ImagePlugin imagePlugin, Partition partition, out string information)
        {
            information = "";
            StringBuilder ISOMetadata = new StringBuilder();
            bool Joliet = false;
            bool Bootable = false;
            bool RockRidge = false;
            byte VDType;                            // Volume Descriptor Type, should be 1 or 2.
            byte[] VDMagic = new byte[5];           // Volume Descriptor magic "CD001"
            byte[] VDSysId = new byte[32];          // System Identifier
            byte[] VDVolId = new byte[32];          // Volume Identifier
            byte[] VDVolSetId = new byte[128];      // Volume Set Identifier
            byte[] VDPubId = new byte[128];         // Publisher Identifier
            byte[] VDDataPrepId = new byte[128];    // Data Preparer Identifier
            byte[] VDAppId = new byte[128];         // Application Identifier
            byte[] VCTime = new byte[17];           // Volume Creation Date and Time
            byte[] VMTime = new byte[17];           // Volume Modification Date and Time
            byte[] VXTime = new byte[17];           // Volume Expiration Date and Time
            byte[] VETime = new byte[17];           // Volume Effective Date and Time

            byte[] JolietMagic = new byte[3];
            byte[] JolietSysId = new byte[32];          // System Identifier
            byte[] JolietVolId = new byte[32];          // Volume Identifier
            byte[] JolietVolSetId = new byte[128];      // Volume Set Identifier
            byte[] JolietPubId = new byte[128];         // Publisher Identifier
            byte[] JolietDataPrepId = new byte[128];    // Data Preparer Identifier
            byte[] JolietAppId = new byte[128];         // Application Identifier
            byte[] JolietCTime = new byte[17];           // Volume Creation Date and Time
            byte[] JolietMTime = new byte[17];           // Volume Modification Date and Time
            byte[] JolietXTime = new byte[17];           // Volume Expiration Date and Time
            byte[] JolietETime = new byte[17];           // Volume Effective Date and Time

            byte[] BootSysId = new byte[32];
            string BootSpec = "";

            byte[] VDPathTableStart = new byte[4];
            byte[] RootDirectoryLocation = new byte[4];

            // ISO9660 is designed for 2048 bytes/sector devices
            if(imagePlugin.GetSectorSize() < 2048)
                return;

            // ISO9660 Primary Volume Descriptor starts at sector 16, so that's minimal size.
            if(partition.End < 16)
                return;

            ulong counter = 0;

            while(true)
            {
                DicConsole.DebugWriteLine("ISO9660 plugin", "Processing VD loop no. {0}", counter);
                // Seek to Volume Descriptor
                DicConsole.DebugWriteLine("ISO9660 plugin", "Reading sector {0}", 16 + counter + partition.Start);
                byte[] vd_sector_tmp = imagePlugin.ReadSector(16 + counter + partition.Start);
                byte[] vd_sector;
                if(vd_sector_tmp.Length == 2336)
                {
                    vd_sector = new byte[2336 - 8];
                    Array.Copy(vd_sector_tmp, 8, vd_sector, 0, 2336 - 8);
                }
                else
                    vd_sector = vd_sector_tmp;

                VDType = vd_sector[0];
                DicConsole.DebugWriteLine("ISO9660 plugin", "VDType = {0}", VDType);

                if(VDType == 255) // Supposedly we are in the PVD.
                {
                    if(counter == 0)
                        return;
                    break;
                }

                Array.Copy(vd_sector, 0x001, VDMagic, 0, 5);

                if(CurrentEncoding.GetString(VDMagic) != "CD001") // Recognized, it is an ISO9660, now check for rest of data.
                {
                    if(counter == 0)
                        return;
                    break;
                }

                switch(VDType)
                {
                    case 0: // TODO
                        {
                            Bootable = true;
                            BootSpec = "Unknown";

                            // Read to boot system identifier
                            Array.Copy(vd_sector, 0x007, BootSysId, 0, 32);

                            if(CurrentEncoding.GetString(BootSysId).Substring(0, 23) == "EL TORITO SPECIFICATION")
                                BootSpec = "El Torito";

                            break;
                        }
                    case 1:
                        {
                            // Read first identifiers
                            Array.Copy(vd_sector, 0x008, VDSysId, 0, 32);
                            Array.Copy(vd_sector, 0x028, VDVolId, 0, 32);

                            // Get path table start
                            Array.Copy(vd_sector, 0x08C, VDPathTableStart, 0, 4);

                            // Read next identifiers
                            Array.Copy(vd_sector, 0x0BE, VDVolSetId, 0, 128);
                            Array.Copy(vd_sector, 0x13E, VDPubId, 0, 128);
                            Array.Copy(vd_sector, 0x1BE, VDDataPrepId, 0, 128);
                            Array.Copy(vd_sector, 0x23E, VDAppId, 0, 128);

                            // Read dates
                            Array.Copy(vd_sector, 0x32D, VCTime, 0, 17);
                            Array.Copy(vd_sector, 0x33E, VMTime, 0, 17);
                            Array.Copy(vd_sector, 0x34F, VXTime, 0, 17);
                            Array.Copy(vd_sector, 0x360, VETime, 0, 17);

                            break;
                        }
                    case 2:
                        {
                            // Check if this is Joliet
                            Array.Copy(vd_sector, 0x058, JolietMagic, 0, 3);
                            if(JolietMagic[0] == '%' && JolietMagic[1] == '/')
                            {
                                if(JolietMagic[2] == '@' || JolietMagic[2] == 'C' || JolietMagic[2] == 'E')
                                {
                                    Joliet = true;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                                break;

                            // Read first identifiers
                            Array.Copy(vd_sector, 0x008, JolietSysId, 0, 32);
                            Array.Copy(vd_sector, 0x028, JolietVolId, 0, 32);

                            // Read next identifiers
                            Array.Copy(vd_sector, 0x0BE, JolietVolSetId, 0, 128);
                            Array.Copy(vd_sector, 0x13E, JolietPubId, 0, 128);
                            Array.Copy(vd_sector, 0x13E, JolietDataPrepId, 0, 128);
                            Array.Copy(vd_sector, 0x13E, JolietAppId, 0, 128);

                            // Read dates
                            Array.Copy(vd_sector, 0x32D, JolietCTime, 0, 17);
                            Array.Copy(vd_sector, 0x33E, JolietMTime, 0, 17);
                            Array.Copy(vd_sector, 0x34F, JolietXTime, 0, 17);
                            Array.Copy(vd_sector, 0x360, JolietETime, 0, 17);

                            break;
                        }
                }

                counter++;
            }

            DecodedVolumeDescriptor decodedVD = new DecodedVolumeDescriptor();
            DecodedVolumeDescriptor decodedJolietVD = new DecodedVolumeDescriptor();

            decodedVD = DecodeVolumeDescriptor(VDSysId, VDVolId, VDVolSetId, VDPubId, VDDataPrepId, VDAppId, VCTime, VMTime, VXTime, VETime);
            if(Joliet)
                decodedJolietVD = DecodeJolietDescriptor(JolietSysId, JolietVolId, JolietVolSetId, JolietPubId, JolietDataPrepId, JolietAppId, JolietCTime, JolietMTime, JolietXTime, JolietETime);


            ulong i = (ulong)BitConverter.ToInt32(VDPathTableStart, 0);
            DicConsole.DebugWriteLine("ISO9660 plugin", "VDPathTableStart = {0} + {1} = {2}", i, partition.Start, i + partition.Start);

            // TODO: Check this
            /*
            if((i + partition.Start) < partition.End)
            {

                byte[] path_table = imagePlugin.ReadSector(i + partition.Start);
                Array.Copy(path_table, 2, RootDirectoryLocation, 0, 4);
                // Check for Rock Ridge
                byte[] root_dir = imagePlugin.ReadSector((ulong)BitConverter.ToInt32(RootDirectoryLocation, 0) + partition.Start);

                byte[] SUSPMagic = new byte[2];
                byte[] RRMagic = new byte[2];

                Array.Copy(root_dir, 0x22, SUSPMagic, 0, 2);
                if(CurrentEncoding.GetString(SUSPMagic) == "SP")
                {
                    Array.Copy(root_dir, 0x29, RRMagic, 0, 2);
                    RockRidge |= CurrentEncoding.GetString(RRMagic) == "RR";
                }
            }*/

            #region SEGA IP.BIN Read and decoding

            bool SegaCD = false;
            bool Saturn = false;
            bool Dreamcast = false;
            StringBuilder IPBinInformation = new StringBuilder();

            byte[] SegaHardwareID = new byte[16];
            byte[] ipbin_sector = imagePlugin.ReadSector(0 + partition.Start);
            Array.Copy(ipbin_sector, 0x000, SegaHardwareID, 0, 16);

            DicConsole.DebugWriteLine("ISO9660 plugin", "SegaHardwareID = \"{0}\"", CurrentEncoding.GetString(SegaHardwareID));

            switch(CurrentEncoding.GetString(SegaHardwareID))
            {
                case "SEGADISCSYSTEM  ":
                case "SEGADATADISC    ":
                case "SEGAOS          ":
                    {
                        SegaCD = true; // Ok, this contains SegaCD IP.BIN

                        DicConsole.DebugWriteLine("ISO9660 plugin", "Found SegaCD IP.BIN");

                        IPBinInformation.AppendLine("--------------------------------");
                        IPBinInformation.AppendLine("SEGA IP.BIN INFORMATION:");
                        IPBinInformation.AppendLine("--------------------------------");

                        // Definitions following
                        byte[] volume_name = new byte[11];      // 0x010, Varies
                        byte[] spare_space1 = new byte[1];      // 0x01B, 0x00
                        byte[] volume_version = new byte[2];    // 0x01C, Volume version in BCD. <100 = Prerelease.
                        byte[] volume_type = new byte[2];       // 0x01E, Bit 0 = 1 => CD-ROM. Rest should be 0.
                        byte[] system_name = new byte[11];      // 0x020, Unknown, varies!
                        byte[] spare_space2 = new byte[1];      // 0x02B, 0x00
                        byte[] system_version = new byte[2];    // 0x02C, Should be 1
                        byte[] spare_space3 = new byte[2];      // 0x02E, 0x0000
                        byte[] ip_address = new byte[4];	    // 0x030, Initial program address
                        byte[] ip_loadsize = new byte[4];       // 0x034, Load size of initial program
                        byte[] ip_entry_address = new byte[4];  // 0x038, Initial program entry address
                        byte[] ip_work_ram_size = new byte[4];  // 0x03C, Initial program work RAM size in bytes
                        byte[] sp_address = new byte[4];	    // 0x040, System program address
                        byte[] sp_loadsize = new byte[4];       // 0x044, Load size of system program
                        byte[] sp_entry_address = new byte[4];  // 0x048, System program entry address
                        byte[] sp_work_ram_size = new byte[4];  // 0x04C, System program work RAM size in bytes
                        byte[] release_date = new byte[8];      // 0x050, MMDDYYYY
                        byte[] unknown1 = new byte[7];          // 0x058, Seems to be all 0x20s
                        byte[] spare_space4 = new byte[1];      // 0x05F, 0x00 ?
                        byte[] system_reserved = new byte[160]; // 0x060, System Reserved Area
                        byte[] hardware_id = new byte[16];      // 0x100, Hardware ID
                        byte[] copyright = new byte[3];         // 0x110, "(C)" -- Can be the developer code directly!, if that is the code release date will be displaced
                        byte[] developer_code = new byte[5];    // 0x113 or 0x110, "SEGA" or "T-xx"
                        byte[] release_date2 = new byte[8];     // 0x118, Another release date, this with month in letters?
                        byte[] domestic_title = new byte[48];   // 0x120, Domestic version of the game title
                        byte[] overseas_title = new byte[48];   // 0x150, Overseas version of the game title
                        byte[] product_code = new byte[13];     // 0x180, Official product code
                        byte[] peripherals = new byte[16];      // 0x190, Supported peripherals, see above
                        byte[] spare_space6 = new byte[16];     // 0x1A0, 0x20
                        byte[] spare_space7 = new byte[64];     // 0x1B0, Inside here should be modem information, but I need to get a modem-enabled game
                        byte[] region_codes = new byte[16];     // 0x1F0, Region codes, space-filled
                        //Reading all data
                        Array.Copy(ipbin_sector, 0x010, volume_name, 0, 11);      // Varies
                        Array.Copy(ipbin_sector, 0x01B, spare_space1, 0, 1);         // 0x00
                        Array.Copy(ipbin_sector, 0x01C, volume_version, 0, 2);       // Volume version in BCD. <100 = Prerelease.
                        Array.Copy(ipbin_sector, 0x01E, volume_type, 0, 2);          // Bit 0 = 1 => CD-ROM. Rest should be 0.
                        Array.Copy(ipbin_sector, 0x020, system_name, 0, 11);      // Unknown, varies!
                        Array.Copy(ipbin_sector, 0x02B, spare_space2, 0, 1);         // 0x00
                        Array.Copy(ipbin_sector, 0x02C, system_version, 0, 2);       // Should be 1
                        Array.Copy(ipbin_sector, 0x02E, spare_space3, 0, 2);         // 0x0000
                        Array.Copy(ipbin_sector, 0x030, ip_address, 0, 4);	      // Initial program address
                        Array.Copy(ipbin_sector, 0x034, ip_loadsize, 0, 4);          // Load size of initial program
                        Array.Copy(ipbin_sector, 0x038, ip_entry_address, 0, 4);     // Initial program entry address
                        Array.Copy(ipbin_sector, 0x03C, ip_work_ram_size, 0, 4);     // Initial program work RAM size in bytes
                        Array.Copy(ipbin_sector, 0x040, sp_address, 0, 4);	      // System program address
                        Array.Copy(ipbin_sector, 0x044, sp_loadsize, 0, 4);          // Load size of system program
                        Array.Copy(ipbin_sector, 0x048, sp_entry_address, 0, 4);     // System program entry address
                        Array.Copy(ipbin_sector, 0x04C, sp_work_ram_size, 0, 4);     // System program work RAM size in bytes
                        Array.Copy(ipbin_sector, 0x050, release_date, 0, 8);      // MMDDYYYY
                        Array.Copy(ipbin_sector, 0x058, unknown1, 0, 7);          // Seems to be all 0x20s
                        Array.Copy(ipbin_sector, 0x05F, spare_space4, 0, 1);         // 0x00 ?
                        Array.Copy(ipbin_sector, 0x060, system_reserved, 0, 160); // System Reserved Area
                        Array.Copy(ipbin_sector, 0x100, hardware_id, 0, 16);      // Hardware ID
                        Array.Copy(ipbin_sector, 0x110, copyright, 0, 3);         // "(C)" -- Can be the developer code directly!, if that is the code release date will be displaced
                        if(CurrentEncoding.GetString(copyright) == "(C)")
                            Array.Copy(ipbin_sector, 0x113, developer_code, 0, 5);    // "SEGA" or "T-xx"
                        else
                            Array.Copy(ipbin_sector, 0x110, developer_code, 0, 5);    // "SEGA" or "T-xx"
                        Array.Copy(ipbin_sector, 0x118, release_date2, 0, 8);     // Another release date, this with month in letters?
                        Array.Copy(ipbin_sector, 0x120, domestic_title, 0, 48);   // Domestic version of the game title
                        Array.Copy(ipbin_sector, 0x150, overseas_title, 0, 48);   // Overseas version of the game title
                        //Array.Copy(ipbin_sector, 0x000, application_type, 0, 2);  // Application type
                        //Array.Copy(ipbin_sector, 0x000, space_space5, 0, 1);         // 0x20
                        Array.Copy(ipbin_sector, 0x180, product_code, 0, 13);      // Official product code
                        Array.Copy(ipbin_sector, 0x190, peripherals, 0, 16);      // Supported peripherals, see above
                        Array.Copy(ipbin_sector, 0x1A0, spare_space6, 0, 16);     // 0x20
                        Array.Copy(ipbin_sector, 0x1B0, spare_space7, 0, 64);     // Inside here should be modem information, but I need to get a modem-enabled game
                        Array.Copy(ipbin_sector, 0x1F0, region_codes, 0, 16);     // Region codes, space-filled

                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.volume_name = \"{0}\"", CurrentEncoding.GetString(volume_name));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.system_name = \"{0}\"", CurrentEncoding.GetString(system_name));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.volume_version = \"{0}\"", CurrentEncoding.GetString(volume_version));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.volume_type = 0x{0}", BitConverter.ToInt16(volume_type, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.system_version = 0x{0}", BitConverter.ToInt16(system_version, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.ip_address = 0x{0}", BitConverter.ToInt32(ip_address, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.ip_loadsize = {0}", BitConverter.ToInt32(ip_loadsize, 0));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.ip_entry_address = 0x{0}", BitConverter.ToInt32(ip_entry_address, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.ip_work_ram_size = {0}", BitConverter.ToInt32(ip_work_ram_size, 0));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.sp_address = 0x{0}", BitConverter.ToInt32(sp_address, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.sp_loadsize = {0}", BitConverter.ToInt32(sp_loadsize, 0));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.sp_entry_address = 0x{0}", BitConverter.ToInt32(sp_entry_address, 0).ToString("X"));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.sp_work_ram_size = {0}", BitConverter.ToInt32(sp_work_ram_size, 0));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.release_date = \"{0}\"", CurrentEncoding.GetString(release_date));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.release_date2 = \"{0}\"", CurrentEncoding.GetString(release_date2));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.developer_code = \"{0}\"", CurrentEncoding.GetString(developer_code));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.domestic_title = \"{0}\"", CurrentEncoding.GetString(domestic_title));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.overseas_title = \"{0}\"", CurrentEncoding.GetString(overseas_title));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.product_code = \"{0}\"", CurrentEncoding.GetString(product_code));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.peripherals = \"{0}\"", CurrentEncoding.GetString(peripherals));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "segacd_ipbin.region_codes = \"{0}\"", CurrentEncoding.GetString(region_codes));

                        // Decoding all data
                        DateTime ipbindate = DateTime.MinValue;
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        try
                        {
                            ipbindate = DateTime.ParseExact(CurrentEncoding.GetString(release_date), "MMddyyyy", provider);
                        }
                        catch
                        {
                            try
                            {
                                ipbindate = DateTime.ParseExact(CurrentEncoding.GetString(release_date2), "yyyy.MMM", provider);
                            }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                            catch { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                        }

                        /*
                        switch (CurrentEncoding.GetString(application_type))
                        {
                            case "GM":
                                IPBinInformation.AppendLine("Disc is a game.");
                                break;
                            case "AI":
                                IPBinInformation.AppendLine("Disc is an application.");
                                break;
                            default:
                                IPBinInformation.AppendLine("Disc is from unknown type.");
                                break;
                        }
                        */

                        IPBinInformation.AppendFormat("Volume name: {0}", CurrentEncoding.GetString(volume_name)).AppendLine();
                        //IPBinInformation.AppendFormat("Volume version: {0}", CurrentEncoding.GetString(volume_version)).AppendLine();
                        //IPBinInformation.AppendFormat("{0}", CurrentEncoding.GetString(volume_type)).AppendLine();
                        IPBinInformation.AppendFormat("System name: {0}", CurrentEncoding.GetString(system_name)).AppendLine();
                        //IPBinInformation.AppendFormat("System version: {0}", CurrentEncoding.GetString(system_version)).AppendLine();
                        IPBinInformation.AppendFormat("Initial program address: 0x{0}", BitConverter.ToInt32(ip_address, 0).ToString("X")).AppendLine();
                        IPBinInformation.AppendFormat("Initial program load size: {0} bytes", BitConverter.ToInt32(ip_loadsize, 0)).AppendLine();
                        IPBinInformation.AppendFormat("Initial program entry address: 0x{0}", BitConverter.ToInt32(ip_entry_address, 0).ToString("X")).AppendLine();
                        IPBinInformation.AppendFormat("Initial program work RAM: {0} bytes", BitConverter.ToInt32(ip_work_ram_size, 0)).AppendLine();
                        IPBinInformation.AppendFormat("System program address: 0x{0}", BitConverter.ToInt32(sp_address, 0).ToString("X")).AppendLine();
                        IPBinInformation.AppendFormat("System program load size: {0} bytes", BitConverter.ToInt32(sp_loadsize, 0)).AppendLine();
                        IPBinInformation.AppendFormat("System program entry address: 0x{0}", BitConverter.ToInt32(sp_entry_address, 0).ToString("X")).AppendLine();
                        IPBinInformation.AppendFormat("System program work RAM: {0} bytes", BitConverter.ToInt32(sp_work_ram_size, 0)).AppendLine();
                        if(ipbindate != DateTime.MinValue)
                            IPBinInformation.AppendFormat("Release date: {0}", ipbindate).AppendLine();
                        //IPBinInformation.AppendFormat("Release date (other format): {0}", CurrentEncoding.GetString(release_date2)).AppendLine();
                        IPBinInformation.AppendFormat("Hardware ID: {0}", CurrentEncoding.GetString(hardware_id)).AppendLine();
                        IPBinInformation.AppendFormat("Developer code: {0}", CurrentEncoding.GetString(developer_code)).AppendLine();
                        IPBinInformation.AppendFormat("Domestic title: {0}", CurrentEncoding.GetString(domestic_title)).AppendLine();
                        IPBinInformation.AppendFormat("Overseas title: {0}", CurrentEncoding.GetString(overseas_title)).AppendLine();
                        IPBinInformation.AppendFormat("Product code: {0}", CurrentEncoding.GetString(product_code)).AppendLine();
                        IPBinInformation.AppendFormat("Peripherals:").AppendLine();
                        foreach(byte peripheral in peripherals)
                        {
                            switch((char)peripheral)
                            {
                                case 'A':
                                    IPBinInformation.AppendLine("Game supports analog controller.");
                                    break;
                                case 'B':
                                    IPBinInformation.AppendLine("Game supports trackball.");
                                    break;
                                case 'G':
                                    IPBinInformation.AppendLine("Game supports light gun.");
                                    break;
                                case 'J':
                                    IPBinInformation.AppendLine("Game supports JoyPad.");
                                    break;
                                case 'K':
                                    IPBinInformation.AppendLine("Game supports keyboard.");
                                    break;
                                case 'M':
                                    IPBinInformation.AppendLine("Game supports mouse.");
                                    break;
                                case 'O':
                                    IPBinInformation.AppendLine("Game supports Master System's JoyPad.");
                                    break;
                                case 'P':
                                    IPBinInformation.AppendLine("Game supports printer interface.");
                                    break;
                                case 'R':
                                    IPBinInformation.AppendLine("Game supports serial (RS-232C) interface.");
                                    break;
                                case 'T':
                                    IPBinInformation.AppendLine("Game supports tablet interface.");
                                    break;
                                case 'V':
                                    IPBinInformation.AppendLine("Game supports paddle controller.");
                                    break;
                                case ' ':
                                    break;
                                default:
                                    IPBinInformation.AppendFormat("Game supports unknown peripheral {0}.", peripheral).AppendLine();
                                    break;
                            }
                        }
                        IPBinInformation.AppendLine("Regions supported:");
                        foreach(byte region in region_codes)
                        {
                            switch((char)region)
                            {
                                case 'J':
                                    IPBinInformation.AppendLine("Japanese NTSC.");
                                    break;
                                case 'U':
                                    IPBinInformation.AppendLine("USA NTSC.");
                                    break;
                                case 'E':
                                    IPBinInformation.AppendLine("Europe PAL.");
                                    break;
                                case ' ':
                                    break;
                                default:
                                    IPBinInformation.AppendFormat("Game supports unknown region {0}.", region).AppendLine();
                                    break;
                            }
                        }

                        break;
                    }
                case "SEGA SEGASATURN ":
                    {
                        Saturn = true;

                        DicConsole.DebugWriteLine("ISO9660 plugin", "Found Sega Saturn IP.BIN");

                        IPBinInformation.AppendLine("--------------------------------");
                        IPBinInformation.AppendLine("SEGA IP.BIN INFORMATION:");
                        IPBinInformation.AppendLine("--------------------------------");

                        // Definitions following
                        byte[] maker_id = new byte[16];         // 0x010, "SEGA ENTERPRISES"
                        byte[] product_no = new byte[10];       // 0x020, Product number
                        byte[] product_version = new byte[6];   // 0x02A, Product version
                        byte[] release_date = new byte[8];      // 0x030, YYYYMMDD
                        byte[] saturn_media = new byte[3];      // 0x038, "CD-"
                        byte[] disc_no = new byte[1];           // 0x03B, Disc number
                        byte[] disc_no_separator = new byte[1]; // 0x03C, '/'
                        byte[] disc_total_nos = new byte[1];    // 0x03D, Total number of discs
                        byte[] spare_space1 = new byte[2];	    // 0x03E, "  "
                        byte[] region_codes = new byte[16];     // 0x040, Region codes, space-filled
                        byte[] peripherals = new byte[16];      // 0x050, Supported peripherals, see above
                        byte[] product_name = new byte[112];	// 0x060, Game name, space-filled
                        // Reading all data
                        Array.Copy(ipbin_sector, 0x010, maker_id, 0, 16);         // "SEGA ENTERPRISES"
                        Array.Copy(ipbin_sector, 0x020, product_no, 0, 10);       // Product number
                        Array.Copy(ipbin_sector, 0x02A, product_version, 0, 6);   // Product version
                        Array.Copy(ipbin_sector, 0x030, release_date, 0, 8);      // YYYYMMDD
                        Array.Copy(ipbin_sector, 0x038, saturn_media, 0, 3);      // "CD-"
                        Array.Copy(ipbin_sector, 0x03B, disc_no, 0, 1);           // Disc number
                        Array.Copy(ipbin_sector, 0x03C, disc_no_separator, 0, 1); // '/'
                        Array.Copy(ipbin_sector, 0x03D, disc_total_nos, 0, 1);    // Total number of discs
                        Array.Copy(ipbin_sector, 0x03E, spare_space1, 0, 2);	  // "  "
                        Array.Copy(ipbin_sector, 0x040, region_codes, 0, 16);     // Region codes, space-filled
                        Array.Copy(ipbin_sector, 0x050, peripherals, 0, 16);      // Supported peripherals, see above
                        Array.Copy(ipbin_sector, 0x060, product_name, 0, 112);	  // Game name, space-filled

                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.maker_id = \"{0}\"", CurrentEncoding.GetString(maker_id));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.product_no = \"{0}\"", CurrentEncoding.GetString(product_no));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.product_version = \"{0}\"", CurrentEncoding.GetString(product_version));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.release_datedate = \"{0}\"", CurrentEncoding.GetString(release_date));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.saturn_media = \"{0}\"", CurrentEncoding.GetString(saturn_media));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.disc_no = {0}", CurrentEncoding.GetString(disc_no));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.disc_no_separator = \"{0}\"", CurrentEncoding.GetString(disc_no_separator));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.disc_total_nos = {0}", CurrentEncoding.GetString(disc_total_nos));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.release_date = \"{0}\"", CurrentEncoding.GetString(release_date));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.spare_space1 = \"{0}\"", CurrentEncoding.GetString(spare_space1));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.region_codes = \"{0}\"", CurrentEncoding.GetString(region_codes));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.peripherals = \"{0}\"", CurrentEncoding.GetString(peripherals));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "saturn_ipbin.product_name = \"{0}\"", CurrentEncoding.GetString(product_name));

                        // Decoding all data
                        DateTime ipbindate;
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        ipbindate = DateTime.ParseExact(CurrentEncoding.GetString(release_date), "yyyyMMdd", provider);
                        IPBinInformation.AppendFormat("Product name: {0}", CurrentEncoding.GetString(product_name)).AppendLine();
                        IPBinInformation.AppendFormat("Product number: {0}", CurrentEncoding.GetString(product_no)).AppendLine();
                        IPBinInformation.AppendFormat("Product version: {0}", CurrentEncoding.GetString(product_version)).AppendLine();
                        IPBinInformation.AppendFormat("Release date: {0}", ipbindate).AppendLine();
                        IPBinInformation.AppendFormat("Disc number {0} of {1}", CurrentEncoding.GetString(disc_no), CurrentEncoding.GetString(disc_total_nos)).AppendLine();

                        IPBinInformation.AppendFormat("Peripherals:").AppendLine();
                        foreach(byte peripheral in peripherals)
                        {
                            switch((char)peripheral)
                            {
                                case 'A':
                                    IPBinInformation.AppendLine("Game supports analog controller.");
                                    break;
                                case 'J':
                                    IPBinInformation.AppendLine("Game supports JoyPad.");
                                    break;
                                case 'K':
                                    IPBinInformation.AppendLine("Game supports keyboard.");
                                    break;
                                case 'M':
                                    IPBinInformation.AppendLine("Game supports mouse.");
                                    break;
                                case 'S':
                                    IPBinInformation.AppendLine("Game supports analog steering controller.");
                                    break;
                                case 'T':
                                    IPBinInformation.AppendLine("Game supports multitap.");
                                    break;
                                case ' ':
                                    break;
                                default:
                                    IPBinInformation.AppendFormat("Game supports unknown peripheral {0}.", peripheral).AppendLine();
                                    break;
                            }
                        }
                        IPBinInformation.AppendLine("Regions supported:");
                        foreach(byte region in region_codes)
                        {
                            switch((char)region)
                            {
                                case 'J':
                                    IPBinInformation.AppendLine("Japanese NTSC.");
                                    break;
                                case 'U':
                                    IPBinInformation.AppendLine("North America NTSC.");
                                    break;
                                case 'E':
                                    IPBinInformation.AppendLine("Europe PAL.");
                                    break;
                                case 'T':
                                    IPBinInformation.AppendLine("Asia NTSC.");
                                    break;
                                case ' ':
                                    break;
                                default:
                                    IPBinInformation.AppendFormat("Game supports unknown region {0}.", region).AppendLine();
                                    break;
                            }
                        }

                        break;
                    }
                case "SEGA SEGAKATANA ":
                    {
                        Dreamcast = true;

                        DicConsole.DebugWriteLine("ISO9660 plugin", "Found Sega Dreamcast IP.BIN");

                        IPBinInformation.AppendLine("--------------------------------");
                        IPBinInformation.AppendLine("SEGA IP.BIN INFORMATION:");
                        IPBinInformation.AppendLine("--------------------------------");

                        // Declarations following
                        byte[] maker_id = new byte[16];         // 0x010, "SEGA ENTERPRISES"
                        byte[] dreamcast_crc = new byte[4];     // 0x020, CRC of product_no and product_version
                        byte[] spare_space1 = new byte[1];      // 0x024, " "
                        byte[] dreamcast_media = new byte[6];   // 0x025, "GD-ROM"
                        byte[] disc_no = new byte[1];           // 0x02B, Disc number
                        byte[] disc_no_separator = new byte[1]; // 0x02C, '/'
                        byte[] disc_total_nos = new byte[1];    // 0x02D, Total number of discs
                        byte[] spare_space2 = new byte[2];	    // 0x02E, "  "
                        byte[] region_codes = new byte[8];      // 0x030, Region codes, space-filled
                        byte[] peripherals = new byte[7];       // 0x038, Supported peripherals, bitwise
                        byte[] product_no = new byte[10];       // 0x03C, Product number
                        byte[] product_version = new byte[6];	// 0x046, Product version
                        byte[] release_date = new byte[8];      // 0x04C, YYYYMMDD
                        byte[] spare_space3 = new byte[8];      // 0x054, "  "
                        byte[] boot_filename = new byte[12];    // 0x05C, Usually "1ST_READ.BIN" or "0WINCE.BIN  "
                        byte[] producer = new byte[16];         // 0x068, Game producer, space-filled
                        byte[] product_name = new byte[128];    // 0x078, Game name, space-filled
                        // Reading all data
                        Array.Copy(ipbin_sector, 0x010, maker_id, 0, 16);      // "SEGA ENTERPRISES"
                        Array.Copy(ipbin_sector, 0x020, dreamcast_crc, 0, 4);         // CRC of product_no and product_version (hex)
                        Array.Copy(ipbin_sector, 0x024, spare_space1, 0, 1);       // " "
                        Array.Copy(ipbin_sector, 0x025, dreamcast_media, 0, 6);          // "GD-ROM"
                        Array.Copy(ipbin_sector, 0x02B, disc_no, 0, 1);         // Disc number
                        Array.Copy(ipbin_sector, 0x02C, disc_no_separator, 0, 1);       // '/'
                        Array.Copy(ipbin_sector, 0x02D, disc_total_nos, 0, 1);         // Total number of discs
                        Array.Copy(ipbin_sector, 0x02E, spare_space2, 0, 2);	      // "  "
                        Array.Copy(ipbin_sector, 0x030, region_codes, 0, 8);          // Region codes, space-filled
                        Array.Copy(ipbin_sector, 0x038, peripherals, 0, 7);     // Supported peripherals, hexadecimal
                        Array.Copy(ipbin_sector, 0x040, product_no, 0, 10);     // Product number
                        Array.Copy(ipbin_sector, 0x04A, product_version, 0, 6);	     // Product version
                        Array.Copy(ipbin_sector, 0x050, release_date, 0, 8);     // YYYYMMDD
                        Array.Copy(ipbin_sector, 0x058, spare_space3, 0, 8);        // "  "
                        Array.Copy(ipbin_sector, 0x060, boot_filename, 0, 12);     // Usually "1ST_READ.BIN" or "0WINCE.BIN  "
                        Array.Copy(ipbin_sector, 0x070, producer, 0, 16);     // Game producer, space-filled
                        Array.Copy(ipbin_sector, 0x080, product_name, 0, 128);     // Game name, space-filled

                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.maker_id = \"{0}\"", CurrentEncoding.GetString(maker_id));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.dreamcast_crc = 0x{0}", CurrentEncoding.GetString(dreamcast_crc));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.spare_space1 = \"{0}\"", CurrentEncoding.GetString(spare_space1));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.dreamcast_media = \"{0}\"", CurrentEncoding.GetString(dreamcast_media));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.disc_no = {0}", CurrentEncoding.GetString(disc_no));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.disc_no_separator = \"{0}\"", CurrentEncoding.GetString(disc_no_separator));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.disc_total_nos = \"{0}\"", CurrentEncoding.GetString(disc_total_nos));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.spare_space2 = \"{0}\"", CurrentEncoding.GetString(spare_space2));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.region_codes = \"{0}\"", CurrentEncoding.GetString(region_codes));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.peripherals = \"{0}\"", CurrentEncoding.GetString(peripherals));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.product_no = \"{0}\"", CurrentEncoding.GetString(product_no));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.product_version = \"{0}\"", CurrentEncoding.GetString(product_version));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.release_date = \"{0}\"", CurrentEncoding.GetString(release_date));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.spare_space3 = \"{0}\"", CurrentEncoding.GetString(spare_space3));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.boot_filename = \"{0}\"", CurrentEncoding.GetString(boot_filename));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.producer = \"{0}\"", CurrentEncoding.GetString(producer));
                        DicConsole.DebugWriteLine("ISO9660 plugin", "dreamcast_ipbin.product_name = \"{0}\"", CurrentEncoding.GetString(product_name));

                        // Decoding all data
                        DateTime ipbindate;
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        ipbindate = DateTime.ParseExact(CurrentEncoding.GetString(release_date), "yyyyMMdd", provider);
                        IPBinInformation.AppendFormat("Product name: {0}", CurrentEncoding.GetString(product_name)).AppendLine();
                        IPBinInformation.AppendFormat("Product version: {0}", CurrentEncoding.GetString(product_version)).AppendLine();
                        IPBinInformation.AppendFormat("Producer: {0}", CurrentEncoding.GetString(producer)).AppendLine();
                        IPBinInformation.AppendFormat("Disc media: {0}", CurrentEncoding.GetString(dreamcast_media)).AppendLine();
                        IPBinInformation.AppendFormat("Disc number {0} of {1}", CurrentEncoding.GetString(disc_no), CurrentEncoding.GetString(disc_total_nos)).AppendLine();
                        IPBinInformation.AppendFormat("Release date: {0}", ipbindate).AppendLine();
                        switch(CurrentEncoding.GetString(boot_filename))
                        {
                            case "1ST_READ.BIN":
                                IPBinInformation.AppendLine("Disc boots natively.");
                                break;
                            case "0WINCE.BIN  ":
                                IPBinInformation.AppendLine("Disc boots using Windows CE.");
                                break;
                            default:
                                IPBinInformation.AppendFormat("Disc boots using unknown loader: {0}.", CurrentEncoding.GetString(boot_filename)).AppendLine();
                                break;
                        }
                        IPBinInformation.AppendLine("Regions supported:");
                        foreach(byte region in region_codes)
                        {
                            switch((char)region)
                            {
                                case 'J':
                                    IPBinInformation.AppendLine("Japanese NTSC.");
                                    break;
                                case 'U':
                                    IPBinInformation.AppendLine("North America NTSC.");
                                    break;
                                case 'E':
                                    IPBinInformation.AppendLine("Europe PAL.");
                                    break;
                                case ' ':
                                    break;
                                default:
                                    IPBinInformation.AppendFormat("Game supports unknown region {0}.", region).AppendLine();
                                    break;
                            }
                        }

                        int iPeripherals = int.Parse(CurrentEncoding.GetString(peripherals), NumberStyles.HexNumber);

                        if((iPeripherals & 0x00000001) == 0x00000001)
                            IPBinInformation.AppendLine("Game uses Windows CE.");

                        IPBinInformation.AppendFormat("Peripherals:").AppendLine();

                        if((iPeripherals & 0x00000010) == 0x00000010)
                            IPBinInformation.AppendLine("Game supports the VGA Box.");
                        if((iPeripherals & 0x00000100) == 0x00000100)
                            IPBinInformation.AppendLine("Game supports other expansion.");
                        if((iPeripherals & 0x00000200) == 0x00000200)
                            IPBinInformation.AppendLine("Game supports Puru Puru pack.");
                        if((iPeripherals & 0x00000400) == 0x00000400)
                            IPBinInformation.AppendLine("Game supports Mike Device.");
                        if((iPeripherals & 0x00000800) == 0x00000800)
                            IPBinInformation.AppendLine("Game supports Memory Card.");
                        if((iPeripherals & 0x00001000) == 0x00001000)
                            IPBinInformation.AppendLine("Game requires A + B + Start buttons and D-Pad.");
                        if((iPeripherals & 0x00002000) == 0x00002000)
                            IPBinInformation.AppendLine("Game requires C button.");
                        if((iPeripherals & 0x00004000) == 0x00004000)
                            IPBinInformation.AppendLine("Game requires D button.");
                        if((iPeripherals & 0x00008000) == 0x00008000)
                            IPBinInformation.AppendLine("Game requires X button.");
                        if((iPeripherals & 0x00010000) == 0x00010000)
                            IPBinInformation.AppendLine("Game requires Y button.");
                        if((iPeripherals & 0x00020000) == 0x00020000)
                            IPBinInformation.AppendLine("Game requires Z button.");
                        if((iPeripherals & 0x00040000) == 0x00040000)
                            IPBinInformation.AppendLine("Game requires expanded direction buttons.");
                        if((iPeripherals & 0x00080000) == 0x00080000)
                            IPBinInformation.AppendLine("Game requires analog R trigger.");
                        if((iPeripherals & 0x00100000) == 0x00100000)
                            IPBinInformation.AppendLine("Game requires analog L trigger.");
                        if((iPeripherals & 0x00200000) == 0x00200000)
                            IPBinInformation.AppendLine("Game requires analog horizontal controller.");
                        if((iPeripherals & 0x00400000) == 0x00400000)
                            IPBinInformation.AppendLine("Game requires analog vertical controller.");
                        if((iPeripherals & 0x00800000) == 0x00800000)
                            IPBinInformation.AppendLine("Game requires expanded analog horizontal controller.");
                        if((iPeripherals & 0x01000000) == 0x01000000)
                            IPBinInformation.AppendLine("Game requires expanded analog vertical controller.");
                        if((iPeripherals & 0x02000000) == 0x02000000)
                            IPBinInformation.AppendLine("Game supports Gun.");
                        if((iPeripherals & 0x04000000) == 0x04000000)
                            IPBinInformation.AppendLine("Game supports Keyboard.");
                        if((iPeripherals & 0x08000000) == 0x08000000)
                            IPBinInformation.AppendLine("Game supports Mouse.");

                        if((iPeripherals & 0xEE) != 0)
                            IPBinInformation.AppendFormat("Game supports unknown peripherals mask {0:X2}", (iPeripherals & 0xEE));

                        break;
                    }
            }
            #endregion

            ISOMetadata.AppendFormat("ISO9660 file system").AppendLine();
            if(Joliet)
                ISOMetadata.AppendFormat("Joliet extensions present.").AppendLine();
            if(RockRidge)
                ISOMetadata.AppendFormat("Rock Ridge Interchange Protocol present.").AppendLine();
            if(Bootable)
                ISOMetadata.AppendFormat("Disc bootable following {0} specifications.", BootSpec).AppendLine();
            if(SegaCD)
            {
                ISOMetadata.AppendLine("This is a SegaCD / MegaCD disc.");
                ISOMetadata.AppendLine(IPBinInformation.ToString());
            }
            if(Saturn)
            {
                ISOMetadata.AppendLine("This is a Sega Saturn disc.");
                ISOMetadata.AppendLine(IPBinInformation.ToString());
            }
            if(Dreamcast)
            {
                ISOMetadata.AppendLine("This is a Sega Dreamcast disc.");
                ISOMetadata.AppendLine(IPBinInformation.ToString());
            }
            ISOMetadata.AppendLine("--------------------------------");
            ISOMetadata.AppendLine("VOLUME DESCRIPTOR INFORMATION:");
            ISOMetadata.AppendLine("--------------------------------");
            ISOMetadata.AppendFormat("System identifier: {0}", decodedVD.SystemIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Volume identifier: {0}", decodedVD.VolumeIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Volume set identifier: {0}", decodedVD.VolumeSetIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Publisher identifier: {0}", decodedVD.PublisherIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Data preparer identifier: {0}", decodedVD.DataPreparerIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Application identifier: {0}", decodedVD.ApplicationIdentifier).AppendLine();
            ISOMetadata.AppendFormat("Volume creation date: {0}", decodedVD.CreationTime).AppendLine();
            if(decodedVD.HasModificationTime)
                ISOMetadata.AppendFormat("Volume modification date: {0}", decodedVD.ModificationTime).AppendLine();
            else
                ISOMetadata.AppendFormat("Volume has not been modified.").AppendLine();
            if(decodedVD.HasExpirationTime)
                ISOMetadata.AppendFormat("Volume expiration date: {0}", decodedVD.ExpirationTime).AppendLine();
            else
                ISOMetadata.AppendFormat("Volume does not expire.").AppendLine();
            if(decodedVD.HasEffectiveTime)
                ISOMetadata.AppendFormat("Volume effective date: {0}", decodedVD.EffectiveTime).AppendLine();
            else
                ISOMetadata.AppendFormat("Volume has always been effective.").AppendLine();

            if(Joliet)
            {
                ISOMetadata.AppendLine("---------------------------------------");
                ISOMetadata.AppendLine("JOLIET VOLUME DESCRIPTOR INFORMATION:");
                ISOMetadata.AppendLine("---------------------------------------");
                ISOMetadata.AppendFormat("System identifier: {0}", decodedJolietVD.SystemIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Volume identifier: {0}", decodedJolietVD.VolumeIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Volume set identifier: {0}", decodedJolietVD.VolumeSetIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Publisher identifier: {0}", decodedJolietVD.PublisherIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Data preparer identifier: {0}", decodedJolietVD.DataPreparerIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Application identifier: {0}", decodedJolietVD.ApplicationIdentifier).AppendLine();
                ISOMetadata.AppendFormat("Volume creation date: {0}", decodedJolietVD.CreationTime).AppendLine();
                if(decodedJolietVD.HasModificationTime)
                    ISOMetadata.AppendFormat("Volume modification date: {0}", decodedJolietVD.ModificationTime).AppendLine();
                else
                    ISOMetadata.AppendFormat("Volume has not been modified.").AppendLine();
                if(decodedJolietVD.HasExpirationTime)
                    ISOMetadata.AppendFormat("Volume expiration date: {0}", decodedJolietVD.ExpirationTime).AppendLine();
                else
                    ISOMetadata.AppendFormat("Volume does not expire.").AppendLine();
                if(decodedJolietVD.HasEffectiveTime)
                    ISOMetadata.AppendFormat("Volume effective date: {0}", decodedJolietVD.EffectiveTime).AppendLine();
                else
                    ISOMetadata.AppendFormat("Volume has always been effective.").AppendLine();
            }

            xmlFSType = new Schemas.FileSystemType();
            xmlFSType.Type = "ISO9660";

            if(Joliet)
            {
                xmlFSType.VolumeName = decodedJolietVD.VolumeIdentifier;

                if(decodedJolietVD.SystemIdentifier == null || decodedVD.SystemIdentifier.Length > decodedJolietVD.SystemIdentifier.Length)
                    xmlFSType.SystemIdentifier = decodedVD.SystemIdentifier;
                else
                    xmlFSType.SystemIdentifier = decodedJolietVD.SystemIdentifier;

                if(decodedJolietVD.VolumeSetIdentifier == null || decodedVD.VolumeSetIdentifier.Length > decodedJolietVD.VolumeSetIdentifier.Length)
                    xmlFSType.VolumeSetIdentifier = decodedVD.VolumeSetIdentifier;
                else
                    xmlFSType.VolumeSetIdentifier = decodedJolietVD.VolumeSetIdentifier;

                if(decodedJolietVD.PublisherIdentifier == null || decodedVD.PublisherIdentifier.Length > decodedJolietVD.PublisherIdentifier.Length)
                    xmlFSType.PublisherIdentifier = decodedVD.PublisherIdentifier;
                else
                    xmlFSType.PublisherIdentifier = decodedJolietVD.PublisherIdentifier;

                if(decodedJolietVD.DataPreparerIdentifier == null || decodedVD.DataPreparerIdentifier.Length > decodedJolietVD.DataPreparerIdentifier.Length)
                    xmlFSType.DataPreparerIdentifier = decodedVD.DataPreparerIdentifier;
                else
                    xmlFSType.DataPreparerIdentifier = decodedJolietVD.SystemIdentifier;

                if(decodedJolietVD.ApplicationIdentifier == null || decodedVD.ApplicationIdentifier.Length > decodedJolietVD.ApplicationIdentifier.Length)
                    xmlFSType.ApplicationIdentifier = decodedVD.ApplicationIdentifier;
                else
                    xmlFSType.ApplicationIdentifier = decodedJolietVD.SystemIdentifier;

                xmlFSType.CreationDate = decodedJolietVD.CreationTime;
                xmlFSType.CreationDateSpecified = true;
                if(decodedJolietVD.HasModificationTime)
                {
                    xmlFSType.ModificationDate = decodedJolietVD.ModificationTime;
                    xmlFSType.ModificationDateSpecified = true;
                }
                if(decodedJolietVD.HasExpirationTime)
                {
                    xmlFSType.ExpirationDate = decodedJolietVD.ExpirationTime;
                    xmlFSType.ExpirationDateSpecified = true;
                }
                if(decodedJolietVD.HasEffectiveTime)
                {
                    xmlFSType.EffectiveDate = decodedJolietVD.EffectiveTime;
                    xmlFSType.EffectiveDateSpecified = true;
                }
            }
            else
            {
                xmlFSType.SystemIdentifier = decodedVD.SystemIdentifier;
                xmlFSType.VolumeName = decodedVD.VolumeIdentifier;
                xmlFSType.VolumeSetIdentifier = decodedVD.VolumeSetIdentifier;
                xmlFSType.PublisherIdentifier = decodedVD.PublisherIdentifier;
                xmlFSType.DataPreparerIdentifier = decodedVD.DataPreparerIdentifier;
                xmlFSType.ApplicationIdentifier = decodedVD.ApplicationIdentifier;
                xmlFSType.CreationDate = decodedVD.CreationTime;
                xmlFSType.CreationDateSpecified = true;
                if(decodedVD.HasModificationTime)
                {
                    xmlFSType.ModificationDate = decodedVD.ModificationTime;
                    xmlFSType.ModificationDateSpecified = true;
                }
                if(decodedVD.HasExpirationTime)
                {
                    xmlFSType.ExpirationDate = decodedVD.ExpirationTime;
                    xmlFSType.ExpirationDateSpecified = true;
                }
                if(decodedVD.HasEffectiveTime)
                {
                    xmlFSType.EffectiveDate = decodedVD.EffectiveTime;
                    xmlFSType.EffectiveDateSpecified = true;
                }
            }

            xmlFSType.Bootable |= Bootable || SegaCD || Saturn || Dreamcast;
            xmlFSType.Clusters = (long)(partition.End - partition.Start + 1);
            xmlFSType.ClusterSize = 2048;

            information = ISOMetadata.ToString();
        }

        static DecodedVolumeDescriptor DecodeJolietDescriptor(byte[] VDSysId, byte[] VDVolId, byte[] VDVolSetId, byte[] VDPubId, byte[] VDDataPrepId, byte[] VDAppId, byte[] VCTime, byte[] VMTime, byte[] VXTime, byte[] VETime)
        {
            DecodedVolumeDescriptor decodedVD = new DecodedVolumeDescriptor();

            decodedVD.SystemIdentifier = Encoding.BigEndianUnicode.GetString(VDSysId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.VolumeIdentifier = Encoding.BigEndianUnicode.GetString(VDVolId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.VolumeSetIdentifier = Encoding.BigEndianUnicode.GetString(VDVolSetId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.PublisherIdentifier = Encoding.BigEndianUnicode.GetString(VDPubId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.DataPreparerIdentifier = Encoding.BigEndianUnicode.GetString(VDDataPrepId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.ApplicationIdentifier = Encoding.BigEndianUnicode.GetString(VDAppId).TrimEnd().Trim(new[] { '\u0000' });
            if(VCTime[0] < 0x31 || VCTime[0] > 0x39)
                decodedVD.CreationTime = DateTime.MinValue;
            else
                decodedVD.CreationTime = DateHandlers.ISO9660ToDateTime(VCTime);

            if(VMTime[0] < 0x31 || VMTime[0] > 0x39)
            {
                decodedVD.HasModificationTime = false;
            }
            else
            {
                decodedVD.HasModificationTime = true;
                decodedVD.ModificationTime = DateHandlers.ISO9660ToDateTime(VMTime);
            }

            if(VXTime[0] < 0x31 || VXTime[0] > 0x39)
            {
                decodedVD.HasExpirationTime = false;
            }
            else
            {
                decodedVD.HasExpirationTime = true;
                decodedVD.ExpirationTime = DateHandlers.ISO9660ToDateTime(VXTime);
            }

            if(VETime[0] < 0x31 || VETime[0] > 0x39)
            {
                decodedVD.HasEffectiveTime = false;
            }
            else
            {
                decodedVD.HasEffectiveTime = true;
                decodedVD.EffectiveTime = DateHandlers.ISO9660ToDateTime(VETime);
            }

            return decodedVD;
        }

        static DecodedVolumeDescriptor DecodeVolumeDescriptor(byte[] VDSysId, byte[] VDVolId, byte[] VDVolSetId, byte[] VDPubId, byte[] VDDataPrepId, byte[] VDAppId, byte[] VCTime, byte[] VMTime, byte[] VXTime, byte[] VETime)
        {
            DecodedVolumeDescriptor decodedVD = new DecodedVolumeDescriptor();

            decodedVD.SystemIdentifier = Encoding.ASCII.GetString(VDSysId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.VolumeIdentifier = Encoding.ASCII.GetString(VDVolId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.VolumeSetIdentifier = Encoding.ASCII.GetString(VDVolSetId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.PublisherIdentifier = Encoding.ASCII.GetString(VDPubId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.DataPreparerIdentifier = Encoding.ASCII.GetString(VDDataPrepId).TrimEnd().Trim(new[] { '\u0000' });
            decodedVD.ApplicationIdentifier = Encoding.ASCII.GetString(VDAppId).TrimEnd().Trim(new[] { '\u0000' });
            if(VCTime[0] == '0' || VCTime[0] == 0x00)
                decodedVD.CreationTime = DateTime.MinValue;
            else
                decodedVD.CreationTime = DateHandlers.ISO9660ToDateTime(VCTime);

            if(VMTime[0] == '0' || VMTime[0] == 0x00)
            {
                decodedVD.HasModificationTime = false;
            }
            else
            {
                decodedVD.HasModificationTime = true;
                decodedVD.ModificationTime = DateHandlers.ISO9660ToDateTime(VMTime);
            }

            if(VXTime[0] == '0' || VXTime[0] == 0x00)
            {
                decodedVD.HasExpirationTime = false;
            }
            else
            {
                decodedVD.HasExpirationTime = true;
                decodedVD.ExpirationTime = DateHandlers.ISO9660ToDateTime(VXTime);
            }

            if(VETime[0] == '0' || VETime[0] == 0x00)
            {
                decodedVD.HasEffectiveTime = false;
            }
            else
            {
                decodedVD.HasEffectiveTime = true;
                decodedVD.EffectiveTime = DateHandlers.ISO9660ToDateTime(VETime);
            }

            return decodedVD;
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