using System.CodeDom.Compiler;
using HKW.SourceGeneratorUtils.Extensions;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 字段构建信息
/// </summary>
public class FieldGenerateInfo : IMemberGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    public FieldGenerateInfo(string name, ITypeSymbol type)
    {
        Name = name;
        Type = type;
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型名称</param>
    public FieldGenerateInfo(string name, string typeName)
    {
        Name = name;
        TypeName = typeName;
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

    /// <inheritdoc/>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 默认值
    /// </summary>
    public string Default { get; set; } = string.Empty;

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
        writer.WriteIf(Accessibility.ToStr(), " ");
        writer.Write(typeName);
        writer.Write(' ');
        writer.Write(Name);

        if (string.IsNullOrWhiteSpace(Default) is false)
            writer.Write($" = {Default}");

        writer.WriteLine(";");
    }
}
