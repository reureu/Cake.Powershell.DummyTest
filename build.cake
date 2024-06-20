//////////////////////////////////////////////////////////////////////
// ADD-INS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Powershell&version=3.0.0

// Comment out the following line to reproduce https://github.com/SharpeRAD/Cake.Powershell/issues/95
#addin nuget:?package=Microsoft.PowerShell.Commands.Management&version=7.4.3

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var _target = Argument("target", "Default");
var _myParam = Argument("myParam", "param");
var _secret = Argument("secret", "secret");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("DoSomething")
    .Does(() => {

        var cwd= System.IO.Directory.GetCurrentDirectory();
        Console.WriteLine($"cwd: {cwd}");
		const string ps1Path = "./scripts/DoSomething.ps1";
        if(!System.IO.File.Exists(ps1Path))
		{
			throw new FileNotFoundException(ps1Path);
		}
        try
        {
            StartPowershellFile("./scripts/DoSomething.ps1", new PowershellSettings()
                {
                    FormatOutput = false,
                    LogOutput = false,
                    BypassExecutionPolicy = true
                }
                .WithArguments(args =>
                {
					args
						.AppendQuoted("myParam", _myParam)
						.AppendSecret("secret", _secret);
                }));
        }
        catch(Exception e){
			Error($"Exception was thrown: {e}");
			throw;
		}
                

        Information($"Done something.");
    });


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
Task("Default")
    .IsDependentOn("DoSomething");

RunTarget(_target);