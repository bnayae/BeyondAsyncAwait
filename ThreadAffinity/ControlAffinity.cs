#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Permissions;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

#endregion // Using

namespace ThreadAffinity
{
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
    public class ControlAffinity
    {
        #region Win 32

        [DllImport("Kernel32.dll")]
        [SuppressUnmanagedCodeSecurity]
        public static extern int GetCurrentProcessorNumber();

        [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
        public static extern uint NtGetCurrentProcessorNumber();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        private static extern IntPtr SetThreadAffinityMask(IntPtr hThread, IntPtr dwThreadAffinityMask);

        #endregion // Win 32

        #region Run

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void Run()
        {
            var hash = new HashSet<int>();

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < 100)
            {
                int procNum = GetCurrentProcessorNumber();
                Thread.Sleep(1);
                if (!hash.Contains(procNum))
                    hash.Add(procNum);
            }
            sw.Stop();
            var info = string.Join(",", hash).PadRight(10);
            Console.WriteLine($"Running on [{info}] cores");
        }

        #endregion // Run

        #region RunWithWin32AffinityMask

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void RunWithWin32AffinityMask()
        {
            int initProcNum = GetCurrentProcessorNumber();
            IntPtr hCurr = GetCurrentThread();
            SetThreadAffinityMask(hCurr, new IntPtr(1 << initProcNum));

            Run(); // run under affinity scope
        }

        #endregion // RunWithWin32AffinityMask

        #region RunWithNetAffinity

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void RunWithNetAffinity()
        {
            // Code that does not have thread affinity goes here.
            try
            {
                Thread.BeginThreadAffinity();

                Run(); // run under affinity scope
            }
            finally
            {
                Thread.EndThreadAffinity();
            }

            // More code that does not have thread affinity.
        }

        #endregion // RunWithNetAffinity

        /*
        * Process Proc = Process.GetCurrentProcess();   
        * long AffinityMask = (long)Proc.ProcessorAffinity;   
        * AffinityMask &= 0x000F; // use only any of the first 4 available processors   
        * Proc.ProcessorAffinity = (IntPtr)AffinityMask;    
        * ProcessThread Thread = Proc.Threads[0];   
        * AffinityMask = 0x0002; // use only the second processor, despite availability   
        * Thread.ProcessorAffinity = (IntPtr)AffinityMask; 
        */
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlThread)]
        public static void SetProcAffinity(params int[] coreMask)
        {
            int coresCount = Environment.ProcessorCount;
            int AffinityMask = 0;
            foreach (var core in coreMask)
            {
                if (coresCount < core)
                    throw new ArgumentException(string.Format("Cannot set core [{0}] on {1} cores machine", core, coresCount));
                AffinityMask |= 1 << core;
            }
            Process Proc = Process.GetCurrentProcess();
            Proc.ProcessorAffinity = (IntPtr)AffinityMask;
        }
    }
}
