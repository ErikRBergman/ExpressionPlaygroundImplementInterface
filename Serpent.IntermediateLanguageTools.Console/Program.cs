using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.IntermediateLanguageTools.Console
{
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Serpent.IntermediateLanguageTools.Console.Test;

    class Program
    {
        static void Main(string[] args)
        {
            var generator = new CSharpIntermediateLanguageGenerator();
            
            var contents = new StringBuilder();

            contents.AppendLine("namespace API.Controllers");
            contents.AppendLine("{");

            contents.AppendLine("public class ExperimentOuput");
            contents.AppendLine("{");


            contents.Append(generator.CreateMethodILGenerator(typeof(Experiments).GetMethod("DoItAsync"), true));
            Debug.WriteLine(contents);

            contents.AppendLine("}");
            contents.AppendLine("}");

            System.IO.File.WriteAllText("C:\\projects\\FG\\FG.SF.Issue4Reproduction\\src\\API\\Controllers\\ExperimentOutput.cs", contents.ToString());

        }



    }
}
