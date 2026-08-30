using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class INamedTypeSymbolExtensions
{
    /// <summary>
    /// 获取最低访问性
    /// </summary>
    /// <param name="typeSymbol">类型符号</param>
    /// <param name="typeSymbols">类型符号</param>
    /// <returns>最低访问性</returns>
    public static Accessibility GetLowestAccessibility(
        this INamedTypeSymbol typeSymbol,
        params INamedTypeSymbol[] typeSymbols
    )
    {
        var lower = typeSymbol;
        foreach (var symbol in typeSymbols)
            lower = lower.DeclaredAccessibility > symbol.DeclaredAccessibility ? symbol : lower;
        return lower.DeclaredAccessibility;
    }
}
