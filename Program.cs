using System.Diagnostics;

// Initial CPU usage:
var initialCpuUsageMs = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
Console.WriteLine($"System.Diagnostics.Process - Initial CPU usage: {Process.GetCurrentProcess().TotalProcessorTime}");
var initalCpuUsageMsFixed = CrossPlatformProcessTime.GetTotalProcessorTimeMilliseconds();
Console.WriteLine($"CrossPlatformProcessTime - Initial CPU usage: {initalCpuUsageMsFixed} ms");

// Burn 5 seconds of CPU time
var sw = Stopwatch.StartNew();
while (sw.ElapsedMilliseconds < 5000) { }

// Final CPU usage:
Console.WriteLine($"System.Diagnostics.Process - Final CPU usage: {Process.GetCurrentProcess().TotalProcessorTime}");
Console.WriteLine($"CrossPlatformProcessTime - Final CPU usage: {CrossPlatformProcessTime.GetTotalProcessorTimeMilliseconds()} ms");

// Delta:
Console.WriteLine($"System.Diagnostics.Process - Delta CPU usage: {Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - initialCpuUsageMs} ms");
Console.WriteLine($"CrossPlatformProcessTime - Delta CPU usage: {CrossPlatformProcessTime.GetTotalProcessorTimeMilliseconds() - initalCpuUsageMsFixed} ms");

// Wall time:
Console.WriteLine($"Wall time (ms): {sw.ElapsedMilliseconds}");