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
    /// 获取第一个特性数据
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <param name="attributeTypeFullName">特性名称</param>
    /// <returns>特性数据</returns>
    public static AttributeData? GetFirstAttribute(
        this ISymbol symbol,
        string attributeTypeFullName
    )
    {
        var isGlobal = attributeTypeFullName.IsGlobalName();
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass!.GetFullName(isGlobal) == attributeTypeFullName);
    }

    /// <summary>
    /// 尝试获取第一个特性数据
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <param name="attributeTypeFullName">特性名称</param>
    /// <param name="attributeData">特性数据</param>
    /// <returns>是否获取成功</returns>
    public static bool TryGetFirstAttribute(
        this ISymbol symbol,
        string attributeTypeFullName,
        out AttributeData attributeData
    )
    {
        var isGlobal = attributeTypeFullName.IsGlobalName();
        attributeData = symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass!.GetFullName(isGlobal) == attributeTypeFullName)!;
        return attributeData is not null;
    }
}
