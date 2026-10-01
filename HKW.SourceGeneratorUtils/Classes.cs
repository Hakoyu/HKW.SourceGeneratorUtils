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
