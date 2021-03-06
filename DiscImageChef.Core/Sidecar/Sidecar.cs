﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Sidecar.cs
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
using DiscImageChef.ImagePlugins;
using Schemas;

namespace DiscImageChef.Core
{
    public static partial class Sidecar
    {
        public static CICMMetadataType Create(ImagePlugin image, string imagePath, System.Guid filterId)
        {
            CICMMetadataType sidecar = new CICMMetadataType();
            PluginBase plugins = new PluginBase();
            plugins.RegisterAllPlugins();

            FileInfo fi = new FileInfo(imagePath);
            FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);

            Checksum imgChkWorker = new Checksum();

            // For fast debugging, skip checksum
            //goto skipImageChecksum;

            byte[] data;
            long position = 0;
            InitProgress();
            while(position < (fi.Length - 1048576))
            {
                data = new byte[1048576];
                fs.Read(data, 0, 1048576);

                UpdateProgress("Hashing image file byte {0} of {1}", position, fi.Length);

                imgChkWorker.Update(data);

                position += 1048576;
            }

            data = new byte[fi.Length - position];
            fs.Read(data, 0, (int)(fi.Length - position));

            UpdateProgress("Hashing image file byte {0} of {1}", position, fi.Length);

            imgChkWorker.Update(data);

            // For fast debugging, skip checksum
            //skipImageChecksum:

            EndProgress();
            fs.Close();

            List<ChecksumType> imgChecksums = imgChkWorker.End();

            switch(image.ImageInfo.xmlMediaType)
            {
                case XmlMediaType.OpticalDisc:
                    OpticalDisc(image, filterId, imagePath, fi, plugins, imgChecksums, ref sidecar);
                    break;
                case XmlMediaType.BlockMedia:
                    BlockMedia(image, filterId, imagePath, fi, plugins, imgChecksums, ref sidecar);
                    break;
                case XmlMediaType.LinearMedia:
                    LinearMedia(image, filterId, imagePath, fi, plugins, imgChecksums, ref sidecar);
                    break;
                case XmlMediaType.AudioMedia:
                    AudioMedia(image, filterId, imagePath, fi, plugins, imgChecksums, ref sidecar);
                    break;
            }

            return sidecar;
        }
    }
}
