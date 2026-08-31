using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 特性参数字典(ParamName, ParamValue)
/// </summary>
public class AttributeParamDictionary : IDictionary<string, AttributeParam>
{
    private readonly Dictionary<string, AttributeParam> _dictionary = [];

    /// <inheritdoc/>
    /// <param name="attributeData">特性数据</param>
    public AttributeParamDictionary(AttributeData attributeData)
    {
        if (
            attributeData?.AttributeConstructor?.Parameters
            is ImmutableArray<IParameterSymbol> constructorParams
        )
        {
            for (var i = 0; i < constructorParams.Length; i++)
            {
                var param = constructorParams[i];
                var arg = attributeData.ConstructorArguments[i];
                // 如果参数是 paras 类型, 则获取数组
                if (arg.Kind is TypedConstantKind.Array)
                    _dictionary.TryAdd(param.Name, new(arg.Values));
                else
                    _dictionary.TryAdd(param.Name, new(arg));
            }
        }
        if (
            attributeData?.NamedArguments
            is ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments
        )
        {
            for (var i = 0; i < namedArguments.Length; i++)
            {
                var param = namedArguments[i];
                _dictionary.TryAdd(param.Key, new(param.Value));
            }
        }
    }

    /// <summary>
    /// 尝试获取参数值
    /// </summary>
    /// <typeparam name="TValue">类型</typeparam>
    /// <param name="parameterName">参数名称</param>
    /// <param name="paramValue"></param>
    /// <returns>是否存在</returns>
    public bool TryGetParam<TValue>(string parameterName, out TValue paramValue)
    {
        if (typeof(TValue).IsArray)
            throw new NotSupportedException();
        var r = _dictionary.TryGetValue(parameterName, out var value);
        paramValue = r ? (TValue)value.Value! : default!;
        return r;
    }

    /// <summary>
    /// 尝试获取参数数组
    /// </summary>
    /// <typeparam name="TValue">类型</typeparam>
    /// <param name="parameterName">参数名称</param>
    /// <param name="parameterArray">参数数组</param>
    /// <returns>是否存在</returns>
    public bool TryGetParams<TValue>(string parameterName, out IEnumerable<TValue> parameterArray)
    {
        var r = _dictionary.TryGetValue(parameterName, out var value);
        parameterArray = r ? value.Values.Cast<TValue>() : default!;
        return r;
    }

    #region IDictionary
    /// <inheritdoc/>
    public AttributeParam this[string key]
    {
        get => _dictionary[key];
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public ICollection<string> Keys => _dictionary.Keys;

    /// <inheritdoc/>
    public ICollection<AttributeParam> Values => _dictionary.Values;

    /// <inheritdoc/>
    public int Count => _dictionary.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    void IDictionary<string, AttributeParam>.Add(string key, AttributeParam value)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, AttributeParam>>.Add(
        KeyValuePair<string, AttributeParam> item
    )
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    bool IDictionary<string, AttributeParam>.Remove(string key)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<string, AttributeParam>>.Remove(
        KeyValuePair<string, AttributeParam> item
    )
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    void ICollection<KeyValuePair<string, AttributeParam>>.Clear()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<string, AttributeParam> item)
    {
        return ((ICollection<KeyValuePair<string, AttributeParam>>)_dictionary).Contains(item);
    }

    /// <inheritdoc/>
    public bool ContainsKey(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<string, AttributeParam>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<string, AttributeParam>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
    public bool TryGetValue(string key, out AttributeParam value)
    {
        return _dictionary.TryGetValue(key, out value);
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<string, AttributeParam>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
}

/// <summary>
/// 特性参数值
/// </summary>
public readonly struct AttributeParam
{
    /// <inheritdoc/>
    /// <param name="typedConstant">类型常量</param>
    public AttributeParam(TypedConstant typedConstant)
    {
        Value = typedConstant.Value;
    }

    /// <inheritdoc/>
    /// <param name="typedConstants">类型常量枚举</param>
    public AttributeParam(IEnumerable<TypedConstant> typedConstants)
    {
        Values = typedConstants.Select(x => x.Value).ToArray();
    }

    /// <summary>
    /// 参数值
    /// </summary>
    public object? Value { get; } = null;

    /// <summary>
    /// 参数值数组, 用于 paras 类型参数
    /// </summary>
    public object?[]? Values { get; } = null;
}
