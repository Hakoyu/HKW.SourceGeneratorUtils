using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 特性信息
/// </summary>
public class AttributeGenerateInfo
{
    /// <inheritdoc/>
    /// <param name="fullName">类型全名</param>
    public AttributeGenerateInfo(string fullName)
    {
        FullName = fullName;
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
    /// <param name="params">参数</param>
    public AttributeGenerateInfo(ITypeSymbol type, params ParameterGenerateInfo[] @params)
    {
        TypeName = type.GetName();
        Params = new(@params);
    }

    /// <summary>
    /// 全名
    /// </summary>
    public string FullName { get; set; } = string.Empty;

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
        if (string.IsNullOrWhiteSpace(FullName) is false)
            return FullName;
        if (Params is null || Params.Count == 0)
            return $"[{TypeName}]";
        else
            return $"[{TypeName}({string.Join(",", Params)})]";
    }
}
