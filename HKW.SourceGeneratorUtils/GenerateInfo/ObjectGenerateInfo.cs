using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

//public class FileGenerateInfo
//{
//    public
//    public string NameSpace { get; set; }
//}

/// <summary>
/// 对象构建信息
/// </summary>
public class ObjectGenerateInfo
{
    /// <summary>
    /// 默认特性
    /// </summary>
    public static AttributeGenerateInfo[]? DefaultAttributes { get; set; }

    /// <inheritdoc/>
    /// <param name="name">名称</param>
    /// <param name="type">类型名称</param>
    public ObjectGenerateInfo(string name, ObjectGenerateType type)
    {
        Name = name;
        GenerateType = type;
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
    /// 添加默认特性
    /// </summary>
    public bool AddDefaultAttributes { get; set; } = true;

    /// <summary>
    /// 可访问性
    /// </summary>
    public Accessibility Accessibility { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 生成类型
    /// </summary>
    public ObjectGenerateType GenerateType { get; set; }

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
        if (AddDefaultAttributes)
            writer.WriteLineCollection(DefaultAttributes);

        writer.WriteIf(Accessibility.ToCode(), " ");
        writer.Write(GenerateType.ToCode());
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

        writer.WriteIf(Accessibility.ToCode(), " ");
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

/// <summary>
/// 对象类型
/// </summary>
public enum ObjectGenerateType
{
    /// <summary>
    /// 类
    /// </summary>
    Class,

    /// <summary>
    /// 静态类
    /// </summary>
    StaticClass,

    /// <summary>
    /// 部分类
    /// </summary>
    PartialClass,

    /// <summary>
    /// 静态部分类
    /// </summary>
    StaticPartialClass,

    /// <summary>
    /// 抽象类
    /// </summary>
    AbstractClass,

    /// <summary>
    /// 抽象部分类
    /// </summary>
    AbstractPartialClass,

    /// <summary>
    /// 密封类
    /// </summary>
    SealedClass,

    /// <summary>
    /// 密封部分类
    /// </summary>
    SealedPartialClass,

    /// <summary>
    /// 结构体
    /// </summary>
    Struct,

    /// <summary>
    /// 部分结构体
    /// </summary>
    PartialStruct,

    /// <summary>
    /// 只读结构体
    /// </summary>
    ReadOnlyStruct,

    /// <summary>
    /// 只读部分结构体
    /// </summary>
    ReadOnlyPartialStruct,

    /// <summary>
    /// 引用结构体
    /// </summary>
    RefStruct,

    /// <summary>
    /// 部分引用结构体
    /// </summary>
    PartialRefStruct,

    /// <summary>
    /// 只读引用结构体
    /// </summary>
    ReadOnlyRefStruct,

    /// <summary>
    /// 只读部分引用结构体
    /// </summary>
    ReadOnlyPartialRefStruct,

    /// <summary>
    /// 记录
    /// </summary>
    Record,

    /// <summary>
    /// 部分记录
    /// </summary>
    PartialRecord,

    /// <summary>
    /// 抽象记录
    /// </summary>
    AbstractRecord,

    /// <summary>
    /// 抽象部分记录
    /// </summary>
    AbstractPartialRecord,

    /// <summary>
    /// 密封记录
    /// </summary>
    SealedRecord,

    /// <summary>
    /// 密封部分记录
    /// </summary>
    SealedPartialRecord,

    /// <summary>
    /// 记录结构体
    /// </summary>
    RecordStruct,

    /// <summary>
    /// 部分记录结构体
    /// </summary>
    PartialRecordStruct,

    /// <summary>
    /// 只读记录结构体
    /// </summary>
    ReadOnlyRecordStruct,

    /// <summary>
    /// 只读部分记录结构体
    /// </summary>
    ReadOnlyPartialRecordStruct,
}
