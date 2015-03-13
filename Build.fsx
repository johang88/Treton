#r @"Tools/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = getBuildParamOrDefault "build-dir" "Build/"

// Targets
Target "Clean" (fun _ -> 
    CleanDir buildDir
)

Target "BuildEngine" (fun _ ->
   !! "Source/**/*.csproj"
     |> MSBuildRelease (buildDir + "Bin") "Build"
     |> Log "AppBuild-Output: "
)

Target "BuildContentBundle" (fun _ ->
    let cmd = "\"" + buildDir + "Bin/PackageCompiler.exe\""
    let args = "--input=Content/ --output=" + buildDir + "Data --packages=Core --platform=x86 --bundle"
    let errorCode = Shell.Exec(cmd, args)
    trace "Content build complete"
)

Target "CleanOutput" (fun _ ->
    let files = 
        !! (buildDir + "Bin/*.xml")
            ++ (buildDir + "Bin/*.pdb")
            ++ (buildDir + "Bin/*.config")

    DeleteFiles files
)

Target "Default" (fun _ ->
    trace "hello world!"
)

// Dependencies
"Clean"
    ==> "BuildEngine"
    ==> "BuildContentBundle"
    ==> "CleanOutput"
    ==> "Default"

RunTargetOrDefault "Default"