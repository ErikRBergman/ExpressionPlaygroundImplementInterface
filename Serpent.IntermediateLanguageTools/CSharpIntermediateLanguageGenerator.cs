using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serpent.IntermediateLanguageTools
{
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    using Mono.Reflection;

    using Serpent.IntermediateLanguageTools.Helpers;

    public class CSharpIntermediateLanguageGenerator
    {

        private struct MethodContext
        {
            public HashSet<LocalVariableInfo> LocalVariables { get; set; }


        }

        public string CreateMethodILGenerator(MethodBase method, bool verbose = false)
        {

            var instructions = method.GetInstructions();
            var builder = new StringBuilder();

            builder.AppendLine("public void Create_" + method.Name + "_Code(System.Reflection.Emit.ILGenerator generator)");

            builder.AppendLine("{");

            // builder.AppendLine("// TODO: Implement the method declaration (in in relation to type generics as well)");
            //builder.AppendLine(GetMethodInformation(method));

            int tabCount = 1;

            var labels = new HashSet<int>(
                instructions
                    .Where(i => i.Operand is Instruction)
                    .Select(i => ((Instruction)i.Operand).Offset));

            builder.AppendLineTabbed(tabCount, "// The local variables");
            var localVariables = GetLocalVariables(instructions).OrderBy(variable => variable.LocalIndex);
            builder.AppendLine(GetDefineLocalsText(localVariables, "generator", tabCount));

            builder.AppendLineTabbed(tabCount, "// The code");

            foreach (var instruction in instructions)
            {
                if (verbose)
                {
                    builder.AppendLineTabbed(tabCount, $"// {instruction}");
                }

                if (labels.Contains(instruction.Offset))
                {
                    builder.AppendLineTabbed(tabCount, $"var label_{instruction.Offset:X2} = generator.DefineLabel();");
                }

                builder.AppendLineTabbed(tabCount, GetInstructionEmitText(instruction) + ";");

                if (verbose)
                {
                    builder.AppendLine();
                }
            }

            builder.AppendLine("}");
            return builder.ToString();
        }

        private static string GetDefineLocalsText(IEnumerable<LocalVariableInfo> localVariables, string generatorName, int tabCount = 0)
        {
            var builder = new StringBuilder();

            foreach (var variable in localVariables)
            {
                builder.AppendLine(Tabs.GetTabs(tabCount) + "var local_" + variable.LocalIndex + " = " + (string.IsNullOrWhiteSpace(generatorName) == false ? generatorName + "." : string.Empty) + GetDefineLocalText(variable) + ";");
            }

            return builder.ToString();
        }

        private static string GetDefineLocalText(LocalVariableInfo variable)
        {
            if (variable.IsPinned)
            {
                return "DeclareLocal(typeof(" + variable.LocalType.FullName + "), true)";
            }

            return "DeclareLocal(typeof(" + variable.LocalType.FullName + "))";
        }

        private static HashSet<LocalVariableInfo> GetLocalVariables(IEnumerable<Instruction> instructions)
        {
            var variables = new HashSet<LocalVariableInfo>();

            foreach (var instruction in instructions)
            {
                if (instruction.Operand is LocalVariableInfo localVariableInfo)
                {
                    variables.Add(localVariableInfo);
                }
            }

            return variables;
        }


        private static string GetInstructionEmitText(Mono.Reflection.Instruction instruction)
        {
            var result = "generator.Emit(System.Reflection.Emit.OpCodes." + GetOpCodeText(instruction.OpCode);

            if (instruction.Operand != null)
            {
                if (instruction.Operand is LocalVariableInfo localVariable)
                {
                    return result + ", local_" + localVariable.LocalIndex + ")";
                }

                if (instruction.Operand is System.Reflection.TypeInfo typeInfo)
                {
                    return result + ", typeof(" + typeInfo.FullName + "))";
                }

                if (instruction.Operand is System.Reflection.MethodInfo methodInfo)
                {
                    return result + ")";
                    //return result + ", typeof(" + methodInfo.FullName + "))";
                }

                if (instruction.Operand is System.Reflection.FieldInfo fieldInfo)
                {
                    return result + ")";
                    //return result + ", typeof(" + typeInfo.FullName + "))";
                }

                if (instruction.Operand is Mono.Reflection.Instruction labelInstruction)
                {
                    return result + $", label_{labelInstruction.Offset:X2})";
                    //return result + ", typeof(" + typeInfo.FullName + "))";
                }

                //throw new NotImplementedException("Can't generate parameter of: " + instruction.Operand);

                var operandType = instruction.Operand?.GetType();

                Debug.WriteLine("unknown operand type: " + operandType.FullName);

                return result + ")";
            }

            return result + ")";
        }

        private static string GetOpCodeText(System.Reflection.Emit.OpCode opCode)
        {
            return opCode.Name.ToUpperFirst().Replace(".", "_").ToUpperFirstAfterFirstInstance('_');

            //switch (opCode.Name)
            //{
            //    case "nop":
            //        return "Nop";
            //    case "ldloca.s":
            //        return "Ldloca_s";
            //    case "initobj":
            //        return "Initobj";
            //    default:
            //        throw new NotImplementedException(opCode.ToString());
            //}
        }

    }

    public static class StringExtensions
    {
        public static string ToUpperFirst(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper();
            }

            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        public static string ToUpperFirst(this string text, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, 1).ToUpper(culture) + text.Substring(1);
        }

        public static string ToUpperFirstAfter(this string text, int startIndex)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper() + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfter(this string text, int startIndex, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(text) || startIndex >= text.Length)
            {
                return text;
            }

            if (text.Length == 1)
            {
                return text.ToUpper(culture);
            }

            return text.Substring(0, startIndex) + text.Substring(startIndex, 1).ToUpper(culture) + (text.Length <= startIndex + 1 ? string.Empty : text.Substring(startIndex + 1));
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance, CultureInfo culture)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1, culture);
        }

        public static string ToUpperFirstAfterFirstInstance(this string text, char firstInstance)
        {
            var index = text.IndexOf(firstInstance);
            if (index == -1)
            {
                return text;
            }

            return text.ToUpperFirstAfter(index + 1);
        }
    }
}
