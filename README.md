# Overview

This is a self-contained minimal reproducible example for `System.Diagnostics.Process` on Mac OS X, with Apple Silicon, reporting incorrect CPU usage times.

## Steps to reproduce

`time dotnet run -c Release`

## Expected behavior

CPU usage should be reported around 5 seconds

## Actual behavior

CPU usage is reported as `120 ms` on an M2 Max

```
time dotnet run -c Release
Initial CPU usage: 00:00:00.0005113
Final CPU usage: 00:00:00.1205400
Delta CPU usage (ms): 120.037
Wall time (ms): 5001
dotnet run -c Release  5.70s user 0.28s system 105% cpu 5.659 total
```

# Fixing

https://stackoverflow.com/a/72915413/878903

The CPU usage times need to be multiplied by factors returned by `mach_timebase_info`.