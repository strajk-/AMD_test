# AMD_test
A small test using Reflection to find an Assembly by Name that causes Ryzen CPUs to slow down considerably after X Tries

This issue does not seem to happen on Intel based Systems.
Interestingly this issue does not happen the moment an AMD System is Virtualized.


### EDIT (fix):
This issue does not seem to be AMD related at all, but instead it's caused because of Thread Count.
At the time this test was written AMD started roling out the Zen Architecture which brough 8 Core / 16 Threads to the mainstream, apparently .NET 4 (the version this test is compiled on) has issues if the Garbage Collector is running on Workstation Mode due to the high number of Threads.

This would explain why it didn't happen on the Intel Devices this was later tested on, since they're usually 4 Core / 4-8 Threads (at the time this was written).
The same reason applies to why it would work on a Virtualized Environment, since those were created with less Cores and Threads as well.

In order to fix this, the Project has to be compiled either in .NET Core 3.0, or Server GC has to be set in the app.config:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
    <gcServer enabled="true"></gcServer>
  </runtime>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.0,Profile=Client"/>
  </startup>
</configuration>
```

This conclusion was reached after creating a [Ticket](https://github.com/dotnet/coreclr/issues/24355) at the [dotnet/coreclr repo](https://github.com/dotnet/coreclr).
