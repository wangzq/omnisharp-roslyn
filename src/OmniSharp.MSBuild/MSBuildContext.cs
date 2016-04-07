using System;
using System.Collections.Generic;
using System.Composition;
using OmniSharp.MSBuild.ProjectFile;

namespace OmniSharp.MSBuild
{
    [Export, Shared]
    public class MSBuildContext
    {
        public Dictionary<Guid, Guid> ProjectGuidToWorkspaceMapping { get; } = new Dictionary<Guid, Guid>();
        public Dictionary<string, ProjectFileInfo> Projects { get; } = new Dictionary<string, ProjectFileInfo>(StringComparer.OrdinalIgnoreCase);
        public string SolutionPath { get; set; }
    }
}
