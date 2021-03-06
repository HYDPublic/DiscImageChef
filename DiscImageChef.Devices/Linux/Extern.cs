// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Extern.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Linux direct device access.
//
// --[ Description ] ----------------------------------------------------------
//
//     Contains the P/Invoke definitions of Linux syscalls used to directly
//     interface devices.
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

using System.Runtime.InteropServices;

namespace DiscImageChef.Devices.Linux
{
    static class Extern
    {
        [DllImport("libc", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int open(
            string pathname,
            [MarshalAs(UnmanagedType.U4)]
            FileFlags flags);

        [DllImport("libc")]
        internal static extern int close(int fd);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int ioctlInt(int fd, LinuxIoctl request, out int value);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int ioctlSg(int fd, LinuxIoctl request, ref sg_io_hdr_t value);

        [DllImport("libc", EntryPoint = "ioctl", SetLastError = true)]
        internal static extern int ioctlMmc(int fd, LinuxIoctl request, ref mmc_ioc_cmd value);

        [DllImport("libc", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern int readlink(string path, System.IntPtr buf, int bufsize);

        [DllImport("libc", CharSet = CharSet.Ansi, EntryPoint = "readlink", SetLastError = true)]
        internal static extern long readlink64(string path, System.IntPtr buf, long bufsize);
    }
}

