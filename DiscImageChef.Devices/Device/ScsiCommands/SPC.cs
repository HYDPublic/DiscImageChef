// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : SPC.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : SCSI Primary Commands.
//
// --[ Description ] ----------------------------------------------------------
//
//     Contains SCSI commands defined in SPC standards.
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
using DiscImageChef.Console;

namespace DiscImageChef.Devices
{
    public partial class Device
    {
        /// <summary>
        /// Sends the SPC INQUIRY command to the device using default device timeout.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer)
        {
            return ScsiInquiry(out buffer, out senseBuffer, Timeout);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device using default device timeout.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, out double duration)
        {
            return ScsiInquiry(out buffer, out senseBuffer, Timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, uint timeout)
        {
            double duration;
            return ScsiInquiry(out buffer, out senseBuffer, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, uint timeout, out double duration)
        {
            buffer = new byte[36];
            senseBuffer = new byte[32];
            byte[] cdb = { (byte)ScsiCommands.Inquiry, 0, 0, 0, 36, 0 };
            bool sense;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            if(sense)
                return true;

            byte pagesLength = (byte)(buffer[4] + 5);

            cdb = new byte[] { (byte)ScsiCommands.Inquiry, 0, 0, 0, pagesLength, 0 };
            buffer = new byte[pagesLength];
            senseBuffer = new byte[32];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "INQUIRY took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device with an Extended Vital Product Data page using default device timeout.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="page">The Extended Vital Product Data</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, byte page)
        {
            return ScsiInquiry(out buffer, out senseBuffer, page, Timeout);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device with an Extended Vital Product Data page using default device timeout.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="page">The Extended Vital Product Data</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, byte page, out double duration)
        {
            return ScsiInquiry(out buffer, out senseBuffer, page, Timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device with an Extended Vital Product Data page.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="page">The Extended Vital Product Data</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, byte page, uint timeout)
        {
            double duration;
            return ScsiInquiry(out buffer, out senseBuffer, page, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC INQUIRY command to the device with an Extended Vital Product Data page.
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI INQUIRY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="page">The Extended Vital Product Data</param>
        public bool ScsiInquiry(out byte[] buffer, out byte[] senseBuffer, byte page, uint timeout, out double duration)
        {
            buffer = new byte[36];
            senseBuffer = new byte[32];
            byte[] cdb = { (byte)ScsiCommands.Inquiry, 1, page, 0, 36, 0 };
            bool sense;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            if(sense)
                return true;

            // This is because INQ was returned instead of EVPD
            if(buffer[1] != page)
                return true;

            byte pagesLength = (byte)(buffer[3] + 4);

            cdb = new byte[] { (byte)ScsiCommands.Inquiry, 1, page, 0, pagesLength, 0 };
            buffer = new byte[pagesLength];
            senseBuffer = new byte[32];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "INQUIRY took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC TEST UNIT READY command to the device
        /// </summary>
        /// <returns><c>true</c>, if unit is NOT ready, <c>false</c> otherwise.</returns>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ScsiTestUnitReady(out byte[] senseBuffer, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = { (byte)ScsiCommands.TestUnitReady, 0, 0, 0, 0, 0 };
            bool sense;
            byte[] buffer = new byte[0];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.None, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "TEST UNIT READY took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(6) command to the device as introduced in SCSI-1
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(6) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ModeSense(out byte[] buffer, out byte[] senseBuffer, uint timeout, out double duration)
        {
            return ModeSense6(out buffer, out senseBuffer, false, ScsiModeSensePageControl.Current, 0, 0, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(6) command to the device as introduced in SCSI-2
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(6) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="DBD">If set to <c>true</c> device MUST not return any block descriptor.</param>
        /// <param name="pageControl">Page control.</param>
        /// <param name="pageCode">Page code.</param>
        public bool ModeSense6(out byte[] buffer, out byte[] senseBuffer, bool DBD, ScsiModeSensePageControl pageControl, byte pageCode, uint timeout, out double duration)
        {
            return ModeSense6(out buffer, out senseBuffer, DBD, pageControl, pageCode, 0, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(6) command to the device as introduced in SCSI-3 SPC-3
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(6) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="DBD">If set to <c>true</c> device MUST not return any block descriptor.</param>
        /// <param name="pageControl">Page control.</param>
        /// <param name="pageCode">Page code.</param>
        /// <param name="subPageCode">Sub-page code.</param>
        public bool ModeSense6(out byte[] buffer, out byte[] senseBuffer, bool DBD, ScsiModeSensePageControl pageControl, byte pageCode, byte subPageCode, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[6];
            buffer = new byte[255];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ModeSense;
            if(DBD)
                cdb[1] = 0x08;
            cdb[2] |= (byte)pageControl;
            cdb[2] |= (byte)(pageCode & 0x3F);
            cdb[3] = subPageCode;
            cdb[4] = (byte)buffer.Length;
            cdb[5] = 0;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            if(sense)
                return true;

            byte modeLength = (byte)(buffer[0] + 1);
            buffer = new byte[modeLength];
            cdb[4] = (byte)buffer.Length;
            senseBuffer = new byte[32];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "MODE SENSE(6) took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(10) command to the device as introduced in SCSI-2
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(10) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="DBD">If set to <c>true</c> device MUST not return any block descriptor.</param>
        /// <param name="pageControl">Page control.</param>
        /// <param name="pageCode">Page code.</param>
        public bool ModeSense10(out byte[] buffer, out byte[] senseBuffer, bool DBD, ScsiModeSensePageControl pageControl, byte pageCode, uint timeout, out double duration)
        {
            return ModeSense10(out buffer, out senseBuffer, false, DBD, pageControl, pageCode, 0, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(10) command to the device as introduced in SCSI-3 SPC-2
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(10) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="DBD">If set to <c>true</c> device MUST not return any block descriptor.</param>
        /// <param name="pageControl">Page control.</param>
        /// <param name="pageCode">Page code.</param>
        /// <param name="LLBAA">If set means 64-bit LBAs are accepted by the caller.</param>
        public bool ModeSense10(out byte[] buffer, out byte[] senseBuffer, bool LLBAA, bool DBD, ScsiModeSensePageControl pageControl, byte pageCode, uint timeout, out double duration)
        {
            return ModeSense10(out buffer, out senseBuffer, LLBAA, DBD, pageControl, pageCode, 0, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC MODE SENSE(10) command to the device as introduced in SCSI-3 SPC-3
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI MODE SENSE(10) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="DBD">If set to <c>true</c> device MUST not return any block descriptor.</param>
        /// <param name="pageControl">Page control.</param>
        /// <param name="pageCode">Page code.</param>
        /// <param name="subPageCode">Sub-page code.</param>
        /// <param name="LLBAA">If set means 64-bit LBAs are accepted by the caller.</param>
        public bool ModeSense10(out byte[] buffer, out byte[] senseBuffer, bool LLBAA, bool DBD, ScsiModeSensePageControl pageControl, byte pageCode, byte subPageCode, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[10];
            buffer = new byte[4096];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ModeSense10;
            if(LLBAA)
                cdb[1] |= 0x10;
            if(DBD)
                cdb[1] |= 0x08;
            cdb[2] |= (byte)pageControl;
            cdb[2] |= (byte)(pageCode & 0x3F);
            cdb[3] = subPageCode;
            cdb[7] = (byte)((buffer.Length & 0xFF00) >> 8);
            cdb[8] = (byte)(buffer.Length & 0xFF);
            cdb[9] = 0;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            if(sense)
                return true;

#pragma warning disable IDE0004 // Cast is necessary or an invalid bitshift happens
            ushort modeLength = (ushort)(((int)buffer[0] << 8) + buffer[1] + 2);
#pragma warning restore IDE0004 // Cast is necessary or an invalid bitshift happens
            buffer = new byte[modeLength];
            cdb[7] = (byte)((buffer.Length & 0xFF00) >> 8);
            cdb[8] = (byte)(buffer.Length & 0xFF);
            senseBuffer = new byte[32];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "MODE SENSE(10) took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC PREVENT ALLOW MEDIUM REMOVAL command to prevent medium removal
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool SpcPreventMediumRemoval(out byte[] senseBuffer, uint timeout, out double duration)
        {
            return SpcPreventAllowMediumRemoval(out senseBuffer, ScsiPreventAllowMode.Prevent, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC PREVENT ALLOW MEDIUM REMOVAL command to allow medium removal
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool SpcAllowMediumRemoval(out byte[] senseBuffer, uint timeout, out double duration)
        {
            return SpcPreventAllowMediumRemoval(out senseBuffer, ScsiPreventAllowMode.Allow, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC PREVENT ALLOW MEDIUM REMOVAL command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="prevent"><c>true</c> to prevent medium removal, <c>false</c> to allow it.</param>
        public bool SpcPreventAllowMediumRemoval(out byte[] senseBuffer, bool prevent, uint timeout, out double duration)
        {
            if(prevent)
                return SpcPreventAllowMediumRemoval(out senseBuffer, ScsiPreventAllowMode.Prevent, timeout, out duration);
            else
                return SpcPreventAllowMediumRemoval(out senseBuffer, ScsiPreventAllowMode.Allow, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC PREVENT ALLOW MEDIUM REMOVAL command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        /// <param name="preventMode">Prevention mode.</param>
        public bool SpcPreventAllowMediumRemoval(out byte[] senseBuffer, ScsiPreventAllowMode preventMode, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[6];
            bool sense;
            byte[] buffer = new byte[0];

            cdb[0] = (byte)ScsiCommands.PreventAllowMediumRemoval;
            cdb[4] = (byte)((byte)preventMode & 0x03);

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.None, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "PREVENT ALLOW MEDIUM REMOVAL took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC READ CAPACITY command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI READ CAPACITY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ReadCapacity(out byte[] buffer, out byte[] senseBuffer, uint timeout, out double duration)
        {
            return ReadCapacity(out buffer, out senseBuffer, false, 0, false, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC READ CAPACITY command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI READ CAPACITY response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="RelAddr">Indicates that <paramref name="address"/> is relative to current medium position</param>
        /// <param name="address">Address where information is requested from, only valid if <paramref name="PMI"/> is set</param>
        /// <param name="PMI">If set, it is requesting partial media capacity</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ReadCapacity(out byte[] buffer, out byte[] senseBuffer, bool RelAddr, uint address, bool PMI, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[10];
            buffer = new byte[8];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ReadCapacity;

            if(PMI)
            {
                cdb[8] = 0x01;
                if(RelAddr)
                    cdb[1] = 0x01;

                cdb[2] = (byte)((address & 0xFF000000) >> 24);
                cdb[3] = (byte)((address & 0xFF0000) >> 16);
                cdb[4] = (byte)((address & 0xFF00) >> 8);
                cdb[5] = (byte)(address & 0xFF);
            }

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "READ CAPACITY took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC READ CAPACITY(16) command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI READ CAPACITY(16) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ReadCapacity16(out byte[] buffer, out byte[] senseBuffer, uint timeout, out double duration)
        {
            return ReadCapacity16(out buffer, out senseBuffer, 0, false, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC READ CAPACITY(16) command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI READ CAPACITY(16) response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="address">Address where information is requested from, only valid if <paramref name="PMI"/> is set</param>
        /// <param name="PMI">If set, it is requesting partial media capacity</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ReadCapacity16(out byte[] buffer, out byte[] senseBuffer, ulong address, bool PMI, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[16];
            buffer = new byte[32];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ServiceActionIn;
            cdb[1] = (byte)ScsiServiceActions.ReadCapacity16;

            if(PMI)
            {
                cdb[14] = 0x01;
                byte[] temp = BitConverter.GetBytes(address);
                cdb[2] = temp[7];
                cdb[3] = temp[6];
                cdb[4] = temp[5];
                cdb[5] = temp[4];
                cdb[6] = temp[3];
                cdb[7] = temp[2];
                cdb[8] = temp[1];
                cdb[9] = temp[0];
            }

            cdb[10] = (byte)((buffer.Length & 0xFF000000) >> 24);
            cdb[11] = (byte)((buffer.Length & 0xFF0000) >> 16);
            cdb[12] = (byte)((buffer.Length & 0xFF00) >> 8);
            cdb[13] = (byte)(buffer.Length & 0xFF);

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "READ CAPACITY(16) took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC READ MEDIA SERIAL NUMBER command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer where the SCSI READ MEDIA SERIAL NUMBER response will be stored</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ReadMediaSerialNumber(out byte[] buffer, out byte[] senseBuffer, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];
            byte[] cdb = new byte[12];
            buffer = new byte[4];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ReadSerialNumber;
            cdb[1] = 0x01;
            cdb[6] = (byte)((buffer.Length & 0xFF000000) >> 24);
            cdb[7] = (byte)((buffer.Length & 0xFF0000) >> 16);
            cdb[8] = (byte)((buffer.Length & 0xFF00) >> 8);
            cdb[9] = (byte)(buffer.Length & 0xFF);

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            if(sense)
                return true;

#pragma warning disable IDE0004 // Cast is necessary or an invalid bitshift happens
            uint strctLength = (uint)(((int)buffer[0] << 24) + ((int)buffer[1] << 16) + ((int)buffer[2] << 8) + buffer[3] + 4);
#pragma warning restore IDE0004 // Cast is necessary or an invalid bitshift happens
            buffer = new byte[strctLength];
            cdb[6] = (byte)((buffer.Length & 0xFF000000) >> 24);
            cdb[7] = (byte)((buffer.Length & 0xFF0000) >> 16);
            cdb[8] = (byte)((buffer.Length & 0xFF00) >> 8);
            cdb[9] = (byte)(buffer.Length & 0xFF);
            senseBuffer = new byte[32];

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "READ MEDIA SERIAL NUMBER took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="partition">Partition number.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="cache">If set to <c>true</c> device can return cached data.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, byte partition, ushort firstAttribute, bool cache, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, 0, partition, firstAttribute, cache, timeout, out duration);
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="cache">If set to <c>true</c> device can return cached data.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, ushort firstAttribute, bool cache, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, 0, 0, firstAttribute, cache, timeout, out duration);
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="partition">Partition number.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, byte partition, ushort firstAttribute, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, 0, partition, firstAttribute, false, timeout, out duration);
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, ushort firstAttribute, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, 0, 0, firstAttribute, false, timeout, out duration);
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="volume">Volume number.</param>
        /// <param name="partition">Partition number.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, byte volume, byte partition, ushort firstAttribute, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, volume, partition, firstAttribute, false, timeout, out duration);
        }

        /// <summary>
        /// Reads an attribute from the medium auxiliary memory
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="action">What to do, <see cref="ScsiAttributeAction"/>.</param>
        /// <param name="volume">Volume number.</param>
        /// <param name="partition">Partition number.</param>
        /// <param name="firstAttribute">First attribute identifier.</param>
        /// <param name="cache">If set to <c>true</c> device can return cached data.</param>
        /// <param name="timeout">Timeout.</param>
        /// <param name="duration">Duration.</param>
        public bool ReadAttribute(out byte[] buffer, out byte[] senseBuffer, ScsiAttributeAction action, byte volume, byte partition, ushort firstAttribute, bool cache, uint timeout, out double duration)
        {
            return ReadAttribute(out buffer, out senseBuffer, action, 0, 0, volume, partition, firstAttribute, cache, timeout, out duration);
        }

        /// <summary>
        /// Sends the SPC MODE SELECT(6) command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer with the data to be sent to the device</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ModeSelect(byte[] buffer, out byte[] senseBuffer, bool pageFormat, bool savePages, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];

            // Prevent overflows
            if(buffer.Length > 255)
            {
                if(platformID != Interop.PlatformID.Win32NT && platformID != Interop.PlatformID.Win32S && platformID != Interop.PlatformID.Win32Windows && platformID != Interop.PlatformID.WinCE && platformID != Interop.PlatformID.WindowsPhone && platformID != Interop.PlatformID.Xbox)
                    lastError = 75;
                else
                    lastError = 111;
                error = true;
                duration = 0;
                return true;
            }

            byte[] cdb = new byte[6];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ModeSelect;
            if(pageFormat)
                cdb[1] += 0x10;
            if(savePages)
                cdb[1] += 0x01;
            cdb[4] = (byte)buffer.Length;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.Out, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "MODE SELECT(6) took {0} ms.", duration);

            return sense;
        }

        /// <summary>
        /// Sends the SPC MODE SELECT(10) command
        /// </summary>
        /// <returns><c>true</c> if the command failed and <paramref name="senseBuffer"/> contains the sense buffer.</returns>
        /// <param name="buffer">Buffer with the data to be sent to the device</param>
        /// <param name="senseBuffer">Sense buffer.</param>
        /// <param name="timeout">Timeout in seconds.</param>
        /// <param name="duration">Duration in milliseconds it took for the device to execute the command.</param>
        public bool ModeSelect10(byte[] buffer, out byte[] senseBuffer, bool pageFormat, bool savePages, uint timeout, out double duration)
        {
            senseBuffer = new byte[32];

            // Prevent overflows
            if(buffer.Length > 65535)
            {
                if(platformID != Interop.PlatformID.Win32NT && platformID != Interop.PlatformID.Win32S && platformID != Interop.PlatformID.Win32Windows && platformID != Interop.PlatformID.WinCE && platformID != Interop.PlatformID.WindowsPhone && platformID != Interop.PlatformID.Xbox)
                    lastError = 75;
                else
                    lastError = 111;
                error = true;
                duration = 0;
                return true;
            }

            byte[] cdb = new byte[10];
            bool sense;

            cdb[0] = (byte)ScsiCommands.ModeSelect10;
            if(pageFormat)
                cdb[1] += 0x10;
            if(savePages)
                cdb[1] += 0x01;
            cdb[7] = (byte)((buffer.Length & 0xFF00) << 8);
            cdb[8] = (byte)(buffer.Length & 0xFF);

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.Out, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "MODE SELECT(10) took {0} ms.", duration);

            return sense;
        }

        public bool RequestSense(out byte[] buffer, uint timeout, out double duration)
        {
            return RequestSense(false, out buffer, timeout, out duration);
        }

        public bool RequestSense(bool descriptor, out byte[] buffer, uint timeout, out double duration)
        {
            byte[] senseBuffer = new byte[32];
            byte[] cdb = new byte[6];
            buffer = new byte[252];
            bool sense;

            cdb[0] = (byte)ScsiCommands.RequestSense;
            if(descriptor)
                cdb[1] = 0x01;
            cdb[2] = 0;
            cdb[3] = 0;
            cdb[4] = (byte)buffer.Length;
            cdb[5] = 0;

            lastError = SendScsiCommand(cdb, ref buffer, out senseBuffer, timeout, ScsiDirection.In, out duration, out sense);
            error = lastError != 0;

            DicConsole.DebugWriteLine("SCSI Device", "REQUEST SENSE took {0} ms.", duration);

            return sense;
        }
    }
}

