using System.Collections.Immutable;
using System.Reflection;
using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HKW.SourceGeneratorUtilsTest;

public static class TestHelper
{
    public static bool GenerateInfoCheck(MethodGenerateInfo generateInfo, MethodInfo methodInfo)
    {
        if (
            generateInfo.Name != methodInfo.Name
            || !TypeNameEquals(generateInfo.TypeName, methodInfo.ReturnType)
            || !AccessibilityEquals(generateInfo.Accessibility, methodInfo)
            || !MethodGenerateTypeEquals(generateInfo.GenerateType, methodInfo)
        )
        {
            return false;
        }

        var expectedParameters = generateInfo.Params ?? [];
        var actualParameters = methodInfo.GetParameters();

        if (expectedParameters.Count != actualParameters.Length)
            return false;

        return expectedParameters
            .Zip(actualParameters)
            .All(pair =>
                pair.First.Name == pair.Second.Name
                && TypeNameEquals(pair.First.TypeName, pair.Second.ParameterType)
                && ParameterGenerateTypeEquals(pair.First.GenerateType, pair.Second)
            );
    }

    public static bool GenerateInfoCheck(
        PropertyGenerateInfo generateInfo,
        PropertyInfo propertyInfo
    )
    {
        var getMethod = propertyInfo.GetGetMethod(nonPublic: true);
        var setMethod = propertyInfo.GetSetMethod(nonPublic: true);

        if (
            getMethod is null
            || generateInfo.Name != propertyInfo.Name
            || !TypeNameEquals(generateInfo.TypeName, propertyInfo.PropertyType)
            || !AccessibilityEquals(generateInfo.Accessibility, getMethod)
            || !AccessibilityEquals(
                GetEffectiveAccessibility(
                    generateInfo.Accessibility,
                    generateInfo.GetMethod.Accessibility
                ),
                getMethod
            )
        )
        {
            return false;
        }

        return generateInfo.SetMethod is null
            ? setMethod is null
            : setMethod is not null
                && AccessibilityEquals(
                    GetEffectiveAccessibility(
                        generateInfo.Accessibility,
                        generateInfo.SetMethod.Accessibility
                    ),
                    setMethod
                );
    }

    public static bool GenerateInfoCheck(FieldGenerateInfo generateInfo, FieldInfo fieldInfo)
    {
        return generateInfo.Name == fieldInfo.Name
            && TypeNameEquals(generateInfo.TypeName, fieldInfo.FieldType)
            && AccessibilityEquals(generateInfo.Accessibility, fieldInfo);
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
        var genericTypeName = genericType.Name[..genericType.Name.IndexOf('`')];
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

    public static Type? GetType(string alias)
    {
        return NameByType.FirstOrDefault(pair => pair.Value == alias).Key ?? Type.GetType(alias);
    }

    public static ImmutableDictionary<Type, string> NameByType { get; } =
        ImmutableDictionary.CreateRange([
            KeyValuePair.Create(typeof(void), "void"),
            KeyValuePair.Create(typeof(bool), "bool"),
            KeyValuePair.Create(typeof(byte), "byte"),
            KeyValuePair.Create(typeof(sbyte), "sbyte"),
            KeyValuePair.Create(typeof(short), "short"),
            KeyValuePair.Create(typeof(ushort), "ushort"),
            KeyValuePair.Create(typeof(int), "int"),
            KeyValuePair.Create(typeof(uint), "uint"),
            KeyValuePair.Create(typeof(long), "long"),
            KeyValuePair.Create(typeof(ulong), "ulong"),
            KeyValuePair.Create(typeof(float), "float"),
            KeyValuePair.Create(typeof(double), "double"),
            KeyValuePair.Create(typeof(decimal), "decimal"),
            KeyValuePair.Create(typeof(char), "char"),
            KeyValuePair.Create(typeof(string), "string"),
            KeyValuePair.Create(typeof(object), "object"),
        ]);

    public static T MethodGenerate<T>(MethodGenerateInfo generateInfo, object?[] inputs)
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
            nameof(MethodGenerate),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.IsNull(error, error?.ToString());

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var method = typeInfo.DeclaredMethods.First();
        Assert.IsTrue(GenerateInfoCheck(generateInfo, method));
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        return (T)method.Invoke(obj, inputs)!;
    }

    public static T PropertyGenerate<T>(PropertyGenerateInfo generateInfo, object? input = null)
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
            nameof(PropertyGenerate),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.IsNull(error, error?.ToString());

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var property = typeInfo.DeclaredProperties.First();
        Assert.IsTrue(GenerateInfoCheck(generateInfo, property));
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        if (input is not null)
            property.SetValue(obj, input);
        return (T)property.GetValue(obj)!;
    }

    public static T FieldGenerate<T>(FieldGenerateInfo generateInfo, object? input = null)
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
            nameof(FieldGenerate),
            [syntaxTree],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        var error = compilation
            .GetDiagnostics(CancellationToken.None)
            .FirstOrDefault(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        Assert.IsNull(error, error?.ToString());

        using var ms = new MemoryStream();
        compilation.Emit(ms, cancellationToken: CancellationToken.None);
        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());
        var typeInfo = assembly.DefinedTypes.First();
        var field = typeInfo.DeclaredFields.First();
        Assert.IsTrue(GenerateInfoCheck(generateInfo, field));
        var obj = assembly.CreateInstance(typeInfo.FullName!);
        if (input is not null)
            field.SetValue(obj, input);
        return (T)field.GetValue(obj)!;
    }
}
