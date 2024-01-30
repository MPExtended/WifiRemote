[![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/MPExtended/WifiRemote?include_prereleases)](https://github.com/MPExtended/WifiRemote/releases)
[![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/downloads-pre/MPExtended/WifiRemote/latest/total?label=release@downloads)](https://github.com/MPExtended/WifiRemote/releases)
[![WiFiRemote x86](https://img.shields.io/badge/WiFiRemote-x86-orange?logo=windows&logoColor=white)](https://github.com/MPExtended/WifiRemote/releases)[![WiFiRemote x64](https://img.shields.io/badge/x64-blue?logoColor=white)](https://github.com/MPExtended/WifiRemote/releases)
[![StandWithUkraine](https://raw.githubusercontent.com/vshymanskyy/StandWithUkraine/main/badges/StandWithUkraine.svg)](https://github.com/vshymanskyy/StandWithUkraine/blob/main/docs/README.md)


# WifiRemote MediaPortal Plugin

### What is WifiRemote
WifiRemote is a process plugin for the popular opensource mediacenter software "[MediaPortal](http://www.team-mediaportal.com)".

It publishes a Bonjour Service on your local network which allows clients (for example an iPhone or Android app) to list all found MediaPortal installations and connect to it.

Those clients can then send commands like up, down, ok, play, pause and so on to the plugin which relays them to MediaPortal - effectively enabling any kind of device on the network to be used as a remote control (requires a client app for the device).

The app also accepts special commands to shutdown/hibernate/suspend/reboot/logoff the htpc and to exit MediaPortal, or to switch the active MediaPortal window to one requested by the client app. This makes it possible to have shortcuts to for example your TV series or your movies in Moving Pictures.


### How to get started?
At the moment you can build the plugin in Visual Studio and run it on your HTPC, but without a client it won't do you much good. After uploading and refining the first version I will upload a C# test app, so you can test the plugin.
Please copy the WifiRemote.xml file to your MediaPortal custom keymaps folder.

### Building a client
You should start by taking a look at the DemoClient app, included in the WifiRemote solution.
This app shows how you can discover a server via Bonjour, connect to it, issue commands and receive messages. It also introduces the concept of the autologin key. The DemoClient app is written in C# but should be relatively easy to understand. If you have questions about this please contact me via PM on the MediaPortal forum (user Shukuyen).

JSON messages sent from your client to WifiRemote are called commands. You can find a list of available commands here:
http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/WifiRemote/Commands

WifiRemote will send JSON messages to your client, a list can be found here:
http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/17_Extensions/3_Plugins/WifiRemote/Messages


### Acknowledgements
WifiRemote uses the following libraries:
  * [DotNetAsyncSocket](http://code.google.com/p/dotnetasyncsocket/)
  * [ZeroConfigNetServices](http://code.google.com/p/zeroconfignetservices/)
  * [Json.NET by James Newton-King](http://james.newtonking.com/pages/json-net.aspx)
  * [Barcodes ("Zebra Crossing")](http://code.google.com/p/zxing/ ZXing)

Huge thanks go to [martinvanderboon](http://code.google.com/u/martinvanderboon/) for his [iPimp project](http://code.google.com/p/ipimp/), an advanced web based remote control and even streaming client. I ended up rewriting and using a lot more code from his MPClientController plugin than I thought possible when starting this project. Thanks!
