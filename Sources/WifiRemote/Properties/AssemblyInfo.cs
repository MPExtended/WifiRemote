using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MediaPortal.Common.Utils;


// Version Compatibility
// http://wiki.team-mediaportal.com/1_MEDIAPORTAL_1/18_Contribute/6_Plugins/Plugin_Related_Changes/1.1.0_to_1.2.0/Version_Compatibility
#region MediaPortal Compatibility
[assembly: CompatibleVersion("1.6.100.0")]
[assembly: UsesSubsystem("MP.SkinEngine")]
[assembly: UsesSubsystem("MP.Input")]
[assembly: UsesSubsystem("MP.Players")]
[assembly: UsesSubsystem("MP.Config")]
[assembly: UsesSubsystem("MP.Plugins.Music")]
[assembly: UsesSubsystem("MP.Plugins.Videos")]
[assembly: UsesSubsystem("MP.Plugins.TV")]
// Makes the plugin incompatible when no tv server installed
// Need to find a solution for that.
// [assembly: UsesSubsystem("MP.TVE")]
#endregion


// Allgemeine Informationen über eine Assembly werden über die folgenden 
// Attribute gesteuert. Ändern Sie diese Attributwerte, um die Informationen zu ändern,
// die mit einer Assembly verknüpft sind.
[assembly: AssemblyTitle("WifiRemote")]
[assembly: AssemblyDescription("WifiRemote process plugin for MediaPortal")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Team MediaPortal")]
[assembly: AssemblyProduct("WifiRemote")]
[assembly: AssemblyCopyright("Copyright © Microsoft 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// für COM-Komponenten. Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen müssen, legen Sie das ComVisible-Attribut für diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

// Die folgende GUID bestimmt die ID der Typbibliothek, wenn dieses Projekt für COM verfügbar gemacht wird
[assembly: Guid("368e4054-fe4a-4490-9387-8b7474084a25")]

// Versionsinformationen für eine Assembly bestehen aus den folgenden vier Werten:
//
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision
//
// Sie können alle Werte angeben oder die standardmäßigen Build- und Revisionsnummern 
// übernehmen, indem Sie "*" eingeben:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.8.2.0")]
[assembly: AssemblyFileVersion("0.8.2.0")]
