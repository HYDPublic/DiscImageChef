// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : MHDDLog.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Core methods.
//
// --[ Description ] ----------------------------------------------------------
//
//     Methods to create a binary media scan log in MHDD's format.
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
// Copyright © 2011-2017 Natalia Portillo
// ****************************************************************************/

using System;
using System.IO;
using System.Text;
using DiscImageChef.Devices;

namespace DiscImageChef.Core.Logging
{
    public class MHDDLog
    {
        static FileStream mhddFs;

        public MHDDLog(string outputFile, Device dev, ulong blocks, ulong blockSize, ulong blocksToRead)
        {
            if(dev != null && !string.IsNullOrEmpty(outputFile))
            {
                mhddFs = new FileStream(outputFile, FileMode.Create);

                string device;
                string mode;
                string fw;
                string sn;
                string sectors;
                string sectorsize;
                string scanblocksize;
                string ver;

                switch(dev.Type)
                {
                    case DeviceType.ATA:
                    case DeviceType.ATAPI:
                        mode = "MODE: IDE";
                        break;
                    case DeviceType.SCSI:
                        mode = "MODE: SCSI";
                        break;
                    case DeviceType.MMC:
                        mode = "MODE: MMC";
                        break;
                    case DeviceType.NVMe:
                        mode = "MODE: NVMe";
                        break;
                    case DeviceType.SecureDigital:
                        mode = "MODE: SD";
                        break;
                    default:
                        mode = "MODE: IDE";
                        break;
                }

                device = string.Format("DEVICE: {0} {1}", dev.Manufacturer, dev.Model);
                fw = string.Format("F/W: {0}", dev.Revision);
                sn = string.Format("S/N: {0}", dev.Serial);
                sectors = string.Format(new System.Globalization.CultureInfo("en-US"), "SECTORS: {0:n0}", blocks);
                sectorsize = string.Format(new System.Globalization.CultureInfo("en-US"), "SECTOR SIZE: {0:n0} bytes", blockSize);
                scanblocksize = string.Format(new System.Globalization.CultureInfo("en-US"), "SCAN BLOCK SIZE: {0:n0} sectors", blocksToRead);
                ver = "VER:2 ";

                byte[] deviceBytes = Encoding.ASCII.GetBytes(device);
                byte[] modeBytes = Encoding.ASCII.GetBytes(mode);
                byte[] fwBytes = Encoding.ASCII.GetBytes(fw);
                byte[] snBytes = Encoding.ASCII.GetBytes(sn);
                byte[] sectorsBytes = Encoding.ASCII.GetBytes(sectors);
                byte[] sectorsizeBytes = Encoding.ASCII.GetBytes(sectorsize);
                byte[] scanblocksizeBytes = Encoding.ASCII.GetBytes(scanblocksize);
                byte[] verBytes = Encoding.ASCII.GetBytes(ver);

                uint Pointer = (uint)(deviceBytes.Length + modeBytes.Length + fwBytes.Length +
                                  snBytes.Length + sectorsBytes.Length + sectorsizeBytes.Length +
                                  scanblocksizeBytes.Length + verBytes.Length +
                                  2 * 9 + // New lines
                                  4); // Pointer

                byte[] newLine = new byte[2];
                newLine[0] = 0x0D;
                newLine[1] = 0x0A;

                mhddFs.Write(BitConverter.GetBytes(Pointer), 0, 4);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(verBytes, 0, verBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(modeBytes, 0, modeBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(deviceBytes, 0, deviceBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(fwBytes, 0, fwBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(snBytes, 0, snBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(sectorsBytes, 0, sectorsBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(sectorsizeBytes, 0, sectorsizeBytes.Length);
                mhddFs.Write(newLine, 0, 2);
                mhddFs.Write(scanblocksizeBytes, 0, scanblocksizeBytes.Length);
                mhddFs.Write(newLine, 0, 2);
            }
        }

        public void Write(ulong sector, double duration)
        {
            if(mhddFs != null)
            {
                byte[] sectorBytes = BitConverter.GetBytes(sector);
                byte[] durationBytes = BitConverter.GetBytes((ulong)(duration * 1000));

                mhddFs.Write(sectorBytes, 0, 8);
                mhddFs.Write(durationBytes, 0, 8);
            }
        }

        public void Close()
        {
            if(mhddFs != null)
                mhddFs.Close();
        }
    }
}


