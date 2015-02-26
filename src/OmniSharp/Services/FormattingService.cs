using System;
using System.Collections.Generic;
using System.Linq;
using EditorConfig.Core;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Framework.OptionsModel;
using OmniSharp.Options;
using OmniSharp.Utilities;
using System.Reflection;
using Microsoft.CodeAnalysis;
using GlobalOption = Microsoft.CodeAnalysis.Formatting.FormattingOptions;
using CSharpOption = Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions;

namespace OmniSharp.Services
{
    public interface IFormattingService
    {
        OptionSet GetDocumentOptionSet(OptionSet baseOptionSet, string filePath);
    }

    public class FormattingService : IFormattingService
    {
        private OmniSharpOptions _omniSharpOptions;
        private FormattingOptions _formattingOptions;
        private EditorConfigParser EditorConfigParser { get; }

        public FormattingService(IOptions<OmniSharpOptions> optionsAccessor)
        {
            _omniSharpOptions = optionsAccessor != null ? optionsAccessor.Options : new OmniSharpOptions();
            _formattingOptions = _omniSharpOptions.FormattingOptions;
            this.EditorConfigParser = new EditorConfigParser();
        }

        public OptionSet GetDocumentOptionSet(OptionSet baseOptionSet, string filePath)
        {
            var fileConfigurations = this.EditorConfigParser.Parse(filePath);
            if (!fileConfigurations.HasAny()) return baseOptionSet;

            var fileConfiguration = fileConfigurations.First();
            //read editorconfig settings for file and create our FormattingOptions out of them
            //FormattingOptions defaults can be specified when starting the server hence the intermediary representation
            var formattingOptions = this.GetFormattingOptions(fileConfiguration);
            //Apply file's formattingoptions to OptionSet
            var optionSet = this.ApplyFormattingOptions(baseOptionSet, formattingOptions);
            return optionSet;
        }

        private FormattingOptions GetFormattingOptions(FileConfiguration fileConfiguration)
        {
            var formattingOptions = new FormattingOptions();
            if (fileConfiguration.Charset.HasValue)
            {
                //Unsupported by default formatter
            }
            if (fileConfiguration.EndOfLine.HasValue)
            {
                var endOfLineChars = string.Empty;
                switch (fileConfiguration.EndOfLine.Value)
                {
                    case EndOfLine.CR:
                        endOfLineChars = "\r";
                        break;
                    case EndOfLine.CRLF:
                        endOfLineChars = "\r\n";
                        break;
                    case EndOfLine.LF:
                        endOfLineChars = "\n";
                        break;

                }
                formattingOptions.NewLine = endOfLineChars;
            }
            if (fileConfiguration.IndentStyle.HasValue)
            {
                formattingOptions.UseTabs = fileConfiguration.IndentStyle.Value == IndentStyle.Tab;
            }
            if (fileConfiguration.IndentSize != null)
            {
                formattingOptions.IndentationSize = fileConfiguration.IndentSize.NumberOfColumns;
                if (fileConfiguration.IndentSize.UseTabWidth)
                    formattingOptions.IndentationSize = fileConfiguration.TabWidth;
            }
            if (fileConfiguration.TabWidth.HasValue)
            {
                formattingOptions.TabSize = fileConfiguration.TabWidth.Value;
            }
            if (fileConfiguration.InsertFinalNewline.HasValue)
            {
                formattingOptions.InsertFinalNewline = fileConfiguration.InsertFinalNewline.Value;
                //Unsupported by default formatter
            }
            if (fileConfiguration.MaxLineLength.HasValue)
            {
                formattingOptions.MaxLineLength = fileConfiguration.MaxLineLength.Value;
                //unsupported by default formatter (also not available in VS extension)
            }
            if (fileConfiguration.TrimTrailingWhitespace.HasValue)
            {
                formattingOptions.TrimTrailingWhitespace = fileConfiguration.TrimTrailingWhitespace.Value;
                //Unsupported by default formatter
            }

            formattingOptions.SmartIndent = fileConfiguration.GetEnumValue<GlobalOption.IndentStyle>("smart_indent");
            formattingOptions.AllowDisjoinSpanMerging = fileConfiguration.GetBoolProperty("allow_disjoin_span_merging");
            formattingOptions.DebugMode = fileConfiguration.GetBoolProperty("debug_mode");
            formattingOptions.SpacingAfterMethodDeclarationName = fileConfiguration.GetBoolProperty("space_after_method_declaration_name");
            formattingOptions.SpaceWithinMethodDeclarationParenthesis = fileConfiguration.GetBoolProperty("space_within_method_declaration_parenthesis");
            formattingOptions.SpaceBetweenEmptyMethodDeclarationParentheses = fileConfiguration.GetBoolProperty("space_between_empty_method_declaration_parentheses");
            formattingOptions.SpaceAfterMethodCallName = fileConfiguration.GetBoolProperty("space_after_method_call_name");
            formattingOptions.SpaceWithinMethodCallParentheses = fileConfiguration.GetBoolProperty("space_within_method_call_parentheses");
            formattingOptions.SpaceBetweenEmptyMethodCallParentheses = fileConfiguration.GetBoolProperty("space_between_empty_method_call_parentheses");
            formattingOptions.SpaceAfterControlFlowStatementKeyword = fileConfiguration.GetBoolProperty("space_after_control_flow_statement_keyword");
            formattingOptions.SpaceWithinExpressionParentheses = fileConfiguration.GetBoolProperty("space_within_expression_parentheses");
            formattingOptions.SpaceWithinCastParentheses = fileConfiguration.GetBoolProperty("space_within_cast_parentheses");
            formattingOptions.SpaceWithinOtherParentheses = fileConfiguration.GetBoolProperty("space_within_other_parentheses");
            formattingOptions.SpaceAfterCast = fileConfiguration.GetBoolProperty("space_after_cast");
            formattingOptions.SpacesIgnoreAroundVariableDeclaration = fileConfiguration.GetBoolProperty("spaces_ignore_around_variable_declaration");
            formattingOptions.SpaceBeforeOpenSquareBracket = fileConfiguration.GetBoolProperty("space_before_open_square_bracket");
            formattingOptions.SpaceBetweenEmptySquareBrackets = fileConfiguration.GetBoolProperty("space_between_empty_square_bracket");
            formattingOptions.SpaceWithinSquareBrackets = fileConfiguration.GetBoolProperty("space_within_square_brackets");
            formattingOptions.SpaceAfterColonInBaseTypeDeclaration = fileConfiguration.GetBoolProperty("space_after_colon_in_base_type_declaration");
            formattingOptions.SpaceAfterComma = fileConfiguration.GetBoolProperty("space_after_comma");
            formattingOptions.SpaceAfterDot = fileConfiguration.GetBoolProperty("space_after_dot");
            formattingOptions.SpaceAfterSemicolonsInForStatement = fileConfiguration.GetBoolProperty("space_after_semicolons_in_for_statement");
            formattingOptions.SpaceBeforeColonInBaseTypeDeclaration = fileConfiguration.GetBoolProperty("space_before_colon_in_base_type_declaration");
            formattingOptions.SpaceBeforeComma = fileConfiguration.GetBoolProperty("space_before_comma");
            formattingOptions.SpaceBeforeDot = fileConfiguration.GetBoolProperty("space_before_dot");
            formattingOptions.SpaceBeforeSemicolonsInForStatement = fileConfiguration.GetBoolProperty("space_before_semicolons_in_for_statement");
            formattingOptions.SpacingAroundBinaryOperator = fileConfiguration.GetEnumValue<BinaryOperatorSpacingOptions>("spacing_around_binary_operator");
            formattingOptions.IndentBraces = fileConfiguration.GetBoolProperty("indent_braces");
            formattingOptions.IndentBlock = fileConfiguration.GetBoolProperty("indent_block");
            formattingOptions.IndentSwitchSection = fileConfiguration.GetBoolProperty("indent_switch_section");
            formattingOptions.IndentSwitchCaseSection = fileConfiguration.GetBoolProperty("indent_switch_case_section");
            formattingOptions.LabelPositioning = fileConfiguration.GetEnumValue<LabelPositionOptions>("label_positioning");
            formattingOptions.WrappingPreserveSingleLine = fileConfiguration.GetBoolProperty("wrapping_preserve_single_line");
            formattingOptions.WrappingKeepStatementsOnSingleLine = fileConfiguration.GetBoolProperty("wrapping_keep_statements_on_single_line");
            formattingOptions.NewLinesForBracesInTypes = fileConfiguration.GetBoolProperty("new_lines_for_braces_in_types");
            formattingOptions.NewLinesForBracesInMethods = fileConfiguration.GetBoolProperty("new_lines_for_braces_in_methods");
            formattingOptions.NewLinesForBracesInAnonymousMethods = fileConfiguration.GetBoolProperty("new_lines_braces_in_anonymous_methods");
            formattingOptions.NewLinesForBracesInControlBlocks = fileConfiguration.GetBoolProperty("new_lines_for_braces_in_control_blocks");
            formattingOptions.NewLinesForBracesInAnonymousTypes = fileConfiguration.GetBoolProperty("new_lines_for_braces_in_anonymous_types");
            formattingOptions.NewLinesForBracesInLambdaExpressionBody = fileConfiguration.GetBoolProperty("new_lines_for_braces_in_lambda_expression_body");
            formattingOptions.NewLineForElse = fileConfiguration.GetBoolProperty("new_line_for_else");
            formattingOptions.NewLineForCatch = fileConfiguration.GetBoolProperty("new_line_for_catch");
            formattingOptions.NewLineForFinally = fileConfiguration.GetBoolProperty("new_line_for_finally");
            formattingOptions.NewLineForMembersInObjectInit = fileConfiguration.GetBoolProperty("new_line_for_members_in_object_init");
            formattingOptions.NewLinesForBracesInObjectInitializers = fileConfiguration.GetBoolProperty("new_line_for_braces_in_object_initializers");
            formattingOptions.NewLineForMembersInAnonymousTypes = fileConfiguration.GetBoolProperty("new_line_for_members_in_anonymous_types");
            formattingOptions.NewLineForClausesInQuery = fileConfiguration.GetBoolProperty("new_line_for_clauses_in_query");
            return formattingOptions;
        }

        private OptionSet ApplyFormattingOptions(OptionSet baseOptionSet, FormattingOptions formattingOptions)
        {
            var o = baseOptionSet;
            var f = formattingOptions;
            if (!string.IsNullOrEmpty(f.NewLine))
                o = o.WithChangedOption(GlobalOption.NewLine, LanguageNames.CSharp, f.NewLine);
            if (f.UseTabs.HasValue)
                o = o.WithChangedOption(GlobalOption.UseTabs, LanguageNames.CSharp, f.UseTabs.Value);
            if (f.TabSize.HasValue)
                o = o.WithChangedOption(GlobalOption.TabSize, LanguageNames.CSharp, f.TabSize.Value);
            if (f.IndentationSize.HasValue)
                o = o.WithChangedOption(GlobalOption.IndentationSize, LanguageNames.CSharp, f.IndentationSize.Value);
            if (f.SmartIndent.HasValue)
                o = o.WithChangedOption(GlobalOption.SmartIndent, LanguageNames.CSharp, f.SmartIndent.Value);
            if (f.UseTabOnlyForIndentation.HasValue)
                o = o.WithChangedOption(GlobalOption.UseTabOnlyForIndentation, LanguageNames.CSharp, f.UseTabOnlyForIndentation.Value);
            if (f.TrimTrailingWhitespace.HasValue)
            {
                //unsupported
                //o = o.WithChangedOption(GlobalOption., LanguageNames.CSharp, f.UseTabs.Value);
            }
            if (f.InsertFinalNewline.HasValue)
            {
                //unsupported
                //o = o.WithChangedOption(GlobalOption.UseTabs, LanguageNames.CSharp, f.UseTabs.Value);
            }
            if (f.MaxLineLength.HasValue)
            {
                //unsupported
                //o = o.WithChangedOption(GlobalOption.UseTabs, LanguageNames.CSharp, f.UseTabs.Value);
            }
            if (!string.IsNullOrEmpty(f.NewLine))
                o = o.WithChangedOption(GlobalOption.NewLine, LanguageNames.CSharp, f.NewLine);

            o =  _o(o, CSharpOption.IndentBlock, f.IndentBlock);
            o =  _o(o, CSharpOption.IndentBraces, f.IndentBraces);
            o =  _o(o, CSharpOption.IndentSwitchCaseSection, f.IndentSwitchCaseSection);
            o =  _o(o, CSharpOption.IndentSwitchSection, f.IndentSwitchSection);
            o =  _o(o, CSharpOption.NewLineForCatch, f.NewLineForCatch);
            o =  _o(o, CSharpOption.NewLineForClausesInQuery, f.NewLineForClausesInQuery);
            o =  _o(o, CSharpOption.NewLineForElse, f.NewLineForElse);
            o =  _o(o, CSharpOption.NewLineForFinally, f.NewLineForFinally);
            o =  _o(o, CSharpOption.NewLineForMembersInAnonymousTypes, f.NewLineForMembersInAnonymousTypes);
            o =  _o(o, CSharpOption.NewLineForMembersInObjectInit, f.NewLineForMembersInObjectInit);
            o =  _o(o, CSharpOption.NewLinesForBracesInAnonymousMethods, f.NewLinesForBracesInAnonymousMethods);
            o =  _o(o, CSharpOption.NewLinesForBracesInAnonymousTypes, f.NewLinesForBracesInAnonymousTypes);
            o =  _o(o, CSharpOption.NewLinesForBracesInControlBlocks, f.NewLinesForBracesInControlBlocks);
            o =  _o(o, CSharpOption.NewLinesForBracesInLambdaExpressionBody, f.NewLinesForBracesInLambdaExpressionBody);
            o =  _o(o, CSharpOption.NewLinesForBracesInMethods, f.NewLinesForBracesInMethods);
            o =  _o(o, CSharpOption.NewLinesForBracesInObjectInitializers, f.NewLinesForBracesInObjectInitializers);
            o =  _o(o, CSharpOption.NewLinesForBracesInTypes, f.NewLinesForBracesInTypes);
            o =  _o(o, CSharpOption.SpaceAfterCast, f.SpaceAfterCast);
            o =  _o(o, CSharpOption.SpaceAfterColonInBaseTypeDeclaration, f.SpaceAfterColonInBaseTypeDeclaration);
            o =  _o(o, CSharpOption.SpaceAfterComma, f.SpaceAfterComma);
            o =  _o(o, CSharpOption.SpaceAfterControlFlowStatementKeyword, f.SpaceAfterControlFlowStatementKeyword);
            o =  _o(o, CSharpOption.SpaceAfterDot, f.SpaceAfterDot);
            o =  _o(o, CSharpOption.SpaceAfterMethodCallName, f.SpaceAfterMethodCallName);
            o =  _o(o, CSharpOption.SpaceAfterSemicolonsInForStatement, f.SpaceAfterSemicolonsInForStatement);
            o =  _o(o, CSharpOption.SpaceBeforeColonInBaseTypeDeclaration, f.SpaceBeforeColonInBaseTypeDeclaration);
            o =  _o(o, CSharpOption.SpaceBeforeComma, f.SpaceBeforeComma);
            o =  _o(o, CSharpOption.SpaceBeforeDot, f.SpaceBeforeDot);
            o =  _o(o, CSharpOption.SpaceBeforeOpenSquareBracket, f.SpaceBeforeOpenSquareBracket);
            o =  _o(o, CSharpOption.SpaceBeforeSemicolonsInForStatement, f.SpaceBeforeSemicolonsInForStatement);
            o =  _o(o, CSharpOption.SpaceBetweenEmptyMethodCallParentheses, f.SpaceBetweenEmptyMethodCallParentheses);
            o =  _o(o, CSharpOption.SpaceBetweenEmptyMethodDeclarationParentheses, f.SpaceBetweenEmptyMethodDeclarationParentheses);
            o =  _o(o, CSharpOption.SpaceBetweenEmptySquareBrackets, f.SpaceBetweenEmptySquareBrackets);
            o =  _o(o, CSharpOption.SpaceWithinCastParentheses, f.SpaceWithinCastParentheses);
            o =  _o(o, CSharpOption.SpaceWithinExpressionParentheses, f.SpaceWithinExpressionParentheses);
            o =  _o(o, CSharpOption.SpaceWithinMethodCallParentheses, f.SpaceWithinMethodCallParentheses);
            o =  _o(o, CSharpOption.SpaceWithinMethodDeclarationParenthesis, f.SpaceWithinMethodDeclarationParenthesis);
            o =  _o(o, CSharpOption.SpaceWithinOtherParentheses, f.SpaceWithinOtherParentheses);
            o =  _o(o, CSharpOption.SpaceWithinSquareBrackets, f.SpaceWithinSquareBrackets);
            o =  _o(o, CSharpOption.SpacesIgnoreAroundVariableDeclaration, f.SpacesIgnoreAroundVariableDeclaration);
            o =  _o(o, CSharpOption.SpacingAfterMethodDeclarationName, f.SpacingAfterMethodDeclarationName);
            o =  _o(o, CSharpOption.WrappingKeepStatementsOnSingleLine, f.WrappingKeepStatementsOnSingleLine);
            o =  _o(o, CSharpOption.WrappingPreserveSingleLine, f.WrappingPreserveSingleLine);
            if (f.LabelPositioning.HasValue)
                o = o.WithChangedOption(CSharpOption.LabelPositioning, f.LabelPositioning.Value);
            if (f.SpacingAroundBinaryOperator.HasValue)
                o = o.WithChangedOption(CSharpOption.SpacingAroundBinaryOperator, f.SpacingAroundBinaryOperator.Value);

            return o;
        }

        OptionSet _o(OptionSet o, Option<bool> option, bool? v) => v.HasValue ? o.WithChangedOption(option, v.Value) : o;
    }

    public static class FileConfigurationExtensions
    {
        public static string GetStringProperty(this FileConfiguration fileConfiguration, string key)
        {
            if (fileConfiguration == null) return null;
            string v;
            return fileConfiguration.Properties.TryGetValue(key, out v) ? v : null;
        }

        public static T? GetEnumValue<T>(this FileConfiguration fileConfiguration, string key)
            where T : struct
        {

#if ASPNETCORE50
            if (!typeof(T).GetTypeInfo().IsEnum) throw new ArgumentException("T must be an enumerated type");
#else
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
#endif
            var v = fileConfiguration.GetStringProperty(key);
            T b;
            return !Enum.TryParse(v, out b) ? (T?)b : null;
        }

        public static bool? GetBoolProperty(this FileConfiguration fileConfiguration, string key)
        {
            var v = fileConfiguration.GetStringProperty(key);
            bool b;
            return !bool.TryParse(v, out b) ? (bool?)b : null;
        }

    }
}
