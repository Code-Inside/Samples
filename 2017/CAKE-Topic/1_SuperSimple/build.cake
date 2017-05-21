// ARGUMENTS
var target = Argument("target", "Default");

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
      MSBuild("1_SuperSimple.sln", settings =>
        settings.SetConfiguration("Release"));

});

// TASK TARGETS
Task("Default").IsDependentOn("Build");

// EXECUTION
RunTarget(target);