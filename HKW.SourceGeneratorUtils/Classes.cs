using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 组件信息
/// </summary>
/// <param name="productionContext">生产环境</param>
/// <param name="compilation">编译</param>
public readonly struct AssemblyInfo(
    SourceProductionContext productionContext,
    Compilation compilation
)
{
    /// <summary>
    /// 生产环境
    /// </summary>
    public readonly SourceProductionContext ProductionContext { get; } = productionContext;

    /// <summary>
    /// 编译
    /// </summary>
    public readonly Compilation Compilation { get; } = compilation;
}

/// <summary>
/// 语法树及其语义模型信息
/// </summary>
/// <param name="syntaxTree">语法树</param>
/// <param name="semanticModel">语义模型</param>
public readonly struct SyntaxTreeInfo(SyntaxTree syntaxTree, SemanticModel semanticModel)
{
    /// <summary>
    /// 语法树
    /// </summary>
    public readonly SyntaxTree SyntaxTree { get; } = syntaxTree;

    /// <summary>
    /// 语义模型
    /// </summary>
    public readonly SemanticModel SemanticModel { get; } = semanticModel;
}

/// <summary>
/// 属性语法及其符号信息
/// </summary>
/// <param name="syntax">属性语法</param>
/// <param name="symbol">属性符号</param>
public readonly struct PropertySS(PropertyDeclarationSyntax syntax, IPropertySymbol symbol)
{
    /// <summary>
    /// 属性语法
    /// </summary>
    public readonly PropertyDeclarationSyntax Syntax { get; } = syntax;

    /// <summary>
    /// 属性符号
    /// </summary>
    public readonly IPropertySymbol Symbol { get; } = symbol;

    /// <summary>
    /// 输出属性语法和属性符号
    /// </summary>
    /// <param name="propertySyntax">属性语法</param>
    /// <param name="propertySymbol">属性符号</param>
    public void OutData(
        out PropertyDeclarationSyntax propertySyntax,
        out IPropertySymbol propertySymbol
    )
    {
        propertySyntax = Syntax;
        propertySymbol = Symbol;
    }
}

/// <summary>
/// 方法语法及其符号信息
/// </summary>
/// <param name="syntax">方法语法</param>
/// <param name="symbol">方法符号</param>
public readonly struct MethodSS(MethodDeclarationSyntax syntax, IMethodSymbol symbol)
{
    /// <summary>
    /// 方法语法
    /// </summary>
    public readonly MethodDeclarationSyntax Syntax { get; } = syntax;

    /// <summary>
    /// 方法符号
    /// </summary>
    public readonly IMethodSymbol Symbol { get; } = symbol;

    /// <summary>
    /// 输出方法语法和方法符号
    /// </summary>
    /// <param name="methodSyntax">方法语法</param>
    /// <param name="methodSymbol">方法符号</param>
    public void OutData(out MethodDeclarationSyntax methodSyntax, out IMethodSymbol methodSymbol)
    {
        methodSyntax = Syntax;
        methodSymbol = Symbol;
    }
}
