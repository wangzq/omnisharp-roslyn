using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Framework.OptionsModel;
using OmniSharp.Models;
using OmniSharp.Options;
using OmniSharp.Services;

namespace OmniSharp
{
    public class FormattingController
    {
        private readonly OmnisharpWorkspace _workspace;
        private readonly IFormattingService _formattingService;

        public FormattingController(OmnisharpWorkspace workspace, IFormattingService service)
        {
            _workspace = workspace;
            _formattingService = service;
        }


        [HttpPost("formatAfterKeystroke")]
        public async Task<FormatRangeResponse> FormatAfterKeystroke([FromBody]FormatAfterKeystrokeRequest request)
        {
            var document = _workspace.GetDocument(request.FileName);
            if (document == null)
            {
                return null;
            }

            var lines = (await document.GetSyntaxTreeAsync()).GetText().Lines;
            var position = lines.GetPosition(new LinePosition(request.Line - 1, request.Column - 1));
            var options = _formattingService.GetDocumentOptionSet(_workspace.Options, request.FileName);
            var changes = await Formatting.GetFormattingChangesAfterKeystroke(_workspace, options, document, position, request.Char);

            return new FormatRangeResponse()
            {
                Changes = changes
            };
        }

        [HttpPost("formatRange")]
        public async Task<FormatRangeResponse> FormatRange([FromBody]FormatRangeRequest request)
        {
            var document = _workspace.GetDocument(request.FileName);
            if (document == null)
            {
                return null;
            }

            var lines = (await document.GetSyntaxTreeAsync()).GetText().Lines;
            var start = lines.GetPosition(new LinePosition(request.Line - 1, request.Column - 1));
            var end = lines.GetPosition(new LinePosition(request.EndLine - 1, request.EndColumn - 1));
            var options = _formattingService.GetDocumentOptionSet(_workspace.Options, request.FileName);
            var changes = await Formatting.GetFormattingChangesForRange(_workspace, options, document, start, end);

            return new FormatRangeResponse()
            {
                Changes = changes
            };
        }

        [HttpPost("codeformat")]
        public async Task<CodeFormatResponse> FormatDocument([FromBody]Request request)
        {
            var document = _workspace.GetDocument(request.FileName);
            if (document == null)
            {
                return null;
            }
            var options = _formattingService.GetDocumentOptionSet(_workspace.Options, request.FileName);
            var newText = await Formatting.GetFormattedDocument(_workspace, options, document);
            return new CodeFormatResponse()
            {
                Buffer = newText
            };
        }
    }
}