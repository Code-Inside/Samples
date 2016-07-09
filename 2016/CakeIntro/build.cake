// ARGUMENTS

var target = Argument("target", "Default");

// TASKS

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("CakeExampleWithWpf.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      MSBuild("CakeExampleWithWpf.sln", settings =>
        settings.SetConfiguration("Release"));

});

// TASK TARGETS

Task("Default").IsDependentOn("Build");

// EXECUTION

RunTarget(target);