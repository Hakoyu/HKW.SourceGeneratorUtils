using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtilsTest;

#pragma warning disable S6562
[TestClass]
public sealed class MethodGenerateInfoTests
{
    [TestMethod]
    [DataRow(Accessibility.Private)]
    [DataRow(Accessibility.Internal)]
    [DataRow(Accessibility.Protected)]
    [DataRow(Accessibility.ProtectedOrInternal)]
    [DataRow(Accessibility.Public)]
    public void DifferentAccessibility(Accessibility accessibility)
    {
        var methodInfo = new MethodGenerateInfo("int", "GetValue", "return 1;")
        {
            Accessibility = accessibility,
        };

        var result = TestHelper.MethodCompilation<int>(methodInfo, []);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ValueTypeReturnAndNoParameters()
    {
        var methodInfo = new MethodGenerateInfo(
            "System.DateTime",
            "GetDate",
            "return new System.DateTime(2026, 9, 1);"
        )
        {
            Accessibility = Accessibility.Public,
        };

        var result = TestHelper.MethodCompilation<DateTime>(methodInfo, []);

        Assert.AreEqual(new DateTime(2026, 9, 1), result);
    }

    [TestMethod]
    public void VoidReturnAndRefParameter()
    {
        var methodInfo = new MethodGenerateInfo("void", "Increase", "value += 1;")
        {
            Accessibility = Accessibility.Internal,
            Params = [new("int", "value") { GenerateType = ParameterGenerateType.Ref }],
        };
        object?[] inputs = [1];

        var result = TestHelper.MethodCompilation<object?>(methodInfo, inputs);

        Assert.IsNull(result);
        Assert.AreEqual(2, inputs[0]);
    }

    [TestMethod]
    public void ClassStructAndReferenceParameters()
    {
        var methodInfo = new MethodGenerateInfo(
            "string",
            "Format",
            """
            number += items.Count;
            created = new System.DateTime(timestamp.Year, 9, 1);
            return $"{items[0]}:{number}:{created:yyyy-MM-dd}";
            """
        )
        {
            Accessibility = Accessibility.ProtectedOrInternal,
            Params =
            [
                new("System.Collections.Generic.List<string>", "items"),
                new("int", "number") { GenerateType = ParameterGenerateType.Ref },
                new("System.DateTime", "created") { GenerateType = ParameterGenerateType.Out },
                new("System.DateTime", "timestamp") { GenerateType = ParameterGenerateType.In },
            ],
        };
        object?[] inputs = [new List<string> { "item" }, 1, null, new DateTime(2026, 1, 1)];

        var result = TestHelper.MethodCompilation<string>(methodInfo, inputs);

        Assert.AreEqual("item:2:2026-09-01", result);
        Assert.AreEqual(2, inputs[1]);

        Assert.AreEqual(new DateTime(2026, 9, 1), inputs[2]);
    }

    [TestMethod]
    public void ParamsParameter()
    {
        var methodInfo = new MethodGenerateInfo("int", "GetLength", "return values.Length;")
        {
            Accessibility = Accessibility.Public,
            Params = [new("int[]", "values") { GenerateType = ParameterGenerateType.Params }],
        };

        var result = TestHelper.MethodCompilation<int>(methodInfo, [new[] { 1, 2, 3 }]);

        Assert.AreEqual(3, result);
    }
}
#pragma warning restore S6562
