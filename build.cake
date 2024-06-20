//////////////////////////////////////////////////////////////////////
// ADD-INS
//////////////////////////////////////////////////////////////////////
#addin nuget:?package=Cake.Powershell&version=3.0.0

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var _target = Argument("target", "Default");
var _myParam = Argument("myParam", "");
var _secret = Argument("secret", "");

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
                    FormatOutput = true,
                    LogOutput = true
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