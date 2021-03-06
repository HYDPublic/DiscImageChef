// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : CDChecksums.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Checksums.
//
// --[ Description ] ----------------------------------------------------------
//
//     Checks a CD checksum.
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.If not, see<http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2017 Natalia Portillo
// ECC algorithm from ECM(c) 2002-2011 Neill Corlett
// ****************************************************************************/

using System;
using DiscImageChef.Console;

namespace DiscImageChef.Checksums
{
    public static class CDChecksums
    {
        static byte[] ECC_F_Table;
        static byte[] ECC_B_Table;
        const uint CDCRC32Poly = 0xD8018001;
        const uint CDCRC32Seed = 0x00000000;

        public static bool? CheckCDSector(byte[] buffer)
        {
            switch(buffer.Length)
            {
                case 2448:
                    {
                        byte[] subchannel = new byte[96];
                        byte[] channel = new byte[2352];

                        Array.Copy(buffer, 0, channel, 0, 2352);
                        Array.Copy(buffer, 2352, subchannel, 0, 96);

                        bool? channelStatus = CheckCDSectorChannel(channel);
                        bool? subchannelStatus = CheckCDSectorSubChannel(subchannel);
                        bool? status = null;

                        if(channelStatus == null && subchannelStatus == null)
                            status = null;
                        if(channelStatus == false || subchannelStatus == false)
                            status = false;
                        if(channelStatus == null && subchannelStatus == true)
                            status = true;
                        if(channelStatus == true && subchannelStatus == null)
                            status = true;
                        if(channelStatus == true && subchannelStatus == true)
                            status = true;

                        return status;
                    }
                case 2352:
                    return CheckCDSectorChannel(buffer);
                default:
                    return null;
            }
        }

        static void ECCInit()
        {
            ECC_F_Table = new byte[256];
            ECC_B_Table = new byte[256];

            for(uint i = 0; i < 256; i++)
            {
                uint j = (uint)((i << 1) ^ ((i & 0x80) == 0x80 ? 0x11D : 0));
                ECC_F_Table[i] = (byte)j;
                ECC_B_Table[i ^ j] = (byte)i;
            }
        }

        static bool CheckECC(
            byte[] address,
            byte[] data,
            uint major_count,
            uint minor_count,
            uint major_mult,
            uint minor_inc,
            byte[] ecc
        )
        {
            uint size = major_count * minor_count;
            uint major;
            for(major = 0; major < major_count; major++)
            {
                uint index = (major >> 1) * major_mult + (major & 1);
                byte ecc_a = 0;
                byte ecc_b = 0;
                uint minor;
                for(minor = 0; minor < minor_count; minor++)
                {
                    byte temp;
                    if(index < 4)
                    {
                        temp = address[index];
                    }
                    else
                    {
                        temp = data[index - 4];
                    }
                    index += minor_inc;
                    if(index >= size)
                    {
                        index -= size;
                    }
                    ecc_a ^= temp;
                    ecc_b ^= temp;
                    ecc_a = ECC_F_Table[ecc_a];
                }
                ecc_a = ECC_B_Table[ECC_F_Table[ecc_a] ^ ecc_b];
                if(
                    ecc[major] != (ecc_a) ||
                    ecc[major + major_count] != (ecc_a ^ ecc_b))
                {
                    return false;
                }
            }
            return true;
        }

        static bool? CheckCDSectorChannel(byte[] channel)
        {
            ECCInit();

            if(
                channel[0x000] == 0x00 && // sync (12 bytes)
                channel[0x001] == 0xFF &&
                channel[0x002] == 0xFF &&
                channel[0x003] == 0xFF &&
                channel[0x004] == 0xFF &&
                channel[0x005] == 0xFF &&
                channel[0x006] == 0xFF &&
                channel[0x007] == 0xFF &&
                channel[0x008] == 0xFF &&
                channel[0x009] == 0xFF &&
                channel[0x00A] == 0xFF &&
                channel[0x00B] == 0x00)
            {
                DicConsole.DebugWriteLine("CD checksums", "Data sector, address {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);

                if(channel[0x00F] == 0x00) // mode (1 byte)
                {
                    DicConsole.DebugWriteLine("CD checksums", "Mode 0 sector at address {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);
                    for(int i = 0x010; i < 0x930; i++)
                    {
                        if(channel[i] != 0x00)
                        {
                            DicConsole.DebugWriteLine("CD checksums", "Mode 0 sector with error at address: {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);
                            return false;
                        }
                    }
                    return true;
                }
                else if(channel[0x00F] == 0x01) // mode (1 byte)
                {
                    DicConsole.DebugWriteLine("CD checksums", "Mode 1 sector at address {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);

                    if(channel[0x814] != 0x00 || // reserved (8 bytes)
                        channel[0x815] != 0x00 ||
                        channel[0x816] != 0x00 ||
                        channel[0x817] != 0x00 ||
                        channel[0x818] != 0x00 ||
                        channel[0x819] != 0x00 ||
                        channel[0x81A] != 0x00 ||
                        channel[0x81B] != 0x00)
                    {
                        DicConsole.DebugWriteLine("CD checksums", "Mode 1 sector with data in reserved bytes at address: {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);
                        return false;
                    }

                    byte[] address = new byte[4];
                    byte[] data = new byte[2060];
                    byte[] data2 = new byte[2232];
                    byte[] ecc_p = new byte[172];
                    byte[] ecc_q = new byte[104];

                    Array.Copy(channel, 0x0C, address, 0, 4);
                    Array.Copy(channel, 0x0C, data, 0, 2060);
                    Array.Copy(channel, 0x0C, data2, 0, 2232);
                    Array.Copy(channel, 0x81C, ecc_p, 0, 172);
                    Array.Copy(channel, 0x8C8, ecc_q, 0, 104);

                    bool FailedECC_P = CheckECC(address, data, 86, 24, 2, 86, ecc_p);
                    bool FailedECC_Q = CheckECC(address, data2, 52, 43, 86, 88, ecc_q);

                    if(FailedECC_P)
                        DicConsole.DebugWriteLine("CD checksums", "Mode 1 sector at address: {0:X2}:{1:X2}:{2:X2}, fails ECC P check", channel[0x00C], channel[0x00D], channel[0x00E]);
                    if(FailedECC_Q)
                        DicConsole.DebugWriteLine("CD checksums", "Mode 1 sector at address: {0:X2}:{1:X2}:{2:X2}, fails ECC Q check", channel[0x00C], channel[0x00D], channel[0x00E]);

                    if(FailedECC_P || FailedECC_Q)
                        return false;

                    byte[] SectorForCheck = new byte[0x810];
                    uint StoredEDC = BitConverter.ToUInt32(channel, 0x810);
                    byte[] CalculatedEDCBytes;
                    Array.Copy(channel, 0, SectorForCheck, 0, 0x810);
                    CRC32Context.Data(SectorForCheck, 0x810, out CalculatedEDCBytes, CDCRC32Poly, CDCRC32Seed);
                    uint CalculatedEDC = BitConverter.ToUInt32(CalculatedEDCBytes, 0);

                    if(CalculatedEDC != StoredEDC)
                    {
                        DicConsole.DebugWriteLine("CD checksums", "Mode 1 sector at address: {0:X2}:{1:X2}:{2:X2}, got CRC 0x{3:X8} expected 0x{4:X8}", channel[0x00C], channel[0x00D], channel[0x00E], CalculatedEDC, StoredEDC);
                        return false;
                    }

                    return true;
                }
                else if(channel[0x00F] == 0x02) // mode (1 byte)
                {
                    DicConsole.DebugWriteLine("CD checksums", "Mode 2 sector at address {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);

                    if((channel[0x012] & 0x20) == 0x20) // mode 2 form 2
                    {
                        if(channel[0x010] != channel[0x014] || channel[0x011] != channel[0x015] || channel[0x012] != channel[0x016] || channel[0x013] != channel[0x017])
                        {
                            DicConsole.DebugWriteLine("CD checksums", "Subheader copies differ in mode 2 form 2 sector at address: {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);
                        }

                        byte[] SectorForCheck = new byte[0x91C];
                        uint StoredEDC = BitConverter.ToUInt32(channel, 0x92C);
                        byte[] CalculatedEDCBytes;
                        Array.Copy(channel, 0x10, SectorForCheck, 0, 0x91C);
                        CRC32Context.Data(SectorForCheck, 0x91C, out CalculatedEDCBytes, CDCRC32Poly, CDCRC32Seed);
                        uint CalculatedEDC = BitConverter.ToUInt32(CalculatedEDCBytes, 0);

                        if(CalculatedEDC != StoredEDC && StoredEDC != 0x00000000)
                        {
                            DicConsole.DebugWriteLine("CD checksums", "Mode 2 form 2 sector at address: {0:X2}:{1:X2}:{2:X2}, got CRC 0x{3:X8} expected 0x{4:X8}", channel[0x00C], channel[0x00D], channel[0x00E], CalculatedEDC, StoredEDC);
                            return false;
                        }
                    }
                    else
                    {
                        if(channel[0x010] != channel[0x014] || channel[0x011] != channel[0x015] || channel[0x012] != channel[0x016] || channel[0x013] != channel[0x017])
                        {
                            DicConsole.DebugWriteLine("CD checksums", "Subheader copies differ in mode 2 form 1 sector at address: {0:X2}:{1:X2}:{2:X2}", channel[0x00C], channel[0x00D], channel[0x00E]);
                        }

                        byte[] address = new byte[4];
                        byte[] data = new byte[2060];
                        byte[] data2 = new byte[2232];
                        byte[] ecc_p = new byte[172];
                        byte[] ecc_q = new byte[104];

                        address[0] = 0;
                        address[1] = 0;
                        address[2] = 0;
                        address[3] = 0;
                        Array.Copy(channel, 0x0C, data, 0, 2060);
                        Array.Copy(channel, 0x0C, data2, 0, 2232);
                        Array.Copy(channel, 0x80C, ecc_p, 0, 172);
                        Array.Copy(channel, 0x8B8, ecc_q, 0, 104);

                        bool FailedECC_P = CheckECC(address, data, 86, 24, 2, 86, ecc_p);
                        bool FailedECC_Q = CheckECC(address, data2, 52, 43, 86, 88, ecc_q);

                        if(FailedECC_P)
                            DicConsole.DebugWriteLine("CD checksums", "Mode 2 form 1 sector at address: {0:X2}:{1:X2}:{2:X2}, fails ECC P check", channel[0x00C], channel[0x00D], channel[0x00E]);
                        if(FailedECC_Q)
                            DicConsole.DebugWriteLine("CD checksums", "Mode 2 form 1 sector at address: {0:X2}:{1:X2}:{2:X2}, fails ECC Q check", channel[0x00F], channel[0x00C], channel[0x00D], channel[0x00E]);

                        if(FailedECC_P || FailedECC_Q)
                            return false;

                        byte[] SectorForCheck = new byte[0x808];
                        uint StoredEDC = BitConverter.ToUInt32(channel, 0x818);
                        byte[] CalculatedEDCBytes;
                        Array.Copy(channel, 0x10, SectorForCheck, 0, 0x808);
                        CRC32Context.Data(SectorForCheck, 0x808, out CalculatedEDCBytes, CDCRC32Poly, CDCRC32Seed);
                        uint CalculatedEDC = BitConverter.ToUInt32(CalculatedEDCBytes, 0);

                        if(CalculatedEDC != StoredEDC)
                        {
                            DicConsole.DebugWriteLine("CD checksums", "Mode 2 form 1 sector at address: {0:X2}:{1:X2}:{2:X2}, got CRC 0x{3:X8} expected 0x{4:X8}", channel[0x00C], channel[0x00D], channel[0x00E], CalculatedEDC, StoredEDC);
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    DicConsole.DebugWriteLine("CD checksums", "Unknown mode {0} sector at address: {1:X2}:{2:X2}:{3:X2}", channel[0x00F], channel[0x00C], channel[0x00D], channel[0x00E]);
                    return null;
                }
            }
            return null;
        }

        static bool? CheckCDSectorSubChannel(byte[] subchannel)
        {
            bool? status = true;
            byte[] QSubChannel = new byte[12];
            byte[] CDTextPack1 = new byte[18];
            byte[] CDTextPack2 = new byte[18];
            byte[] CDTextPack3 = new byte[18];
            byte[] CDTextPack4 = new byte[18];
            byte[] CDSubRWPack1 = new byte[24];
            byte[] CDSubRWPack2 = new byte[24];
            byte[] CDSubRWPack3 = new byte[24];
            byte[] CDSubRWPack4 = new byte[24];

            int i = 0;
            for(int j = 0; j < 12; j++)
                QSubChannel[j] = 0;
            for(int j = 0; j < 18; j++)
            {
                CDTextPack1[j] = 0;
                CDTextPack2[j] = 0;
                CDTextPack3[j] = 0;
                CDTextPack4[j] = 0;
            }
            for(int j = 0; j < 24; j++)
            {
                CDSubRWPack1[j] = 0;
                CDSubRWPack2[j] = 0;
                CDSubRWPack3[j] = 0;
                CDSubRWPack4[j] = 0;
            }

            for(int j = 0; j < 12; j++)
            {
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) << 1));
                QSubChannel[j] = (byte)(QSubChannel[j] | (subchannel[i++] & 0x40));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 1));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 2));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 3));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 4));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 5));
                QSubChannel[j] = (byte)(QSubChannel[j] | ((subchannel[i++] & 0x40) >> 6));
            }

            i = 0;
            for(int j = 0; j < 18; j++)
            {
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j] | ((subchannel[i++] & 0x3F) << 2));
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j++] | ((subchannel[i] & 0xC0) >> 4));
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j] | ((subchannel[i++] & 0x0F) << 4));
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j++] | ((subchannel[i] & 0x3C) >> 2));
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j] | ((subchannel[i++] & 0x03) << 6));
                if(j < 18)
                    CDTextPack1[j] = (byte)(CDTextPack1[j] | (subchannel[i++] & 0x3F));
            }
            for(int j = 0; j < 18; j++)
            {
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j] | ((subchannel[i++] & 0x3F) << 2));
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j++] | ((subchannel[i] & 0xC0) >> 4));
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j] | ((subchannel[i++] & 0x0F) << 4));
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j++] | ((subchannel[i] & 0x3C) >> 2));
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j] | ((subchannel[i++] & 0x03) << 6));
                if(j < 18)
                    CDTextPack2[j] = (byte)(CDTextPack2[j] | (subchannel[i++] & 0x3F));
            }
            for(int j = 0; j < 18; j++)
            {
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j] | ((subchannel[i++] & 0x3F) << 2));
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j++] | ((subchannel[i] & 0xC0) >> 4));
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j] | ((subchannel[i++] & 0x0F) << 4));
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j++] | ((subchannel[i] & 0x3C) >> 2));
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j] | ((subchannel[i++] & 0x03) << 6));
                if(j < 18)
                    CDTextPack3[j] = (byte)(CDTextPack3[j] | (subchannel[i++] & 0x3F));
            }
            for(int j = 0; j < 18; j++)
            {
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j] | ((subchannel[i++] & 0x3F) << 2));
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j++] | ((subchannel[i] & 0xC0) >> 4));
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j] | ((subchannel[i++] & 0x0F) << 4));
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j++] | ((subchannel[i] & 0x3C) >> 2));
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j] | ((subchannel[i++] & 0x03) << 6));
                if(j < 18)
                    CDTextPack4[j] = (byte)(CDTextPack4[j] | (subchannel[i++] & 0x3F));
            }

            i = 0;
            for(int j = 0; j < 24; j++)
            {
                CDSubRWPack1[j] = (byte)(subchannel[i++] & 0x3F);
            }
            for(int j = 0; j < 24; j++)
            {
                CDSubRWPack2[j] = (byte)(subchannel[i++] & 0x3F);
            }
            for(int j = 0; j < 24; j++)
            {
                CDSubRWPack3[j] = (byte)(subchannel[i++] & 0x3F);
            }
            for(int j = 0; j < 24; j++)
            {
                CDSubRWPack4[j] = (byte)(subchannel[i++] & 0x3F);
            }

            switch(CDSubRWPack1[0])
            {
                case 0x00:
                    DicConsole.DebugWriteLine("CD checksums", "Detected Zero Pack in subchannel");
                    break;
                case 0x08:
                    DicConsole.DebugWriteLine("CD checksums", "Detected Line Graphics Pack in subchannel");
                    break;
                case 0x09:
                    DicConsole.DebugWriteLine("CD checksums", "Detected CD+G Pack in subchannel");
                    break;
                case 0x0A:
                    DicConsole.DebugWriteLine("CD checksums", "Detected CD+EG Pack in subchannel");
                    break;
                case 0x14:
                    DicConsole.DebugWriteLine("CD checksums", "Detected CD-TEXT Pack in subchannel");
                    break;
                case 0x18:
                    DicConsole.DebugWriteLine("CD checksums", "Detected CD+MIDI Pack in subchannel");
                    break;
                case 0x38:
                    DicConsole.DebugWriteLine("CD checksums", "Detected User Pack in subchannel");
                    break;
                default:
                    DicConsole.DebugWriteLine("CD checksums", "Detected unknown Pack type in subchannel: mode {0}, item {1}", Convert.ToString(CDSubRWPack1[0] & 0x38, 2), Convert.ToString(CDSubRWPack1[0] & 0x07, 2));
                    break;
            }

            BigEndianBitConverter.IsLittleEndian = true;

            ushort QSubChannelCRC = BigEndianBitConverter.ToUInt16(QSubChannel, 10);
            byte[] QSubChannelForCRC = new byte[10];
            Array.Copy(QSubChannel, 0, QSubChannelForCRC, 0, 10);
            ushort CalculatedQCRC = CalculateCCITT_CRC16(QSubChannelForCRC);

            if(QSubChannelCRC != CalculatedQCRC)
            {
                DicConsole.DebugWriteLine("CD checksums", "Q subchannel CRC 0x{0:X4}, expected 0x{1:X4}", CalculatedQCRC, QSubChannelCRC);
                status = false;
            }

            if((CDTextPack1[0] & 0x80) == 0x80)
            {
                ushort CDTextPack1CRC = BigEndianBitConverter.ToUInt16(CDTextPack1, 16);
                byte[] CDTextPack1ForCRC = new byte[16];
                Array.Copy(CDTextPack1, 0, CDTextPack1ForCRC, 0, 16);
                ushort CalculatedCDTP1CRC = CalculateCCITT_CRC16(CDTextPack1ForCRC);

                if(CDTextPack1CRC != CalculatedCDTP1CRC && CDTextPack1CRC != 0)
                {
                    DicConsole.DebugWriteLine("CD checksums", "CD-Text Pack 1 CRC 0x{0:X4}, expected 0x{1:X4}", CDTextPack1CRC, CalculatedCDTP1CRC);
                    status = false;
                }
            }

            if((CDTextPack2[0] & 0x80) == 0x80)
            {
                ushort CDTextPack2CRC = BigEndianBitConverter.ToUInt16(CDTextPack2, 16);
                byte[] CDTextPack2ForCRC = new byte[16];
                Array.Copy(CDTextPack2, 0, CDTextPack2ForCRC, 0, 16);
                ushort CalculatedCDTP2CRC = CalculateCCITT_CRC16(CDTextPack2ForCRC);
                DicConsole.DebugWriteLine("CD checksums", "Cyclic CDTP2 0x{0:X4}, Calc CDTP2 0x{1:X4}", CDTextPack2CRC, CalculatedCDTP2CRC);

                if(CDTextPack2CRC != CalculatedCDTP2CRC && CDTextPack2CRC != 0)
                {
                    DicConsole.DebugWriteLine("CD checksums", "CD-Text Pack 2 CRC 0x{0:X4}, expected 0x{1:X4}", CDTextPack2CRC, CalculatedCDTP2CRC);
                    status = false;
                }
            }

            if((CDTextPack3[0] & 0x80) == 0x80)
            {
                ushort CDTextPack3CRC = BigEndianBitConverter.ToUInt16(CDTextPack3, 16);
                byte[] CDTextPack3ForCRC = new byte[16];
                Array.Copy(CDTextPack3, 0, CDTextPack3ForCRC, 0, 16);
                ushort CalculatedCDTP3CRC = CalculateCCITT_CRC16(CDTextPack3ForCRC);
                DicConsole.DebugWriteLine("CD checksums", "Cyclic CDTP3 0x{0:X4}, Calc CDTP3 0x{1:X4}", CDTextPack3CRC, CalculatedCDTP3CRC);

                if(CDTextPack3CRC != CalculatedCDTP3CRC && CDTextPack3CRC != 0)
                {
                    DicConsole.DebugWriteLine("CD checksums", "CD-Text Pack 3 CRC 0x{0:X4}, expected 0x{1:X4}", CDTextPack3CRC, CalculatedCDTP3CRC);
                    status = false;
                }
            }

            if((CDTextPack4[0] & 0x80) == 0x80)
            {
                ushort CDTextPack4CRC = BigEndianBitConverter.ToUInt16(CDTextPack4, 16);
                byte[] CDTextPack4ForCRC = new byte[16];
                Array.Copy(CDTextPack4, 0, CDTextPack4ForCRC, 0, 16);
                ushort CalculatedCDTP4CRC = CalculateCCITT_CRC16(CDTextPack4ForCRC);
                DicConsole.DebugWriteLine("CD checksums", "Cyclic CDTP4 0x{0:X4}, Calc CDTP4 0x{1:X4}", CDTextPack4CRC, CalculatedCDTP4CRC);

                if(CDTextPack4CRC != CalculatedCDTP4CRC && CDTextPack4CRC != 0)
                {
                    DicConsole.DebugWriteLine("CD checksums", "CD-Text Pack 4 CRC 0x{0:X4}, expected 0x{1:X4}", CDTextPack4CRC, CalculatedCDTP4CRC);
                    status = false;
                }
            }

            return status;
        }

        static readonly ushort[] CCITT_CRC16Table =
        {
            0x0000,  0x1021,  0x2042,  0x3063,  0x4084,  0x50a5,  0x60c6,  0x70e7,
            0x8108,  0x9129,  0xa14a,  0xb16b,  0xc18c,  0xd1ad,  0xe1ce,  0xf1ef,
            0x1231,  0x0210,  0x3273,  0x2252,  0x52b5,  0x4294,  0x72f7,  0x62d6,
            0x9339,  0x8318,  0xb37b,  0xa35a,  0xd3bd,  0xc39c,  0xf3ff,  0xe3de,
            0x2462,  0x3443,  0x0420,  0x1401,  0x64e6,  0x74c7,  0x44a4,  0x5485,
            0xa56a,  0xb54b,  0x8528,  0x9509,  0xe5ee,  0xf5cf,  0xc5ac,  0xd58d,
            0x3653,  0x2672,  0x1611,  0x0630,  0x76d7,  0x66f6,  0x5695,  0x46b4,
            0xb75b,  0xa77a,  0x9719,  0x8738,  0xf7df,  0xe7fe,  0xd79d,  0xc7bc,
            0x48c4,  0x58e5,  0x6886,  0x78a7,  0x0840,  0x1861,  0x2802,  0x3823,
            0xc9cc,  0xd9ed,  0xe98e,  0xf9af,  0x8948,  0x9969,  0xa90a,  0xb92b,
            0x5af5,  0x4ad4,  0x7ab7,  0x6a96,  0x1a71,  0x0a50,  0x3a33,  0x2a12,
            0xdbfd,  0xcbdc,  0xfbbf,  0xeb9e,  0x9b79,  0x8b58,  0xbb3b,  0xab1a,
            0x6ca6,  0x7c87,  0x4ce4,  0x5cc5,  0x2c22,  0x3c03,  0x0c60,  0x1c41,
            0xedae,  0xfd8f,  0xcdec,  0xddcd,  0xad2a,  0xbd0b,  0x8d68,  0x9d49,
            0x7e97,  0x6eb6,  0x5ed5,  0x4ef4,  0x3e13,  0x2e32,  0x1e51,  0x0e70,
            0xff9f,  0xefbe,  0xdfdd,  0xcffc,  0xbf1b,  0xaf3a,  0x9f59,  0x8f78,
            0x9188,  0x81a9,  0xb1ca,  0xa1eb,  0xd10c,  0xc12d,  0xf14e,  0xe16f,
            0x1080,  0x00a1,  0x30c2,  0x20e3,  0x5004,  0x4025,  0x7046,  0x6067,
            0x83b9,  0x9398,  0xa3fb,  0xb3da,  0xc33d,  0xd31c,  0xe37f,  0xf35e,
            0x02b1,  0x1290,  0x22f3,  0x32d2,  0x4235,  0x5214,  0x6277,  0x7256,
            0xb5ea,  0xa5cb,  0x95a8,  0x8589,  0xf56e,  0xe54f,  0xd52c,  0xc50d,
            0x34e2,  0x24c3,  0x14a0,  0x0481,  0x7466,  0x6447,  0x5424,  0x4405,
            0xa7db,  0xb7fa,  0x8799,  0x97b8,  0xe75f,  0xf77e,  0xc71d,  0xd73c,
            0x26d3,  0x36f2,  0x0691,  0x16b0,  0x6657,  0x7676,  0x4615,  0x5634,
            0xd94c,  0xc96d,  0xf90e,  0xe92f,  0x99c8,  0x89e9,  0xb98a,  0xa9ab,
            0x5844,  0x4865,  0x7806,  0x6827,  0x18c0,  0x08e1,  0x3882,  0x28a3,
            0xcb7d,  0xdb5c,  0xeb3f,  0xfb1e,  0x8bf9,  0x9bd8,  0xabbb,  0xbb9a,
            0x4a75,  0x5a54,  0x6a37,  0x7a16,  0x0af1,  0x1ad0,  0x2ab3,  0x3a92,
            0xfd2e,  0xed0f,  0xdd6c,  0xcd4d,  0xbdaa,  0xad8b,  0x9de8,  0x8dc9,
            0x7c26,  0x6c07,  0x5c64,  0x4c45,  0x3ca2,  0x2c83,  0x1ce0,  0x0cc1,
            0xef1f,  0xff3e,  0xcf5d,  0xdf7c,  0xaf9b,  0xbfba,  0x8fd9,  0x9ff8,
            0x6e17,  0x7e36,  0x4e55,  0x5e74,  0x2e93,  0x3eb2,  0x0ed1,  0x1ef0
        };

        static ushort CalculateCCITT_CRC16(byte[] buffer)
        {
            ushort CRC16 = 0;
            for(int i = 0; i < buffer.Length; i++)
            {
                CRC16 = (ushort)(CCITT_CRC16Table[(CRC16 >> 8) ^ buffer[i]] ^ (CRC16 << 8));
            }
            CRC16 = (ushort)~CRC16;

            return CRC16;
        }
    }
}

