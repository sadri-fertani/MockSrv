using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace MockSrv.Analyzers;

[ExcludeFromCodeCoverage]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidILoggerAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "CWCA0001";

    private static readonly DiagnosticDescriptor Rule = new 
        (
            DiagnosticId,
            "Avoid ILogger<T>",
            "Avoid Using ILogger logging. Use ISanitizedLogger<T> instead.",
            "Security",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);        
        context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.GenericName, SyntaxKind.IdentifierName);
    }

    private static void AnalyzeType(SyntaxNodeAnalysisContext context)
    {
        var nameNode = context.Node;
        var symbolInfo = context.SemanticModel.GetSymbolInfo(nameNode);
        
        if (symbolInfo.Symbol is INamedTypeSymbol typeSymbol &&
            typeSymbol.Name == "ILogger" &&
            typeSymbol.ContainingNamespace.ToDisplayString() == "Microsoft.Extensions.Logging")
        {
            // Check if this is inside the SanitizedLogger class
            var containingClass = GetContainingClass(nameNode);
            if (containingClass?.Identifier.ValueText == "SanitizedLogger")
            {
                return; // Exception: SanitizedLogger is allowed to use ILogger
            }

            var diagnostic = Diagnostic.Create(Rule, nameNode.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static ClassDeclarationSyntax? GetContainingClass(SyntaxNode node)
    {
        var current = node.Parent;
        while (current != null)
        {
            if (current is ClassDeclarationSyntax classDecl)
            {
                return classDecl;
            }
            current = current.Parent;
        }
        return null;
    }
}
