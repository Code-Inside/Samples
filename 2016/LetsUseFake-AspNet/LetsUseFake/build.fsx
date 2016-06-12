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

Target "BuildWebApp" (fun _ ->
   trace "Building WebHosted Connect..."
   !! "**/*.csproj"
     |> MSBuild artifactsBuildDir "Package"
        ["Configuration", "Release"
         "Platform", "AnyCPU"
         "_PackageTempDir", (@"..\" + artifactsDir + @"Release-Ready-WebApp")
         ]
     |> Log "AppBuild-Output: "
)


Target "Default" (fun _ ->
    trace "Default Target invoked."
)

// Dependencies
"Clean"
  ==> "BuildWebApp"
  ==> "Default"

// start build
RunTargetOrDefault "Default"