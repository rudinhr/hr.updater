using System;

namespace HrUpdateCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var definition = HrUpdateDefinitionCreator.Instance.Create("IHRS","C:\\HRD");
            System.Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(definition));
        }
    }
}
