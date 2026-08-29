using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using HKW.SourceGeneratorUtils.Extensions;
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
    /// 名称
    /// </summary>
    public string Name { get; set; }

    //public INamespaceSymbol Namespace { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ITypeSymbol? Type { get; set; }

    /// <summary>
    /// 类型名称, 若 <see cref="Type"/> 存在则使用 <see cref="Type"/> 生成名称
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; }
}

/// <summary>
/// 方法构建信息
/// </summary>
public class MethodGenerateInfo : IMemberGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="content">内容</param>
    public MethodGenerateInfo(string name, ITypeSymbol type, string content)
    {
        Name = name;
        Type = type;
        Content = content;
    }

    /// <inheritdoc/>
    public string Comment { get; set; } = string.Empty;

    /// <inheritdoc/>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public ITypeSymbol? Type { get; set; }

    /// <inheritdoc/>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    public List<ParameterGenerateInfo>? Params { get; set; }

    /// <summary>
    /// 约束
    /// </summary>
    public List<GenericConstraintInfo>? Constraints { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 方法类型
    /// </summary>
    public MethodType MethodType { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringStream = new StringWriter();
        var writer = new IndentedTextWriter(stringStream);
        WriteTo(writer);
        return stringStream.ToString();
    }

    /// <summary>
    /// 写入至
    /// </summary>
    /// <param name="writer">写入器</param>
    public void WriteTo(IndentedTextWriter writer)
    {
        writer.WriteLine();
        writer.Indent++;

        writer.WriteLineCollection(Comment.SplitLine());
        writer.WriteLineCollection(Attributes);
        var typeName = Type is null ? TypeName : Type.GetName();
        if (MethodType == MethodType.Partial)
        {
            writer.Write(MethodType.ToStr());
            writer.Write(' ');
            writer.Write(typeName);
            writer.Write(' ');
            writer.Write(Name);
            writer.Write('(');
            writer.WriteCollection(Params, ",");
            writer.Write(')');
            writer.Write(';');
        }
        else
        {
            writer.WriteIf(Accessibility.ToStr(), " ");
            writer.WriteIf(MethodType.ToStr(), " ");
            writer.Write(typeName);
            writer.Write(' ');
            writer.Write(Name);
            writer.Write('(');
            writer.WriteCollection(Params, ",");
            writer.Write(')');
            writer.WriteLine();

            writer.Indent++;
            writer.WriteLineCollection(Constraints);
            writer.Indent--;

            writer.WriteLine("{");
            writer.Indent++;
            writer.WriteLine(Content);
            writer.Indent--;
            writer.WriteLine("}");
        }
        writer.Indent--;
    }
}

/// <summary>
/// 特性信息
/// </summary>
public class AttributeGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="typeName">类型名称</param>
    public AttributeGenerateInfo(string typeName)
    {
        TypeName = typeName;
    }

    /// <inheritdoc/>
    /// <param name="typeName">类型名称</param>
    /// <param name="params">参数</param>
    public AttributeGenerateInfo(string typeName, params ParameterGenerateInfo[] @params)
    {
        TypeName = typeName;
        Params = new(@params);
    }

    /// <inheritdoc/>
    /// <param name="type">类型</param>
    public AttributeGenerateInfo(ITypeSymbol type)
    {
        Type = type;
    }

    /// <inheritdoc/>
    /// <param name="type">类型</param>
    /// <param name="params">参数</param>
    public AttributeGenerateInfo(ITypeSymbol type, params ParameterGenerateInfo[] @params)
    {
        Type = type;
        Params = new(@params);
    }

    /// <summary>
    /// 类型
    /// </summary>
    public ITypeSymbol? Type { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 参数
    /// </summary>
    public List<ParameterGenerateInfo>? Params { get; } = null;

    /// <inheritdoc/>
    public override string ToString()
    {
        var typeName = Type is null ? TypeName : Type.GetName();
        if (Params is null || Params.Count == 0)
            return $"[{typeName}]";
        else
            return $"[{typeName}({string.Join(",", Params)})]";
    }
}

/// <summary>
/// 参数信息
/// </summary>
public class ParameterGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    public ParameterGenerateInfo(string name, ITypeSymbol type)
    {
        Name = name;
        Type = type;
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        string name,
        ITypeSymbol type,
        params AttributeGenerateInfo[] attributes
    )
    {
        Name = name;
        Type = type;
        Attributes = new(attributes);
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    public ParameterGenerateInfo(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    /// <param name="attributes">特性</param>
    public ParameterGenerateInfo(
        string name,
        string typeName,
        params AttributeGenerateInfo[] attributes
    )
    {
        Name = name;
        TypeName = typeName;
        Attributes = new(attributes);
    }

    /// <summary>
    /// 特性
    /// </summary>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public ITypeSymbol? Type { get; set; }

    /// <summary>
    /// 类型名称
    /// </summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    public string Default { get; set; } = string.Empty;

    /// <inheritdoc/>
    public override string ToString()
    {
        var att = string.Empty;
        if (Attributes is not null && Attributes.Count > 0)
            att = string.Join("", Attributes) + " ";

        var def = string.Empty;
        if (string.IsNullOrWhiteSpace(Default) is false)
            def = $" = {Default}";
        var typeName = Type is null ? TypeName : Type.GetName();
        return $"{att}{typeName} {Name}{def}";
    }
}

/// <summary>
/// 泛型约束信息
/// </summary>
public class GenericConstraintInfo
{
    /// <inheritdoc/>
    /// <param name="genericName">泛型名称</param>
    /// <param name="constraint">约束</param>
    public GenericConstraintInfo(string genericName, string constraint)
    {
        GenericName = genericName;
        Constraint = constraint;
    }

    /// <summary>
    /// 泛型名称
    /// </summary>
    public string GenericName { get; set; }

    /// <summary>
    /// 约束
    /// </summary>
    public string Constraint { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"where {GenericName} : {Constraint}";
    }
}

/// <summary>
/// 方法类型
/// </summary>
public enum MethodType
{
    /// <summary>
    /// 无
    /// </summary>
    None,

    /// <summary>
    /// 静态
    /// </summary>
    Static,

    /// <summary>
    /// 部分
    /// </summary>
    Partial,

    /// <summary>
    /// 重写
    /// </summary>
    Override,

    /// <summary>
    /// 抽象
    /// </summary>
    Abstract,

    /// <summary>
    /// 虚
    /// </summary>
    Virtual,
}
