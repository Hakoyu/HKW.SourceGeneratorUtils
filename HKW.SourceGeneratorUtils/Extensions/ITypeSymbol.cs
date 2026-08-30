using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class ITypeSymbolExtensions
{
    /// <summary>
    /// 符号比较
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <param name="otherSymbol">另一个符号</param>
    /// <returns>是否相等</returns>
    public static bool SymbolEquals(this ISymbol symbol, ISymbol otherSymbol)
    {
        return SymbolEqualityComparer.Default.Equals(symbol, otherSymbol);
    }

    /// <summary>
    /// 获取任务返回值类型
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <returns>返回值类型</returns>
    public static ITypeSymbol GetTaskReturnType(this ISymbol typeSymbol)
    {
        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: 1 } namedTypeSymbol)
            return namedTypeSymbol.TypeArguments[0];
        return SourceGeneratorHelper.TypeVoid;
    }

    /// <summary>
    /// 是空类型
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <returns>是否空类型</returns>
    public static bool IsVoid(this ISymbol symbol)
    {
        return SymbolEqualityComparer.Default.Equals(symbol, SourceGeneratorHelper.TypeVoid);
    }

    /// <summary>
    /// 获取任务返回值
    /// </summary>
    /// <param name="symbol">符号</param>
    /// <returns>如果类型是 <see cref="Task{T}"/> 则返回结果, 否则返回 <see langword="null"/></returns>
    public static INamedTypeSymbol? GetTaskResult(this ITypeSymbol symbol)
    {
        var currentType = symbol;
        while (currentType != null)
        {
            if (
                currentType.OriginalDefinition?.ToString()
                == SourceGeneratorHelper.TaskResultFullName
            )
                return ((INamedTypeSymbol)currentType).TypeArguments[0] as INamedTypeSymbol;
            currentType = currentType.BaseType;
        }
        return null;
    }

    /// <summary>
    /// 获取点替换为下划线的全名
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <returns>全名</returns>
    public static string GetUnderlineFullName(this ITypeSymbol typeSymbol)
    {
        return $"{typeSymbol.ContainingNamespace.ToString().Replace('.', '_')}_{typeSymbol.Name}";
    }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <param name="format">格式化</param>
    /// <returns>名称</returns>
    public static string GetName(this ITypeSymbol typeSymbol, SymbolDisplayFormat? format = null)
    {
        return format is null
            ? typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            : typeSymbol.ToDisplayString(format);
    }

    /// <summary>
    /// 继承自基类类型
    /// </summary>
    /// <param name="typeSymbol">符号类型</param>
    /// <param name="baseTypeFullName">基类全名</param>
    /// <param name="symbolDisplayFormat">显示名称格式</param>
    /// <returns>当类型继承基类时为 <see langword="true"/> 未继承为 <see langword="false"/></returns>
    public static bool InheritedFrom(
        this ITypeSymbol typeSymbol,
        string baseTypeFullName,
        SymbolDisplayFormat? symbolDisplayFormat = null
    )
    {
        var currentType = typeSymbol;
        var typeName = symbolDisplayFormat is null
            ? currentType.ToString()
            : currentType.ToDisplayString(symbolDisplayFormat);
        while (currentType != null)
        {
            if (typeName == baseTypeFullName)
                return true;
            currentType = currentType.BaseType;
        }
        return false;
    }
}
