using System.Collections.Generic;
using System.Linq;

namespace OmniSharp.ProjectSystemSdk
{
    public class GeneralCompilationOptions
    {
        public GeneralOutputKind OutputKind { get; set; }
        public bool WarningsAsErrors { get; set; }
        public bool Optimize { get; set; }
        public bool AllowUnsafe { get; set; }
        public bool ConcurrentBuild { get; set; }
        public string Platform { get; set; }
        public string KeyFile { get; set; }
        public Dictionary<string, ReportDiagnosticOptions> DiagnosticsOptions { get; set; } = new Dictionary<string, ReportDiagnosticOptions>();
        public string LanguageVersion { get; set; }
        public IEnumerable<string> Defines { get; set; } = Enumerable.Empty<string>();
        public bool UseDefaultDesktopAssemblyIdentityComparer { get; set; }
    }
}