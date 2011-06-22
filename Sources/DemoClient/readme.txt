WifiRemote Demo Client version 1.1
==================================

Meta
----
Released: 2011-05-11
Source: http://code.google.com/p/wifiremote/source/browse/#svn%2Ftrunk%2FSources%2FDemoClient
Author: Shukuyen


Links
-----
Download WifiRemote from team-mediaportal.com:
http://www.team-mediaportal.com/extensions/input-output/wifiremote

Documentation for developers:
http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/WifiRemote

WifiRemote forum thread:
http://forum.team-mediaportal.com/mediaportal-plugins-47/wifiremote-tcp-remote-control-server-0-1-2011-05-05-a-96251/


Acknowledgements
----------------
WifiRemote Demo Client is using the following libraries:

Json.NET by James Newton-King
http://james.newtonking.com/pages/json-net.aspx

Zeroconfignetservices by Deusty Software (New BSD License)
http://code.google.com/p/zeroconfignetservices/


How to use
----------
Extract DemoClient.exe, Newtonsoft.Json.Net35.dll, ZeroconfService.dll in the
same folder and double-click DemoClient.exe.

You can now connect to a MediaPortal installation running WifiRemote on your
local network either by selecting it from the list of detected servers (Bonjour 
has to be active on the server) or by entering the hostname and port of the
WifiRemote plugin you want to connect to.

Click on the bottom status bar to open a log window. Here you can see what 
communication takes place between server and client. This can be useful if you
want to develop your own client.