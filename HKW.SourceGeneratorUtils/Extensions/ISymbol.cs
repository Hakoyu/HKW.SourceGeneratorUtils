using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class ISymbolExtensions
{
    /// <summary>
    /// 获取最低访问性字符串
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="typeSymbols"></param>
    /// <returns></returns>
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
