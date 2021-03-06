﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : BlockMedia.cs
// Version        : 1.0
// Author(s)      : Natalia Portillo
//
// Component      : Component
//
// Revision       : $Revision$
// Last change by : $Author$
// Date           : $Date$
//
// --[ Description ] ----------------------------------------------------------
//
// Description
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
// Copyright (C) 2011-2015 Claunia.com
// ****************************************************************************/
// //$Id$

using System.Collections.Generic;
using System.IO;
using DiscImageChef.CommonTypes;
using DiscImageChef.Decoders.PCMCIA;
using DiscImageChef.Filesystems;
using DiscImageChef.ImagePlugins;
using Schemas;

namespace DiscImageChef.Core
{
    public static partial class Sidecar
    {
        static void BlockMedia(ImagePlugin image, System.Guid filterId, string imagePath, FileInfo fi, PluginBase plugins, List<ChecksumType> imgChecksums, ref CICMMetadataType sidecar)
        {
            sidecar.BlockMedia = new[]
            {
	            new BlockMediaType
	            {
	                Checksums = imgChecksums.ToArray(),
	                Image = new ImageType
	                {
	                    format = image.GetImageFormat(),
	                    offset = 0,
	                    offsetSpecified = true,
	                    Value = Path.GetFileName(imagePath)
	                },
	                Size = fi.Length,
	                Sequence = new SequenceType
	                {
	                    MediaTitle = image.GetImageName()
	                }
                }
            };

            if(image.GetMediaSequence() != 0 && image.GetLastDiskSequence() != 0)
            {
                sidecar.BlockMedia[0].Sequence.MediaSequence = image.GetMediaSequence();
                sidecar.BlockMedia[0].Sequence.TotalMedia = image.GetMediaSequence();
            }
            else
            {
                sidecar.BlockMedia[0].Sequence.MediaSequence = 1;
                sidecar.BlockMedia[0].Sequence.TotalMedia = 1;
            }

            foreach(MediaTagType tagType in image.ImageInfo.readableMediaTags)
            {
                switch(tagType)
                {
                    case MediaTagType.ATAPI_IDENTIFY:
                        sidecar.BlockMedia[0].ATA = new ATAType
                        {
                            Identify = new DumpType
                            {
                                Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.ATAPI_IDENTIFY)).ToArray(),
                                Size = image.ReadDiskTag(MediaTagType.ATAPI_IDENTIFY).Length
                            }
                        };
                        break;
                    case MediaTagType.ATA_IDENTIFY:
                        sidecar.BlockMedia[0].ATA = new ATAType
                        {
                            Identify = new DumpType
                            {
                                Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.ATA_IDENTIFY)).ToArray(),
                                Size = image.ReadDiskTag(MediaTagType.ATA_IDENTIFY).Length
                            }
                        };
                        break;
                    case MediaTagType.PCMCIA_CIS:
                        byte[] cis = image.ReadDiskTag(MediaTagType.PCMCIA_CIS);
                        sidecar.BlockMedia[0].PCMCIA = new PCMCIAType
                        {
                            CIS = new DumpType
                            {
                                Checksums = Checksum.GetChecksums(cis).ToArray(),
                                Size = cis.Length
                            }
                        };
                        Tuple[] tuples = CIS.GetTuples(cis);
                        if(tuples != null)
                        {
                            foreach(Tuple tuple in tuples)
                            {
                                if(tuple.Code == TupleCodes.CISTPL_MANFID)
                                {
                                    ManufacturerIdentificationTuple manfid = CIS.DecodeManufacturerIdentificationTuple(tuple);

                                    if(manfid != null)
                                    {
                                        sidecar.BlockMedia[0].PCMCIA.ManufacturerCode = manfid.ManufacturerID;
                                        sidecar.BlockMedia[0].PCMCIA.CardCode = manfid.CardID;
                                        sidecar.BlockMedia[0].PCMCIA.ManufacturerCodeSpecified = true;
                                        sidecar.BlockMedia[0].PCMCIA.CardCodeSpecified = true;
                                    }
                                }
                                else if(tuple.Code == TupleCodes.CISTPL_VERS_1)
                                {
                                    Level1VersionTuple vers = CIS.DecodeLevel1VersionTuple(tuple);

                                    if(vers != null)
                                    {
                                        sidecar.BlockMedia[0].PCMCIA.Manufacturer = vers.Manufacturer;
                                        sidecar.BlockMedia[0].PCMCIA.ProductName = vers.Product;
                                        sidecar.BlockMedia[0].PCMCIA.Compliance = string.Format("{0}.{1}", vers.MajorVersion, vers.MinorVersion);
                                        sidecar.BlockMedia[0].PCMCIA.AdditionalInformation = vers.AdditionalInformation;
                                    }
                                }
                            }
                        }
                        break;
                    case MediaTagType.SCSI_INQUIRY:
                        sidecar.BlockMedia[0].SCSI = new SCSIType
                        {
                            Inquiry = new DumpType
                            {
                                Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.SCSI_INQUIRY)).ToArray(),
                                Size = image.ReadDiskTag(MediaTagType.SCSI_INQUIRY).Length
                            }
                        };
                        break;
                    case MediaTagType.SD_CID:
                        if(sidecar.BlockMedia[0].SecureDigital == null)
                            sidecar.BlockMedia[0].SecureDigital = new SecureDigitalType();
                        sidecar.BlockMedia[0].SecureDigital.CID = new DumpType
                        {
                            Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.SD_CID)).ToArray(),
                            Size = image.ReadDiskTag(MediaTagType.SD_CID).Length
                        };
                        break;
                    case MediaTagType.SD_CSD:
                        if(sidecar.BlockMedia[0].SecureDigital == null)
                            sidecar.BlockMedia[0].SecureDigital = new SecureDigitalType();
                        sidecar.BlockMedia[0].SecureDigital.CSD = new DumpType
                        {
                            Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.SD_CSD)).ToArray(),
                            Size = image.ReadDiskTag(MediaTagType.SD_CSD).Length
                        };
                        break;
                    case MediaTagType.SD_ExtendedCSD:
                        if(sidecar.BlockMedia[0].SecureDigital == null)
                            sidecar.BlockMedia[0].SecureDigital = new SecureDigitalType();
                        sidecar.BlockMedia[0].SecureDigital.ExtendedCSD = new DumpType
                        {
                            Checksums = Checksum.GetChecksums(image.ReadDiskTag(MediaTagType.SD_ExtendedCSD)).ToArray(),
                            Size = image.ReadDiskTag(MediaTagType.SD_ExtendedCSD).Length
                        };
                        break;
                }
            }

            // If there is only one track, and it's the same as the image file (e.g. ".iso" files), don't re-checksum.
            if(image.PluginUUID == new System.Guid("12345678-AAAA-BBBB-CCCC-123456789000") &&
               filterId == new System.Guid("12345678-AAAA-BBBB-CCCC-123456789000"))
            {
                sidecar.BlockMedia[0].ContentChecksums = sidecar.BlockMedia[0].Checksums;
            }
            else
            {
                Checksum contentChkWorker = new Checksum();

                uint sectorsToRead = 512;
                ulong sectors = image.GetSectors();
                ulong doneSectors = 0;

                InitProgress2();
                while(doneSectors < sectors)
                {
                    byte[] sector;

                    if((sectors - doneSectors) >= sectorsToRead)
                    {
                        sector = image.ReadSectors(doneSectors, sectorsToRead);
                        UpdateProgress2("Hashings sector {0} of {1}", (long)doneSectors, (long)sectors);
                        doneSectors += sectorsToRead;
                    }
                    else
                    {
                        sector = image.ReadSectors(doneSectors, (uint)(sectors - doneSectors));
                        UpdateProgress2("Hashings sector {0} of {1}", (long)doneSectors, (long)sectors);
                        doneSectors += (sectors - doneSectors);
                    }

                    contentChkWorker.Update(sector);
                }

                List<ChecksumType> cntChecksums = contentChkWorker.End();

                sidecar.BlockMedia[0].ContentChecksums = cntChecksums.ToArray();

                EndProgress2();
            }

            Metadata.MediaType.MediaTypeToString(image.ImageInfo.mediaType, out string dskType, out string dskSubType);
            sidecar.BlockMedia[0].DiskType = dskType;
            sidecar.BlockMedia[0].DiskSubType = dskSubType;
            Statistics.AddMedia(image.ImageInfo.mediaType, false);

            sidecar.BlockMedia[0].Dimensions = Metadata.Dimensions.DimensionsFromMediaType(image.ImageInfo.mediaType);

            sidecar.BlockMedia[0].LogicalBlocks = (long)image.GetSectors();
            sidecar.BlockMedia[0].LogicalBlockSize = (int)image.GetSectorSize();
            // TODO: Detect it
            sidecar.BlockMedia[0].PhysicalBlockSize = (int)image.GetSectorSize();

            UpdateStatus("Checking filesystems...");

            List<Partition> partitions = Partitions.GetAll(image);
            Partitions.AddSchemesToStats(partitions);

            sidecar.BlockMedia[0].FileSystemInformation = new PartitionType[1];
            if(partitions.Count > 0)
            {
                sidecar.BlockMedia[0].FileSystemInformation = new PartitionType[partitions.Count];
                for(int i = 0; i < partitions.Count; i++)
                {
                    sidecar.BlockMedia[0].FileSystemInformation[i] = new PartitionType
                    {
                        Description = partitions[i].Description,
                        EndSector = (int)(partitions[i].End),
                        Name = partitions[i].Name,
                        Sequence = (int)partitions[i].Sequence,
                        StartSector = (int)partitions[i].Start,
                        Type = partitions[i].Type
                    };
                    List<FileSystemType> lstFs = new List<FileSystemType>();

                    foreach(Filesystem _plugin in plugins.PluginsList.Values)
                    {
                        try
                        {
                            if(_plugin.Identify(image, partitions[i]))
                            {
                                _plugin.GetInformation(image, partitions[i], out string foo);
                                lstFs.Add(_plugin.XmlFSType);
                                Statistics.AddFilesystem(_plugin.XmlFSType.Type);
                            }
                        }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                        catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                        {
                            //DicConsole.DebugWriteLine("Create-sidecar command", "Plugin {0} crashed", _plugin.Name);
                        }
                    }

                    if(lstFs.Count > 0)
                        sidecar.BlockMedia[0].FileSystemInformation[i].FileSystems = lstFs.ToArray();
                }
            }
            else
            {
                sidecar.BlockMedia[0].FileSystemInformation[0] = new PartitionType
                {
                    StartSector = 0,
                    EndSector = (int)(image.GetSectors() - 1)
                };

                Partition wholePart = new Partition
                {
                    Name = "Whole device",
                    Length = image.GetSectors(),
                    Size = image.GetSectors() * image.GetSectorSize()
                };

                List<FileSystemType> lstFs = new List<FileSystemType>();

                foreach(Filesystem _plugin in plugins.PluginsList.Values)
                {
                    try
                    {
                        if(_plugin.Identify(image, wholePart))
                        {
                            _plugin.GetInformation(image, wholePart, out string foo);
                            lstFs.Add(_plugin.XmlFSType);
                            Statistics.AddFilesystem(_plugin.XmlFSType.Type);
                        }
                    }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                    catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                    {
                        //DicConsole.DebugWriteLine("Create-sidecar command", "Plugin {0} crashed", _plugin.Name);
                    }
                }

                if(lstFs.Count > 0)
                    sidecar.BlockMedia[0].FileSystemInformation[0].FileSystems = lstFs.ToArray();
            }

            if(image.ImageInfo.cylinders > 0 && image.ImageInfo.heads > 0 && image.ImageInfo.sectorsPerTrack > 0)
            {
                sidecar.BlockMedia[0].CylindersSpecified = true;
                sidecar.BlockMedia[0].HeadsSpecified = true;
                sidecar.BlockMedia[0].SectorsPerTrackSpecified = true;
                sidecar.BlockMedia[0].Cylinders = image.ImageInfo.cylinders;
                sidecar.BlockMedia[0].Heads = image.ImageInfo.heads;
                sidecar.BlockMedia[0].SectorsPerTrack = image.ImageInfo.sectorsPerTrack;
            }

            if(image.ImageInfo.readableMediaTags.Contains(MediaTagType.ATA_IDENTIFY))
            {
                Decoders.ATA.Identify.IdentifyDevice? ataId = Decoders.ATA.Identify.Decode(image.ReadDiskTag(MediaTagType.ATA_IDENTIFY));
                if(ataId.HasValue)
                {
                    if(ataId.Value.CurrentCylinders > 0 && ataId.Value.CurrentHeads > 0 && ataId.Value.CurrentSectorsPerTrack > 0)
                    {
                        sidecar.BlockMedia[0].CylindersSpecified = true;
                        sidecar.BlockMedia[0].HeadsSpecified = true;
                        sidecar.BlockMedia[0].SectorsPerTrackSpecified = true;
                        sidecar.BlockMedia[0].Cylinders = ataId.Value.CurrentCylinders;
                        sidecar.BlockMedia[0].Heads = ataId.Value.CurrentHeads;
                        sidecar.BlockMedia[0].SectorsPerTrack = ataId.Value.CurrentSectorsPerTrack;
                    }
                    else if(ataId.Value.Cylinders > 0 && ataId.Value.Heads > 0 && ataId.Value.SectorsPerTrack > 0)
                    {
                        sidecar.BlockMedia[0].CylindersSpecified = true;
                        sidecar.BlockMedia[0].HeadsSpecified = true;
                        sidecar.BlockMedia[0].SectorsPerTrackSpecified = true;
                        sidecar.BlockMedia[0].Cylinders = ataId.Value.Cylinders;
                        sidecar.BlockMedia[0].Heads = ataId.Value.Heads;
                        sidecar.BlockMedia[0].SectorsPerTrack = ataId.Value.SectorsPerTrack;
                    }
                }
            }

            // TODO: Implement support for getting CHS from SCSI mode pages
        }
    }
}
