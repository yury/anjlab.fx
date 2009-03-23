using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Framework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Framework")]
[assembly: AssemblyCopyright("Copyright ©  2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: InternalsVisibleTo("AnjLab.FX.Tests, PublicKey="
  +  "00240000048000009400000006020000002400005253413100040000010001005155c1119100aa"
  +  "2f8a5c9053aea6233bd2419c627d92ea72e6728820138e84d82f7b757e1c589f8e03009fb5d660"
  +  "8a25a5396ad8bc51d00eb880731fd294749d830be0fe1789582cdcc4013c0fa72317a6ef8e44d0"
  +  "709ffce062f6e187c7295d597d2dc126e50e82a1c96a9ccad863fe3620f62b0ea1651d4403d517"
  +  "6e89c798")]
// The AllowPartiallyTrustedCallersAttribute requires the assembly to be signed with a strong name key.
// This attribute is necessary since the control is called by either an intranet or Internet
// Web page that should be running under restricted permissions.

//[assembly: FileIOPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
[assembly: CLSCompliant(true)]
// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c4545760-82a6-4b54-b40c-0ad0fdf03026")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      BuildMapper Number
//      Revision
//
// You can specify all the values or you can default the Revision and BuildMapper Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
