using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 成员构建信息接口
/// </summary>
public interface IMemberGenerateInfo
{
    /// <summary>
    /// 注释
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// 特性
    /// </summary>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <summary>
    /// 添加默认特性
    /// </summary>
    public bool AddDefaultAttributes { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 写入至
    /// </summary>
    /// <param name="writer">写入器</param>
    public void WriteTo(IndentedTextWriter writer);
}
