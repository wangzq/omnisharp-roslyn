using System;

namespace OmniSharp.Abstractions.ProjectSystem
{
    public interface ICompilationWorkspace
    {
        Guid CreateNewProjectID();

        string GetProjectPathFromDocumentPath(string path);

        void AddProject(Guid id, string name, string assemblyName, string language, string filePath);

        void RemoveProject(Guid id);

        void AddFileReference(Guid projectId, string filePath);

        void RemoveFileReference(Guid projectId, string filePath);

        void AddProjectReference(Guid projectId, Guid referencedProjectId);

        void RemoveProjectReference(Guid projectId, Guid referencedProjectId);

        Guid AddDocument(Guid projectId, string filePath);

        void RemoveDocument(Guid projectId, Guid id);

        void SetCSharpCompilationOptions(Guid projectId, string projectPath, GeneralCompilationOptions options);

        void SetParsingOptions(Guid projectId, GeneralCompilationOptions option);
    }
}