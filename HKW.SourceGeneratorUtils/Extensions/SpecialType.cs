using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class SpecialTypeExtensions
{
    /// <summary>
    /// 获取特殊符号类型
    /// </summary>
    /// <param name="specialType">特殊类型</param>
    /// <returns>符号类型</returns>
    public static ITypeSymbol GetTypeSymbol(this SpecialType specialType)
    {
        return GeneratorHelper.Compilation.GetSpecialType(specialType);
    }
}
