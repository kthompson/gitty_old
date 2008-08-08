using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gitty.Util;

namespace Gitty.Lib
{
    class UnpackedObjectCache
    {
        private static WeakReference<Entry> Dead;

        public class Entry
        {
            public byte[] Data { get; private set; }

            public ObjectType Type { get; private set; }


            public Entry(byte[] data, ObjectType type)
            {
                this.Data = data;
                this.Type = type;
            }
        }

        private class Slot
        {
            Slot lruPrev;

            Slot lruNext;

            WindowedFile provider;

            long position;

            WeakReference<Entry> data = Dead;
        }
    }
}
