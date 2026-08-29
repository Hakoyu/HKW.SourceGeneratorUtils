using System.CodeDom.Compiler;
using HKW.SourceGeneratorUtils.Extensions;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

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
    /// <param name="name">名称</param>
    /// <param name="typeName">类型</param>
    /// <param name="content">内容</param>
    public MethodGenerateInfo(string name, string typeName, string content)
    {
        Name = name;
        TypeName = typeName;
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

    /// <inheritdoc/>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 参数
    /// </summary>
    public List<ParameterGenerateInfo>? Params { get; set; }

    /// <summary>
    /// 约束
    /// </summary>
    public List<GenericConstraintInfo>? Constraints { get; set; }

    /// <inheritdoc/>
    public string Content { get; set; }

    /// <summary>
    /// 方法类型
    /// </summary>
    public MethodGenerateType MethodType { get; set; }

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
        var typeName = Type is null ? TypeName : Type.GetName();
        if (MethodType == MethodGenerateType.Partial)
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
    }
}

/// <summary>
/// 方法类型
/// </summary>
public enum MethodGenerateType
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
