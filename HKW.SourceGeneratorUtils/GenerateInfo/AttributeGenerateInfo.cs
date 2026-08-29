using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

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
