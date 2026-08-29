using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HKW.SourceGeneratorUtils;

public readonly struct AssemblyInfo(
    SourceProductionContext productionContext,
    Compilation compilation
)
{
    public readonly SourceProductionContext ProductionContext { get; } = productionContext;
    public readonly Compilation Compilation { get; } = compilation;
}

public readonly struct SyntaxTreeInfo(SyntaxTree syntaxTree, SemanticModel semanticModel)
{
    public readonly SyntaxTree SyntaxTree { get; } = syntaxTree;
    public readonly SemanticModel SemanticModel { get; } = semanticModel;
}

public readonly struct PropertySS(PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
{
    public readonly PropertyDeclarationSyntax Syntax { get; } = syntax;
    public readonly IPropertySymbol Symbol { get; } = symbol;

    public void OutData(
        out PropertyDeclarationSyntax propertySyntax,
        out IPropertySymbol propertySymbol
    )
    {
        propertySyntax = Syntax;
        propertySymbol = Symbol;
    }
}

public readonly struct MethodSS(MethodDeclarationSyntax syntax, IMethodSymbol symbol)
{
    public readonly MethodDeclarationSyntax Syntax { get; } = syntax;
    public readonly IMethodSymbol Symbol { get; } = symbol;

    public void OutData(out MethodDeclarationSyntax methodSyntax, out IMethodSymbol methodSymbol)
    {
        methodSyntax = Syntax;
        methodSymbol = Symbol;
    }
}
