using System;
using System.Collections.Generic;
using Microsoft.DotNet.ProjectModel;
using NuGet.Frameworks;

namespace OmniSharp.DotNet.Cache
{
    public class ProjectState
    {
        public ProjectState(Guid id, ProjectContext context)
        {
            Id = id;
            ProjectContext = context;
        }

        public Guid Id { get; }

        public ProjectContext ProjectContext { get; set; }

        public HashSet<string> FileMetadataReferences { get; } = new HashSet<string>();
        
        public HashSet<Tuple<string, NuGetFramework>> ProjectReferences { get; } = new HashSet<Tuple<string, NuGetFramework>>();

        public Dictionary<string, Guid> DocumentReferences { get; } = new Dictionary<string, Guid>();

        public override string ToString()
        {
            return $"[{nameof(ProjectState)}] {ProjectContext.ProjectFile.Name}/{ProjectContext.TargetFramework}";
        }
    }
}
