﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : ResumeSupport.cs
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
using System;
using System.Collections.Generic;
using DiscImageChef.Metadata;
using Extents;
using Schemas;

namespace DiscImageChef.Core.Devices.Dumping
{
    public static class ResumeSupport
    {
        public static void Process(bool isLba, bool removable, ulong blocks, string Manufacturer, string Model, string Serial, Interop.PlatformID platform, ref Resume resume, ref DumpHardwareType currentTry, ref ExtentsULong extents)
        {
            if(resume != null)
            {
                if(!isLba)
                    throw new NotImplementedException("Resuming CHS devices is currently not supported.");

                if(resume.Removable != removable)
                    throw new Exception(string.Format("Resume file specifies a {0} device but you're requesting to dump a {1} device, not continuing...",
                                                      resume.Removable ? "removable" : "non removable",
                                                      removable ? "removable" : "non removable"));

                if(resume.LastBlock != blocks - 1)
                    throw new Exception(string.Format("Resume file specifies a device with {0} blocks but you're requesting to dump one with {1} blocks, not continuing...",
                                                      resume.LastBlock + 1, blocks));


                foreach(DumpHardwareType oldtry in resume.Tries)
                {
                    if(oldtry.Manufacturer != Manufacturer && !removable)
                        throw new Exception(string.Format("Resume file specifies a device manufactured by {0} but you're requesting to dump one by {1}, not continuing...",
                                                          oldtry.Manufacturer, Manufacturer));

                    if(oldtry.Model != Model && !removable)
                        throw new Exception(string.Format("Resume file specifies a device model {0} but you're requesting to dump model {1}, not continuing...",
                                                          oldtry.Model, Model));

                    if(oldtry.Serial != Serial && !removable)
                        throw new Exception(string.Format("Resume file specifies a device with serial {0} but you're requesting to dump one with serial {1}, not continuing...",
                                                          oldtry.Serial, Serial));

                    if(oldtry.Software == null)
                        throw new Exception("Found corrupt resume file, cannot continue...");

                    if(oldtry.Software.Name == "DiscImageChef" && oldtry.Software.OperatingSystem == platform.ToString() && oldtry.Software.Version == Version.GetVersion())
                    {
                        if(removable && (oldtry.Manufacturer != Manufacturer || oldtry.Model != Model || oldtry.Serial != Serial))
                            continue;
                        
                        currentTry = oldtry;
                        extents = ExtentsConverter.FromMetadata(currentTry.Extents);
                        break;
                    }
                }

                if(currentTry == null)
                {
                    currentTry = new DumpHardwareType
                    {
                        Software = Version.GetSoftwareType(platform),
                        Manufacturer = Manufacturer,
                        Model = Model,
                        Serial = Serial,
                    };
                    resume.Tries.Add(currentTry);
                    extents = new ExtentsULong();
                }
            }
            else
            {
                resume = new Resume
                {
                    Tries = new List<DumpHardwareType>(),
                    CreationDate = DateTime.UtcNow,
                    BadBlocks = new List<ulong>(),
                    LastBlock = blocks - 1
                };
                currentTry = new DumpHardwareType
                {
                    Software = Version.GetSoftwareType(platform),
                    Manufacturer = Manufacturer,
                    Model = Model,
                    Serial = Serial
                };
                resume.Tries.Add(currentTry);
                extents = new ExtentsULong();
                resume.Removable = removable;
            }
        }
    }
}
