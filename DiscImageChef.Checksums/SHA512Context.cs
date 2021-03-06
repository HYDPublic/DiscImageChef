// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : SHA512Context.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Checksums.
//
// --[ Description ] ----------------------------------------------------------
//
//     Wraps up .NET SHA512 implementation to a Init(), Update(), Final() context.
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

using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace DiscImageChef.Checksums
{
    /// <summary>
    /// Provides a UNIX similar API to .NET SHA512.
    /// </summary>
    public class SHA512Context
    {
        SHA512 _sha512Provider;

        /// <summary>
        /// Initializes the SHA512 hash provider
        /// </summary>
        public void Init()
        {
            _sha512Provider = SHA512.Create();
        }

        /// <summary>
        /// Updates the hash with data.
        /// </summary>
        /// <param name="data">Data buffer.</param>
        /// <param name="len">Length of buffer to hash.</param>
        public void Update(byte[] data, uint len)
        {
            _sha512Provider.TransformBlock(data, 0, (int)len, data, 0);
        }

        /// <summary>
        /// Updates the hash with data.
        /// </summary>
        /// <param name="data">Data buffer.</param>
        public void Update(byte[] data)
        {
            Update(data, (uint)data.Length);
        }

        /// <summary>
        /// Returns a byte array of the hash value.
        /// </summary>
        public byte[] Final()
        {
            _sha512Provider.TransformFinalBlock(new byte[0], 0, 0);
            return _sha512Provider.Hash;
        }

        /// <summary>
        /// Returns a hexadecimal representation of the hash value.
        /// </summary>
        public string End()
        {
            _sha512Provider.TransformFinalBlock(new byte[0], 0, 0);
            StringBuilder sha512Output = new StringBuilder();

            for(int i = 0; i < _sha512Provider.Hash.Length; i++)
            {
                sha512Output.Append(_sha512Provider.Hash[i].ToString("x2"));
            }

            return sha512Output.ToString();
        }

        /// <summary>
        /// Gets the hash of a file
        /// </summary>
        /// <param name="filename">File path.</param>
        public byte[] File(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            byte[] result = _sha512Provider.ComputeHash(fileStream);
            fileStream.Close();
            return result;
        }

        /// <summary>
        /// Gets the hash of a file in hexadecimal and as a byte array.
        /// </summary>
        /// <param name="filename">File path.</param>
        /// <param name="hash">Byte array of the hash value.</param>
        public string File(string filename, out byte[] hash)
        {
            FileStream fileStream = new FileStream(filename, FileMode.Open);
            hash = _sha512Provider.ComputeHash(fileStream);
            StringBuilder sha512Output = new StringBuilder();

            for(int i = 0; i < hash.Length; i++)
            {
                sha512Output.Append(hash[i].ToString("x2"));
            }

            fileStream.Close();

            return sha512Output.ToString();
        }

        /// <summary>
        /// Gets the hash of the specified data buffer.
        /// </summary>
        /// <param name="data">Data buffer.</param>
        /// <param name="len">Length of the data buffer to hash.</param>
        /// <param name="hash">Byte array of the hash value.</param>
        public string Data(byte[] data, uint len, out byte[] hash)
        {
            hash = _sha512Provider.ComputeHash(data, 0, (int)len);
            StringBuilder sha512Output = new StringBuilder();

            for(int i = 0; i < hash.Length; i++)
            {
                sha512Output.Append(hash[i].ToString("x2"));
            }

            return sha512Output.ToString();
        }

        /// <summary>
        /// Gets the hash of the specified data buffer.
        /// </summary>
        /// <param name="data">Data buffer.</param>
        /// <param name="hash">Byte array of the hash value.</param>
        public string Data(byte[] data, out byte[] hash)
        {
            return Data(data, (uint)data.Length, out hash);
        }
    }
}

