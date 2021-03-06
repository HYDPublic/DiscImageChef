// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : VendorString.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Device structures decoders.
//
// --[ Description ] ----------------------------------------------------------
//
//     Contains SCSI vendor strings.
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

namespace DiscImageChef.Decoders.SCSI
{
    public static class VendorString
    {
        public static string Prettify(string SCSIVendorString)
        {
            switch(SCSIVendorString)
            {
                case "0B4C":
                    return "MOOSIK Ltd.";
                case "13FE":
                    return "PHISON";
                case "2AI":
                    return "2AI (Automatisme et Avenir Informatique)";
                case "3M":
                    return "3M Company";
                case "3nhtech":
                    return "3NH Technologies";
                case "3PARdata":
                    return "3PARdata, Inc.";
                case "A-Max":
                    return "A-Max Technology Co., Ltd";
                case "ABSOLUTE":
                    return "Absolute Analysis";
                case "ACARD":
                    return "ACARD Technology Corp.";
                case "Accusys":
                    return "Accusys INC.";
                case "Acer":
                    return "Acer, Inc.";
                case "ACL":
                    return "Automated Cartridge Librarys, Inc.";
                case "Actifio":
                    return "Actifio";
                case "Acuid":
                    return "Acuid Corporation Ltd.";
                case "AcuLab":
                    return "AcuLab, Inc. (Tulsa, OK)";
                case "ADAPTEC":
                    return "Adaptec";
                case "ADIC":
                    return "Advanced Digital Information Corporation";
                case "ADSI":
                    return "Adaptive Data Systems, Inc. (a Western Digital subsidiary)";
                case "ADTX":
                    return "ADTX Co., Ltd.";
                case "ADVA":
                    return "ADVA Optical Networking AG";
                case "AEM":
                    return "AEM Performance Electronics";
                case "AERONICS":
                    return "Aeronics, Inc.";
                case "AGFA":
                    return "AGFA";
                case "Agilent":
                    return "Agilent Technologies";
                case "AIC":
                    return "Advanced Industrial Computer, Inc.";
                case "AIPTEK":
                    return "AIPTEK International Inc.";
                case "Alcohol":
                    return "Alcohol Soft";
                case "ALCOR":
                    return "Alcor Micro, Corp.";
                case "AMCC":
                    return "Applied Micro Circuits Corporation";
                case "AMCODYNE":
                    return "Amcodyne";
                case "Amgeon":
                    return "Amgeon LLC";
                case "AMI":
                    return "American Megatrends, Inc.";
                case "AMPEX":
                    return "Ampex Data Systems";
                case "Amphenol":
                    return "Amphenol";
                case "Amtl":
                    return "Tenlon Technology Co.,Ltd";
                case "ANAMATIC":
                    return "Anamartic Limited (England)";
                case "Ancor":
                    return "Ancor Communications, Inc.";
                case "ANCOT":
                    return "ANCOT Corp.";
                case "ANDATACO":
                    return "Andataco";
                case "andiamo":
                    return "Andiamo Systems, Inc.";
                case "ANOBIT":
                    return "Anobit";
                case "ANRITSU":
                    return "Anritsu Corporation";
                case "ANTONIO":
                    return "Antonio Precise Products Manufactory Ltd.";
                case "AoT":
                    return "Art of Technology AG";
                case "APPLE":
                    return "Apple Computer, Inc.";
                case "ARCHIVE":
                    return "Archive";
                case "ARDENCE":
                    return "Ardence Inc";
                case "Areca":
                    return "Areca Technology Corporation";
                case "Arena":
                    return "MaxTronic International Co., Ltd.";
                case "Argent":
                    return "Argent Data Systems, Inc.";
                case "ARIO":
                    return "Ario Data Networks, Inc.";
                case "ARISTOS":
                    return "Aristos Logic Corp.";
                case "ARK":
                    return "ARK Research Corporation";
                case "ARL:UT@A":
                    return "Applied Research Laboratories : University of Texas at Austin";
                case "ARTECON":
                    return "Artecon Inc.";
                case "Artistic":
                    return "Artistic Licence (UK) Ltd";
                case "ARTON":
                    return "Arton Int.";
                case "ASACA":
                    return "ASACA Corp.";
                case "ASC":
                    return "Advanced Storage Concepts, Inc.";
                case "ASPEN":
                    return "Aspen Peripherals";
                case "AST":
                    return "AST Research";
                case "ASTEK":
                    return "Astek Corporation";
                case "ASTK":
                    return "Alcatel STK A/S";
                case "AStor":
                    return "AccelStor, Inc.";
                case "ASTUTE":
                    return "Astute Networks, Inc.";
                case "AT&T":
                    return "AT&T";
                case "ATA":
                    return "SCSI / ATA Translator Software (Organization Not Specified)";
                case "ATARI":
                    return "Atari Corporation";
                case "ATech":
                    return "ATech electronics";
                case "ATG CYG":
                    return "ATG Cygnet Inc.";
                case "ATL":
                    return "Quantum|ATL Products";
                case "ATTO":
                    return "ATTO Technology Inc.";
                case "ATTRATEC":
                    return "Attratech Ltd liab. Co";
                case "ATX":
                    return "Alphatronix";
                case "AURASEN":
                    return "Aurasen Limited";
                case "Avago":
                    return "Avago Technologies";
                case "AVC":
                    return "AVC Technology Ltd";
                case "AVIDVIDR":
                    return "AVID Technologies, Inc.";
                case "AVR":
                    return "Advanced Vision Research";
                case "AXSTOR":
                    return "AXSTOR";
                case "Axxana":
                    return "Axxana Ltd.";
                case "B*BRIDGE":
                    return "Blockbridge Networks LLC";
                case "BALLARD":
                    return "Ballard Synergy Corp.";
                case "Barco":
                    return "Barco";
                case "BAROMTEC":
                    return "Barom Technologies Co., Ltd.";
                case "Bassett":
                    return "Bassett Electronic Systems Ltd";
                case "BC Hydro":
                    return "BC Hydro";
                case "BDT":
                    return "BDT AG";
                case "BECEEM":
                    return "Beceem Communications, Inc";
                case "BENQ":
                    return "BENQ Corporation.";
                case "BERGSWD":
                    return "Berg Software Design";
                case "BEZIER":
                    return "Bezier Systems, Inc.";
                case "BHTi":
                    return "Breece Hill Technologies";
                case "biodata":
                    return "Biodata Devices SL";
                case "BIOS":
                    return "BIOS Corporation";
                case "BIR":
                    return "Bio-Imaging Research, Inc.";
                case "BiT":
                    return "BiT Microsystems";
                case "BITMICRO":
                    return "BiT Microsystems, Inc.";
                case "Blendlgy":
                    return "Blendology Limited";
                case "BLOOMBAS":
                    return "Bloombase Technologies Limited";
                case "BlueArc":
                    return "BlueArc Corporation";
                case "bluecog":
                    return "bluecog";
                case "BME-HVT":
                    return "Broadband Infocommunicatons and Electromagnetic Theory Department";
                case "BNCHMARK":
                    return "Benchmark Tape Systems Corporation";
                case "Bosch":
                    return "Robert Bosch GmbH";
                case "Botman":
                    return "Botmanfamily Electronics";
                case "BoxHill":
                    return "Box Hill Systems Corporation";
                case "BRDGWRKS":
                    return "Bridgeworks Ltd.";
                case "BREA":
                    return "BREA Technologies, Inc.";
                case "BREECE":
                    return "Breece Hill LLC";
                case "BreqLabs":
                    return "BreqLabs Inc.";
                case "Broadcom":
                    return "Broadcom Corporation";
                case "BROCADE":
                    return "Brocade Communications Systems, Incorporated";
                case "BUFFALO":
                    return "BUFFALO INC.";
                case "BULL":
                    return "Bull Peripherals Corp.";
                case "BUSLOGIC":
                    return "BusLogic Inc.";
                case "BVIRTUAL":
                    return "B-Virtual N.V.";
                case "CACHEIO":
                    return "CacheIO LLC";
                case "CalComp":
                    return "CalComp, A Lockheed Company";
                case "CALCULEX":
                    return "CALCULEX, Inc.";
                case "CALIPER":
                    return "Caliper (California Peripheral Corp.)";
                case "CAMBEX":
                    return "Cambex Corporation";
                case "CAMEOSYS":
                    return "Cameo Systems Inc.";
                case "CANDERA":
                    return "Candera Inc.";
                case "CAPTION":
                    return "CAPTION BANK";
                case "CAST":
                    return "Advanced Storage Tech";
                case "CATALYST":
                    return "Catalyst Enterprises";
                case "CCDISK":
                    return "iSCSI Cake";
                case "CDC":
                    return "Control Data or MPI";
                case "CDP":
                    return "Columbia Data Products";
                case "Celsia":
                    return "A M Bromley Limited";
                case "CenData":
                    return "Central Data Corporation";
                case "Cereva":
                    return "Cereva Networks Inc.";
                case "CERTANCE":
                    return "Certance";
                case "Chantil":
                    return "Chantil Technology";
                case "CHEROKEE":
                    return "Cherokee Data Systems";
                case "CHINON":
                    return "Chinon";
                case "CHRISTMA":
                    return "Christmann Informationstechnik + Medien GmbH & Co KG";
                case "CIE&YED":
                    return "YE Data, C.Itoh Electric Corp.";
                case "CIPHER":
                    return "Cipher Data Products";
                case "Ciprico":
                    return "Ciprico, Inc.";
                case "CIRRUSL":
                    return "Cirrus Logic Inc.";
                case "CISCO":
                    return "Cisco Systems, Inc.";
                case "CLEARSKY":
                    return "ClearSky Data, Inc.";
                case "CLOVERLF":
                    return "Cloverleaf Communications, Inc";
                case "CLS":
                    return "Celestica";
                case "CMD":
                    return "CMD Technology Inc.";
                case "CMTechno":
                    return "CMTech";
                case "CNGR SFW":
                    return "Congruent Software, Inc.";
                case "CNSi":
                    return "Chaparral Network Storage, Inc.";
                case "CNT":
                    return "Computer Network Technology";
                case "COBY":
                    return "Coby Electronics Corporation, USA";
                case "COGITO":
                    return "Cogito";
                case "COMAY":
                    return "Corerise Electronics";
                case "COMPAQ":
                    return "Compaq Computer Corporation";
                case "COMPELNT":
                    return "Compellent Technologies, Inc.";
                case "COMPORT":
                    return "Comport Corp.";
                case "COMPSIG":
                    return "Computer Signal Corporation";
                case "COMPTEX":
                    return "Comptex Pty Limited";
                case "CONNER":
                    return "Conner Peripherals";
                case "COPANSYS":
                    return "COPAN SYSTEMS INC";
                case "CORAID":
                    return "Coraid, Inc";
                case "CORE":
                    return "Core International, Inc.";
                case "CORERISE":
                    return "Corerise Electronics";
                case "COVOTE":
                    return "Covote GmbH & Co KG";
                case "COWON":
                    return "COWON SYSTEMS, Inc.";
                case "CPL":
                    return "Cross Products Ltd";
                case "CPU TECH":
                    return "CPU Technology, Inc.";
                case "CREO":
                    return "Creo Products Inc.";
                case "CROSFLD":
                    return "Crosfield Electronics";
                case "CROSSRDS":
                    return "Crossroads Systems, Inc.";
                case "crosswlk":
                    return "Crosswalk, Inc.";
                case "CSCOVRTS":
                    return "Cisco - Veritas";
                case "CSM, INC":
                    return "Computer SM, Inc.";
                case "Cunuqui":
                    return "CUNUQUI SLU";
                case "CYBERNET":
                    return "Cybernetics";
                case "Cygnal":
                    return "Dekimo";
                case "CYPRESS":
                    return "Cypress Semiconductor Corp.";
                case "D Bit":
                    return "Digby's Bitpile, Inc. DBA D Bit";
                case "DALSEMI":
                    return "Dallas Semiconductor";
                case "DANEELEC":
                    return "Dane-Elec";
                case "DANGER":
                    return "Danger Inc.";
                case "DAT-MG":
                    return "DAT Manufacturers Group";
                case "Data Com":
                    return "Data Com Information Systems Pty. Ltd.";
                case "DATABOOK":
                    return "Databook, Inc.";
                case "DATACOPY":
                    return "Datacopy Corp.";
                case "DataCore":
                    return "DataCore Software Corporation";
                case "DataG":
                    return "DataGravity";
                case "DATAPT":
                    return "Datapoint Corp.";
                case "DATARAM":
                    return "Dataram Corporation";
                case "DATC":
                    return "Datum Champion Technology Co., Ltd";
                case "DAVIS":
                    return "Daviscomms (S) Pte Ltd";
                case "DCS":
                    return "ShenZhen DCS Group Co.,Ltd";
                case "DDN":
                    return "DataDirect Networks, Inc.";
                case "DDRDRIVE":
                    return "DDRdrive LLC";
                case "DE":
                    return "Dimension Engineering LLC";
                case "DEC":
                    return "Digital Equipment Corporation";
                case "DEI":
                    return "Digital Engineering, Inc.";
                case "DELL":
                    return "Dell, Inc.";
                case "Dell(tm)":
                    return "Dell, Inc";
                case "DELPHI":
                    return "Delphi Data Div. of Sparks Industries, Inc.";
                case "DENON":
                    return "Denon/Nippon Columbia";
                case "DenOptix":
                    return "DenOptix, Inc.";
                case "DEST":
                    return "DEST Corp.";
                case "DFC":
                    return "DavioFranke.com";
                case "DFT":
                    return "Data Fault Tolerance System CO.,LTD.";
                case "DGC":
                    return "Data General Corp.";
                case "DIGIDATA":
                    return "Digi-Data Corporation";
                case "DigiIntl":
                    return "Digi International";
                case "Digital":
                    return "Digital Equipment Corporation";
                case "DILOG":
                    return "Distributed Logic Corp.";
                case "DISC":
                    return "Document Imaging Systems Corp.";
                case "DiscSoft":
                    return "Disc Soft Ltd";
                case "DLNET":
                    return "Driveline";
                case "DNS":
                    return "Data and Network Security";
                case "DNUK":
                    return "Digital Networks Uk Ltd";
                case "DotHill":
                    return "Dot Hill Systems Corp.";
                case "DP":
                    return "Dell, Inc.";
                case "DPT":
                    return "Distributed Processing Technology";
                case "Drewtech":
                    return "Drew Technologies, Inc.";
                case "DROBO":
                    return "Data Robotics, Inc.";
                case "DSC":
                    return "DigitalStream Corporation";
                case "DSI":
                    return "Data Spectrum, Inc.";
                case "DSM":
                    return "Deterner Steuerungs- und Maschinenbau GmbH & Co.";
                case "DSNET":
                    return "Cleversafe, Inc.";
                case "DT":
                    return "Double-Take Software, INC.";
                case "DTC QUME":
                    return "Data Technology Qume";
                case "DXIMAGIN":
                    return "DX Imaging";
                case "E-Motion":
                    return "E-Motion LLC";
                case "EARTHLAB":
                    return "EarthLabs";
                case "EarthLCD":
                    return "Earth Computer Technologies, Inc.";
                case "ECCS":
                    return "ECCS, Inc.";
                case "ECMA":
                    return "European Computer Manufacturers Association";
                case "EDS":
                    return "Embedded Data Systems";
                case "EIM":
                    return "InfoCore";
                case "ELE Intl":
                    return "ELE International";
                case "ELEGANT":
                    return "Elegant Invention, LLC";
                case "Elektron":
                    return "Elektron Music Machines MAV AB";
                case "elipsan":
                    return "Elipsan UK Ltd.";
                case "Elms":
                    return "Elms Systems Corporation";
                case "ELSE":
                    return "ELSE Ltd.";
                case "ELSEC":
                    return "Littlemore Scientific";
                case "EMASS":
                    return "EMASS, Inc.";
                case "EMC":
                    return "EMC Corp.";
                case "EMiT":
                    return "EMiT Conception Eletronique";
                case "EMTEC":
                    return "EMTEC Magnetics";
                case "EMULEX":
                    return "Emulex";
                case "ENERGY-B":
                    return "Energybeam Corporation";
                case "ENGENIO":
                    return "Engenio Information Technologies, Inc.";
                case "ENMOTUS":
                    return "Enmotus Inc";
                case "Entacore":
                    return "Entacore";
                case "EPOS":
                    return "EPOS Technologies Ltd.";
                case "EPSON":
                    return "Epson";
                case "EQLOGIC":
                    return "EqualLogic";
                case "Eris/RSI":
                    return "RSI Systems, Inc.";
                case "ETERNE":
                    return "EterneData Technology Co.,Ltd.(China PRC.)";
                case "EuroLogc":
                    return "Eurologic Systems Limited";
                case "evolve":
                    return "Evolution Technologies, Inc";
                case "EXABYTE":
                    return "Exabyte Corp.";
                case "EXATEL":
                    return "Exatelecom Co., Ltd.";
                case "EXAVIO":
                    return "Exavio, Inc.";
                case "Exsequi":
                    return "Exsequi Ltd";
                case "Exxotest":
                    return "Annecy Electronique";
                case "FAIRHAVN":
                    return "Fairhaven Health, LLC";
                case "FALCON":
                    return "FalconStor, Inc.";
                case "FDS":
                    return "Formation Data Systems";
                case "FFEILTD":
                    return "FujiFilm Electonic Imaging Ltd";
                case "Fibxn":
                    return "Fiberxon, Inc.";
                case "FID":
                    return "First International Digital, Inc.";
                case "FILENET":
                    return "FileNet Corp.";
                case "FirmFact":
                    return "Firmware Factory Ltd";
                case "FLYFISH":
                    return "Flyfish Technologies";
                case "FOXCONN":
                    return "Foxconn Technology Group";
                case "FRAMDRV":
                    return "FRAMEDRIVE Corp.";
                case "FREECION":
                    return "Nable Communications, Inc.";
                case "FRESHDTK":
                    return "FreshDetect GmbH";
                case "FSC":
                    return "Fujitsu Siemens Computers";
                case "FTPL":
                    return "Frontline Technologies Pte Ltd";
                case "FUJI":
                    return "Fuji Electric Co., Ltd. (Japan)";
                case "FUJIFILM":
                    return "Fuji Photo Film, Co., Ltd.";
                case "FUJITSU":
                    return "Fujitsu";
                case "FUNAI":
                    return "Funai Electric Co., Ltd.";
                case "FUSIONIO":
                    return "Fusion-io Inc.";
                case "FUTURED":
                    return "Future Domain Corp.";
                case "G&D":
                    return "Giesecke & Devrient GmbH";
                case "G.TRONIC":
                    return "Globaltronic - Electronica e Telecomunicacoes, S.A.";
                case "Gadzoox":
                    return "Gadzoox Networks, Inc.";
                case "Gammaflx":
                    return "Gammaflux L.P.";
                case "GDI":
                    return "Generic Distribution International";
                case "GEMALTO":
                    return "gemalto";
                case "Gen_Dyn":
                    return "General Dynamics";
                case "Generic":
                    return "Generic Technology Co., Ltd.";
                case "GENSIG":
                    return "General Signal Networks";
                case "GEO":
                    return "Green Energy Options Ltd";
                case "GIGATAPE":
                    return "GIGATAPE GmbH";
                case "GIGATRND":
                    return "GigaTrend Incorporated";
                case "Global":
                    return "Global Memory Test Consortium";
                case "Gnutek":
                    return "Gnutek Ltd.";
                case "Goidelic":
                    return "Goidelic Precision, Inc.";
                case "GoldKey":
                    return "GoldKey Security Corporation";
                case "GoldStar":
                    return "LG Electronics Inc.";
                case "GOOGLE":
                    return "Google, Inc.";
                case "GORDIUS":
                    return "Gordius";
                case "GOULD":
                    return "Gould";
                case "HAGIWARA":
                    return "Hagiwara Sys-Com Co., Ltd.";
                case "HAPP3":
                    return "Inventec Multimedia and Telecom co., ltd";
                case "HDS":
                    return "Horizon Data Systems, Inc.";
                case "Helldyne":
                    return "Helldyne, Inc";
                case "Heydays":
                    return "Mazo Technology Co., Ltd.";
                case "HGST":
                    return "HGST a Western Digital Company";
                case "HI-TECH":
                    return "HI-TECH Software Pty. Ltd.";
                case "HITACHI":
                    return "Hitachi America Ltd or Nissei Sangyo America Ltd";
                case "HL-DT-ST":
                    return "Hitachi-LG Data Storage, Inc.";
                case "HONEYWEL":
                    return "Honeywell Inc.";
                case "Hoptroff":
                    return "HexWax Ltd";
                case "HORIZONT":
                    return "Horizontigo Software";
                case "HP":
                    return "Hewlett Packard";
                case "HPE":
                    return "Hewlett Packard Enterprise";
                case "HPI":
                    return "HP Inc.";
                case "HPQ":
                    return "Hewlett Packard";
                case "HUALU":
                    return "CHINA HUALU GROUP CO., LTD";
                case "HUASY":
                    return "Huawei Symantec Technologies Co., Ltd.";
                case "HYLINX":
                    return "Hylinx Ltd.";
                case "HYUNWON":
                    return "HYUNWON inc";
                case "i-cubed":
                    return "i-cubed ltd.";
                case "IBM":
                    return "International Business Machines";
                case "Icefield":
                    return "Icefield Tools Corporation";
                case "Iceweb":
                    return "Iceweb Storage Corp";
                case "ICL":
                    return "ICL";
                case "ICP":
                    return "ICP vortex Computersysteme GmbH";
                case "IDE":
                    return "International Data Engineering, Inc.";
                case "IDG":
                    return "Interface Design Group";
                case "IET":
                    return "ISCSI ENTERPRISE TARGET";
                case "IFT":
                    return "Infortrend Technology, Inc.";
                case "IGR":
                    return "Intergraph Corp.";
                case "IMAGINE":
                    return "Imagine Communications Corp.";
                case "IMAGO":
                    return "IMAGO SOFTWARE SL";
                case "IMATION":
                    return "Imation";
                case "IMPLTD":
                    return "Integrated Micro Products Ltd.";
                case "IMPRIMIS":
                    return "Imprimis Technology Inc.";
                case "INCIPNT":
                    return "Incipient Technologies Inc.";
                case "INCITS":
                    return "InterNational Committee for Information Technology";
                case "INDCOMP":
                    return "Industrial Computing Limited";
                case "Indigita":
                    return "Indigita Corporation";
                case "INFOCORE":
                    return "InfoCore";
                case "INITIO":
                    return "Initio Corporation";
                case "INRANGE":
                    return "INRANGE Technologies Corporation";
                case "Insight":
                    return "L-3 Insight Technology Inc";
                case "INSITE":
                    return "Insite Peripherals";
                case "integrix":
                    return "Integrix, Inc.";
                case "INTEL":
                    return "Intel Corporation";
                case "Intransa":
                    return "Intransa, Inc.";
                case "IOC":
                    return "I/O Concepts, Inc.";
                case "iofy":
                    return "iofy Corporation";
                case "IOMEGA":
                    return "Iomega";
                case "IOT":
                    return "IO Turbine, Inc.";
                case "iPaper":
                    return "intelliPaper, LLC";
                case "iqstor":
                    return "iQstor Networks, Inc.";
                case "iQue":
                    return "iQue";
                case "ISi":
                    return "Information Storage inc.";
                case "Isilon":
                    return "Isilon Systems, Inc.";
                case "ISO":
                    return "International Standards Organization";
                case "iStor":
                    return "iStor Networks, Inc.";
                case "ITC":
                    return "International Tapetronics Corporation";
                case "iTwin":
                    return "iTwin Pte Ltd";
                case "IVIVITY":
                    return "iVivity, Inc.";
                case "IVMMLTD":
                    return "InnoVISION Multimedia Ltd.";
                case "JABIL001":
                    return "Jabil Circuit";
                case "JETWAY":
                    return "Jetway Information Co., Ltd";
                case "JMR":
                    return "JMR Electronics Inc.";
                case "JOFEMAR":
                    return "Jofemar";
                case "JOLLYLOG":
                    return "Jolly Logic";
                case "JPC Inc.":
                    return "JPC Inc.";
                case "JSCSI":
                    return "jSCSI Project";
                case "Juniper":
                    return "Juniper Networks";
                case "JVC":
                    return "JVC Information Products Co.";
                case "KASHYA":
                    return "Kashya, Inc.";
                case "KENNEDY":
                    return "Kennedy Company";
                case "KENWOOD":
                    return "KENWOOD Corporation";
                case "KEWL":
                    return "Shanghai KEWL Imp&Exp Co., Ltd.";
                case "Key Tech":
                    return "Key Technologies, Inc";
                case "KMNRIO":
                    return "Kaminario Technologies Ltd.";
                case "KODAK":
                    return "Eastman Kodak";
                case "KONAN":
                    return "Konan";
                case "koncepts":
                    return "koncepts International Ltd.";
                case "KONICA":
                    return "Konica Japan";
                case "KOVE":
                    return "KOVE";
                case "KSCOM":
                    return "KSCOM Co. Ltd.,";
                case "KUDELSKI":
                    return "Nagravision SA - Kudelski Group";
                case "Kyocera":
                    return "Kyocera Corporation";
                case "Lapida":
                    return "Gonmalo Electronics";
                case "LAPINE":
                    return "Lapine Technology";
                case "LASERDRV":
                    return "LaserDrive Limited";
                case "LASERGR":
                    return "Lasergraphics, Inc.";
                case "LeapFrog":
                    return "LeapFrog Enterprises, Inc.";
                case "LEFTHAND":
                    return "LeftHand Networks";
                case "Leica":
                    return "Leica Camera AG";
                case "Lexar":
                    return "Lexar Media, Inc.";
                case "LEYIO":
                    return "LEYIO";
                case "LG":
                    return "LG Electronics Inc.";
                case "LGE":
                    return "LG Electronics Inc.";
                case "LIBNOVA":
                    return "LIBNOVA, SL Digital Preservation Systems";
                case "LION":
                    return "Lion Optics Corporation";
                case "LMS":
                    return "Laser Magnetic Storage International Company";
                case "LoupTech":
                    return "Loup Technologies, Inc.";
                case "LSI":
                    return "LSI Corp. (was LSI Logic Corp.)";
                case "LSILOGIC":
                    return "LSI Logic Storage Systems, Inc.";
                case "LTO-CVE":
                    return "Linear Tape - Open, Compliance Verification Entity";
                case "LUXPRO":
                    return "Luxpro Corporation";
                case "MacroSAN":
                    return "MacroSAN Technologies Co., Ltd.";
                case "Malakite":
                    return "Malachite Technologies (New VID is: Sandial)";
                case "MarcBoon":
                    return "marcboon.com";
                case "Marner":
                    return "Marner Storage Technologies, Inc.";
                case "MARVELL":
                    return "Marvell Semiconductor, Inc.";
                case "Matrix":
                    return "Matrix Orbital Corp.";
                case "MATSHITA":
                    return "Matsushita";
                case "MAXELL":
                    return "Hitachi Maxell, Ltd.";
                case "MAXIM-IC":
                    return "Maxim Integrated Products";
                case "MaxOptix":
                    return "Maxoptix Corp.";
                case "MAXSTRAT":
                    return "Maximum Strategy, Inc.";
                case "MAXTOR":
                    return "Maxtor Corp.";
                case "MaXXan":
                    return "MaXXan Systems, Inc.";
                case "MAYCOM":
                    return "maycom Co., Ltd.";
                case "MBEAT":
                    return "K-WON C&C Co.,Ltd";
                case "MCC":
                    return "Measurement Computing Corporation";
                case "McDATA":
                    return "McDATA Corporation";
                case "MCUBE":
                    return "Mcube Technology Co., Ltd.";
                case "MDI":
                    return "Micro Design International, Inc.";
                case "MEADE":
                    return "Meade Instruments Corporation";
                case "mediamat":
                    return "mediamatic";
                case "MegaElec":
                    return "Mega Electronics Ltd";
                case "MEII":
                    return "Mountain Engineering II, Inc.";
                case "MELA":
                    return "Mitsubishi Electronics America";
                case "MELCO":
                    return "Mitsubishi Electric (Japan)";
                case "mellanox":
                    return "Mellanox Technologies Ltd.";
                case "MEMOREX":
                    return "Memorex Telex Japan Ltd.";
                case "MEMREL":
                    return "Memrel Corporation";
                case "MEMTECH":
                    return "MemTech Technology";
                case "Mendocin":
                    return "Mendocino Software";
                case "MendoCno":
                    return "Mendocino Software";
                case "MERIDATA":
                    return "Oy Meridata Finland Ltd";
                case "METHODEI":
                    return "Methode Electronics India pvt ltd";
                case "METRUM":
                    return "Metrum, Inc.";
                case "MHTL":
                    return "Matsunichi Hi-Tech Limited";
                case "MICROBTX":
                    return "Microbotics Inc.";
                case "Microchp":
                    return "Microchip Technology, Inc.";
                case "MICROLIT":
                    return "Microlite Corporation";
                case "MICRON":
                    return "Micron Technology, Inc.";
                case "MICROP":
                    return "Micropolis";
                case "MICROTEK":
                    return "Microtek Storage Corp";
                case "Minitech":
                    return "Minitech (UK) Limited";
                case "Minolta":
                    return "Minolta Corporation";
                case "MINSCRIB":
                    return "Miniscribe";
                case "MiraLink":
                    return "MiraLink Corporation";
                case "Mirifica":
                    return "Mirifica s.r.l.";
                case "MITSUMI":
                    return "Mitsumi Electric Co., Ltd.";
                case "MKM":
                    return "Mitsubishi Kagaku Media Co., LTD.";
                case "Mobii":
                    return "Mobii Systems (Pty.) Ltd.";
                case "MOL":
                    return "Petrosoft Sdn. Bhd.";
                case "MOSAID":
                    return "Mosaid Technologies Inc.";
                case "MOTOROLA":
                    return "Motorola";
                case "MP-400":
                    return "Daiwa Manufacturing Limited";
                case "MPC":
                    return "MPC Corporation";
                case "MPCCORP":
                    return "MPC Computers";
                case "MPEYE":
                    return "Touchstone Technology Co., Ltd";
                case "MPIO":
                    return "DKT Co.,Ltd";
                case "MPM":
                    return "Mitsubishi Paper Mills, Ltd.";
                case "MPMan":
                    return "MPMan.com, Inc.";
                case "MSFT":
                    return "Microsoft Corporation";
                case "MSI":
                    return "Micro-Star International Corp.";
                case "MST":
                    return "Morning Star Technologies, Inc.";
                case "MSystems":
                    return "M-Systems Flash Disk Pioneers";
                case "MTI":
                    return "MTI Technology Corporation";
                case "MTNGATE":
                    return "MountainGate Data Systems";
                case "MXI":
                    return "Memory Experts International";
                case "nac":
                    return "nac Image Technology Inc.";
                case "NAGRA":
                    return "Nagravision SA - Kudelski Group";
                case "NAI":
                    return "North Atlantic Industries";
                case "NAKAMICH":
                    return "Nakamichi Corporation";
                case "NatInst":
                    return "National Instruments";
                case "NatSemi":
                    return "National Semiconductor Corp.";
                case "NCITS":
                    return "InterNational Committee for Information Technology Standards (INCITS)";
                case "NCL":
                    return "NCL America";
                case "NCR":
                    return "NCR Corporation";
                case "NDBTECH":
                    return "NDB Technologie Inc.";
                case "Neartek":
                    return "Neartek, Inc.";
                case "NEC":
                    return "NEC";
                case "NETAPP":
                    return "NetApp, Inc. (was Network Appliance)";
                case "NetBSD":
                    return "The NetBSD Foundation";
                case "Netcom":
                    return "Netcom Storage";
                case "NETENGIN":
                    return "NetEngine, Inc.";
                case "NEWISYS":
                    return "Newisys Data Storage";
                case "Newtech":
                    return "Newtech Co., Ltd.";
                case "NEXSAN":
                    return "Nexsan Technologies, Ltd.";
                case "NFINIDAT":
                    return "Infinidat Ltd.";
                case "NHR":
                    return "NH Research, Inc.";
                case "Nike":
                    return "Nike, Inc.";
                case "Nimble":
                    return "Nimble Storage";
                case "NISCA":
                    return "NISCA Inc.";
                case "NISHAN":
                    return "Nishan Systems Inc.";
                case "Nitz":
                    return "Nitz Associates, Inc.";
                case "NKK":
                    return "NKK Corp.";
                case "NRC":
                    return "Nakamichi Research Corporation";
                case "NSD":
                    return "Nippon Systems Development Co.,Ltd.";
                case "NSM":
                    return "NSM Jukebox GmbH";
                case "nStor":
                    return "nStor Technologies, Inc.";
                case "NT":
                    return "Northern Telecom";
                case "NUCONNEX":
                    return "NuConnex";
                case "NUSPEED":
                    return "NuSpeed, Inc.";
                case "NVIDIA":
                    return "NVIDIA Corporation";
                case "NVMe":
                    return "NVM Express Working Group";
                case "OAI":
                    return "Optical Access International";
                case "OCE":
                    return "Oce Graphics";
                case "ODS":
                    return "ShenZhen DCS Group Co.,Ltd";
                case "OHDEN":
                    return "Ohden Co., Ltd.";
                case "OKI":
                    return "OKI Electric Industry Co.,Ltd (Japan)";
                case "Olidata":
                    return "Olidata S.p.A.";
                case "OMI":
                    return "Optical Media International";
                case "OMNIFI":
                    return "Rockford Corporation - Omnifi Media";
                case "OMNIS":
                    return "OMNIS Company (FRANCE)";
                case "Ophidian":
                    return "Ophidian Designs";
                case "opslag":
                    return "Tyrone Systems";
                case "Optelec":
                    return "Optelec BV";
                case "Optiarc":
                    return "Sony Optiarc Inc.";
                case "OPTIMEM":
                    return "Cipher/Optimem";
                case "OPTOTECH":
                    return "Optotech";
                case "ORACLE":
                    return "Oracle Corporation";
                case "ORANGE":
                    return "Orange Micro, Inc.";
                case "ORCA":
                    return "Orca Technology";
                case "Origin":
                    return "Origin Energy";
                case "OSI":
                    return "Optical Storage International";
                case "OSNEXUS":
                    return "OS NEXUS, Inc.";
                case "OTL":
                    return "OTL Engineering";
                case "OVERLAND":
                    return "Overland Storage Inc.";
                case "pacdigit":
                    return "Pacific Digital Corp";
                case "Packard":
                    return "Parkard Bell";
                case "Panasas":
                    return "Panasas, Inc.";
                case "PARALAN":
                    return "Paralan Corporation";
                case "PASCOsci":
                    return "Pasco Scientific";
                case "PATHLGHT":
                    return "Pathlight Technology, Inc.";
                case "PCS":
                    return "Pro Charging Systems, LLC";
                case "PerStor":
                    return "Perstor";
                case "PERTEC":
                    return "Pertec Peripherals Corporation";
                case "PFTI":
                    return "Performance Technology Inc.";
                case "PFU":
                    return "PFU Limited";
                case "Phigment":
                    return "Phigment Technologies";
                case "PHILIPS":
                    return "Philips Electronics";
                case "PICO":
                    return "Packard Instrument Company";
                case "PIK":
                    return "TECHNILIENT & MCS";
                case "Pillar":
                    return "Pillar Data Systems";
                case "PIONEER":
                    return "Pioneer Electronic Corp.";
                case "Pirus":
                    return "Pirus Networks";
                case "PIVOT3":
                    return "Pivot3, Inc.";
                case "PLASMON":
                    return "Plasmon Data";
                case "Pliant":
                    return "Pliant Technology, Inc.";
                case "PMCSIERA":
                    return "PMC-Sierra";
                case "PME":
                    return "Precision Measurement Engineering";
                case "PNNMed":
                    return "PNN Medical SA";
                case "POKEN":
                    return "Poken SA";
                case "POLYTRON":
                    return "PT. HARTONO ISTANA TEKNOLOGI";
                case "PRAIRIE":
                    return "PrairieTek";
                case "PREPRESS":
                    return "PrePRESS Solutions";
                case "PRESOFT":
                    return "PreSoft Architects";
                case "PRESTON":
                    return "Preston Scientific";
                case "PRIAM":
                    return "Priam";
                case "PRIMAGFX":
                    return "Primagraphics Ltd";
                case "PRIMOS":
                    return "Primos";
                case "PROCOM":
                    return "Procom Technology";
                case "PROLIFIC":
                    return "Prolific Technology Inc.";
                case "PROMISE":
                    return "PROMISE TECHNOLOGY, Inc";
                case "PROSTOR":
                    return "ProStor Systems, Inc.";
                case "PROSUM":
                    return "PROSUM";
                case "PROWARE":
                    return "Proware Technology Corp.";
                case "PTI":
                    return "Peripheral Technology Inc.";
                case "PTICO":
                    return "Pacific Technology International";
                case "PURE":
                    return "PURE Storage";
                case "Qi-Hardw":
                    return "Qi Hardware";
                case "QIC":
                    return "Quarter-Inch Cartridge Drive Standards, Inc.";
                case "QLogic":
                    return "QLogic Corporation";
                case "QNAP":
                    return "QNAP Systems";
                case "Qsan":
                    return "QSAN Technology, Inc.";
                case "QUALSTAR":
                    return "Qualstar";
                case "QUANTEL":
                    return "Quantel Ltd.";
                case "QUANTUM":
                    return "Quantum Corp.";
                case "QUIX":
                    return "Quix Computerware AG";
                case "R-BYTE":
                    return "R-Byte, Inc.";
                case "RACALREC":
                    return "Racal Recorders";
                case "RADITEC":
                    return "Radikal Technologies Deutschland GmbH";
                case "RADSTONE":
                    return "Radstone Technology";
                case "RAIDINC":
                    return "RAID Inc.";
                case "RASSYS":
                    return "Rasilient Systems Inc.";
                case "RASVIA":
                    return "Rasvia Systems, Inc.";
                case "rave-mp":
                    return "Go Video";
                case "RDKMSTG":
                    return "MMS Dipl. Ing. Rolf-Dieter Klein";
                case "RDStor":
                    return "Rorke China";
                case "Readboy":
                    return "Readboy Ltd Co.";
                case "Realm":
                    return "Realm Systems";
                case "realtek":
                    return "Realtek Semiconductor Corp.";
                case "REDUXIO":
                    return "Reduxio Systems Ltd.";
                case "rehanltd":
                    return "Rehan Electronics Ltd";
                case "REKA":
                    return "REKA HEALTH PTE LTD";
                case "RELDATA":
                    return "RELDATA Inc";
                case "RENAGmbH":
                    return "RENA GmbH";
                case "ReThinkM":
                    return "RETHINK MEDICAL, INC";
                case "Revivio":
                    return "Revivio, Inc.";
                case "RGBLaser":
                    return "RGB Lasersysteme GmbH";
                case "RGI":
                    return "Raster Graphics, Inc.";
                case "RHAPSODY":
                    return "Rhapsody Networks, Inc.";
                case "RHS":
                    return "Racal-Heim Systems GmbH";
                case "RICOH":
                    return "Ricoh";
                case "RODIME":
                    return "Rodime";
                case "Rorke":
                    return "RD DATA Technology (ShenZhen) Limited";
                case "Royaltek":
                    return "RoyalTek company Ltd.";
                case "RPS":
                    return "RPS";
                case "RTI":
                    return "Reference Technology";
                case "S-D":
                    return "Sauer-Danfoss";
                case "S-flex":
                    return "Storageflex Inc";
                case "S-SYSTEM":
                    return "S-SYSTEM";
                case "S1":
                    return "storONE";
                case "SAMSUNG":
                    return "Samsung Electronics Co., Ltd.";
                case "SAN":
                    return "Storage Area Networks, Ltd.";
                case "Sandial":
                    return "Sandial Systems, Inc.";
                case "SanDisk":
                    return "SanDisk Corporation";
                case "SANKYO":
                    return "Sankyo Seiki";
                case "SANRAD":
                    return "SANRAD Inc.";
                case "SANYO":
                    return "SANYO Electric Co., Ltd.";
                case "SC.Net":
                    return "StorageConnections.Net";
                case "SCALE":
                    return "Scale Computing, Inc.";
                case "SCIENTEK":
                    return "SCIENTEK CORP";
                case "SCInc.":
                    return "Storage Concepts, Inc.";
                case "SCREEN":
                    return "Dainippon Screen Mfg. Co., Ltd.";
                case "SDI":
                    return "Storage Dimensions, Inc.";
                case "SDS":
                    return "Solid Data Systems";
                case "SEAC":
                    return "SeaChange International, Inc.";
                case "SEAGATE":
                    return "Seagate";
                case "SEAGRAND":
                    return "SEAGRAND In Japan";
                case "Seanodes":
                    return "Seanodes";
                case "Sec. Key":
                    return "SecureKey Technologies Inc.";
                case "SEQUOIA":
                    return "Sequoia Advanced Technologies, Inc.";
                case "SGI":
                    return "Silicon Graphics International";
                case "Shannon":
                    return "Shannon Systems Co., Ltd.";
                case "Shinko":
                    return "Shinko Electric Co., Ltd.";
                case "SIEMENS":
                    return "Siemens";
                case "SigmaTel":
                    return "SigmaTel, Inc.";
                case "SII":
                    return "Seiko Instruments Inc.";
                case "SIMPLE":
                    return "SimpleTech, Inc.";
                case "SIVMSD":
                    return "IMAGO SOFTWARE SL";
                case "SKhynix":
                    return "SK hynix Inc.";
                case "SLCNSTOR":
                    return "SiliconStor, Inc.";
                case "SLI":
                    return "Sierra Logic, Inc.";
                case "SMCI":
                    return "Super Micro Computer, Inc.";
                case "SmrtStor":
                    return "Smart Storage Systems";
                case "SMS":
                    return "Scientific Micro Systems/OMTI";
                case "SMSC":
                    return "SMSC Storage, Inc.";
                case "SMX":
                    return "Smartronix, Inc.";
                case "SNYSIDE":
                    return "Sunnyside Computing Inc.";
                case "SoftLock":
                    return "Softlock Digital Security Provider";
                case "SolidFir":
                    return "SolidFire, Inc.";
                case "SONIC":
                    return "Sonic Solutions";
                case "SoniqCas":
                    return "SoniqCast";
                case "SONY":
                    return "Sony Corporation Japan";
                case "SOUL":
                    return "Soul Storage Technology (Wuxi) Co., Ltd";
                case "SPD":
                    return "Storage Products Distribution, Inc.";
                case "SPECIAL":
                    return "Special Computing Co.";
                case "SPECTRA":
                    return "Spectra Logic, a Division of Western Automation Labs, Inc.";
                case "SPERRY":
                    return "Sperry";
                case "Spintso":
                    return "Spintso International AB";
                case "STARBORD":
                    return "Starboard Storage Systems, Inc.";
                case "STARWIND":
                    return "StarWind Software, Inc.";
                case "STEC":
                    return "STEC, Inc.";
                case "Sterling":
                    return "Sterling Diagnostic Imaging, Inc.";
                case "STK":
                    return "Storage Technology Corporation";
                case "STNWOOD":
                    return "Stonewood Group";
                case "STONEFLY":
                    return "StoneFly Networks, Inc.";
                case "STOR":
                    return "StorageNetworks, Inc.";
                case "STORAPP":
                    return "StorageApps, Inc.";
                case "STORCIUM":
                    return "Intelligent Systems Services Inc.";
                case "STORCOMP":
                    return "Storage Computer Corporation";
                case "STORM":
                    return "Storm Technology, Inc.";
                case "StorMagc":
                    return "StorMagic";
                case "Stratus":
                    return "Stratus Technologies";
                case "StrmLgc":
                    return "StreamLogic Corp.";
                case "SUMITOMO":
                    return "Sumitomo Electric Industries, Ltd.";
                case "SUN":
                    return "Sun Microsystems, Inc.";
                case "SUNCORP":
                    return "SunCorporation";
                case "suntx":
                    return "Suntx System Co., Ltd";
                case "SUSE":
                    return "SUSE Linux";
                case "Swinxs":
                    return "Swinxs BV";
                case "SYMANTEC":
                    return "Symantec Corporation";
                case "SYMBIOS":
                    return "Symbios Logic Inc.";
                case "SYMWAVE":
                    return "Symwave, Inc.";
                case "SYNCSORT":
                    return "Syncsort Incorporated";
                case "SYNERWAY":
                    return "Synerway";
                case "SYNOLOGY":
                    return "Synology, Inc.";
                case "SyQuest":
                    return "SyQuest Technology, Inc.";
                case "SYSGEN":
                    return "Sysgen";
                case "T-MITTON":
                    return "Transmitton England";
                case "T-MOBILE":
                    return "T-Mobile USA, Inc.";
                case "T11":
                    return "INCITS Technical Committee T11";
                case "TALARIS":
                    return "Talaris Systems, Inc.";
                case "TALLGRAS":
                    return "Tallgrass Technologies";
                case "TANDBERG":
                    return "Tandberg Data A/S";
                case "TANDEM":
                    return "Tandem";
                case "TANDON":
                    return "Tandon";
                case "TCL":
                    return "TCL Shenzhen ASIC MIcro-electronics Ltd";
                case "TDK":
                    return "TDK Corporation";
                case "TEAC":
                    return "TEAC Japan";
                case "TECOLOTE":
                    return "Tecolote Designs";
                case "TEGRA":
                    return "Tegra Varityper";
                case "Teilch":
                    return "Teilch";
                case "Tek":
                    return "Tektronix";
                case "TELLERT":
                    return "Tellert Elektronik GmbH";
                case "TENTIME":
                    return "Laura Technologies, Inc.";
                case "TFDATACO":
                    return "TimeForge";
                case "TGEGROUP":
                    return "TGE Group Co.,LTD.";
                case "Thecus":
                    return "Thecus Technology Corp.";
                case "TI-DSG":
                    return "Texas Instruments";
                case "TiGi":
                    return "TiGi Corporation";
                case "TILDESGN":
                    return "Tildesign bv";
                case "Tite":
                    return "Tite Technology Limited";
                case "TKS Inc.":
                    return "TimeKeeping Systems, Inc.";
                case "TLMKS":
                    return "Telemakus LLC";
                case "TMS":
                    return "Texas Memory Systems, Inc.";
                case "TMS100":
                    return "TechnoVas";
                case "TOLISGRP":
                    return "The TOLIS Group";
                case "TOSHIBA":
                    return "Toshiba Japan";
                case "TOYOU":
                    return "TOYOU FEIJI ELECTRONICS CO.,LTD.";
                case "Tracker":
                    return "Tracker, LLC";
                case "TRIOFLEX":
                    return "Trioflex Oy";
                case "TRIPACE":
                    return "Tripace";
                case "TRLogger":
                    return "TrueLogger Ltd.";
                case "TROIKA":
                    return "Troika Networks, Inc.";
                case "TRULY":
                    return "TRULY Electronics MFG. LTD.";
                case "TRUSTED":
                    return "Trusted Data Corporation";
                case "TSSTcorp":
                    return "Toshiba Samsung Storage Technology Corporation";
                case "TZM":
                    return "TZ Medical";
                case "UD-DVR":
                    return "Bigstone Project.";
                case "UDIGITAL":
                    return "United Digital Limited";
                case "UIT":
                    return "United Infomation Technology";
                case "ULTRA":
                    return "UltraStor Corporation";
                case "UNISTOR":
                    return "Unistor Networks, Inc.";
                case "UNISYS":
                    return "Unisys";
                case "USCORE":
                    return "Underscore, Inc.";
                case "USDC":
                    return "US Design Corp.";
                case "Top VASCO":
                    return "Vasco Data Security";
                case "VDS":
                    return "Victor Data Systems Co., Ltd.";
                case "VELDANA":
                    return "VELDANA MEDICAL SA";
                case "VENTANA":
                    return "Ventana Medical Systems";
                case "Verari":
                    return "Verari Systems, Inc.";
                case "VERBATIM":
                    return "Verbatim Corporation";
                case "Vercet":
                    return "Vercet LLC";
                case "VERITAS":
                    return "VERITAS Software Corporation";
                case "Vexata":
                    return "Vexata Inc";
                case "VEXCEL":
                    return "VEXCEL IMAGING GmbH";
                case "VICOMSL1":
                    return "Vicom Systems, Inc.";
                case "VicomSys":
                    return "Vicom Systems, Inc.";
                case "VIDEXINC":
                    return "Videx, Inc.";
                case "VIOLIN":
                    return "Violin Memory, Inc.";
                case "VIRIDENT":
                    return "Virident Systems, Inc.";
                case "VITESSE":
                    return "Vitesse Semiconductor Corporation";
                case "VIXEL":
                    return "Vixel Corporation";
                case "VLS":
                    return "Van Lent Systems BV";
                case "VMAX":
                    return "VMAX Technologies Corp.";
                case "VMware":
                    return "VMware Inc.";
                case "Vobis":
                    return "Vobis Microcomputer AG";
                case "VOLTAIRE":
                    return "Voltaire Ltd.";
                case "VRC":
                    return "Vermont Research Corp.";
                case "VRugged":
                    return "Vanguard Rugged Storage";
                case "VTGadget":
                    return "Vermont Gadget Company";
                case "Waitec":
                    return "Waitec NV";
                case "WangDAT":
                    return "WangDAT";
                case "WANGTEK":
                    return "Wangtek";
                case "Wasabi":
                    return "Wasabi Systems";
                case "WAVECOM":
                    return "Wavecom";
                case "WD":
                    return "Western Digital Corporation";
                case "WDC":
                    return "Western Digital Corporation";
                case "WDIGTL":
                    return "Western Digital";
                case "WDTI":
                    return "Western Digital Technologies, Inc.";
                case "WEARNES":
                    return "Wearnes Technology Corporation";
                case "WeeraRes":
                    return "Weera Research Pte Ltd";
                case "Wildflwr":
                    return "Wildflower Technologies, Inc.";
                case "WSC0001":
                    return "Wisecom, Inc.";
                case "X3":
                    return "InterNational Committee for Information Technology Standards (INCITS)";
                case "XEBEC":
                    return "Xebec Corporation";
                case "XENSRC":
                    return "XenSource, Inc.";
                case "Xerox":
                    return "Xerox Corporation";
                case "XIOtech":
                    return "XIOtech Corporation";
                case "XIRANET":
                    return "Xiranet Communications GmbH";
                case "XIV":
                    return "XIV";
                case "XtremIO":
                    return "XtremIO";
                case "XYRATEX":
                    return "Xyratex";
                case "YINHE":
                    return "NUDT Computer Co.";
                case "YIXUN":
                    return "Yixun Electronic Co.,Ltd.";
                case "YOTTA":
                    return "YottaYotta, Inc.";
                case "Zarva":
                    return "Zarva Digital Technology Co., Ltd.";
                case "ZETTA":
                    return "Zetta Systems, Inc.";
                case "ZTE":
                    return "ZTE Corporation";
                case "ZVAULT":
                    return "Zetavault";
                default:
                    return SCSIVendorString;
            }
        }
    }
}

