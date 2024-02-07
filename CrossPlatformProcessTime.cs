using System.Diagnostics;
using System.Runtime.Versioning;
using System.Runtime.InteropServices;

public class CrossPlatformProcessTime
{
  [DllImport("libc", SetLastError = true)]
  private static extern int getrusage(int who, ref RUsage usage);

  [DllImport("libc", SetLastError = true)]
  private static extern int mach_timebase_info(ref MachTimebaseInfo info);

  private const int RUSAGE_INFO_V3 = 3;

  [StructLayout(LayoutKind.Sequential)]
  internal struct rusage_info_v3
  {
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
      internal byte[]          ri_uuid;
      internal ulong          ri_user_time;
      internal ulong          ri_system_time;
      internal ulong          ri_pkg_idle_wkups;
      internal ulong          ri_interrupt_wkups;
      internal ulong          ri_pageins;
      internal ulong          ri_wired_size;
      internal ulong          ri_resident_size;
      internal ulong          ri_phys_footprint;
      internal ulong          ri_proc_start_abstime;
      internal ulong          ri_proc_exit_abstime;
      internal ulong          ri_child_user_time;
      internal ulong          ri_child_system_time;
      internal ulong          ri_child_pkg_idle_wkups;
      internal ulong          ri_child_interrupt_wkups;
      internal ulong          ri_child_pageins;
      internal ulong          ri_child_elapsed_abstime;
      internal ulong          ri_diskio_bytesread;
      internal ulong          ri_diskio_byteswritten;
      internal ulong          ri_cpu_time_qos_default;
      internal ulong          ri_cpu_time_qos_maintenance;
      internal ulong          ri_cpu_time_qos_background;
      internal ulong          ri_cpu_time_qos_utility;
      internal ulong          ri_cpu_time_qos_legacy;
      internal ulong          ri_cpu_time_qos_user_initiated;
      internal ulong          ri_cpu_time_qos_user_interactive;
      internal ulong          ri_billed_system_time;
      internal ulong          ri_serviced_system_time;
  }

  /// <summary>
  /// Gets the rusage information for the process identified by the PID
  /// </summary>
  /// <param name="pid">The process to retrieve the rusage for</param>
  /// <param name="flavor">Specifies the type of struct that is passed in to <paramref>buffer</paramref>. Should be RUSAGE_INFO_V3 to specify a rusage_info_v3 struct.</param>
  /// <param name="buffer">A buffer to be filled with rusage_info data</param>
  /// <returns>Returns 0 on success; on fail, -1 and errno is set with the error code</returns>
  // [LibraryImport(Interop.Libraries.libproc, SetLastError = true)]
  [DllImport("libproc", SetLastError = true)]
  private static extern long proc_pid_rusage(
      int pid,
      int flavor,
      ref rusage_info_v3 buffer);

  private const int RUSAGE_SELF = 0;

  [StructLayout(LayoutKind.Sequential)]
  private struct TimeValue
  {
    public long Seconds;
    public int MicroSeconds;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct RUsage
  {
    public TimeValue UserTime;
    public TimeValue SystemTime;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
    public long[] ru_opaque;
  }

  [StructLayout(LayoutKind.Sequential)]
  private struct MachTimebaseInfo
  {
    public uint Numer;
    public uint Denom;
  }

  public static double GetTotalProcessorTimeMilliseconds()
  {
    if (OperatingSystem.IsMacOS())
    {
      return GetTotalProcessorTimeMillisecondsMacOS();
    }
    else
    {
      return Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
    }
  }

  #if true
  // This uses the same methods that the DotNet runtime interop layer uses
  // This needs the MachTimebaseInfo factor applied to the user and system times
  [SupportedOSPlatform("macos")]
  private static double GetTotalProcessorTimeMillisecondsMacOS()
  {
    var usage = new rusage_info_v3();
    var err = proc_pid_rusage(Environment.ProcessId, RUSAGE_INFO_V3, ref usage);
    if (err != 0)
    {
      Console.WriteLine($"proc_pid_rusage failed: {err}, {System.Runtime.InteropServices.Marshal.GetLastPInvokeError()}");
      throw new InvalidOperationException($"proc_pid_rusage failed {System.Runtime.InteropServices.Marshal.GetLastPInvokeError()}");
    }

    var info = new MachTimebaseInfo();
    if (mach_timebase_info(ref info) != 0)
    {
      throw new InvalidOperationException("mach_timebase_info failed");
    }

    var userTime = usage.ri_user_time;
    var systemTime = usage.ri_system_time;

    var totalProcessorTime = (userTime + systemTime) * info.Numer / info.Denom;

    return totalProcessorTime / 1000000; // Convert nanoseconds to milliseconds
  }
  #else
  // getrusage does not need the MachTimebaseInfo factor applied
  [SupportedOSPlatform("macos")]
  private static double GetTotalProcessorTimeMillisecondsMacOS()
  {
    var usage = new RUsage();
    if (getrusage(RUSAGE_SELF, ref usage) != 0)
    {
      throw new InvalidOperationException("getrusage failed");
    }

    var info = new MachTimebaseInfo();
    if (mach_timebase_info(ref info) != 0)
    {
      throw new InvalidOperationException("mach_timebase_info failed");
    }

    var userTime = usage.UserTime.Seconds + usage.UserTime.MicroSeconds / 1e6;
    var systemTime = usage.SystemTime.Seconds + usage.SystemTime.MicroSeconds / 1e6;

    var totalProcessorTime = (userTime + systemTime);

    return totalProcessorTime * 1000; // Convert to milliseconds
  }
  #endif
}