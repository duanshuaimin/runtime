// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//------------------------------------------------------------------------------
// <copyright file="ReplicationCursor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 */

namespace System.DirectoryServices.ActiveDirectory
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class ReplicationCursorCollection : ReadOnlyCollectionBase
    {
        private DirectoryServer _server = null;
        internal ReplicationCursorCollection(DirectoryServer server)
        {
            _server = server;
        }

        public ReplicationCursor this[int index]
        {
            get
            {
                return (ReplicationCursor)InnerList[index];
            }
        }

        public bool Contains(ReplicationCursor cursor)
        {
            if (cursor == null)
                throw new ArgumentNullException("cursor");

            return InnerList.Contains(cursor);
        }

        public int IndexOf(ReplicationCursor cursor)
        {
            if (cursor == null)
                throw new ArgumentNullException("cursor");

            return InnerList.IndexOf(cursor);
        }

        public void CopyTo(ReplicationCursor[] values, int index)
        {
            InnerList.CopyTo(values, index);
        }

        private int Add(ReplicationCursor cursor)
        {
            return InnerList.Add(cursor);
        }

        internal void AddHelper(string partition, object cursors, bool advanced, IntPtr info)
        {
            // get the count
            int count = 0;
            if (advanced)
                count = ((DS_REPL_CURSORS_3)cursors).cNumCursors;
            else
                count = ((DS_REPL_CURSORS)cursors).cNumCursors;

            IntPtr addr = (IntPtr)0;

            for (int i = 0; i < count; i++)
            {
                if (advanced)
                {
                    addr = IntPtr.Add(info, Marshal.SizeOf(typeof(int)) * 2 + i * Marshal.SizeOf(typeof(DS_REPL_CURSOR_3)));
                    DS_REPL_CURSOR_3 cursor = new DS_REPL_CURSOR_3();
                    Marshal.PtrToStructure(addr, cursor);

                    ReplicationCursor managedCursor = new ReplicationCursor(_server,
                                                                         partition,
                                                                         cursor.uuidSourceDsaInvocationID,
                                                                         cursor.usnAttributeFilter,
                                                                         cursor.ftimeLastSyncSuccess,
                                                                         cursor.pszSourceDsaDN);
                    Add(managedCursor);
                }
                else
                {
                    addr = IntPtr.Add(info, Marshal.SizeOf(typeof(int)) * 2 + i * Marshal.SizeOf(typeof(DS_REPL_CURSOR)));
                    DS_REPL_CURSOR cursor = new DS_REPL_CURSOR();
                    Marshal.PtrToStructure(addr, cursor);

                    ReplicationCursor managedCursor = new ReplicationCursor(_server,
                                                                         partition,
                                                                         cursor.uuidSourceDsaInvocationID,
                                                                         cursor.usnAttributeFilter);
                    Add(managedCursor);
                }
            }
        }
    }
}
