// ARGUMENTS
var target = Argument("target", "Default");

var rootAbsoluteDir = MakeAbsolute(Directory("./")).FullPath;

// TASKS
Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("1_SuperSimple.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild(@"./1_SuperSimple.ConsoleApp/1_SuperSimple.ConsoleApp.csproj", new MSBuildSettings().SetConfiguration("Release")
																								  .WithProperty("OutDir", rootAbsoluteDir + @"/artifacts/app/"));

});

// TASK TARGETS
Task("Default").IsDependentOn("Build");

// EXECUTION
RunTarget(target);