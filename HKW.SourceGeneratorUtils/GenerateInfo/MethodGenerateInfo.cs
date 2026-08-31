using System.CodeDom.Compiler;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 方法构建信息
/// </summary>
public class MethodGenerateInfo : IMemberGenerateInfo
{
    /// <summary>
    /// 默认特性
    /// </summary>
    public static AttributeGenerateInfo[]? DefaultAttributes { get; set; }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="content">内容</param>
    public MethodGenerateInfo(string name, ITypeSymbol type, string content)
        : this(name, type.GetName(), content) { }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型</param>
    /// <param name="content">内容</param>
    public MethodGenerateInfo(string name, string typeName, string content)
    {
        Name = name;
        TypeName = typeName;
        if (string.IsNullOrWhiteSpace(content) is false)
            Contents.Add(content);
    }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="contents">内容</param>
    public MethodGenerateInfo(string name, ITypeSymbol type, IEnumerable<string> contents)
        : this(name, type.GetName(), contents) { }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="typeName">类型</param>
    /// <param name="contents">内容</param>
    public MethodGenerateInfo(string name, string typeName, IEnumerable<string> contents)
    {
        Name = name;
        TypeName = typeName;
        Contents.AddRange(contents);
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
    public List<string> Contents { get; set; } = [];

    /// <summary>
    /// 生成类型
    /// </summary>
    public MethodGenerateType GenerateType { get; set; }

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
        if (GenerateType == MethodGenerateType.Partial)
        {
            writer.Write(GenerateType.ToCode());
            writer.Write(' ');
            writer.Write(TypeName);
            writer.Write(' ');
            writer.Write(Name);
            writer.Write('(');
            writer.WriteCollection(Params, ",");
            writer.Write(')');
            writer.Write(';');
        }
        else
        {
            writer.WriteIf(Accessibility.ToCode(), " ");
            writer.WriteIf(GenerateType.ToCode(), " ");
            writer.Write(TypeName);
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
            writer.WriteLineCollection(Contents);
            writer.Indent--;
            writer.WriteLine("}");
        }
    }
}

/// <summary>
/// 方法生成类型
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
