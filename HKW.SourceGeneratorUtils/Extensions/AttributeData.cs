using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
///
/// </summary>
public static class AttributeDataExtensions
{
    /// <summary>
    /// 获取特性参数值
    /// </summary>
    /// <param name="attributeData">特性数据</param>
    /// <returns>特性参数字典</returns>
    public static AttributeParamDictionary GetParams(this AttributeData attributeData)
    {
        return new AttributeParamDictionary(attributeData);
    }
}
