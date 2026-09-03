using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HKW.SourceGeneratorUtils;

/// <summary>
/// 单元测试助手
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// 验证生成的方法是否与构建信息一致
    /// </summary>
    /// <exception cref="GenerateInfoException">方法信息不匹配时抛出</exception>
    public static void GenerateInfoCheck(MethodGenerateInfo generateInfo, MethodInfo methodInfo)
    {
        // 验证方法的基本信息
        if (generateInfo.Name != methodInfo.Name)
            throw new GenerateInfoException("Method name does not match.");
        if (TypeNameEquals(generateInfo.TypeName, methodInfo.ReturnType) is false)
            throw new GenerateInfoException("Method return type does not match.");
        if (AccessibilityEquals(generateInfo.Accessibility, methodInfo) is false)
            throw new GenerateInfoException("Method accessibility does not match.");
        if (MethodGenerateTypeEquals(generateInfo.GenerateType, methodInfo) is false)
            throw new GenerateInfoException("Method generation type does not match.");

        var expectedParameters = generateInfo.Params ?? [];
        var actualParameters = methodInfo.GetParameters();

        // 验证参数数量
        if (expectedParameters.Count != actualParameters.Length)
            throw new GenerateInfoException("Method parameter count does not match.");

        // 逐一验证参数信息
        foreach (
            var (expected, actual) in expectedParameters.Zip(actualParameters, (e, a) => (e, a))
        )
        {
            if (expected.Name != actual.Name)
                throw new GenerateInfoException(
                    $"Method parameter name does not match: {expected.Name}."
                );
            if (TypeNameEquals(expected.TypeName, actual.ParameterType) is false)
                throw new GenerateInfoException(
                    $"Method parameter type does not match: {expected.Name}."
                );
            if (ParameterGenerateTypeEquals(expected.GenerateType, actual) is false)
                throw new GenerateInfoException(
                    $"Method parameter generation type does not match: {expected.Name}."
                );
        }
    }

    /// <summary>
    /// 验证生成的属性是否与构建信息一致
    /// </summary>
    /// <exception cref="GenerateInfoException">属性信息不匹配时抛出</exception>
    public static void GenerateInfoCheck(
        PropertyGenerateInfo generateInfo,
        PropertyInfo propertyInfo
    )
    {
        var getMethod = propertyInfo.GetGetMethod(nonPublic: true);
        var setMethod = propertyInfo.GetSetMethod(nonPublic: true);

        // 验证属性及其 get 访问器的基本信息
        if (getMethod is null)
            throw new GenerateInfoException("Property does not contain a get accessor.");
        if (generateInfo.Name != propertyInfo.Name)
            throw new GenerateInfoException("Property name does not match.");
        if (TypeNameEquals(generateInfo.TypeName, propertyInfo.PropertyType) is false)
            throw new GenerateInfoException("Property type does not match.");
        if (AccessibilityEquals(generateInfo.Accessibility, getMethod) is false)
            throw new GenerateInfoException("Property accessibility does not match.");
        if (
            AccessibilityEquals(
                GetEffectiveAccessibility(
                    generateInfo.Accessibility,
                    generateInfo.GetMethod.Accessibility
                ),
                getMethod
            )
            is false
        )
            throw new GenerateInfoException("Property get accessor accessibility does not match.");

        // 验证 set 访问器是否存在且可访问性正确
        if (generateInfo.SetMethod is null && setMethod is not null)
            throw new GenerateInfoException("Property should not contain a set accessor.");
        if (generateInfo.SetMethod is not null && setMethod is null)
            throw new GenerateInfoException("Property does not contain a set accessor.");
        if (
            generateInfo.SetMethod is not null
            && setMethod is not null
            && AccessibilityEquals(
                GetEffectiveAccessibility(
                    generateInfo.Accessibility,
                    generateInfo.SetMethod.Accessibility
                ),
                setMethod
            )
                is false
        )
            throw new GenerateInfoException("Property set accessor accessibility does not match.");
    }

    /// <summary>
    /// 验证生成的字段是否与构建信息一致
    /// </summary>
    /// <exception cref="GenerateInfoException">字段信息不匹配时抛出</exception>
    public static void GenerateInfoCheck(FieldGenerateInfo generateInfo, FieldInfo fieldInfo)
    {
        // 验证字段的基本信息
        if (generateInfo.Name != fieldInfo.Name)
            throw new GenerateInfoException("Field name does not match.");
        if (TypeNameEquals(generateInfo.TypeName, fieldInfo.FieldType) is false)
            throw new GenerateInfoException("Field type does not match.");
        if (AccessibilityEquals(generateInfo.Accessibility, fieldInfo) is false)
            throw new GenerateInfoException("Field accessibility does not match.");
    }

    private static Accessibility GetEffectiveAccessibility(
        Accessibility memberAccessibility,
        Accessibility accessorAccessibility
    )
    {
        return accessorAccessibility == Accessibility.NotApplicable
            ? memberAccessibility
            : accessorAccessibility;
    }

    private static bool AccessibilityEquals(Accessibility accessibility, MethodInfo methodInfo)
    {
        return accessibility switch
        {
            Accessibility.Public => methodInfo.IsPublic,
            Accessibility.Private => methodInfo.IsPrivate,
            Accessibility.Internal or Accessibility.Friend => methodInfo.IsAssembly,
            Accessibility.Protected => methodInfo.IsFamily,
            Accessibility.ProtectedAndInternal
            or Accessibility.ProtectedAndFriend
            or Accessibility.ProtectedOrInternal
            or Accessibility.ProtectedOrFriend => methodInfo.IsFamilyOrAssembly,
            _ => methodInfo.IsPrivate,
        };
    }

    private static bool AccessibilityEquals(Accessibility accessibility, FieldInfo fieldInfo)
    {
        return accessibility switch
        {
            Accessibility.Public => fieldInfo.IsPublic,
            Accessibility.Private => fieldInfo.IsPrivate,
            Accessibility.Internal or Accessibility.Friend => fieldInfo.IsAssembly,
            Accessibility.Protected => fieldInfo.IsFamily,
            Accessibility.ProtectedAndInternal
            or Accessibility.ProtectedAndFriend
            or Accessibility.ProtectedOrInternal
            or Accessibility.ProtectedOrFriend => fieldInfo.IsFamilyOrAssembly,
            _ => fieldInfo.IsPrivate,
        };
    }

    private static bool MethodGenerateTypeEquals(
        MethodGenerateType generateType,
        MethodInfo methodInfo
    )
    {
        return generateType switch
        {
            MethodGenerateType.Static => methodInfo.IsStatic,
            MethodGenerateType.Abstract => methodInfo.IsAbstract,
            MethodGenerateType.Virtual => methodInfo.IsVirtual
                && methodInfo.GetBaseDefinition().DeclaringType == methodInfo.DeclaringType,
            MethodGenerateType.Override => methodInfo.IsVirtual
                && methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType,
            MethodGenerateType.None => !methodInfo.IsStatic
                && !methodInfo.IsAbstract
                && !methodInfo.IsVirtual,
            _ => !methodInfo.IsStatic,
        };
    }

    private static bool ParameterGenerateTypeEquals(
        ParameterGenerateType generateType,
        ParameterInfo parameterInfo
    )
    {
        return generateType switch
        {
            ParameterGenerateType.Ref => parameterInfo.ParameterType.IsByRef
                && !parameterInfo.IsIn
                && !parameterInfo.IsOut,
            ParameterGenerateType.Out => parameterInfo.IsOut,
            ParameterGenerateType.In => parameterInfo.IsIn && !parameterInfo.IsOut,
            ParameterGenerateType.Params => !parameterInfo.ParameterType.IsByRef
                && parameterInfo.GetCustomAttribute<ParamArrayAttribute>() is not null,
            _ => !parameterInfo.ParameterType.IsByRef,
        };
    }

    private static bool TypeNameEquals(string typeName, Type type)
    {
        var expectedTypeName = typeName.Replace("global::", string.Empty).Trim();
        if (
            expectedTypeName == type.Name
            || expectedTypeName == type.FullName?.Replace('+', '.')
            || expectedTypeName == GetCSharpTypeName(type, fullyQualified: false)
            || expectedTypeName == GetCSharpTypeName(type, fullyQualified: true)
        )
        {
            return true;
        }
        return false;
    }

    private static string GetCSharpTypeName(Type type, bool fullyQualified)
    {
        if (NameByType.TryGetValue(type, out var alias))
            return alias;

        if (type.IsByRef)
            return GetCSharpTypeName(type.GetElementType()!, fullyQualified);

        if (type.IsArray)
            return $"{GetCSharpTypeName(type.GetElementType()!, fullyQualified)}[]";

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
            return $"{GetCSharpTypeName(underlyingType, fullyQualified)}?";

        if (!type.IsGenericType)
            return fullyQualified ? type.FullName?.Replace('+', '.') ?? type.Name : type.Name;

        var genericType = type.GetGenericTypeDefinition();
        var genericTypeName = genericType.Name.Substring(0, genericType.Name.IndexOf('`'));
        var prefix = fullyQualified
            ? $"{genericType.Namespace}.{genericTypeName}"
            : genericTypeName;

        var arguments = string.Join(
            ", ",
            type.GetGenericArguments()
                .Select(argument => GetCSharpTypeName(argument, fullyQualified))
        );

        return $"{prefix}<{arguments}>";
    }

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="typeName">类型名称</param>
    /// <returns>类型</returns>
    public static Type? GetType(string typeName)
    {
        return typeName switch
        {
            "void" => typeof(void),
            "bool" => typeof(bool),
            "byte" => typeof(byte),
            "sbyte" => typeof(sbyte),
            "short" => typeof(short),
            "ushort" => typeof(ushort),
            "int" => typeof(int),
            "uint" => typeof(uint),
            "long" => typeof(long),
            "ulong" => typeof(ulong),
            "float" => typeof(float),
            "double" => typeof(double),
            "decimal" => typeof(decimal),
            "char" => typeof(char),
            "string" => typeof(string),
            "object" => typeof(object),
            _ => Type.GetType(typeName),
        };
    }

    /// <summary>
    /// 类型的别名
    /// </summary>
    public static ImmutableDictionary<Type, string> NameByType { get; } =
        ImmutableDictionary.CreateRange([
            new KeyValuePair<Type, string>(typeof(void), "void"),
            new KeyValuePair<Type, string>(typeof(bool), "bool"),
            new KeyValuePair<Type, string>(typeof(byte), "byte"),
            new KeyValuePair<Type, string>(typeof(sbyte), "sbyte"),
            new KeyValuePair<Type, string>(typeof(short), "short"),
            new KeyValuePair<Type, string>(typeof(ushort), "ushort"),
            new KeyValuePair<Type, string>(typeof(int), "int"),
            new KeyValuePair<Type, string>(typeof(uint), "uint"),
            new KeyValuePair<Type, string>(typeof(long), "long"),
            new KeyValuePair<Type, string>(typeof(ulong), "ulong"),
            new KeyValuePair<Type, string>(typeof(float), "float"),
            new KeyValuePair<Type, string>(typeof(double), "double"),
            new KeyValuePair<Type, string>(typeof(decimal), "decimal"),
            new KeyValuePair<Type, string>(typeof(char), "char"),
            new KeyValuePair<Type, string>(typeof(string), "string"),
            new KeyValuePair<Type, string>(typeof(object), "object"),
        ]);

    /// <summary>
    /// 方法编译
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="generateInfo">方法构建信息</param>
    /// <param name="inputs">参数</param>
    /// <returns>返回值</returns>
    public static T MethodCompilation<T>(MethodGenerateInfo generateInfo, object?[] inputs)
    {
        var fullInfo = $$"""
            internal class Program
            {
                {{generateInfo}}
            }
            """;
        var syntaxTree = CSharpSyntaxTree.ParseText(
            fullInfo,
            cancellationToken: CancellationToken.None
        );

        var compilation = CSharpCompilation.Create(
            nameof(MethodCompilation),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        DiagnosticException.ThrowIfError(error);

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var method = typeInfo.DeclaredMethods.First();
        GenerateInfoCheck(generateInfo, method);
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        return (T)method.Invoke(obj, inputs)!;
    }

    /// <summary>
    /// 属性编译
    /// </summary>
    /// <typeparam name="T">属性类型</typeparam>
    /// <param name="generateInfo">属性构建信息</param>
    /// <param name="input">输入</param>
    /// <returns>属性值</returns>
    public static T PropertyCompilation<T>(PropertyGenerateInfo generateInfo, object? input = null)
    {
        var fullInfo = $$"""
            internal class Program
            {
                {{generateInfo}}
            }
            """;
        var syntaxTree = CSharpSyntaxTree.ParseText(
            fullInfo,
            cancellationToken: CancellationToken.None
        );

        var compilation = CSharpCompilation.Create(
            nameof(PropertyCompilation),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        DiagnosticException.ThrowIfError(error);

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var property = typeInfo.DeclaredProperties.First();
        GenerateInfoCheck(generateInfo, property);
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        if (input is not null)
            property.SetValue(obj, input);
        return (T)property.GetValue(obj)!;
    }

    /// <summary>
    /// 字段编译
    /// </summary>
    /// <typeparam name="T">字段类型</typeparam>
    /// <param name="generateInfo">字段构建信息</param>
    /// <param name="input">输入</param>
    /// <returns>字段值</returns>
    public static T FieldCompilation<T>(FieldGenerateInfo generateInfo, object? input = null)
    {
        var fullInfo = $$"""
            internal class Program
            {
                {{generateInfo}}
            }
            """;
        var syntaxTree = CSharpSyntaxTree.ParseText(
            fullInfo,
            cancellationToken: CancellationToken.None
        );

        var compilation = CSharpCompilation.Create(
            nameof(FieldCompilation),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        DiagnosticException.ThrowIfError(error);

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var field = typeInfo.DeclaredFields.First();
        GenerateInfoCheck(generateInfo, field);
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        if (input is not null)
            field.SetValue(obj, input);
        return (T)field.GetValue(obj)!;
    }
}

/// <summary>
/// 诊断异常
/// </summary>
public class DiagnosticException : Exception
{
    /// <summary>
    /// 当诊断错误时抛出异常
    /// </summary>
    /// <param name="diagnostic">诊断</param>
    /// <exception cref="DiagnosticException">诊断异常</exception>
    public static void ThrowIfError(Diagnostic? diagnostic)
    {
        if (diagnostic is not null)
            throw new DiagnosticException(diagnostic);
    }

    /// <inheritdoc/>
    /// <param name="diagnostic">诊断</param>
    public DiagnosticException(Diagnostic diagnostic)
        : base(diagnostic.ToString())
    {
        Diagnostic = diagnostic;
    }

    /// <summary>
    /// 诊断
    /// </summary>
    public Diagnostic Diagnostic { get; }
}

/// <summary>
/// 构建信息异常
/// </summary>
public class GenerateInfoException : Exception
{
    /// <inheritdoc/>
    /// <param name="message">信息</param>
    public GenerateInfoException(string message)
        : base(message) { }
}
