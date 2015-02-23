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

Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Default"

// start build
RunTargetOrDefault "Default"