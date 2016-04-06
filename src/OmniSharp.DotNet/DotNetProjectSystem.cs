using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.ProjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OmniSharp.DotNet.Cache;
using OmniSharp.DotNet.Extensions;
using OmniSharp.DotNet.Models;
using OmniSharp.DotNet.Tools;
using OmniSharp.Models;
using OmniSharp.Models.v1;
using OmniSharp.Services;
using OmniSharp.Abstractions.ProjectSystem;

namespace OmniSharp.DotNet
{
    [Export(typeof(IProjectSystem)), Shared]
    public class DotNetProjectSystem : IProjectSystem
    {
        private readonly ILogger _logger;
        private readonly IEventEmitter _emitter;
        private readonly IFileSystemWatcher _watcher;
        private readonly IOmnisharpEnvironment _environment;
        private readonly IMetadataFileReferenceCache _metadataFileReferenceCache;
        private readonly string _compilationConfiguration = "Debug";
        private readonly PackagesRestoreTool _packageRestore;
        private readonly ICompilationWorkspace _workspace;
        private readonly ProjectStatesCache _projectStates;
        private WorkspaceContext _workspaceContext;
        private bool _enableRestorePackages = false;

        [ImportingConstructor]
        public DotNetProjectSystem(IOmnisharpEnvironment environment,
                                   ICompilationWorkspace workspace,
                                   IMetadataFileReferenceCache metadataFileReferenceCache,
                                   ILoggerFactory loggerFactory,
                                   IFileSystemWatcher watcher,
                                   IEventEmitter emitter)
        {
            _environment = environment;
            _workspace = workspace;
            _logger = loggerFactory.CreateLogger("O# .NET Project System");
            _emitter = emitter;
            _metadataFileReferenceCache = metadataFileReferenceCache;
            _watcher = watcher;

            _packageRestore = new PackagesRestoreTool(loggerFactory, _emitter);
            _projectStates = new ProjectStatesCache(loggerFactory, _emitter, _workspace);
        }

        public IEnumerable<string> Extensions { get; } = new string[] { ".cs" };

        public string Key => "DotNet";

        // https://github.com/dotnet/roslyn/blob/9bac4a6f86515f2d6f9a09d07dc73bc7e81dd7e4/src/Compilers/Core/Portable/Symbols/LanguageNames.cs#L15
        public string Language => "C#";

        public Task<object> GetInformationModel(WorkspaceInformationRequest request)
        {
            var workspaceInfo = new DotNetWorkspaceInformation(
                entries: _projectStates.GetStates,
                includeSourceFiles: !request.ExcludeSourceFiles);

            return Task.FromResult<object>(workspaceInfo);
        }

        public Task<object> GetProjectModel(string path)
        {
            _logger.LogDebug($"GetProjectModel {path}");
            var projectPath = _workspace.GetProjectPathFromDocumentPath(path);
            if (projectPath == null)
            {
                return Task.FromResult<object>(null);
            }

            _logger.LogDebug($"GetProjectModel {path}=>{projectPath}");
            var projectEntry = _projectStates.GetOrAddEntry(projectPath);
            var projectInformation = new DotNetProjectInformation(projectEntry);
            return Task.FromResult<object>(projectInformation);
        }

        public void Initalize(IConfiguration configuration)
        {
            _logger.LogInformation($"Initializing in {_environment.Path}");

            if (!bool.TryParse(configuration["enablePackageRestore"], out _enableRestorePackages))
            {
                _enableRestorePackages = false;
            }

            _logger.LogInformation($"Auto package restore: {_enableRestorePackages}");

            _workspaceContext = WorkspaceContext.Create();
            var projects = ProjectSearcher.Search(_environment.Path);
            _logger.LogInformation($"Originated from {projects.Count()} projects.");
            foreach (var path in projects)
            {
                _workspaceContext.AddProject(path);
            }

            Update(allowRestore: true);
        }

        public void Update(bool allowRestore)
        {
            _logger.LogInformation("Update workspace context");
            _workspaceContext.Refresh();

            var projectPaths = _workspaceContext.GetAllProjects();

            _projectStates.RemoveExcept(projectPaths, entry =>
            {
                foreach (var state in entry.ProjectStates)
                {
                    _workspace.RemoveProject(state.Id);
                    _logger.LogInformation($"Removing project {state.Id}.");
                }
            });

            foreach (var projectPath in projectPaths)
            {
                UpdateProject(projectPath);
            }

            _logger.LogInformation("Resolving projects references");
            foreach (var state in _projectStates.GetValues())
            {
                _logger.LogInformation($"  Processing {state}");

                var lens = new ProjectContextLens(state.ProjectContext, _compilationConfiguration);
                UpdateFileReferences(state, lens.FileReferences);
                UpdateProjectReferences(state, lens.ProjectReferences);
                UpdateUnresolvedDependencies(state, allowRestore);
                UpdateCompilationOption(state);
                UpdateSourceFiles(state, lens.SourceFiles);
            }
        }

        private void UpdateProject(string projectDirectory)
        {
            _logger.LogInformation($"Update project {projectDirectory}");
            var contexts = _workspaceContext.GetProjectContexts(projectDirectory);

            if (!contexts.Any())
            {
                _logger.LogWarning($"Cannot create any {nameof(ProjectContext)} from project {projectDirectory}");
                return;
            }

            _projectStates.Update(projectDirectory, contexts, AddProject, RemoveProject);

            var projectFilePath = contexts.First().ProjectFile.ProjectFilePath;
            _watcher.Watch(projectFilePath, file =>
            {
                _logger.LogInformation($"Watcher: {file} updated.");
                Update(true);
            });

            _watcher.Watch(Path.ChangeExtension(projectFilePath, "lock.json"), file =>
            {
                _logger.LogInformation($"Watcher: {file} updated.");
                Update(false);
            });
        }

        private void AddProject(Guid id, ProjectContext context)
        {
            _workspace.AddProject(
                id: id,
                name: $"{context.ProjectFile.Name}+{context.TargetFramework.GetShortFolderName()}",
                assemblyName: context.ProjectFile.Name,
                language: Language,
                filePath: context.ProjectFile.ProjectFilePath);

            _logger.LogInformation($"Add project {context.ProjectFile.ProjectFilePath} => {id}");
        }

        private void RemoveProject(Guid projectId)
        {
            _workspace.RemoveProject(projectId);
        }

        private void UpdateFileReferences(ProjectState state, IEnumerable<string> fileReferences)
        {
            var metadataReferences = new List<string>();
            var fileReferencesToRemove = state.FileMetadataReferences.ToHashSet();

            foreach (var fileReference in fileReferences)
            {
                if (!File.Exists(fileReference))
                {
                    continue;
                }

                if (fileReferencesToRemove.Remove(fileReference))
                {
                    continue;
                }

                metadataReferences.Add(fileReference);
                state.FileMetadataReferences.Add(fileReference);
                _logger.LogDebug($"    Add file reference {fileReference}");
            }

            foreach (var reference in metadataReferences)
            {
                _workspace.AddFileReference(state.Id, reference);
            }

            foreach (var reference in fileReferencesToRemove)
            {
                state.FileMetadataReferences.Remove(reference);
                _workspace.RemoveFileReference(state.Id, reference);
                _logger.LogDebug($"    Remove file reference {reference}");
            }

            if (metadataReferences.Count != 0 || fileReferencesToRemove.Count != 0)
            {
                _logger.LogInformation($"    Added {metadataReferences.Count} and removed {fileReferencesToRemove.Count} file references");
            }
        }

        private void UpdateProjectReferences(ProjectState state, IEnumerable<ProjectDescription> projectReferencesLatest)
        {
            var projectReferences = new List<Guid>();
            var projectReferencesToRemove = state.ProjectReferences.ToHashSet();

            foreach (var description in projectReferencesLatest)
            {
                var key = Tuple.Create(Path.GetDirectoryName(description.Path), description.Framework);
                if (projectReferencesToRemove.Remove(key))
                {
                    continue;
                }

                var referencedProjectState = _projectStates.Find(key.Item1, description.Framework);
                projectReferences.Add(referencedProjectState.Id);
                state.ProjectReferences.Add(key);

                _logger.LogDebug($"    Add project reference {description.Path}");
            }

            foreach (var reference in projectReferences)
            {
                _workspace.AddProjectReference(state.Id, reference);
            }

            foreach (var reference in projectReferencesToRemove)
            {
                var toRemove = _projectStates.Find(reference.Item1, reference.Item2);
                state.ProjectReferences.Remove(reference);
                _workspace.RemoveProjectReference(state.Id, toRemove.Id);

                _logger.LogDebug($"    Remove project reference {reference}");
            }

            if (projectReferences.Count != 0 || projectReferencesToRemove.Count != 0)
            {
                _logger.LogInformation($"    Added {projectReferences.Count} and removed {projectReferencesToRemove.Count} project references");
            }
        }

        private void UpdateUnresolvedDependencies(ProjectState state, bool allowRestore)
        {
            var libraryManager = state.ProjectContext.LibraryManager;
            var allDiagnostics = libraryManager.GetAllDiagnostics();
            var unresolved = libraryManager.GetLibraries().Where(dep => !dep.Resolved);
            var needRestore = allDiagnostics.Any(diag => diag.ErrorCode == ErrorCodes.NU1006) || unresolved.Any();

            if (needRestore)
            {
                if (allowRestore && _enableRestorePackages)
                {
                    _packageRestore.Restore(state.ProjectContext.ProjectDirectory, onFailure: () =>
                    {
                        _emitter.Emit(EventTypes.UnresolvedDependencies, new UnresolvedDependenciesMessage()
                        {
                            FileName = state.ProjectContext.ProjectFile.ProjectFilePath,
                            UnresolvedDependencies = unresolved.Select(d => new PackageDependency { Name = d.Identity.Name, Version = d.Identity.Version?.ToString() })
                        });
                    });
                }
                else
                {
                    _emitter.Emit(EventTypes.UnresolvedDependencies, new UnresolvedDependenciesMessage()
                    {
                        FileName = state.ProjectContext.ProjectFile.ProjectFilePath,
                        UnresolvedDependencies = unresolved.Select(d => new PackageDependency { Name = d.Identity.Name, Version = d.Identity.Version?.ToString() })
                    });
                }
            }
        }

        private void UpdateCompilationOption(ProjectState state)
        {
            var context = state.ProjectContext;
            var project = context.ProjectFile;
            var commonOption = project.GetCompilerOptions(context.TargetFramework, _compilationConfiguration);
            var option = new GeneralCompilationOptions
            {
                EmitEntryPoint = commonOption.EmitEntryPoint.GetValueOrDefault(),
                WarningsAsErrors = commonOption.WarningsAsErrors.GetValueOrDefault(),
                Optimize = commonOption.Optimize.GetValueOrDefault(),
                AllowUnsafe = commonOption.AllowUnsafe.GetValueOrDefault(),
                ConcurrentBuild = false,
                Platform = commonOption.Platform,
                KeyFile = commonOption.KeyFile,
                DiagnosticsOptions = new Dictionary<string, ReportDiagnosticOptions>{
                    { "CS1701", ReportDiagnosticOptions.Suppress },
                    { "CS1702", ReportDiagnosticOptions.Suppress },
                    { "CS1705", ReportDiagnosticOptions.Suppress },
                },
                LanguageVersion = commonOption.LanguageVersion,
                Defines = commonOption.Defines.ToArray()
            };

            _workspace.SetCSharpCompilationOptions(state.Id, project.ProjectDirectory, option);
            _workspace.SetParsingOptions(state.Id, option);
        }

        private void UpdateSourceFiles(ProjectState state, IEnumerable<string> sourceFiles)
        {
            sourceFiles = sourceFiles.Where(filename => Path.GetExtension(filename) == ".cs");

            var existingFiles = new HashSet<string>(state.DocumentReferences.Keys);

            var added = 0;
            var removed = 0;

            foreach (var file in sourceFiles)
            {
                if (existingFiles.Remove(file))
                {
                    continue;
                }

                var docId = _workspace.AddDocument(state.Id, file);
                state.DocumentReferences[file] = docId;

                _logger.LogDebug($"    Added document {file}.");
                added++;
            }

            foreach (var file in existingFiles)
            {
                _workspace.RemoveDocument(state.Id, state.DocumentReferences[file]);
                state.DocumentReferences.Remove(file);
                _logger.LogDebug($"    Removed document {file}.");
                removed++;
            }

            if (added != 0 || removed != 0)
            {
                _logger.LogInformation($"    Added {added} and removed {removed} documents.");
            }
        }
    }
}
