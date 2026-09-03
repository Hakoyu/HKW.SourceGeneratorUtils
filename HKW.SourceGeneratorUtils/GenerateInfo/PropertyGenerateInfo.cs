using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 属性构建信息
/// </summary>
public class PropertyGenerateInfo : IMemberGenerateInfo
{
    /// <summary>
    /// 默认特性
    /// </summary>
    public static AttributeGenerateInfo[]? DefaultAttributes { get; set; }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型</param>
    /// <param name="getMethod">Get方法</param>
    public PropertyGenerateInfo(
        string name,
        ITypeSymbol type,
        PropertyGetMethodGenerateInfo getMethod
    )
        : this(type.GetName(), name, getMethod) { }

    /// <inheritdoc/>
    /// <param name="typeName">类型名称</param>
    /// <param name="name">名称</param>
    /// <param name="getMethod">Get方法</param>
    public PropertyGenerateInfo(
        string typeName,
        string name,
        PropertyGetMethodGenerateInfo getMethod
    )
    {
        TypeName = typeName;
        Name = name;
        GetMethod = getMethod;
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
    /// Get 方法
    /// </summary>
    public PropertyGetMethodGenerateInfo GetMethod { get; set; }

    /// <summary>
    /// Set 方法
    /// </summary>
    public PropertySetMethodGenerateInfo? SetMethod { get; set; }

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
        writer.WriteLineCollection(Comment.SplitLine());
        writer.WriteLineCollection(Attributes);
        if (AddDefaultAttributes)
            writer.WriteLineCollection(DefaultAttributes);

        writer.WriteIf(Accessibility.ToCode(), " ");
        writer.Write(TypeName);
        writer.Write(' ');
        writer.Write(Name);
        writer.WriteLine();

        writer.WriteLine("{");
        writer.Indent++;

        GetMethod?.WriteTo(writer);
        SetMethod?.WriteTo(writer);

        writer.Indent--;
        writer.Write("}");

        if (string.IsNullOrWhiteSpace(Default) is false)
        {
            writer.Write($" = {Default}");
            writer.WriteLine(";");
        }
        else
        {
            writer.WriteLine();
        }
    }
}

/// <summary>
/// 属性Get方法生成信息
/// </summary>
public class PropertyGetMethodGenerateInfo : PropertyMethodGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="content">内容</param>
    public PropertyGetMethodGenerateInfo(string content)
        : base(content, PropertyMethodGenerateType.Get) { }
}

/// <summary>
/// 属性Set方法生成信息
/// </summary>
public class PropertySetMethodGenerateInfo : PropertyMethodGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="content">内容</param>
    public PropertySetMethodGenerateInfo(string content)
        : base(content, PropertyMethodGenerateType.Set) { }
}

/// <summary>
/// 属性方法生成信息
/// <para>
/// <c>;</c> 表示自动访问器，<c>=>...;</c>和<c>{...}</c>表示访问器主体
/// </para>
/// </summary>
public class PropertyMethodGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="content">内容</param>
    /// <param name="generateType">生成类型</param>
    public PropertyMethodGenerateInfo(string content, PropertyMethodGenerateType generateType)
    {
        Content = content;
        GenerateType = generateType;
    }

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 生成类型
    /// </summary>
    public PropertyMethodGenerateType GenerateType { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
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
        writer.WriteIf(Accessibility.ToCode(), " ");
        writer.Write(GenerateType.ToCode());

        if (Content == ";" || Content.StartsWith("=>"))
            writer.WriteLine(Content);
        else
            writer.WriteLineCollection(Content.SplitLine());
    }
}

/// <summary>
/// 属性方法生成类型
/// </summary>
public enum PropertyMethodGenerateType
{
    /// <summary>
    /// Get方法
    /// </summary>
    Get,

    /// <summary>
    /// Set方法
    /// </summary>
    Set,
}
