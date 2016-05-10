using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using OmniSharp.Mef;
using OmniSharp.Models;
using OmniSharp.Services;

namespace OmniSharp.Roslyn.CSharp.Services.Navigation
{
    [OmniSharpHandler(OmnisharpEndpoints.GotoDefinition, LanguageNames.CSharp)]
    public class GotoDefinitionService : RequestHandler<GotoDefinitionRequest, GotoDefinitionResponse>
    {
        private readonly MetadataHelper _metadataHelper;
        private readonly OmnisharpWorkspace _workspace;
		private readonly IMetadataFileReferenceCache _metadataCache;

        [ImportingConstructor]
        public GotoDefinitionService(OmnisharpWorkspace workspace, MetadataHelper metadataHelper, IMetadataFileReferenceCache metadataCache)
        {
            _workspace = workspace;
            _metadataHelper = metadataHelper;
			_metadataCache = metadataCache;
        }

        public async Task<GotoDefinitionResponse> Handle(GotoDefinitionRequest request)
        {
            var quickFixes = new List<QuickFix>();

            var document = _workspace.GetDocument(request.FileName);
            var response = new GotoDefinitionResponse();
            if (document != null)
            {
                var semanticModel = await document.GetSemanticModelAsync();
                var syntaxTree = semanticModel.SyntaxTree;
                var sourceText = await document.GetTextAsync();
                var position = sourceText.Lines.GetPosition(new LinePosition(request.Line, request.Column));
                var symbol = await SymbolFinder.FindSymbolAtPositionAsync(semanticModel, position, _workspace);

                if (symbol != null)
                {
                    var location = symbol.Locations.First();

                    if (location.IsInSource)
                    {
                        var lineSpan = symbol.Locations.First().GetMappedLineSpan();
                        response = new GotoDefinitionResponse
                        {
                            FileName = lineSpan.Path,
                            Line = lineSpan.StartLinePosition.Line,
                            Column = lineSpan.StartLinePosition.Character
                        };
                    }
                    else if (location.IsInMetadata && request.WantMetadata)
                    {
                        var cancellationSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(request.Timeout));
                        var metadataDocument = await _metadataHelper.GetDocumentFromMetadata(document.Project, symbol, cancellationSource.Token);
                        if (metadataDocument != null)
                        {
                            cancellationSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(request.Timeout));
                            var metadataLocation = await _metadataHelper.GetSymbolLocationFromMetadata(symbol, metadataDocument, cancellationSource.Token);
                            var lineSpan = metadataLocation.GetMappedLineSpan();

                            response = new GotoDefinitionResponse
                            {
                                Line = lineSpan.StartLinePosition.Line,
                                Column = lineSpan.StartLinePosition.Character,
                                MetadataSource = new MetadataSource()
                                {
                                    AssemblyName = symbol.ContainingAssembly.Name,
                                    ProjectName = document.Project.Name,
                                    TypeName = _metadataHelper.GetSymbolName(symbol)
                                },
                            };
                        }
                    }
                    else if (location.IsInMetadata)
                    {
						// This is an experiment, to see if we can jump to the disassembled source code of a symbol.
						// Ideally we should return the assembly file path and xml id and let the client to determine
						// how to locate the symbol in a disassembler, but this also means we need to modify the client
						// code to see the effect. Besides, this is mainly written for my own use in Vim. Currently this 
						// will work in Visual Studio Code without changes, but you will find DnSpy will be invoked whenever
						// you hold Control key and hover on some symbols only in metadata.
						//
                        // How to get MetadataReference from IAssemblySymbol: https://github.com/dotnet/roslyn/issues/7764
						var compilation = await document.Project.GetCompilationAsync();
						var metadataRef = compilation.GetMetadataReference(symbol.ContainingAssembly) as PortableExecutableReference;
                        
						if (metadataRef != null) {
							// The metadata reference is from in-memory stream so we cannot use its FilePath property directly: var filepath = metadataRef.FilePath;
							var filepath = _metadataCache.GetFilePath(metadataRef);
							if (filepath != null) {
								var xmlId = symbol.GetDocumentationCommentId();
								Process.Start(@"c:\tools\dnspy\dnspy.exe", $"\"{filepath}\" --select \"{xmlId}\"");
							}
						}
					}
                }
            }

            return response;
        }
    }
}
