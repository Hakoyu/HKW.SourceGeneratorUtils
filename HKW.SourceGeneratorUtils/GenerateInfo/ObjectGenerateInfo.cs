using System.CodeDom.Compiler;
using HKW.SourceGeneratorUtils.Extensions;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 对象构建信息
/// </summary>
public class ObjectGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    public ObjectGenerateInfo(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
    }

    /// <summary>
    /// 注释
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// 特性
    /// </summary>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 类型名称, 如 <c>class</c>, <c>sealed class</c>, <c>struct</c>
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public List<ConstructorGenerateInfo>? Constructors { get; set; }

    /// <summary>
    /// 成员
    /// </summary>
    public List<IMemberGenerateInfo> Members { get; } = [];

    /// <summary>
    /// 继承的基类和实现的接口
    /// </summary>
    public List<string>? Inherits { get; set; }

    /// <summary>
    /// 写入至
    /// </summary>
    /// <param name="writer">写入器</param>
    public void WriteTo(IndentedTextWriter writer)
    {
        writer.WriteLine();

        writer.WriteLineCollection(Comment.SplitLine());
        writer.WriteLineCollection(Attributes);

        writer.WriteIf(Accessibility.ToStr(), " ");
        writer.Write(TypeName);
        writer.Write(' ');
        writer.Write(Name);

        if (Inherits is not null && Inherits.Count > 0)
        {
            writer.Write(" : ");
            writer.WriteCollection(Inherits, ", ");
        }

        writer.WriteLine();

        writer.WriteLine("{");
        writer.Indent++;

        if (Constructors is not null)
        {
            foreach (var ctor in Constructors)
                ctor.WriteTo(writer);
        }

        foreach (var member in Members)
            member.WriteTo(writer);

        writer.Indent--;
        writer.WriteLine("}");
    }
}

/// <summary>
/// 构造函数构建信息
/// </summary>
public class ConstructorGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="content">内容</param>
    /// <param name="params">参数</param>
    public ConstructorGenerateInfo(
        string name,
        string content,
        params ParameterGenerateInfo[] @params
    )
    {
        Name = name;
        Content = content;
        Params = new(@params);
    }

    /// <inheritdoc/>
    public string Comment { get; set; } = string.Empty;

    /// <inheritdoc/>
    public List<AttributeGenerateInfo>? Attributes { get; set; }

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    public List<ParameterGenerateInfo> Params { get; }

    /// <summary>
    /// 构造函数初始化器，如 <c>this(...)</c> 或 <c>base(...)</c>
    /// </summary>
    public string Initializer { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string Content { get; set; }

    /// <inheritdoc/>
    public override string ToString()
    {
        var stringStream = new StringWriter();
        var writer = new IndentedTextWriter(stringStream);
        WriteTo(writer);
        return stringStream.ToString();
    }

    /// <inheritdoc/>
    public void WriteTo(IndentedTextWriter writer)
    {
        writer.WriteLine();

        writer.WriteLineCollection(Comment.SplitLine());
        writer.WriteLineCollection(Attributes);

        writer.WriteIf(Accessibility.ToStr(), " ");
        writer.Write(' ');
        writer.Write(Name);
        writer.Write('(');
        writer.WriteCollection(Params, ",");
        writer.Write(')');

        if (string.IsNullOrWhiteSpace(Initializer) is false)
        {
            writer.Write(" : ");
            writer.Write(Initializer);
        }

        writer.WriteLine();

        writer.WriteLine("{");
        writer.Indent++;
        writer.WriteLine(Content);
        writer.Indent--;
        writer.WriteLine("}");
    }
}
