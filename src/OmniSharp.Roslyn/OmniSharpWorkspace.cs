using System;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Composition.Hosting.Core;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;
using OmniSharp.Mef;
using OmniSharp.Options;
using OmniSharp.Roslyn;
using OmniSharp.Services;
using OmniSharp.Stdio.Services;

namespace OmniSharp
{
    [Export, Shared]
    public class OmnisharpWorkspace : Workspace
    {
        public bool Initialized { get; set; }

        public BufferManager BufferManager { get; private set; }

        [ImportingConstructor]
        public OmnisharpWorkspace(HostServicesBuilder builder) : base(builder.GetHostServices(), "Custom")
        {
            BufferManager = new BufferManager(this);
        }

        public void AddProject(ProjectInfo projectInfo)
        {
            OnProjectAdded(projectInfo);
        }

        public void AddProjectReference(ProjectId projectId, ProjectReference projectReference)
        {
            OnProjectReferenceAdded(projectId, projectReference);
        }

        public void RemoveProjectReference(ProjectId projectId, ProjectReference projectReference)
        {
            OnProjectReferenceRemoved(projectId, projectReference);
        }

        public void AddMetadataReference(ProjectId projectId, MetadataReference metadataReference)
        {
            OnMetadataReferenceAdded(projectId, metadataReference);
        }

        public void RemoveMetadataReference(ProjectId projectId, MetadataReference metadataReference)
        {
            OnMetadataReferenceRemoved(projectId, metadataReference);
        }

        public void AddDocument(DocumentInfo documentInfo)
        {
            OnDocumentAdded(documentInfo);
        }

        public void RemoveDocument(DocumentId documentId)
        {
            OnDocumentRemoved(documentId);
        }

        public void RemoveProject(ProjectId projectId)
        {
            OnProjectRemoved(projectId);
        }

        public void SetCompilationOptions(ProjectId projectId, CompilationOptions options)
        {
            OnCompilationOptionsChanged(projectId, options);
        }

        public void SetParseOptions(ProjectId projectId, ParseOptions parseOptions)
        {
            OnParseOptionsChanged(projectId, parseOptions);
        }

        public void OnDocumentChanged(DocumentId documentId, SourceText text)
        {
            OnDocumentTextChanged(documentId, text, PreservationMode.PreserveIdentity);
        }

        public DocumentId GetDocumentId(string filePath)
        {
            var documentIds = CurrentSolution.GetDocumentIdsWithFilePath(filePath);
            return documentIds.FirstOrDefault();
        }

        public IEnumerable<Document> GetDocuments(string filePath)
        {
            return CurrentSolution.GetDocumentIdsWithFilePath(filePath).Select(id => CurrentSolution.GetDocument(id));
        }

        public Document GetDocument(string filePath)
        {
            var documentId = GetDocumentId(filePath);
            if (documentId == null)
            {
                return null;
            }
            return CurrentSolution.GetDocument(documentId);
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return true;
        }

        public void AddAnalyzer(ProjectId projectId, IEnumerable<string> files)
        {
#if DNX451
            files = Directory.EnumerateFiles(@"C:\Users\David\.dnx\packages\StyleCop.Analyzers\1.0.0-beta015\analyzers\dotnet\cs");
            Console.WriteLine("Loading analyzers " + string.Join(",", files));
            foreach (var file in files)
            {
                this.SetCurrentSolution(CurrentSolution.AddAnalyzerReference(projectId,
                    new AnalyzerFileReference(file)));
            }
#endif
        }
    }
}
