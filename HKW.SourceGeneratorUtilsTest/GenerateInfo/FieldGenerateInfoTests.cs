using HKW.SourceGeneratorUtils;
using Microsoft.CodeAnalysis;

namespace HKW.SourceGeneratorUtilsTest;

#pragma warning disable S6562
[TestClass]
public sealed class FieldGenerateInfoTests
{
    [TestMethod]
    [DataRow(Accessibility.Private)]
    [DataRow(Accessibility.Internal)]
    [DataRow(Accessibility.Protected)]
    [DataRow(Accessibility.ProtectedOrInternal)]
    [DataRow(Accessibility.Public)]
    public void DifferentAccessibility(Accessibility accessibility)
    {
        var fieldInfo = new FieldGenerateInfo("Value", "int")
        {
            Accessibility = accessibility,
            Default = "1",
        };

        var result = TestHelper.FieldCompilation<int>(fieldInfo);

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void ValueTypeFieldCanBeAssigned()
    {
        var fieldInfo = new FieldGenerateInfo("Created", "System.DateTime")
        {
            Accessibility = Accessibility.Public,
        };

        var expected = new DateTime(2026, 9, 1);

        var result = TestHelper.FieldCompilation<DateTime>(fieldInfo, expected);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ReferenceTypeFieldWithDefaultValue()
    {
        var fieldInfo = new FieldGenerateInfo("Values", "System.Collections.Generic.List<int>")
        {
            Accessibility = Accessibility.Internal,
            Default = "new System.Collections.Generic.List<int> { 1, 2, 3 }",
        };

        var result = TestHelper.FieldCompilation<List<int>>(fieldInfo);

        CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
    }
}
#pragma warning restore S6562
