using System.Reflection;
using System.Runtime.CompilerServices;

#if SILVERLIGHT
[assembly: AssemblyTitle("SharpCompress.Silverlight")]
[assembly: AssemblyProduct("SharpCompress.Silverlight")]
#else
#if PORTABLE
[assembly: AssemblyTitle("SharpCompress.Silverlight")]
[assembly: AssemblyProduct("SharpCompress.Silverlight")]
#else
#if THREEFIVE
[assembly: AssemblyTitle("SharpCompress.3.5")]
[assembly: AssemblyProduct("SharpCompress.3.5")]
#else
[assembly: AssemblyTitle("SharpCompress")]
[assembly: AssemblyProduct("SharpCompress")]
[assembly: InternalsVisibleTo("SharpCompress.Test")]
#endif
#endif
#endif
