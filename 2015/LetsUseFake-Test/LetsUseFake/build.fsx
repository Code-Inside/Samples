// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

RestorePackages()

// Properties
let artifactsDir = @".\artifacts\"
let artifactsBuildDir = "./artifacts/build/"
let artifactsTestsDir  = "./artifacts/tests/"

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

Target "BuildTests" (fun _ ->
    trace "Building Tests..."
    !! "**/*Tests.csproj"
      |> MSBuildDebug artifactsTestsDir "Build"
      |> Log "TestBuild-Output: "
)

Target "RunTests" (fun _ ->
    trace "Running Tests..."
    !! (artifactsTestsDir + @"\*Tests.dll") 
      |> xUnit (fun p -> {p with OutputDir = artifactsTestsDir })
)

Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "BuildTests"
  ==> "RunTests"
  ==> "Default"

// start build
RunTargetOrDefault "Default"