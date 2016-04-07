namespace OmniSharp.ProjectSystemSdk
{
    // The underlying values must keep in line with Microsoft.CodeAnalysis.ReportDiagnostics
    // https://github.com/dotnet/roslyn/blob/11f4d607a8b6068cc31f76ec57b4b1654604aa23/src/Compilers/Core/Portable/Diagnostic/ReportDiagnostic.cs
    public enum ReportDiagnosticOptions
    {
        Default = 0,
        Error = 1,
        Warn = 2,
        Info = 3,
        Hidden = 4,
        Suppress = 5
    }
}