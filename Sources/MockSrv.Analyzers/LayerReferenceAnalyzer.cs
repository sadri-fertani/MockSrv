using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace MockSrv.Analyzers;

[ExcludeFromCodeCoverage]
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LayerReferenceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "LAY001";

    private static readonly DiagnosticDescriptor ForbiddenReferenceRule = new
        (
            DiagnosticId,
            "Référence interdite entre couches",
            "La couche '{0}' ne peut pas référencer la couche '{1}'",
            "Architecture",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            customTags: [WellKnownDiagnosticTags.CompilationEnd]
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ForbiddenReferenceRule);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);        
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var assemblyName = context.Compilation.AssemblyName ?? "";

        var currentLayer = GetLayer(assemblyName);
        if (currentLayer == Layer.Unknown)
            return;

        foreach (var reference in context.Compilation.ReferencedAssemblyNames)
        {
            var referencedLayer = GetLayer(reference.Name);

            if (referencedLayer == Layer.Unknown)
                continue;

            if (!IsAllowedReference(currentLayer, referencedLayer))
            {
                var diagnostic = Diagnostic.Create(
                    ForbiddenReferenceRule,
                    Location.None,
                    currentLayer.ToString(),
                    referencedLayer.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private enum Layer
    {
        Unknown,
        Common,
        Domain,
        Application,
        Persistence,
        Presentation,
        Startup
    }

    private Layer GetLayer(string assemblyName)
    {
        if (assemblyName.Contains("MockSrv.Common")) return Layer.Common;
        if (assemblyName.Contains("MockSrv.Domain")) return Layer.Domain;
        if (assemblyName.Contains("MockSrv.Application")) return Layer.Application;
        if (assemblyName.Contains("MockSrv.Persistence")) return Layer.Persistence;
        if (assemblyName.Contains("MockSrv.Api")) return Layer.Presentation;
        if (assemblyName.Contains("MockSrv.Startup")) return Layer.Startup;

        return Layer.Unknown;
    }

    private bool IsAllowedReference(Layer from, Layer to)
    {
        return (from, to) switch
        {
            // Common ne dépend de personne
            (Layer.Common, Layer.Common) => true,

            // Domain → Common uniquement
            (Layer.Domain, Layer.Common) => true,
            (Layer.Domain, _) => false,

            // Application → Domain + Common
            (Layer.Application, Layer.Domain) => true,
            (Layer.Application, Layer.Common) => true,
            (Layer.Application, _) => false,

            // Persistence → Application + Domain + Common
            (Layer.Persistence, Layer.Application) => true,
            (Layer.Persistence, Layer.Domain) => true,
            (Layer.Persistence, Layer.Common) => true,
            (Layer.Persistence, _) => false,

            // Presentation → Application + Domain + Common
            (Layer.Presentation, Layer.Application) => true,
            (Layer.Presentation, Layer.Domain) => true,
            (Layer.Presentation, Layer.Common) => true,
            (Layer.Presentation, _) => false,

            // Startup → tout le monde
            (Layer.Startup, _) => true,

            _ => false
        };
    }
}
