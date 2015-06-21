// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
#r "System.Xml.Linq"
open Fake
open System.Xml.Linq

RestorePackages()

// Properties
let artifactsNuGetDir = @"./artifacts/nuget/"
let artifactsBuildDir = "./artifacts/build/"

// Targets
Target "Clean" (fun _ ->
    trace "Cleanup..."
    CleanDir artifactsNuGetDir
    CleanDir artifactsBuildDir
)

Target "BuildApp" (fun _ ->
   trace "Building App..."
   !! "**/*.csproj"
     |> MSBuildRelease artifactsBuildDir "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildNuGet" (fun _ ->
   
    let doc = System.Xml.Linq.XDocument.Load("./CreateNuGetViaFake/Test.nuspec")
    let vers = doc.Descendants(XName.Get("version", doc.Root.Name.NamespaceName)) 

    NuGet (fun p -> 
    {p with
        Version = (Seq.head vers).Value
        OutputPath = artifactsNuGetDir
        WorkingDir = artifactsBuildDir
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
