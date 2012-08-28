Set wshShell = WScript.CreateObject ("WSCript.shell")
wshshell.run """%windir%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe"" ""HAP Quota Service.exe""", 0, True
set wshshell = nothing