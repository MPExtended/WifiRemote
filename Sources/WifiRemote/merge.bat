@ECHO OFF

IF EXIST WifiRemote_TMP.dll DEL WifiRemote_TMP.dll 
IF EXIST WifiRemote_TMP.pdb DEL WifiRemote_TMP.pdb 

ilmerge /out:WifiRemote_TMP.dll WifiRemote.dll ZeroconfService.dll Newtonsoft.Json.dll zxing.dll zxing.presentation.dll /targetplatform:v4,"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"

IF EXIST WifiRemote.dll DEL WifiRemote.dll
IF EXIST WifiRemote.pdb DEL WifiRemote.pdb

IF EXIST WifiRemote_TMP.dll REN WifiRemote_TMP.dll WifiRemote.dll
IF EXIST WifiRemote_TMP.pdb REN WifiRemote_TMP.pdb WifiRemote.pdb
