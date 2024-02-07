using System.Diagnostics;

// Initial CPU usage:
var initialCpuUsageMs = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
Console.WriteLine($"Initial CPU usage: {Process.GetCurrentProcess().TotalProcessorTime}");

// Burn 5 seconds of CPU time
var sw = Stopwatch.StartNew();
while (sw.ElapsedMilliseconds < 5000) { }

// Final CPU usage:
Console.WriteLine($"Final CPU usage: {Process.GetCurrentProcess().TotalProcessorTime}");

// Delta:
Console.WriteLine($"Delta CPU usage (ms): {Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds - initialCpuUsageMs}");

// Wall time:
Console.WriteLine($"Wall time (ms): {sw.ElapsedMilliseconds}");