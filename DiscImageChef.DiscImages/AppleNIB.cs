// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : AppleNIB.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disc image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Manages Apple nibbelized disc images.
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
using System.IO;
using System.Linq;
using DiscImageChef.CommonTypes;
using DiscImageChef.Console;
using DiscImageChef.Decoders.Floppy;
using DiscImageChef.Filters;

namespace DiscImageChef.ImagePlugins
{
	// TODO: Checksum sectors
	public class AppleNIB : ImagePlugin
	{
		Dictionary<ulong, byte[]> longSectors;
		Dictionary<ulong, byte[]> cookedSectors;
		Dictionary<ulong, byte[]> addressFields;

		readonly ulong[] dosSkewing = { 0, 7, 14, 6, 13, 5, 12, 4, 11, 3, 10, 2, 9, 1, 8, 15 };
		readonly ulong[] proDosSkewing = { 0, 8, 1, 9, 2, 10, 3, 11, 4, 12, 5, 13, 6, 14, 7, 15 };

		readonly byte[] pascal_sign = { 0x08, 0xA5, 0x0F, 0x29 };
		readonly byte[] pascal2_sign = { 0xFF, 0xA2, 0x00, 0x8E };
		readonly byte[] dos_sign = { 0xA2, 0x02, 0x8E, 0x52 };
		readonly byte[] sos_sign = { 0xC9, 0x20, 0xF0, 0x3E };
		readonly byte[] apple3_sign = { 0x8D, 0xD0, 0x03, 0x4C, 0xC7, 0xA4 };
		readonly byte[] cpm_sign = { 0xA2, 0x55, 0xA9, 0x00, 0x9D, 0x00, 0x0D, 0xCA };
		readonly byte[] prodos_string = { 0x50, 0x52, 0x4F, 0x44, 0x4F, 0x53 };
		readonly byte[] pascal_string = { 0x53, 0x59, 0x53, 0x54, 0x45, 0x2E, 0x41, 0x50, 0x50, 0x4C, 0x45 };
		readonly byte[] dri_string = { 0x43, 0x4F, 0x50, 0x59, 0x52, 0x49, 0x47, 0x48, 0x54, 0x20, 0x28, 0x43,
			0x29, 0x20, 0x31, 0x39, 0x37, 0x39, 0x2C, 0x20, 0x44, 0x49, 0x47, 0x49, 0x54, 0x41, 0x4C, 0x20,
			0x52, 0x45, 0x53, 0x45, 0x41, 0x52, 0x43, 0x48};

		public AppleNIB()
		{
			Name = "Apple NIB";
			PluginUUID = new Guid("AE171AE8-6747-49CC-B861-9D450B7CD42E");
			ImageInfo = new ImageInfo();
			ImageInfo.readableSectorTags = new List<SectorTagType>();
			ImageInfo.readableMediaTags = new List<MediaTagType>();
			ImageInfo.imageHasPartitions = false;
			ImageInfo.imageHasSessions = false;
			ImageInfo.imageVersion = null;
			ImageInfo.imageApplication = null;
			ImageInfo.imageApplicationVersion = null;
			ImageInfo.imageCreator = null;
			ImageInfo.imageComments = null;
			ImageInfo.mediaManufacturer = null;
			ImageInfo.mediaModel = null;
			ImageInfo.mediaSerialNumber = null;
			ImageInfo.mediaBarcode = null;
			ImageInfo.mediaPartNumber = null;
			ImageInfo.mediaSequence = 0;
			ImageInfo.lastMediaSequence = 0;
			ImageInfo.driveManufacturer = null;
			ImageInfo.driveModel = null;
			ImageInfo.driveSerialNumber = null;
			ImageInfo.driveFirmwareRevision = null;
		}

		public override bool IdentifyImage(Filter imageFilter)
		{
			Stream stream = imageFilter.GetDataForkStream();
			stream.Seek(0, SeekOrigin.Begin);

			if(stream.Length < 512)
				return false;

			byte[] test = new byte[512];
			stream.Read(test, 0, 512);

			return Apple2.IsApple2GCR(test);
		}

		public override bool OpenImage(Filter imageFilter)
		{
			Stream stream = imageFilter.GetDataForkStream();
			stream.Seek(0, SeekOrigin.Begin);

			if(stream.Length < 512)
				return false;

			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);

			DicConsole.DebugWriteLine("Apple NIB Plugin", "Decoding whole image");
			List<Apple2.RawTrack> tracks = Apple2.MarshalDisk(buffer);
			DicConsole.DebugWriteLine("Apple NIB Plugin", "Got {0} tracks", tracks.Count);

			Dictionary<ulong, Apple2.RawSector> rawSectors = new Dictionary<ulong, Apple2.RawSector>();

			int spt = 0;
			bool allTracksEqual = true;
			for(int i = 1; i < tracks.Count; i++)
			{
				allTracksEqual &= tracks[i - 1].sectors.Length == tracks[i].sectors.Length;
			}

			if(allTracksEqual)
				spt = tracks[0].sectors.Length;

			bool skewed = spt == 16;
			ulong[] skewing = dosSkewing;

			// Detect ProDOS skewed disks
			if(skewed)
			{
				byte[] sector1 = null;
				byte[] sector0 = null;

				foreach(Apple2.RawSector sector in tracks[0].sectors)
				{
					if(sector.addressField.sector.SequenceEqual(new byte[] { 170, 171 }))
						sector1 = Apple2.DecodeSector(sector);
					if(sector.addressField.sector.SequenceEqual(new byte[] { 170, 170 }))
						sector0 = Apple2.DecodeSector(sector);
				}

				if(sector1 != null)
				{
					byte[] tmpAt0Sz4 = new byte[4];
					byte[] tmpAt0Sz6 = new byte[6];
					byte[] tmpAt0Sz8 = new byte[8];
					byte[] tmpAt3Sz6 = new byte[6];
					byte[] tmpAt24Sz36 = new byte[36];
					byte[] tmpAt33Sz6 = new byte[6];

					Array.Copy(sector1, 0, tmpAt0Sz4, 0, 4);
					Array.Copy(sector1, 0, tmpAt0Sz6, 0, 6);
					Array.Copy(sector1, 0, tmpAt0Sz8, 0, 8);
					Array.Copy(sector1, 3, tmpAt3Sz6, 0, 6);
					Array.Copy(sector1, 24, tmpAt24Sz36, 0, 36);
					Array.Copy(sector1, 33, tmpAt33Sz6, 0, 6);

					if(tmpAt0Sz4.SequenceEqual(sos_sign))
						skewing = proDosSkewing;
					if(tmpAt0Sz4.SequenceEqual(dos_sign))
						skewing = proDosSkewing;
					if(tmpAt0Sz4.SequenceEqual(pascal2_sign))
						skewing = proDosSkewing;
					if(tmpAt0Sz6.SequenceEqual(apple3_sign))
						skewing = proDosSkewing;
					if(tmpAt0Sz8.SequenceEqual(cpm_sign))
						skewing = proDosSkewing;
					if(tmpAt3Sz6.SequenceEqual(prodos_string))
						skewing = proDosSkewing;
					if(tmpAt24Sz36.SequenceEqual(dri_string))
						skewing = proDosSkewing;
					if(tmpAt33Sz6.SequenceEqual(prodos_string))
						skewing = proDosSkewing;

					if(sector0 != null)
					{
						byte[] tmpAt215Sz12 = new byte[12];
						Array.Copy(sector0, 215, tmpAt215Sz12, 0, 12);
						if(tmpAt215Sz12.SequenceEqual(pascal_string) && tmpAt0Sz4.SequenceEqual(pascal_sign))
							skewing = proDosSkewing;
					}

					DicConsole.DebugWriteLine("Apple NIB Plugin", "Image is skewed");
				}
			}

			for(int i = 0; i < tracks.Count; i++)
			{
				foreach(Apple2.RawSector sector in tracks[i].sectors)
				{
					if(skewed && spt != 0)
					{
						ulong sectorNo = (ulong)((((sector.addressField.sector[0] & 0x55) << 1) | (sector.addressField.sector[1] & 0x55)) & 0xFF);
						DicConsole.DebugWriteLine("Apple NIB Plugin", "Hardware sector {0} of track {1} goes to logical sector {2}", sectorNo, i, skewing[sectorNo] + (ulong)(i * spt));
						rawSectors.Add(skewing[sectorNo] + (ulong)(i * spt), sector);
						ImageInfo.sectors++;
					}
					else
					{
						rawSectors.Add(ImageInfo.sectors, sector);
						ImageInfo.sectors++;
					}
				}
			}

			DicConsole.DebugWriteLine("Apple NIB Plugin", "Got {0} sectors", ImageInfo.sectors);

			DicConsole.DebugWriteLine("Apple NIB Plugin", "Cooking sectors");

			longSectors = new Dictionary<ulong, byte[]>();
			cookedSectors = new Dictionary<ulong, byte[]>();
			addressFields = new Dictionary<ulong, byte[]>();

			foreach(KeyValuePair<ulong, Apple2.RawSector> kvp in rawSectors)
			{
				byte[] cooked = Apple2.DecodeSector(kvp.Value);
				byte[] raw = Apple2.MarshalSector(kvp.Value);
				byte[] addr = Apple2.MarshalAddressField(kvp.Value.addressField);
				longSectors.Add(kvp.Key, raw);
				cookedSectors.Add(kvp.Key, cooked);
				addressFields.Add(kvp.Key, addr);
			}

			ImageInfo.imageSize = (ulong)imageFilter.GetDataForkLength();
			ImageInfo.imageCreationTime = imageFilter.GetCreationTime();
			ImageInfo.imageLastModificationTime = imageFilter.GetLastWriteTime();
			ImageInfo.imageName = Path.GetFileNameWithoutExtension(imageFilter.GetFilename());
			if(ImageInfo.sectors == 455)
				ImageInfo.mediaType = MediaType.Apple32SS;
			else if(ImageInfo.sectors == 560)
				ImageInfo.mediaType = MediaType.Apple33SS;
			else
				ImageInfo.mediaType = MediaType.Unknown;
			ImageInfo.sectorSize = 256;
			ImageInfo.xmlMediaType = XmlMediaType.BlockMedia;
			ImageInfo.readableSectorTags.Add(SectorTagType.FloppyAddressMark);
			switch(ImageInfo.mediaType)
			{
				case MediaType.Apple32SS:
					ImageInfo.cylinders = 35;
					ImageInfo.heads = 1;
					ImageInfo.sectorsPerTrack = 13;
					break;
				case MediaType.Apple33SS:
					ImageInfo.cylinders = 35;
					ImageInfo.heads = 1;
					ImageInfo.sectorsPerTrack = 16;
					break;
			}

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
			return "Apple nibbles";
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

		public override MediaType GetMediaType()
		{
			switch(ImageInfo.sectors)
			{
				case 455:
					return MediaType.Apple32SS;
				case 560:
					return MediaType.Apple33SS;
				default:
					return MediaType.Unknown;
			}
		}

		public override byte[] ReadSector(ulong sectorAddress)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			byte[] temp;
			cookedSectors.TryGetValue(sectorAddress, out temp);
			return temp;
		}

		public override byte[] ReadSectors(ulong sectorAddress, uint length)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			if(sectorAddress + length > ImageInfo.sectors)
				throw new ArgumentOutOfRangeException(nameof(length), "Requested more sectors than available");

			MemoryStream ms = new MemoryStream();

			for(uint i = 0; i < length; i++)
			{
				byte[] sector = ReadSector(sectorAddress + i);
				ms.Write(sector, 0, sector.Length);
			}

			return ms.ToArray();
		}

		public override byte[] ReadSectorTag(ulong sectorAddress, SectorTagType tag)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			if(tag != SectorTagType.FloppyAddressMark)
				throw new FeatureUnsupportedImageException(string.Format("Tag {0} not supported by image format", tag));

			byte[] temp;
			addressFields.TryGetValue(sectorAddress, out temp);
			return temp;
		}

		public override byte[] ReadSectorsTag(ulong sectorAddress, uint length, SectorTagType tag)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			if(sectorAddress + length > ImageInfo.sectors)
				throw new ArgumentOutOfRangeException(nameof(length), "Requested more sectors than available");

			if(tag != SectorTagType.FloppyAddressMark)
				throw new FeatureUnsupportedImageException(string.Format("Tag {0} not supported by image format", tag));

			MemoryStream ms = new MemoryStream();

			for(uint i = 0; i < length; i++)
			{
				byte[] sector = ReadSectorTag(sectorAddress + i, tag);
				ms.Write(sector, 0, sector.Length);
			}

			return ms.ToArray();
		}

		public override byte[] ReadSectorLong(ulong sectorAddress)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			byte[] temp;
			longSectors.TryGetValue(sectorAddress, out temp);
			return temp;
		}

		public override byte[] ReadSectorsLong(ulong sectorAddress, uint length)
		{
			if(sectorAddress > ImageInfo.sectors - 1)
				throw new ArgumentOutOfRangeException(nameof(sectorAddress), string.Format("Sector address {0} not found", sectorAddress));

			if(sectorAddress + length > ImageInfo.sectors)
				throw new ArgumentOutOfRangeException(nameof(length), "Requested more sectors than available");

			MemoryStream ms = new MemoryStream();

			for(uint i = 0; i < length; i++)
			{
				byte[] sector = ReadSectorLong(sectorAddress + i);
				ms.Write(sector, 0, sector.Length);
			}

			return ms.ToArray();
		}

		#region Unsupported features

		public override byte[] ReadDiskTag(MediaTagType tag)
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

		public override byte[] ReadSectors(ulong sectorAddress, uint length, uint track)
		{
			throw new FeatureUnsupportedImageException("Feature not supported by image format");
		}

		public override byte[] ReadSectorsTag(ulong sectorAddress, uint length, uint track, SectorTagType tag)
		{
			throw new FeatureUnsupportedImageException("Feature not supported by image format");
		}

		public override byte[] ReadSectorLong(ulong sectorAddress, uint track)
		{
			throw new FeatureUnsupportedImageException("Feature not supported by image format");
		}

		public override byte[] ReadSectorsLong(ulong sectorAddress, uint length, uint track)
		{
			throw new FeatureUnsupportedImageException("Feature not supported by image format");
		}

		public override string GetMediaManufacturer()
		{
			return null;
		}

		public override string GetMediaModel()
		{
			return null;
		}

		public override string GetMediaSerialNumber()
		{
			return null;
		}

		public override string GetMediaBarcode()
		{
			return null;
		}

		public override string GetMediaPartNumber()
		{
			return null;
		}

		public override int GetMediaSequence()
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

		public override List<Partition> GetPartitions()
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
			for(ulong i = 0; i < ImageInfo.sectors; i++)
				UnknownLBAs.Add(i);
			return null;
		}

		public override bool? VerifySectors(ulong sectorAddress, uint length, uint track, out List<ulong> FailingLBAs, out List<ulong> UnknownLBAs)
		{
			throw new FeatureUnsupportedImageException("Feature not supported by image format");
		}

		public override bool? VerifyMediaImage()
		{
			return null;
		}

		#endregion
	}
}