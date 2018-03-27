namespace Serpent.IntermediateLanguageTools
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Text;

    using Mono.Reflection;

    using Serpent.IntermediateLanguageTools.Constants;
    using Serpent.IntermediateLanguageTools.Helpers;
    using Serpent.IntermediateLanguageTools.Models;

    public class CSharpIntermediateLanguageGenerator
    {
        private static readonly Dictionary<string, string> opcodeNames;

        static CSharpIntermediateLanguageGenerator()
        {
            opcodeNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in typeof(OpCodes).GetFields().Select(f => f.Name))
            {
                opcodeNames.Add(name, name);
            }
        }

        public string CreateMethodILGenerator(CreateMethodILGeneratorParameters createMethodIlGeneratorParameters)
        {
            var instructions = createMethodIlGeneratorParameters.Method.GetInstructions();
            var builder = new StringBuilder();


            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "public void Create_" + createMethodIlGeneratorParameters.Method.Name + "_Code(System.Reflection.Emit.ILGenerator generator)");
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "{");

            // builder.AppendLine("// TODO: Implement the method declaration (in in relation to type generics as well)");
            // builder.AppendLine(GetMethodInformation(method));
            createMethodIlGeneratorParameters.TabCount++;

            // Local variables
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// The local variables");

            var body = createMethodIlGeneratorParameters.Method.GetMethodBody();
            var localVariables = body.LocalVariables.OrderBy(lv => lv.LocalIndex).ToArray();
            builder.Append(GetDefineLocalsText(localVariables, "generator", createMethodIlGeneratorParameters.TabCount));

            // add labels
            var labels = instructions.Where(i => i.Operand is Instruction).Select(i => ((Instruction)i.Operand).Offset).Distinct().OrderBy(i => i);

            var labelSet = new HashSet<int>(labels);

            builder.AppendLine();
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// Add labels");

            foreach (var label in labels)
            {
                builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"var label_{label:X2} = generator.DefineLabel();");
            }

            builder.AppendLine();
            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "// The code");

            foreach (var instruction in instructions)
            {
                if (createMethodIlGeneratorParameters.Verbose)
                {
                    builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"// {GetInstructionVerboseText(instruction, localVariables)}");
                }

                if (labelSet.Contains(instruction.Offset))
                {
                    builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, $"generator.MarkLabel(label_{instruction.Offset:X2});");
                }

                builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, GetInstructionEmitText(instruction, createMethodIlGeneratorParameters.DotNetType.SourceType) + ";");

                if (createMethodIlGeneratorParameters.Verbose)
                {
                    builder.AppendLine();
                }
            }

            createMethodIlGeneratorParameters.TabCount--;

            builder.AppendLineTabbed(createMethodIlGeneratorParameters.TabCount, "}");
            return builder.ToString();
        }

        private static string GetInstructionVerboseText(Instruction instruction, LocalVariableInfo[] locals, bool isStatic = false)
        {
            int opcodeValue = (int)instruction.OpCode.Value;
            string opcodeNetName = instruction.OpCode.Name.Replace(".", "_");
            string opcodeDescription = InstructionTexts.Texts[opcodeNetName];

            if (instruction.OpCode == OpCodes.Ldarg_0)
            {
                return instruction.ToString() + " - " + opcodeDescription + " Should be \"this.\" in c#";
            }

            if (opcodeValue >= (int)OpCodeValues.Ldloc_0 && opcodeValue <= (int)OpCodeValues.Ldloc_3)
            {
                var variableNumber = opcodeValue - (int)OpCodeValues.Ldloc_0;
                return instruction.OpCode.Name + $" {locals[variableNumber].LocalType} ({variableNumber}) - " + opcodeDescription;
            }

            if (instruction.OpCode == OpCodes.Ldloc)
            {
                var variableNumber = ((LocalVariableInfo)instruction.Operand).LocalIndex;
                return instruction.OpCode.Name + $" {locals[variableNumber].LocalType} ({variableNumber}) - " + opcodeDescription;
            }

            if (opcodeValue >= (int)OpCodeValues.Stloc_0 && opcodeValue <= (int)OpCodeValues.Stloc_3)
            {
                var variableNumber = opcodeValue - (int)OpCodeValues.Stloc_0;
                return instruction.OpCode.Name + $" {locals[variableNumber].LocalType} ({variableNumber}) - " + opcodeDescription;
            }

            if (instruction.OpCode == OpCodes.Stloc || instruction.OpCode == OpCodes.Stloc_S)
            {
                var variableNumber = ((LocalVariableInfo)instruction.Operand).LocalIndex;
                return instruction.OpCode.Name + $" {locals[variableNumber].LocalType} ({variableNumber}) - " + opcodeDescription;
            }

            return instruction.ToString() + " - " + opcodeDescription;
        }

        private static string GetDefineLocalsText(IEnumerable<LocalVariableInfo> localVariables, string generatorName, int tabCount = 0)
        {
            var builder = new StringBuilder();

            foreach (var variable in localVariables)
            {
                builder.AppendLineTabbed(
                    tabCount,
                    "var local_"
                    + variable.LocalIndex
                    + " = "
                    + (string.IsNullOrWhiteSpace(generatorName) == false ? generatorName + "." : string.Empty)
                    + GetDefineLocalText(variable)
                    + ";");
            }

            return builder.ToString();
        }

        private static string GetDefineLocalText(LocalVariableInfo variable)
        {
            var declareLocalStart = "DeclareLocal(typeof(" + variable.LocalType.GetCSharpName(true, true) + ")";

            if (variable.IsPinned)
            {
                return declareLocalStart + ", true)";
            }

            return declareLocalStart + ")";
        }

        private static string GetInstructionEmitText(Instruction instruction, Type declaringType = null)
        {
            var result = "generator.Emit(System.Reflection.Emit.OpCodes." + GetOpCodeText(instruction.OpCode);

            if (instruction.Operand != null)
            {
                if (instruction.Operand is LocalVariableInfo localVariable)
                {
                    return $"{result}, local_{localVariable.LocalIndex})";
                }

                if (instruction.Operand is TypeInfo typeInfo)
                {
                    return $"{result}, typeof({typeInfo.GetCSharpName(true, true)}))";
                }

                if (instruction.Operand is MethodInfo methodInfo)
                {
                    ////var method = methodInfo.DeclaringType.GetMethod(
                    ////    methodInfo.Name,
                    ////    BindingFlags.Public | BindingFlags.NonPublic,
                    ////    null,
                    ////    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
                    ////    null);

                    // TODO: Make it possible to reference a method not created yet, in a type not created yet
                    var parameters = methodInfo.GetParameters();

                    string parameterTypes;

                    if (parameters.Length == 0)
                    {
                        parameterTypes = "System.Type.EmptyTypes";
                    }
                    else
                    {
                        parameterTypes = $"new[] {{ {string.Join(", ", parameters.Select(p => $"typeof({p.ParameterType.GetCSharpName(true, true)})"))} }}";
                    }

                    return
                        $"{result}, typeof({methodInfo.DeclaringType.GetCSharpName(true, true)}).GetMethod(\"{methodInfo.Name}\", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic, null, {parameterTypes}, null))";
                }

                if (instruction.Operand is FieldInfo fieldInfo)
                {
                    // TODO: Make it possible to reference a field not created yet, in a type not created yet
                    var field = fieldInfo.DeclaringType.GetField(fieldInfo.Name, BindingFlags.Public | BindingFlags.NonPublic);

                    return
                        $"{result}, typeof({fieldInfo.DeclaringType.GetCSharpName(true, true)}).GetField(\"{fieldInfo.Name}\", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))";
                }

                if (instruction.Operand is Instruction labelInstruction)
                {
                    return result + $", label_{labelInstruction.Offset:X2})";
                }

                if (instruction.Operand is sbyte || instruction.Operand is short || instruction.Operand is int ||
                        instruction.Operand is byte || instruction.Operand is ushort || instruction.Operand is uint ||
                        instruction.Operand is long || instruction.Operand is ulong || instruction.Operand is float ||
                    instruction.Operand is double || instruction.Operand is decimal)
                {
                    return result + $", {instruction.Operand})";
                }

                if (instruction.Operand is string stringOperand)
                {
                    stringOperand = "\"" + stringOperand.Replace("\"", "\"\"") + "\"";
                    return result + $", {stringOperand})";
                }


                var operandType = instruction.Operand?.GetType();

                Debug.WriteLine("unknown operand type: " + operandType.FullName);
                throw new NotImplementedException("Can't generate parameter of: " + operandType.FullName + ", value: " + instruction.Operand);

                return result + ")";
            }

            return result + ")";
        }

        private static string GetOpCodeText(OpCode opCode)
        {
            return opcodeNames[opCode.Name.Replace(".", "_")];
            // return opCode.Name.ToUpperFirst().Replace(".", "_").ToUpperFirstAfterFirstInstance('_');
        }

        private struct MethodContext
        {
            public HashSet<LocalVariableInfo> LocalVariables { get; set; }
        }
    }
}