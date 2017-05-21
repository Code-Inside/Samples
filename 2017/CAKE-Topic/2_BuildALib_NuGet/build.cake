#tool "nuget:?package=xunit.runner.console"

// ARGUMENTS
var target = Argument("target", "Default");

var rootAbsoluteDir = MakeAbsolute(Directory("./")).FullPath;

// TASKS
Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("2_BuildALib.sln");
});

Task("RunTests")
    .IsDependentOn("BuildTests")
    .Does(() =>
{
    Information("Start Running Tests");
    XUnit2("./artifacts/_tests/**/*Tests.dll");
});

Task("BuildTests")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	var parsedSolution = ParseSolution("./2_BuildALib.sln");

	foreach(var project in parsedSolution.Projects)
	{
	
	if(project.Name.EndsWith("Tests"))
		{
        Information("Start Building Test: " + project.Name);

        MSBuild(project.Path, new MSBuildSettings()
                .SetConfiguration("Debug")
                .SetMSBuildPlatform(MSBuildPlatform.Automatic)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("SolutionDir", @".\")
                .WithProperty("OutDir", rootAbsoluteDir + @"\artifacts\_tests\" + project.Name + @"\"));
		}
	
	}    

});

Task("BuildPackages")
    .IsDependentOn("Restore-NuGet-Packages")
	.IsDependentOn("RunTests")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings
	{
		OutputDirectory = rootAbsoluteDir + @"\artifacts\",
		IncludeReferencedProjects = true,
		Properties = new Dictionary<string, string>
		{
			{ "Configuration", "Release" }
		}
	};

    MSBuild("./2_BuildALib.Lib/2_BuildALib.Lib.csproj", new MSBuildSettings().SetConfiguration("Release"));
    NuGetPack("./2_BuildALib.Lib/2_BuildALib.Lib.csproj", nuGetPackSettings);
});


// TASK TARGETS
Task("Default").IsDependentOn("BuildPackages");

// EXECUTION
RunTarget(target);