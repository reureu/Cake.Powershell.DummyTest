# Cake.Powershell.DummyTest
Basic test project to try out [Cake.Powershell](https://github.com/SharpeRAD/Cake.Powershell)

This project demonstrates that Cake.Powershell does not work with Cake.NET 4.0.0 on reasonably recent operating systems, as described in [issue 95](https://github.com/SharpeRAD/Cake.Powershell/issues/95).

# How to start
1. Ensure a reasonably recent version of Powershell is installed on your machine.
2. Launch Powershell
3. From the root folder of this project, launch `build.ps1`

## Actual results
* OS: Windows 10 Version 22H2 (OS Build 19045.4412)
* PowerShell: 7.3.3
```
PS D:\Source\_DummyProjects\Cake.Powershell.DummyTest> ./build.ps1
Tool 'cake.tool' (version '4.0.0') was restored. Available commands: dotnet-cake

Restore was successful.
The assembly 'Cake.Powershell, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null'
is referencing an older version of Cake.Core (3.0.0).
For best compatibility it should target Cake.Core version 4.0.0.
Could not load D:\Source\_DummyProjects\Cake.Powershell.DummyTest\tools\Addins\Microsoft.Management.Infrastructure.CimCmdlets.7.3.3\runtimes\win\lib\net7.0\Microsoft.Management.Infrastructure.CimCmdlets.dll (missing Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35)
Could not load D:\Source\_DummyProjects\Cake.Powershell.DummyTest\tools\Addins\Microsoft.PowerShell.Commands.Management.7.3.3\runtimes\win\lib\net7.0\Microsoft.PowerShell.Commands.Management.dll (missing Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35)

========================================
DoSomething
========================================
cwd: D:\Source\_DummyProjects\Cake.Powershell.DummyTest
Executing: &"D:/Source/_DummyProjects/Cake.Powershell.DummyTest/scripts/DoSomething.ps1" -myParam "" -secret [REDACTED]
Exception was thrown: System.IO.FileNotFoundException: Could not load file or assembly 'Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'. The system cannot find the file specified.
File name: 'Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'
   at System.Reflection.RuntimeAssembly.GetExportedTypes()
   at System.Management.Automation.Runspaces.PSSnapInHelpers.GetAssemblyTypes(Assembly assembly, String name)
   at System.Management.Automation.Runspaces.PSSnapInHelpers.AnalyzeModuleAssemblyWithReflection(Assembly assembly, String name, PSSnapInInfo psSnapInInfo, PSModuleInfo moduleInfo, String helpFile, Dictionary`2& cmdlets, Dictionary`2& aliases, Dictionary`2& providers)
   at System.Management.Automation.Runspaces.PSSnapInHelpers.AnalyzePSSnapInAssembly(Assembly assembly, String name, PSSnapInInfo psSnapInInfo, PSModuleInfo moduleInfo, Dictionary`2& cmdlets, Dictionary`2& aliases, Dictionary`2& providers, String& helpFile)
   at System.Management.Automation.Runspaces.InitialSessionState.ImportPSSnapIn(PSSnapInInfo psSnapInInfo, PSSnapInException& warning)
   at System.Management.Automation.Runspaces.InitialSessionState.CreateDefault()
   at System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(PSHost host)
   at Cake.Powershell.PowershellRunner.Invoke(String script, PowershellSettings settings) in C:\projects\cake-powershell\src\Cake.Powershell\Runner\PowershellRunner.cs:line 271
   at Cake.Powershell.PowershellRunner.Start(FilePath path, PowershellSettings settings) in C:\projects\cake-powershell\src\Cake.Powershell\Runner\PowershellRunner.cs:line 140
   at Cake.Powershell.PowershellAliases.StartPowershellFile(ICakeContext context, FilePath path, PowershellSettings settings) in C:\projects\cake-powershell\src\Cake.Powershell\Aliases\PowershellAliases.cs:line 99
   at Submission#0.StartPowershellFile(FilePath path, PowershellSettings settings)
   at Submission#0.<<Initialize>>b__0_0()
An error occurred when executing task 'DoSomething'.
Error: Could not load file or assembly 'Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35'. The system cannot find the file specified.
```

## Analysis
As stated in [issue 95](https://github.com/SharpeRAD/Cake.Powershell/issues/95), some dependencies seem to be missing in Cake.Powershell.
Cake can resolve the dependencies, hence we add this in the [cake.config](cake.config) file. 
```
[NuGet]
LoadDependencies=true
```
So, Cake resolves most of the dependencies, until it bumps into `Microsoft.Management.Infrastructure, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35` 

Now, there is something funny about this assembly: 
* It exists in my global assembly cache and targets the .NET Framework 4.0.
* It is also part of the Powershell 7.3.3 installation package and targets the .NET Framework 4.5.
* It seems to be a dependency of Microsoft.PowerShell.Commands.Management 7.3.3.

## Workaround
Let's dig a little deeper...

* Does Microsoft.PowerShell.Commands.Management 7.3.3 refer a wrong version of Microsoft.Management.Infrastructure ?
* So what happens if I tell cake to use Microsoft.PowerShell.Commands.Management 7.4.3 instead of 7.3.3.

So let's amend the [build.cake](build.cake) file!
```
//////////////////////////////////////////////////////////////////////
// ADD-INS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Powershell&version=3.0.0
#addin nuget:?package=Microsoft.PowerShell.Commands.Management&version=7.4.3
```

This allows my build.cake to successfully do its job. 