using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 字段构建信息
/// </summary>
public class FieldGenerateInfo : IMemberGenerateInfo
{
    /// <summary>
    /// 默认特性
    /// </summary>
    public static AttributeGenerateInfo[]? DefaultAttributes { get; set; }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    public FieldGenerateInfo(string name, ITypeSymbol type)
    {
        Name = name;
        TypeName = type.GetName();
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
    public bool AddDefaultAttributes { get; set; } = true;

    /// <inheritdoc/>
    public string Name { get; set; }

    /// <inheritdoc/>
    public string TypeName { get; set; }

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
        if (AddDefaultAttributes)
            writer.WriteLineCollection(DefaultAttributes);

        writer.WriteIf(Accessibility.ToCode(), " ");
        writer.Write(TypeName);
        writer.Write(' ');
        writer.Write(Name);

        if (string.IsNullOrWhiteSpace(Default) is false)
            writer.Write($" = {Default}");

        writer.WriteLine(";");
    }
}
