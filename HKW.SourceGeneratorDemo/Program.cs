using HKW.SourceGenerator;

namespace HKW.SourceGeneratorDemo;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var t = new PTest();
        //Console.WriteLine(t.FullName2Func());
    }
}

[SourceGeneratorTest]
partial class PTest
{
    public List<int> GetList
    {
        get
        {
            var list = new List<int>();
            for (int i = 0; i < 10; i++)
                list.Add(i);
            return list;
        }
    }
    //private readonly string _fullName = "";
    //public string FullName => _fullName;
    //public string FullName1 => $"{FirstName}_{LastName}";

    //public string FullName2
    //{
    //    get
    //    {
    //        for (var i = 1; i < FirstName.Length; i++)
    //            Console.WriteLine(i);
    //        return $"{FirstName}_{LastName}";
    //    }
    //}
    //public string FirstName { get; } = "FirstName";
    //public string LastName { get; } = "LastName";

    //global::System.Threading.Tasks.Task Task1 { get; } = new(() => { });
}

//[SourceGeneratorTest]
//class Test1 { }

//[SourceGeneratorTest("1", "2", "3", S1 = "1")]
//class Test2 { }

//[SourceGeneratorTest("1", "2")]
//class Test3 { }

//[SourceGeneratorTest(1, 2, 3)]
//class Test4 { }
