using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace MockSrv.Analyzers.UnitTests;

public class AvoidILoggerAnalyzerUnitTests
{
    [Obsolete]
    [Fact]
    public async Task AvoidILoggerAnalyzer_Should_Report_Diagnostic_When_ILogger_Is_Used()
    {
        var testCode = @"
using Microsoft.Extensions.Logging;

public class MyClass
{
    private readonly ILogger<MyClass> _logger;

    public MyClass(ILogger<MyClass> logger)
    {
        _logger = logger;
    }
}";

        var test = new CSharpAnalyzerTest<AvoidILoggerAnalyzer, XUnitVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("Microsoft.Extensions.Logging.Abstractions", "8.0.0")])
        };

        test.ExpectedDiagnostics.Add
            (
                new DiagnosticResult(AvoidILoggerAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(6, 22, 6, 38)
                    .WithMessage("Avoid Using ILogger logging. Use ISanitizedLogger<T> instead.")
            );

        test.ExpectedDiagnostics.Add
            (
                new DiagnosticResult(AvoidILoggerAnalyzer.DiagnosticId, DiagnosticSeverity.Error)
                    .WithSpan(8, 20, 8, 36)
                    .WithMessage("Avoid Using ILogger logging. Use ISanitizedLogger<T> instead.")
            );

        await test.RunAsync();
    }

    [Obsolete]
    [Fact]
    public async Task AvoidILoggerAnalyzer_Should_Not_Report_Diagnostic_When_ILogger_Is_Used_In_SanitizedLogger_Class()
    {
        var testCode = @"
using Microsoft.Extensions.Logging;

public class SanitizedLogger
{
    private readonly ILogger<SanitizedLogger> _logger;

    public SanitizedLogger(ILogger<SanitizedLogger> logger)
    {
        _logger = logger;
    }
}";

        var test = new CSharpAnalyzerTest<AvoidILoggerAnalyzer, XUnitVerifier>
        {
            TestCode = testCode,
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80.AddPackages([new PackageIdentity("Microsoft.Extensions.Logging.Abstractions", "8.0.0")])
        };

        // No expected diagnostics for SanitizedLogger class
        await test.RunAsync();
    }
}
