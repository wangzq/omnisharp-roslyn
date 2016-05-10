﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OmniSharp
{
    class ReflectionNamespaces
    {
        internal const string WorkspacesAsmName = ", Microsoft.CodeAnalysis.Workspaces";
        internal const string CSWorkspacesAsmName = ", Microsoft.CodeAnalysis.CSharp.Workspaces";
        internal const string CAAsmName = ", Microsoft.CodeAnalysis";
        internal const string CACSharpAsmName = ", Microsoft.CodeAnalysis.CSharp";
    }


    public class CSharpSyntaxContext
    {
        readonly static Type typeInfoCSharpSyntaxContext;
        readonly static Type typeInfoAbstractSyntaxContext;
        readonly static MethodInfo createContextMethod;
        readonly static PropertyInfo leftTokenProperty;
        readonly static PropertyInfo targetTokenProperty;
        readonly static FieldInfo isIsOrAsTypeContextField;
        readonly static FieldInfo isInstanceContextField;
        readonly static FieldInfo isNonAttributeExpressionContextField;
        readonly static FieldInfo isPreProcessorKeywordContextField;
        readonly static FieldInfo isPreProcessorExpressionContextField;
        readonly static FieldInfo containingTypeDeclarationField;
        readonly static FieldInfo isGlobalStatementContextField;
        readonly static FieldInfo isParameterTypeContextField;
        readonly static PropertyInfo syntaxTreeProperty;


        object instance;

        public SyntaxToken LeftToken
        {
            get
            {
                return (SyntaxToken)leftTokenProperty.GetValue(instance);
            }
        }

        public SyntaxToken TargetToken
        {
            get
            {
                return (SyntaxToken)targetTokenProperty.GetValue(instance);
            }
        }

        public bool IsIsOrAsTypeContext
        {
            get
            {
                return (bool)isIsOrAsTypeContextField.GetValue(instance);
            }
        }

        public bool IsInstanceContext
        {
            get
            {
                return (bool)isInstanceContextField.GetValue(instance);
            }
        }

        public bool IsNonAttributeExpressionContext
        {
            get
            {
                return (bool)isNonAttributeExpressionContextField.GetValue(instance);
            }
        }

        public bool IsPreProcessorKeywordContext
        {
            get
            {
                return (bool)isPreProcessorKeywordContextField.GetValue(instance);
            }
        }

        public bool IsPreProcessorExpressionContext
        {
            get
            {
                return (bool)isPreProcessorExpressionContextField.GetValue(instance);
            }
        }

        public TypeDeclarationSyntax ContainingTypeDeclaration
        {
            get
            {
                return (TypeDeclarationSyntax)containingTypeDeclarationField.GetValue(instance);
            }
        }

        public bool IsGlobalStatementContext
        {
            get
            {
                return (bool)isGlobalStatementContextField.GetValue(instance);
            }
        }

        public bool IsParameterTypeContext
        {
            get
            {
                return (bool)isParameterTypeContextField.GetValue(instance);
            }
        }

        public SyntaxTree SyntaxTree
        {
            get
            {
                return (SyntaxTree)syntaxTreeProperty.GetValue(instance);
            }
        }


        readonly static MethodInfo isMemberDeclarationContextMethod;

        public bool IsMemberDeclarationContext(
            ISet<SyntaxKind> validModifiers = null,
            ISet<SyntaxKind> validTypeDeclarations = null,
            bool canBePartial = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return (bool)isMemberDeclarationContextMethod.Invoke(instance, new object[] {
                        validModifiers,
                        validTypeDeclarations,
                        canBePartial,
                        cancellationToken
                    });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        readonly static MethodInfo isTypeDeclarationContextMethod;

        public bool IsTypeDeclarationContext(
            ISet<SyntaxKind> validModifiers = null,
            ISet<SyntaxKind> validTypeDeclarations = null,
            bool canBePartial = false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                return (bool)isTypeDeclarationContextMethod.Invoke(instance, new object[] {
                        validModifiers,
                        validTypeDeclarations,
                        canBePartial,
                        cancellationToken
                    });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        readonly static PropertyInfo isPreProcessorDirectiveContextProperty;

        public bool IsPreProcessorDirectiveContext
        {
            get
            {
                return (bool)isPreProcessorDirectiveContextProperty.GetValue(instance);
            }
        }

        readonly static FieldInfo isInNonUserCodeField;

        public bool IsInNonUserCode
        {
            get
            {
                return (bool)isInNonUserCodeField.GetValue(instance);
            }
        }

        readonly static FieldInfo isIsOrAsContextField;

        public bool IsIsOrAsContext
        {
            get
            {
                return (bool)isIsOrAsContextField.GetValue(instance);
            }
        }

        readonly static MethodInfo isTypeAttributeContextMethod;

        public bool IsTypeAttributeContext(CancellationToken cancellationToken)
        {
            try
            {
                return (bool)isTypeAttributeContextMethod.Invoke(instance, new object[] { cancellationToken });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }
        }

        readonly static PropertyInfo isAnyExpressionContextProperty;

        public bool IsAnyExpressionContext
        {
            get
            {
                return (bool)isAnyExpressionContextProperty.GetValue(instance);
            }
        }

        readonly static PropertyInfo isStatementContextProperty;

        public bool IsStatementContext
        {
            get
            {
                return (bool)isStatementContextProperty.GetValue(instance);
            }
        }

        readonly static FieldInfo isDefiniteCastTypeContextField;

        public bool IsDefiniteCastTypeContext
        {
            get
            {
                return (bool)isDefiniteCastTypeContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isObjectCreationTypeContextField;

        public bool IsObjectCreationTypeContext
        {
            get
            {
                return (bool)isObjectCreationTypeContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isGenericTypeArgumentContextField;

        public bool IsGenericTypeArgumentContext
        {
            get
            {
                return (bool)isGenericTypeArgumentContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isLocalVariableDeclarationContextField;

        public bool IsLocalVariableDeclarationContext
        {
            get
            {
                return (bool)isLocalVariableDeclarationContextField.GetValue(instance);
            }
        }


        readonly static FieldInfo isFixedVariableDeclarationContextField;

        public bool IsFixedVariableDeclarationContext
        {
            get
            {
                return (bool)isFixedVariableDeclarationContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isPossibleLambdaOrAnonymousMethodParameterTypeContextField;

        public bool IsPossibleLambdaOrAnonymousMethodParameterTypeContext
        {
            get
            {
                return (bool)isPossibleLambdaOrAnonymousMethodParameterTypeContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isImplicitOrExplicitOperatorTypeContextField;

        public bool IsImplicitOrExplicitOperatorTypeContext
        {
            get
            {
                return (bool)isImplicitOrExplicitOperatorTypeContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isPrimaryFunctionExpressionContextField;

        public bool IsPrimaryFunctionExpressionContext
        {
            get
            {
                return (bool)isPrimaryFunctionExpressionContextField.GetValue(instance);
            }
        }


        readonly static FieldInfo isCrefContextField;

        public bool IsCrefContext
        {
            get
            {
                return (bool)isCrefContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isDelegateReturnTypeContextField;

        public bool IsDelegateReturnTypeContext
        {
            get
            {
                return (bool)isDelegateReturnTypeContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isEnumBaseListContextField;

        public bool IsEnumBaseListContext
        {
            get
            {
                return (bool)isEnumBaseListContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo isConstantExpressionContextField;

        public bool IsConstantExpressionContext
        {
            get
            {
                return (bool)isConstantExpressionContextField.GetValue(instance);
            }
        }

        readonly static MethodInfo isMemberAttributeContextMethod;
        public bool IsMemberAttributeContext(ISet<SyntaxKind> validTypeDeclarations, CancellationToken cancellationToken)
        {
            try
            {
                return (bool)isMemberAttributeContextMethod.Invoke(instance, new object[] {
                        validTypeDeclarations,
                        cancellationToken
                    });
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return false;
            }

        }

        readonly static FieldInfo precedingModifiersField;

        public ISet<SyntaxKind> PrecedingModifiers
        {
            get
            {
                return (ISet<SyntaxKind>)precedingModifiersField.GetValue(instance);
            }
        }

        readonly static FieldInfo isTypeOfExpressionContextField;

        public bool IsTypeOfExpressionContext
        {
            get
            {
                return (bool)isTypeOfExpressionContextField.GetValue(instance);
            }
        }

        readonly static FieldInfo containingTypeOrEnumDeclarationField;

        public BaseTypeDeclarationSyntax ContainingTypeOrEnumDeclaration
        {
            get
            {
                return (BaseTypeDeclarationSyntax)containingTypeOrEnumDeclarationField.GetValue(instance);
            }
        }
        static readonly PropertyInfo isAttributeNameContextProperty;

        public bool IsAttributeNameContext
        {
            get
            {
                return (bool)isAttributeNameContextProperty.GetValue(instance);
            }
        }

        static readonly PropertyInfo isInQueryProperty;
        public bool IsInQuery
        {
            get
            {
                return (bool)isInQueryProperty.GetValue(instance);
            }
        }


        static CSharpSyntaxContext()
        {
            typeInfoAbstractSyntaxContext = Type.GetType("Microsoft.CodeAnalysis.Shared.Extensions.ContextQuery.AbstractSyntaxContext" + ReflectionNamespaces.WorkspacesAsmName, true);
            typeInfoCSharpSyntaxContext = Type.GetType("Microsoft.CodeAnalysis.CSharp.Extensions.ContextQuery.CSharpSyntaxContext" + ReflectionNamespaces.CSWorkspacesAsmName, true);

            createContextMethod = typeInfoCSharpSyntaxContext.GetMethod("CreateContext", BindingFlags.Static | BindingFlags.Public);
            leftTokenProperty = typeInfoAbstractSyntaxContext.GetProperty("LeftToken");
            targetTokenProperty = typeInfoAbstractSyntaxContext.GetProperty("TargetToken");
            isIsOrAsTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsIsOrAsTypeContext");
            isInstanceContextField = typeInfoCSharpSyntaxContext.GetField("IsInstanceContext");
            isNonAttributeExpressionContextField = typeInfoCSharpSyntaxContext.GetField("IsNonAttributeExpressionContext");
            isPreProcessorKeywordContextField = typeInfoCSharpSyntaxContext.GetField("IsPreProcessorKeywordContext");
            isPreProcessorExpressionContextField = typeInfoCSharpSyntaxContext.GetField("IsPreProcessorExpressionContext");
            containingTypeDeclarationField = typeInfoCSharpSyntaxContext.GetField("ContainingTypeDeclaration");
            isGlobalStatementContextField = typeInfoCSharpSyntaxContext.GetField("IsGlobalStatementContext");
            isParameterTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsParameterTypeContext");
            isMemberDeclarationContextMethod = typeInfoCSharpSyntaxContext.GetMethod("IsMemberDeclarationContext", BindingFlags.Instance | BindingFlags.Public);
            isTypeDeclarationContextMethod = typeInfoCSharpSyntaxContext.GetMethod("IsTypeDeclarationContext", BindingFlags.Instance | BindingFlags.Public);
            syntaxTreeProperty = typeInfoAbstractSyntaxContext.GetProperty("SyntaxTree");
            isPreProcessorDirectiveContextProperty = typeInfoAbstractSyntaxContext.GetProperty("IsPreProcessorDirectiveContext");
            isInNonUserCodeField = typeInfoCSharpSyntaxContext.GetField("IsInNonUserCode");
            isIsOrAsContextField = typeInfoCSharpSyntaxContext.GetField("IsIsOrAsContext");
            isTypeAttributeContextMethod = typeInfoCSharpSyntaxContext.GetMethod("IsTypeAttributeContext", BindingFlags.Instance | BindingFlags.Public);
            isAnyExpressionContextProperty = typeInfoAbstractSyntaxContext.GetProperty("IsAnyExpressionContext");
            isStatementContextProperty = typeInfoAbstractSyntaxContext.GetProperty("IsStatementContext");
            isDefiniteCastTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsDefiniteCastTypeContext");
            isObjectCreationTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsObjectCreationTypeContext");
            isGenericTypeArgumentContextField = typeInfoCSharpSyntaxContext.GetField("IsGenericTypeArgumentContext");
            isLocalVariableDeclarationContextField = typeInfoCSharpSyntaxContext.GetField("IsLocalVariableDeclarationContext");
            isFixedVariableDeclarationContextField = typeInfoCSharpSyntaxContext.GetField("IsFixedVariableDeclarationContext");
            isPossibleLambdaOrAnonymousMethodParameterTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsPossibleLambdaOrAnonymousMethodParameterTypeContext");
            isImplicitOrExplicitOperatorTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsImplicitOrExplicitOperatorTypeContext");
            isPrimaryFunctionExpressionContextField = typeInfoCSharpSyntaxContext.GetField("IsPrimaryFunctionExpressionContext");
            isCrefContextField = typeInfoCSharpSyntaxContext.GetField("IsCrefContext");
            isDelegateReturnTypeContextField = typeInfoCSharpSyntaxContext.GetField("IsDelegateReturnTypeContext");
            isEnumBaseListContextField = typeInfoCSharpSyntaxContext.GetField("IsEnumBaseListContext");
            isConstantExpressionContextField = typeInfoCSharpSyntaxContext.GetField("IsConstantExpressionContext");
            isMemberAttributeContextMethod = typeInfoCSharpSyntaxContext.GetMethod("IsMemberAttributeContext", BindingFlags.Instance | BindingFlags.Public);
            precedingModifiersField = typeInfoCSharpSyntaxContext.GetField("PrecedingModifiers");
            isTypeOfExpressionContextField = typeInfoCSharpSyntaxContext.GetField("IsTypeOfExpressionContext");
            containingTypeOrEnumDeclarationField = typeInfoCSharpSyntaxContext.GetField("ContainingTypeOrEnumDeclaration");

            isAttributeNameContextProperty = typeInfoAbstractSyntaxContext.GetProperty("IsAttributeNameContext");
            isInQueryProperty = typeInfoAbstractSyntaxContext.GetProperty("IsInQuery");
        }

        public SemanticModel SemanticModel
        {
            get;
            private set;
        }

        public int Position
        {
            get;
            private set;
        }

        CSharpSyntaxContext(object instance)
        {
            this.instance = instance;
        }

        public static CSharpSyntaxContext CreateContext(Workspace workspace, SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            try
            {
                return new CSharpSyntaxContext(createContextMethod.Invoke(null, new object[] {
                        workspace,
                        semanticModel,
                        position,
                        cancellationToken
                    }))
                {
                    SemanticModel = semanticModel,
                    Position = position
                };
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                return null;
            }
        }
    }




    class CSharpTypeInferenceService
    {
        readonly static Type typeInfo;
        readonly static MethodInfo inferTypesMethod;
        readonly static MethodInfo inferTypes2Method;
        readonly object instance;

        static CSharpTypeInferenceService()
        {
            typeInfo = Type.GetType("Microsoft.CodeAnalysis.CSharp.CSharpTypeInferenceService" + ReflectionNamespaces.CSWorkspacesAsmName, true);

            inferTypesMethod = typeInfo.GetMethod("InferTypes", new[] { typeof(SemanticModel), typeof(int), typeof(CancellationToken) });
            inferTypes2Method = typeInfo.GetMethod("InferTypes", new[] { typeof(SemanticModel), typeof(SyntaxNode), typeof(CancellationToken) });
        }

        public CSharpTypeInferenceService()
        {
            instance = Activator.CreateInstance(typeInfo);
        }

        public IEnumerable<ITypeSymbol> InferTypes(SemanticModel semanticModel, int position, CancellationToken cancellationToken)
        {
            return (IEnumerable<ITypeSymbol>)inferTypesMethod.Invoke(instance, new object[] { semanticModel, position, cancellationToken });
        }

        public IEnumerable<ITypeSymbol> InferTypes(SemanticModel semanticModel, SyntaxNode expression, CancellationToken cancellationToken)
        {
            return (IEnumerable<ITypeSymbol>)inferTypes2Method.Invoke(instance, new object[] { semanticModel, expression, cancellationToken });
        }
    }

    class CaseCorrector
    {
        readonly static Type typeInfo;
        readonly static MethodInfo caseCorrectAsyncMethod;

        static CaseCorrector()
        {
            typeInfo = Type.GetType("Microsoft.CodeAnalysis.CaseCorrection.CaseCorrector" + ReflectionNamespaces.WorkspacesAsmName, true);

            Annotation = (SyntaxAnnotation)typeInfo.GetField("Annotation", BindingFlags.Public | BindingFlags.Static).GetValue(null);

            caseCorrectAsyncMethod = typeInfo.GetMethod("CaseCorrectAsync", new[] { typeof(Document), typeof(SyntaxAnnotation), typeof(CancellationToken) });
        }

        public static readonly SyntaxAnnotation Annotation;

        public static Task<Document> CaseCorrectAsync(Document document, SyntaxAnnotation annotation, CancellationToken cancellationToken)
        {
            return (Task<Document>)caseCorrectAsyncMethod.Invoke(null, new object[] { document, annotation, cancellationToken });
        }
    }

    class SpeculationAnalyzer
    {
        readonly static Type typeInfo;
        readonly static MethodInfo symbolsForOriginalAndReplacedNodesAreCompatibleMethod;
        readonly static MethodInfo replacementChangesSemanticsMethod;
        readonly object instance;

        static SpeculationAnalyzer()
        {
            Type[] abstractSpeculationAnalyzerGenericParams = new[]
            {
                Type.GetType("Microsoft.CodeAnalysis.SyntaxNode" + ReflectionNamespaces.CAAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.TypeSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.AttributeSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.ForEachStatementSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.CSharp.Syntax.ThrowStatementSyntax" + ReflectionNamespaces.CACSharpAsmName, true),
                Type.GetType("Microsoft.CodeAnalysis.SemanticModel" + ReflectionNamespaces.CAAsmName, true)
            };
            typeInfo = Type.GetType("Microsoft.CodeAnalysis.Shared.Utilities.AbstractSpeculationAnalyzer`8" + ReflectionNamespaces.WorkspacesAsmName, true)
                .MakeGenericType(abstractSpeculationAnalyzerGenericParams);

            symbolsForOriginalAndReplacedNodesAreCompatibleMethod = typeInfo.GetMethod("SymbolsForOriginalAndReplacedNodesAreCompatible", BindingFlags.Public | BindingFlags.Instance);
            replacementChangesSemanticsMethod = typeInfo.GetMethod("ReplacementChangesSemantics", BindingFlags.Public | BindingFlags.Instance);

            typeInfo = Type.GetType("Microsoft.CodeAnalysis.CSharp.Utilities.SpeculationAnalyzer" + ReflectionNamespaces.CSWorkspacesAsmName, true);
        }

        public SpeculationAnalyzer(ExpressionSyntax expression, ExpressionSyntax newExpression, SemanticModel semanticModel, CancellationToken cancellationToken, bool skipVerificationForReplacedNode = false, bool failOnOverloadResolutionFailuresInOriginalCode = false)
        {
            instance = Activator.CreateInstance(typeInfo, new object[] { expression, newExpression, semanticModel, cancellationToken, skipVerificationForReplacedNode, failOnOverloadResolutionFailuresInOriginalCode });
        }

        public bool SymbolsForOriginalAndReplacedNodesAreCompatible()
        {
            return (bool)symbolsForOriginalAndReplacedNodesAreCompatibleMethod.Invoke(instance, new object[0]);
        }

        public bool ReplacementChangesSemantics()
        {
            return (bool)replacementChangesSemanticsMethod.Invoke(instance, new object[0]);
        }
    }
}
