1. Build the csproj
2. Copy WpfToUwpTestApp.exe, appxmanifest.xml, Assets/*.* to a new folder UWP/_App
3. Copy _Scripts/*.* to the UWP folder
4. Invoke 1.MakeAppx.cmd (which points to _App)
5. Invoke 2.MakeCert (if not already in place)
5.1 -> Import pfx to trusted authorities (maybe I'm doing something wrong... I couldn't do it without this step.)
6. Invoke 3.SignTool