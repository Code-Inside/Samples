// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"

// Targets
Target "Clean" (fun _ ->
    trace "Cleanup..."
    CleanDirs [artifactsDir]
)

Target "BuildApp" (fun _ ->
   trace "Building App..."
   !! "**/*.csproj"
     |> MSBuildRelease artifactsBuildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildNuGet" (fun _ ->
   
    NuGetPack (fun p -> 
    {p with
        OutputPath = artifactsDir
        WorkingDir = "./CreateNuGetViaFake/bin/Debug/"
        })  "./CreateNuGetViaFake/Test.nuspec"
)


Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildNuGet"
  ==> "Default"

// start build
RunTargetOrDefault "Default"