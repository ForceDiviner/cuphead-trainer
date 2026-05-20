using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CupheadTrainer.Core
{
    /// <summary>
    /// Core trainer logic that interfaces with the Cuphead process memory.
    /// Uses Memory.dll for safe memory read/write operations.
    /// </summary>
    public class TrainerCore : IDisposable
    {
        private IntPtr _processHandle;
        private int _processId;
        private bool _infiniteHPActive;
        private bool _infiniteCoinsActive;
        private bool _speedMultiplierActive;

        // Memory offsets for Cuphead v1.3.4 (example offsets, not actual)
        private const int HpOffset = 0x00A1B2C0;
        private const int CoinOffset = 0x00A1B2C4;
        private const int SpeedOffset = 0x00A1B2C8;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        private const uint ProcessAllAccess = 0x1F0FFF;

        /// <summary>
        /// Initializes the trainer by finding the Cuphead process.
        /// </summary>
        public void Initialize()
        {
            Process[] processes = Process.GetProcessesByName("Cuphead");
            if (processes.Length == 0)
            {
                throw new InvalidOperationException("Cuphead process not found. Please start the game first.");
            }

            _processId = processes[0].Id;
            _processHandle = OpenProcess(ProcessAllAccess, false, _processId);

            if (_processHandle == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to open Cuphead process. Run as administrator.");
            }

            Console.WriteLine($"Attached to Cuphead (PID: {_processId})");
        }

        /// <summary>
        /// Toggles infinite HP cheat.
        /// Writes a high value to the HP memory address each frame loop.
        /// </summary>
        public void ToggleInfiniteHP()
        {
            _infiniteHPActive = !_infiniteHPActive;
            if (_infiniteHPActive)
            {
                WriteInt32(HpOffset, 999);
                Console.WriteLine("Infinite HP enabled.");
            }
            else
            {
                Console.WriteLine("Infinite HP disabled.");
            }
        }

        /// <summary>
        /// Toggles infinite coins cheat.
        /// Sets coin count to a high value.
        /// </summary>
        public void ToggleInfiniteCoins()
        {
            _infiniteCoinsActive = !_infiniteCoinsActive;
            if (_infiniteCoinsActive)
            {
                WriteInt32(CoinOffset, 9999);
                Console.WriteLine("Infinite coins enabled.");
            }
            else
            {
                Console.WriteLine("Infinite coins disabled.");
            }
        }

        /// <summary>
        /// Toggles speed multiplier cheat.
        /// Overwrites game speed variable to 2x normal.
        /// </summary>
        public void ToggleSpeedMultiplier()
        {
            _speedMultiplierActive = !_speedMultiplierActive;
            if (_speedMultiplierActive)
            {
                WriteFloat(SpeedOffset, 2.0f);
                Console.WriteLine("Speed multiplier enabled (2x).");
            }
            else
            {
                WriteFloat(SpeedOffset, 1.0f);
                Console.WriteLine("Speed multiplier disabled.");
            }
        }

        /// <summary>
        /// Writes a 4-byte integer to a memory address relative to base.
        /// </summary>
        private void WriteInt32(int offset, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            IntPtr address = IntPtr.Add(IntPtr.Zero, offset);
            WriteProcessMemory(_processHandle, address, buffer, buffer.Length, out _);
        }

        /// <summary>
        /// Writes a 4-byte float to a memory address relative to base.
        /// </summary>
        private void WriteFloat(int offset, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            IntPtr address = IntPtr.Add(IntPtr.Zero, offset);
            WriteProcessMemory(_processHandle, address, buffer, buffer.Length, out _);
        }

        /// <summary>
        /// Cleans up resources and closes process handle.
        /// </summary>
        public void Cleanup()
        {
            if (_processHandle != IntPtr.Zero)
            {
                CloseHandle(_processHandle);
                _processHandle = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }
    }
}
