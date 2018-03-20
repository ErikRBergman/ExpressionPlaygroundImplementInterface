using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.IntermediateLanguageTools.Console
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    using API.Controllers;

    using Serpent.IntermediateLanguageTools.Helpers;
    using Serpent.IntermediateLanguageTools.Models;

    class Program
    {
        static void Main(string[] args)
        {
            var generator = new CSharpIntermediateLanguageGenerator();

            var contents = new StringBuilder();

            int tabCount = 0;

            contents.AppendLine("namespace API.Controllers");
            contents.AppendLine("{");

            tabCount++;



            contents.AppendLineTabbed(tabCount, "public class ExperimentOuput");
            contents.AppendLineTabbed(tabCount, "{");

            tabCount++;

            var sourceType = new DotNetType(typeof(Experiments));

            foreach (var field in sourceType.SourceType.GetFields())
            {
                sourceType.FieldNames.Add(field.Name);
            }



            var parameters = new CreateMethodILGeneratorParameters(typeof(Experiments).GetMethod("DoItAsync"))
            {
                DotNetType = sourceType,
                TabCount = tabCount,
                Verbose = true
            };

            contents.Append(generator.CreateMethodILGenerator(parameters));
            Debug.WriteLine(contents);
            tabCount--;

            contents.AppendLineTabbed(tabCount, "}");
            tabCount--;
            contents.AppendLineTabbed(tabCount, "}");

            System.IO.File.WriteAllText("C:\\projects\\FG\\FG.SF.Issue4Reproduction\\src\\API\\Controllers\\ExperimentOutput.cs", contents.ToString());

        }



    }
}
