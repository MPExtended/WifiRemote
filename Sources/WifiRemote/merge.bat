@echo off
IF EXIST WifiRemote_UNMERGED.dll del WifiRemote_UNMERGED.dll 
ren WifiRemote.dll WifiRemote_UNMERGED.dll 
ilmerge /ndebug /out:WifiRemote.dll WifiRemote_UNMERGED.dll ZeroconfService.dll Newtonsoft.Json.Net35.dll zxing.dll
