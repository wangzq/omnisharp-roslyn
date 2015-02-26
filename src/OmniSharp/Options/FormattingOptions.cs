using Microsoft.CodeAnalysis.CSharp.Formatting;
using MsFormatting = Microsoft.CodeAnalysis.Formatting.FormattingOptions;
using Microsoft.CodeAnalysis.Options;

namespace OmniSharp.Options
{
    public class FormattingOptions
    {
        public bool? UseTabs { get; set; }

        public int? TabSize { get; set; }

        public int? IndentationSize { get; set; }

        public MsFormatting.IndentStyle? SmartIndent { get; set; }

        public bool? UseTabOnlyForIndentation { get; set; }

        public bool? TrimTrailingWhitespace { get; set; }

        public bool? InsertFinalNewline { get; set; }

        public int? MaxLineLength { get; set; }

        public string NewLine { get; set; }

        public bool? DebugMode { get; set; }

        public bool? AllowDisjoinSpanMerging { get; set; }

        public bool? SpacingAfterMethodDeclarationName { get; set; }
        public bool? SpaceWithinMethodDeclarationParenthesis { get; set; }
        public bool? SpaceBetweenEmptyMethodDeclarationParentheses { get; set; }
        public bool? SpaceAfterMethodCallName { get; set; }
        public bool? SpaceWithinMethodCallParentheses { get; set; }
        public bool? SpaceBetweenEmptyMethodCallParentheses { get; set; }
        public bool? SpaceAfterControlFlowStatementKeyword { get; set; }
        public bool? SpaceWithinExpressionParentheses { get; set; }
        public bool? SpaceWithinCastParentheses { get; set; }
        public bool? SpaceWithinOtherParentheses { get; set; }
        public bool? SpaceAfterCast { get; set; }
        public bool? SpacesIgnoreAroundVariableDeclaration { get; set; }
        public bool? SpaceBeforeOpenSquareBracket { get; set; }
        public bool? SpaceBetweenEmptySquareBrackets { get; set; }
        public bool? SpaceWithinSquareBrackets { get; set; }
        public bool? SpaceAfterColonInBaseTypeDeclaration { get; set; }
        public bool? SpaceAfterComma { get; set; }
        public bool? SpaceAfterDot { get; set; }
        public bool? SpaceAfterSemicolonsInForStatement { get; set; }
        public bool? SpaceBeforeColonInBaseTypeDeclaration { get; set; }
        public bool? SpaceBeforeComma { get; set; }
        public bool? SpaceBeforeDot { get; set; }
        public bool? SpaceBeforeSemicolonsInForStatement { get; set; }
        public BinaryOperatorSpacingOptions? SpacingAroundBinaryOperator { get; set; }
        public bool? IndentBraces { get; set; }
        public bool? IndentBlock { get; set; }
        public bool? IndentSwitchSection { get; set; }
        public bool? IndentSwitchCaseSection { get; set; }
        public LabelPositionOptions? LabelPositioning { get; set; }
        public bool? WrappingPreserveSingleLine { get; set; }
        public bool? WrappingKeepStatementsOnSingleLine { get; set; }
        public bool? NewLinesForBracesInTypes { get; set; }
        public bool? NewLinesForBracesInMethods { get; set; }
        public bool? NewLinesForBracesInAnonymousMethods { get; set; }
        public bool? NewLinesForBracesInControlBlocks { get; set; }
        public bool? NewLinesForBracesInAnonymousTypes { get; set; }
        public bool? NewLinesForBracesInLambdaExpressionBody { get; set; }
        public bool? NewLineForElse { get; set; }
        public bool? NewLineForCatch { get; set; }
        public bool? NewLineForFinally { get; set; }
        public bool? NewLineForMembersInObjectInit { get; set; }
        public bool? NewLinesForBracesInObjectInitializers { get; set; }
        public bool? NewLineForMembersInAnonymousTypes { get; set; }
        public bool? NewLineForClausesInQuery { get; set; }
    }


}

