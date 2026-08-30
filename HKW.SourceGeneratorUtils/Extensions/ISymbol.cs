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
    /// 获取第一个特性数据
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <param name="attributeName">特性名称</param>
    /// <returns>特性数据</returns>
    public static AttributeData? GetFirstAttribute(this ISymbol symbol, string attributeName)
    {
        return symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass!.ToString() == attributeName);
    }

    /// <summary>
    /// 尝试获取第一个特征数据
    /// </summary>
    /// <param name="symbol">符号类型</param>
    /// <param name="attributeName">特征名称</param>
    /// <param name="attributeData">特征数据</param>
    /// <returns>是否获取成功</returns>
    public static bool TryGetFirstAttribute(
        this ISymbol symbol,
        string attributeName,
        out AttributeData attributeData
    )
    {
        attributeData = symbol
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass!.ToString() == attributeName)!;
        return attributeData is not null;
    }
}
