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
    /// 获取符号类型
    /// </summary>
    /// <param name="specialType"></param>
    /// <returns></returns>
    public static ITypeSymbol GetTypeSymbol(this SpecialType specialType)
    {
        return SourceGeneratorExtensions.Compilation.GetSpecialType(specialType);
    }
}
