﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Futech.Video
{
    public unsafe class memory
    {
        // Handle for the process heap. This handle is used in all calls to the HeapXXX APIs in the methods below.
        static int ph = GetProcessHeap();

        // Private instance constructor to prevent instantiation.
        private memory() { }

        // Allocates a memory block of the given size. The allocated memory is automatically initialized to zero.
        public static void* Alloc(int size)
        {
            void* result = HeapAlloc(ph, HEAP_ZERO_MEMORY, size);

            if (result == null) throw new OutOfMemoryException();

            return result;
        }

        // Copies count bytes from src to dst. The source and destination blocks are premitted to overlpap.
        //
        public static void Copy(void* src, void* dst, int count)
        {
            byte* ps = (byte*)src;
            byte* pd = (byte*)dst;

            if (ps > pd)
            {
                for (; count != 0; count--) *pd++ = *ps++;
            }
            else if (ps < pd)
            {
                for (ps += count, pd += count; count != 0; count--) *--pd = *--ps;
            }
        }

        // Frees a memory block.
        //
        public static void free(void* block)
        {
            //if (!HeapFree(ph, 0, block)) throw new InvalidOperationException();
            try
            {
                HeapFree(ph, 0, block);//) throw new InvalidOperationException();
            }
            catch
            { }
        }

        // Re-allocated a memory block. If the reallocation request is for a larger size, the additional region of memory 
        // is automatically initialized to zero.
        public static void* ReAlloc(void* block, int size)
        {
            void* result = HeapReAlloc(ph, HEAP_ZERO_MEMORY, block, size);

            if (result == null) throw new OutOfMemoryException();

            return result;
        }

        // Heap API flags
        const int HEAP_ZERO_MEMORY = 0x00000008;

        // Heap API functions
        [DllImport("kernel32")]

        static extern int GetProcessHeap();
        [DllImport("kernel32")]

        static extern void* HeapAlloc(int hHeap, int flags, int size);
        [DllImport("kernel32")]

        static extern bool HeapFree(int hHeap, int flags, void* block);
        [DllImport("kernel32")]

        static extern void* HeapReAlloc(int hHeap, int flags, void* block, int size);

        [DllImport("kernel32")]
        static extern int HeapSize(int hHeap, int flags, void* block);
    }
}
