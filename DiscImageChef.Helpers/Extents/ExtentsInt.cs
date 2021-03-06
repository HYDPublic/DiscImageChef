﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : ExtentsInt.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Component
//
// --[ Description ] ----------------------------------------------------------
//
//     Description
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
//     License aint with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2017 Natalia Portillo
// ****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Extents
{
    public class ExtentsInt
    {
        List<Tuple<int, int>> backend;

        public ExtentsInt()
        {
            backend = new List<Tuple<int, int>>();
        }

        public ExtentsInt(List<Tuple<int, int>> list)
        {
            backend = list.OrderBy(t => t.Item1).ToList();
        }

        public int Count { get { return backend.Count; } }

        public void Add(int item)
        {
            Tuple<int, int> removeOne = null;
            Tuple<int, int> removeTwo = null;
            Tuple<int, int> itemToAdd = null;

            for(int i = 0; i < backend.Count; i++)
            {
                // Already contained in an extent
                if(item >= backend[i].Item1 && item <= backend[i].Item2)
                    return;

                // Expands existing extent start
                if(item == backend[i].Item1 - 1)
                {
                    removeOne = backend[i];

                    if(i > 0 && item == backend[i - 1].Item2 + 1)
                    {
                        removeTwo = backend[i - 1];
                        itemToAdd = new Tuple<int, int>(backend[i - 1].Item1, backend[i].Item2);
                    }
                    else
                        itemToAdd = new Tuple<int, int>(item, backend[i].Item2);

                    break;
                }

                // Expands existing extent end
                if(item == backend[i].Item2 + 1)
                {
                    removeOne = backend[i];

                    if(i < backend.Count - 1 && item == backend[i + 1].Item1 - 1)
                    {
                        removeTwo = backend[i + 1];
                        itemToAdd = new Tuple<int, int>(backend[i].Item1, backend[i + 1].Item2);
                    }
                    else
                        itemToAdd = new Tuple<int, int>(backend[i].Item1, item);

                    break;
                }
            }

            if(itemToAdd != null)
            {
                backend.Remove(removeOne);
                backend.Remove(removeTwo);
                backend.Add(itemToAdd);
            }
            else
                backend.Add(new Tuple<int, int>(item, item));

            // Sort
            backend = backend.OrderBy(t => t.Item1).ToList();
        }

        public void Add(int start, int end)
        {
            Add(start, end, false);
        }

        public void Add(int start, int end, bool run)
        {
            int realEnd;
            if(run)
                realEnd = start + end - 1;
            else
                realEnd = end;

            // TODO: Optimize this
            for(int t = start; t <= realEnd; t++)
                Add(t);
        }

        public bool Contains(int item)
        {
            foreach(Tuple<int, int> extent in backend)
                if(item >= extent.Item1 && item <= extent.Item2)
                    return true;
            return false;
        }

        public void Clear()
        {
            backend.Clear();
        }

        public bool Remove(int item)
        {
            Tuple<int, int> toRemove = null;
            Tuple<int, int> toAddOne = null;
            Tuple<int, int> toAddTwo = null;

            foreach(Tuple<int, int> extent in backend)
            {
                // Extent is contained and not a border
                if(item > extent.Item1 && item < extent.Item2)
                {
                    toRemove = extent;
                    toAddOne = new Tuple<int, int>(extent.Item1, item - 1);
                    toAddTwo = new Tuple<int, int>(item + 1, extent.Item2);
                    break;
                }

                // Extent is left border, but not only element
                if(item == extent.Item1 && item != extent.Item2)
                {
                    toRemove = extent;
                    toAddOne = new Tuple<int, int>(item + 1, extent.Item2);
                    break;
                }

                // Extent is right border, but not only element
                if(item != extent.Item1 && item == extent.Item2)
                {
                    toRemove = extent;
                    toAddOne = new Tuple<int, int>(extent.Item1, item - 1);
                    break;
                }

                // Extent is only element
                if(item == extent.Item1 && item == extent.Item2)
                {
                    toRemove = extent;
                    break;
                }
            }

            // Item not found
            if(toRemove == null)
                return false;

            backend.Remove(toRemove);
            if(toAddOne != null)
                backend.Add(toAddOne);
            if(toAddTwo != null)
                backend.Add(toAddTwo);

            // Sort
            backend = backend.OrderBy(t => t.Item1).ToList();

            return true;
        }

        public Tuple<int, int>[] ToArray()
        {
            return backend.ToArray();
        }
    }
}
