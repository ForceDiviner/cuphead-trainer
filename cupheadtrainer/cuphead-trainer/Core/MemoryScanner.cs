using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CupheadTrainer.Core
{
    /// <summary>
    /// Utility class for scanning memory regions to find dynamic addresses.
    /// Useful for finding offsets when game updates change locations.
    /// </summary>
    public class MemoryScanner
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint ProcessAllAccess = 0x1F0FFF;

        /// <summary>
        /// Scans memory for a specific 4-byte integer value within a given range.
        /// </summary>
        /// <param name="processName">Name of the target process.</param>
        /// <param name="startAddress">Start of memory region to scan.</param>
        /// <param name="endAddress">End of memory region to scan.</param>
        /// <param name="value">Integer value to search for.</param>
        /// <returns>List of addresses where value was found.</returns>
        public List<IntPtr> ScanForInt32(string processName, IntPtr startAddress, IntPtr endAddress, int value)
        {
            var results = new List<IntPtr>();
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return results;

            IntPtr hProcess = OpenProcess(ProcessAllAccess, false, processes[0].Id);
            if (hProcess == IntPtr.Zero)
                return results;

            byte[] buffer = new byte[4];
            long current = (long)startAddress;
            long end = (long)endAddress;

            while (current < end)
            {
                if (ReadProcessMemory(hProcess, (IntPtr)current, buffer, 4, out int bytesRead) && bytesRead == 4)
                {
                    int readValue = BitConverter.ToInt32(buffer, 0);
                    if (readValue == value)
                    {
                        results.Add((IntPtr)current);
                    }
                }
                current += 4;
            }

            CloseHandle(hProcess);
            return results;
        }

        /// <summary>
        /// Scans memory for a specific float value within a given range.
        /// </summary>
        /// <param name="processName">Name of the target process.</param>
        /// <param name="startAddress">Start of memory region to scan.</param>
        /// <param name="endAddress">End of memory region to scan.</param>
        /// <param name="value">Float value to search for.</param>
        /// <returns>List of addresses where value was found.</returns>
        public List<IntPtr> ScanForFloat(string processName, IntPtr startAddress, IntPtr endAddress, float value)
        {
            var results = new List<IntPtr>();
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return results;

            IntPtr hProcess = OpenProcess(ProcessAllAccess, false, processes[0].Id);
            if (hProcess == IntPtr.Zero)
                return results;

            byte[] buffer = new byte[4];
            long current = (long)startAddress;
            long end = (long)endAddress;

            while (current < end)
            {
                if (ReadProcessMemory(hProcess, (IntPtr)current, buffer, 4, out int bytesRead) && bytesRead == 4)
                {
                    float readValue = BitConverter.ToSingle(buffer, 0);
                    if (Math.Abs(readValue - value) < 0.001f)
                    {
                        results.Add((IntPtr)current);
                    }
                }
                current += 4;
            }

            CloseHandle(hProcess);
            return results;
        }
    }
}
