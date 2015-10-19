﻿/***************************************************************************
The Disc Image Chef
----------------------------------------------------------------------------
 
Filename       : DiskCopy42.cs
Version        : 1.0
Author(s)      : Natalia Portillo
 
Component      : Disc image plugins

Revision       : $Revision$
Last change by : $Author$
Date           : $Date$
 
--[ Description ] ----------------------------------------------------------
 
Manages Apple DiskCopy 4.2 disc images, including unofficial modifications.
 
--[ License ] --------------------------------------------------------------
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

----------------------------------------------------------------------------
Copyright (C) 2011-2014 Claunia.com
****************************************************************************/
//$Id$
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiscImageChef.Console;

namespace DiscImageChef.ImagePlugins
{
    public class Apple2MG : ImagePlugin
    {
        #region Internal Structures

        // DiskCopy 4.2 header, big-endian, data-fork, start of file, 84 bytes
        struct A2IMGHeader
        {
            /// <summary>
            /// Offset 0x00, magic
            /// </summary>
            public UInt32 magic;
            /// <summary>
            /// Offset 0x04, disk image creator ID
            /// </summary>
            public UInt32 creator;
            /// <summary>
            /// Offset 0x08, header size, constant 0x0040
            /// </summary>
            public UInt16 headerSize;
            /// <summary>
            /// Offset 0x0A, disk image version
            /// </summary>
            public UInt16 version;
            /// <summary>
            /// Offset 0x0C, disk image format
            /// </summary>
            public UInt32 imageFormat;
            /// <summary>
            /// Offset 0x10, flags and volume number
            /// </summary>
            public UInt32 flags;
            /// <summary>
            /// Offset 0x14, blocks for ProDOS, 0 otherwise
            /// </summary>
            public UInt32 blocks;
            /// <summary>
            /// Offset 0x18, offset to data
            /// </summary>
            public UInt32 dataOffset;
            /// <summary>
            /// Offset 0x1C, data size in bytes
            /// </summary>
            public UInt32 dataSize;
            /// <summary>
            /// Offset 0x20, offset to optional comment
            /// </summary>
            public UInt32 commentOffset;
            /// <summary>
            /// Offset 0x24, length of optional comment
            /// </summary>
            public UInt32 commentSize;
            /// <summary>
            /// Offset 0x28, offset to creator specific chunk
            /// </summary>
            public UInt32 creatorSpecificOffset;
            /// <summary>
            /// Offset 0x2C, creator specific chunk size
            /// </summary>
            public UInt32 creatorSpecificSize;
            /// <summary>
            /// Offset 0x30, reserved, should be zero
            /// </summary>
            public UInt32 reserved1;
            /// <summary>
            /// Offset 0x34, reserved, should be zero
            /// </summary>
            public UInt32 reserved2;
            /// <summary>
            /// Offset 0x38, reserved, should be zero
            /// </summary>
            public UInt32 reserved3;
            /// <summary>
            /// Offset 0x3C, reserved, should be zero
            /// </summary>
            public UInt32 reserved4;
        }

        #endregion

        #region Internal Constants

        /// <summary>
        /// Magic number, "2IMG"
        /// </summary>
        public const UInt32 MAGIC = 0x474D4932;
        /// <summary>
        /// Disk image created by ASIMOV2, "!nfc"
        /// </summary>
        public const UInt32 CreatorAsimov = 0x63666E21;
        /// <summary>
        /// Disk image created by Bernie ][ the Rescue, "B2TR"
        /// </summary>
        public const UInt32 CreatorBernie = 0x52543242;
        /// <summary>
        /// Disk image created by Catakig, "CTKG"
        /// </summary>
        public const UInt32 CreatorCatakig = 0x474B5443;
        /// <summary>
        /// Disk image created by Sheppy's ImageMaker, "ShIm"
        /// </summary>
        public const UInt32 CreatorSheppy = 0x6D496853;
        /// <summary>
        /// Disk image created by Sweet16, "WOOF"
        /// </summary>
        public const UInt32 CreatorSweet = 0x464F4F57;
        /// <summary>
        /// Disk image created by XGS, "XGS!"
        /// </summary>
        public const UInt32 CreatorXGS = 0x21534758;

        public const UInt32 DOSSectorOrder = 0x00000000;
        public const UInt32 ProDOSSectorOrder = 0x00000001;
        public const UInt32 NIBSectorOrder = 0x00000002;

        public const UInt32 LockedDisk = 0x80000000;
        public const UInt32 ValidVolumeNumber = 0x00000100;
        public const UInt32 VolumeNumberMask = 0x000000FF;

        #endregion

        #region Internal variables

        A2IMGHeader ImageHeader;
        string a2mgImagePath;

        #endregion

        public Apple2MG()
        {
            Name = "Apple 2IMG";
            PluginUUID = new Guid("CBAF8824-BA5F-415F-953A-19A03519B2D1");
            ImageInfo = new ImageInfo();
            ImageInfo.readableSectorTags = new List<SectorTagType>();
            ImageInfo.readableDiskTags = new List<DiskTagType>();
            ImageInfo.imageHasPartitions = false;
            ImageInfo.imageHasSessions = false;
            ImageInfo.imageVersion = null;
            ImageInfo.imageApplication = null;
            ImageInfo.imageApplicationVersion = null;
            ImageInfo.imageCreator = null;
            ImageInfo.imageComments = null;
            ImageInfo.diskManufacturer = null;
            ImageInfo.diskModel = null;
            ImageInfo.diskSerialNumber = null;
            ImageInfo.diskBarcode = null;
            ImageInfo.diskPartNumber = null;
            ImageInfo.diskSequence = 0;
            ImageInfo.lastDiskSequence = 0;
            ImageInfo.driveManufacturer = null;
            ImageInfo.driveModel = null;
            ImageInfo.driveSerialNumber = null;
        }

        public override bool IdentifyImage(string imagePath)
        {
            FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            stream.Seek(0, SeekOrigin.Begin);

            if (stream.Length < 65)
                return false;

            byte[] header = new byte[64];
            stream.Read(header, 0, 64);

            UInt32 magic = BitConverter.ToUInt32(header, 0x00);
            if (magic != MAGIC)
                return false;

            UInt32 dataoff = BitConverter.ToUInt32(header, 0x18);
            if (dataoff > stream.Length)
                return false;

            UInt32 datasize = BitConverter.ToUInt32(header, 0x1C);
            // There seems to be incorrect endian in some images on the wild
            if (datasize == 0x00800C00)
                datasize = 0x000C8000;
            if (dataoff + datasize > stream.Length)
                return false;

            UInt32 commentoff = BitConverter.ToUInt32(header, 0x20);
            if (commentoff > stream.Length)
                return false;

            UInt32 commentsize = BitConverter.ToUInt32(header, 0x24);
            if (commentoff + commentsize > stream.Length)
                return false;

            UInt32 creatoroff = BitConverter.ToUInt32(header, 0x28);
            if (creatoroff > stream.Length)
                return false;

            UInt32 creatorsize = BitConverter.ToUInt32(header, 0x2C);
            return creatoroff + creatorsize <= stream.Length;
        }

        public override bool OpenImage(string imagePath)
        {
            FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            stream.Seek(0, SeekOrigin.Begin);

            ImageHeader = new A2IMGHeader();

            byte[] header = new byte[64];
            stream.Read(header, 0, 64);
            byte[] magic = new byte[4];
            byte[] creator = new byte[4];

            Array.Copy(header, 0, magic, 0, 4);
            Array.Copy(header, 4, creator, 0, 4);

            ImageHeader.magic = BitConverter.ToUInt32(header, 0x00);
            ImageHeader.creator = BitConverter.ToUInt32(header, 0x04);
            ImageHeader.headerSize = BitConverter.ToUInt16(header, 0x08);
            ImageHeader.version = BitConverter.ToUInt16(header, 0x0A);
            ImageHeader.imageFormat = BitConverter.ToUInt32(header, 0x0C);
            ImageHeader.flags = BitConverter.ToUInt32(header, 0x10);
            ImageHeader.blocks = BitConverter.ToUInt32(header, 0x14);
            ImageHeader.dataOffset = BitConverter.ToUInt32(header, 0x18);
            ImageHeader.dataSize = BitConverter.ToUInt32(header, 0x1C);
            ImageHeader.commentOffset = BitConverter.ToUInt32(header, 0x20);
            ImageHeader.commentSize = BitConverter.ToUInt32(header, 0x24);
            ImageHeader.creatorSpecificOffset = BitConverter.ToUInt32(header, 0x28);
            ImageHeader.creatorSpecificSize = BitConverter.ToUInt32(header, 0x2C);
            ImageHeader.reserved1 = BitConverter.ToUInt32(header, 0x30);
            ImageHeader.reserved2 = BitConverter.ToUInt32(header, 0x34);
            ImageHeader.reserved3 = BitConverter.ToUInt32(header, 0x38);
            ImageHeader.reserved4 = BitConverter.ToUInt32(header, 0x3C);

            if (ImageHeader.dataSize == 0x00800C00)
            {
                ImageHeader.dataSize = 0x000C8000;
                DicConsole.DebugWriteLine("2MG plugin", "Detected incorrect endian on data size field, correcting.");
            }

            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.magic = \"{0}\"", Encoding.ASCII.GetString(magic));
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.creator = \"{0}\"", Encoding.ASCII.GetString(creator));
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.headerSize = {0}", ImageHeader.headerSize);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.version = {0}", ImageHeader.version);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.imageFormat = {0}", ImageHeader.imageFormat);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.flags = 0x{0:X8}", ImageHeader.flags);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.blocks = {0}", ImageHeader.blocks);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.dataOffset = 0x{0:X8}", ImageHeader.dataOffset);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.dataSize = {0}", ImageHeader.dataSize);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.commentOffset = 0x{0:X8}", ImageHeader.commentOffset);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.commentSize = {0}", ImageHeader.commentSize);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.creatorSpecificOffset = 0x{0:X8}", ImageHeader.creatorSpecificOffset);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.creatorSpecificSize = {0}", ImageHeader.creatorSpecificSize);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.reserved1 = 0x{0:X8}", ImageHeader.reserved1);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.reserved2 = 0x{0:X8}", ImageHeader.reserved2);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.reserved3 = 0x{0:X8}", ImageHeader.reserved3);
            DicConsole.DebugWriteLine("2MG plugin", "ImageHeader.reserved4 = 0x{0:X8}", ImageHeader.reserved4);

            if (ImageHeader.dataSize == 0 && ImageHeader.blocks == 0 && ImageHeader.imageFormat != ProDOSSectorOrder)
                return false;

            if (ImageHeader.imageFormat == ProDOSSectorOrder && ImageHeader.blocks == 0)
                return false;

            if (ImageHeader.imageFormat == ProDOSSectorOrder)
                ImageHeader.dataSize = ImageHeader.blocks * 512;
            else if (ImageHeader.blocks == 0 && ImageHeader.dataSize != 0)
                ImageHeader.blocks = ImageHeader.dataSize / 256;
            else if (ImageHeader.dataSize == 0 && ImageHeader.blocks != 0)
                ImageHeader.dataSize = ImageHeader.blocks * 256;

            ImageInfo.sectorSize = (uint)(ImageHeader.imageFormat == ProDOSSectorOrder ? 512 : 256);

            ImageInfo.sectors = ImageHeader.blocks;
            ImageInfo.imageSize = ImageHeader.dataSize;

            switch (ImageHeader.creator)
            {
                
                case CreatorAsimov:
                    ImageInfo.imageApplication = "ASIMOV2";
                    break;
                case CreatorBernie:
                    ImageInfo.imageApplication = "Bernie ][ the Rescue";
                    break;
                case CreatorCatakig:
                    ImageInfo.imageApplication = "Catakig";
                    break;
                case CreatorSheppy:
                    ImageInfo.imageApplication = "Sheppy's ImageMaker";
                    break;
                case CreatorSweet:
                    ImageInfo.imageApplication = "Sweet16";
                    break;
                case CreatorXGS:
                    ImageInfo.imageApplication = "XGS";
                    break;
                default:
                    ImageInfo.imageApplication = String.Format("Unknown creator code \"{0}\"", Encoding.ASCII.GetString(creator));
                    break;
            }

            ImageInfo.imageVersion = ImageHeader.version.ToString();

            if (ImageHeader.commentOffset != 0 && ImageHeader.commentSize != 0)
            {
                stream.Seek(ImageHeader.commentOffset, SeekOrigin.Begin);

                byte[] comments = new byte[ImageHeader.commentSize];
                stream.Read(comments, 0, (int)ImageHeader.commentSize);
                ImageInfo.imageComments = Encoding.ASCII.GetString(comments);
            }

            FileInfo fi = new FileInfo(imagePath);
            ImageInfo.imageCreationTime = fi.CreationTimeUtc;
            ImageInfo.imageLastModificationTime = fi.LastWriteTimeUtc;
            ImageInfo.imageName = Path.GetFileNameWithoutExtension(imagePath);

            stream.Close();

            a2mgImagePath = imagePath;

            return true;
        }

        public override bool ImageHasPartitions()
        {
            return false;
        }

        public override ulong GetImageSize()
        {
            return ImageInfo.imageSize;
        }

        public override ulong GetSectors()
        {
            return ImageInfo.sectors;
        }

        public override uint GetSectorSize()
        {
            return ImageInfo.sectorSize;
        }

        public override string GetImageFormat()
        {
            return "Apple 2IMG";
        }

        public override string GetImageVersion()
        {
            return ImageInfo.imageVersion;
        }

        public override string GetImageApplication()
        {
            return ImageInfo.imageApplication;
        }

        public override string GetImageApplicationVersion()
        {
            return ImageInfo.imageApplicationVersion;
        }

        public override string GetImageCreator()
        {
            return ImageInfo.imageCreator;
        }

        public override DateTime GetImageCreationTime()
        {
            return ImageInfo.imageCreationTime;
        }

        public override DateTime GetImageLastModificationTime()
        {
            return ImageInfo.imageLastModificationTime;
        }

        public override string GetImageName()
        {
            return ImageInfo.imageName;
        }

        public override string GetImageComments()
        {
            return ImageInfo.imageComments;
        }

        public override DiskType GetDiskType()
        {
            switch (ImageInfo.sectors)
            {
                case 455:
                    return DiskType.Apple32SS;
                case 910:
                    return DiskType.Apple32DS;
                case 560:
                    return DiskType.Apple33SS;
                case 1120:
                    return DiskType.Apple33DS;
                case 800:
                    return DiskType.AppleSonySS;
                case 1600:
                    return DiskType.AppleSonyDS;
                default:
                    return DiskType.Unknown;
            }
        }

        public override byte[] ReadSector(ulong sectorAddress)
        {
            return ReadSectors(sectorAddress, 1);
        }

        public override byte[] ReadSectors(ulong sectorAddress, uint length)
        {
            if (sectorAddress > ImageInfo.sectors - 1)
                throw new ArgumentOutOfRangeException("sectorAddress", "Sector address not found");

            if (sectorAddress + length > ImageInfo.sectors)
                throw new ArgumentOutOfRangeException("length", "Requested more sectors than available");

            byte[] buffer = new byte[length * ImageInfo.sectorSize];

            FileStream stream = new FileStream(a2mgImagePath, FileMode.Open, FileAccess.Read);

            stream.Seek((long)(ImageHeader.dataOffset + sectorAddress * ImageInfo.sectorSize), SeekOrigin.Begin);

            stream.Read(buffer, 0, (int)(length * ImageInfo.sectorSize));

            stream.Close();

            return buffer;
        }

        #region Unsupported features

        public override byte[] ReadDiskTag(DiskTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorTag(ulong sectorAddress, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSector(ulong sectorAddress, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorTag(ulong sectorAddress, uint track, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorsTag(ulong sectorAddress, uint length, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectors(ulong sectorAddress, uint length, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorsTag(ulong sectorAddress, uint length, uint track, SectorTagType tag)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorLong(ulong sectorAddress)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorLong(ulong sectorAddress, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorsLong(ulong sectorAddress, uint length)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override byte[] ReadSectorsLong(ulong sectorAddress, uint length, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override string GetDiskManufacturer()
        {
            return null;
        }

        public override string GetDiskModel()
        {
            return null;
        }

        public override string GetDiskSerialNumber()
        {
            return null;
        }

        public override string GetDiskBarcode()
        {
            return null;
        }

        public override string GetDiskPartNumber()
        {
            return null;
        }

        public override int GetDiskSequence()
        {
            return 0;
        }

        public override int GetLastDiskSequence()
        {
            return 0;
        }

        public override string GetDriveManufacturer()
        {
            return null;
        }

        public override string GetDriveModel()
        {
            return null;
        }

        public override string GetDriveSerialNumber()
        {
            return null;
        }

        public override List<CommonTypes.Partition> GetPartitions()
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override List<Track> GetTracks()
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override List<Track> GetSessionTracks(Session session)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override List<Track> GetSessionTracks(ushort session)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override List<Session> GetSessions()
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override bool? VerifySector(ulong sectorAddress)
        {
            return null;
        }

        public override bool? VerifySector(ulong sectorAddress, uint track)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override bool? VerifySectors(ulong sectorAddress, uint length, out List<ulong> FailingLBAs, out List<ulong> UnknownLBAs)
        {
            FailingLBAs = new List<ulong>();
            UnknownLBAs = new List<ulong>();
            for (ulong i = 0; i < ImageInfo.sectors; i++)
                UnknownLBAs.Add(i);
            return null;
        }

        public override bool? VerifySectors(ulong sectorAddress, uint length, uint track, out List<ulong> FailingLBAs, out List<ulong> UnknownLBAs)
        {
            throw new FeatureUnsupportedImageException("Feature not supported by image format");
        }

        public override bool? VerifyDiskImage()
        {
            return null;
        }

        #endregion
    }
}