using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WireShark
{
    internal unsafe class QuickLinkedList<T>
    {
        public readonly T* cachePtr = (T*) Marshal.AllocHGlobal(1024 * 1024 * 10 * sizeof(T));
        private int tail = 0;
        
        ~QuickLinkedList()
        {
            Marshal.FreeHGlobal((IntPtr) cachePtr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLast(T t)
        {
            cachePtr[tail] = t;
            ++tail;
        }

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return tail; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            tail = 0;
        }
    }
}
