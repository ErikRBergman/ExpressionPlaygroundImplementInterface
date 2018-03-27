﻿namespace Serpent.IntermediateLanguageTools.Constants
{
    using System;
    using System.Collections.Generic;

    public static class InstructionTexts
    {
        static InstructionTexts()
        {
            // Texts taken from https://msdn.microsoft.com/en-us/library/system.reflection.emit.opcodes_fields(v=vs.110).aspx
            Texts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["Add"] = "Adds two values and pushes the result onto the evaluation stack.",
                            ["Add_Ovf"] =
                                "Adds two integers, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["Add_Ovf_Un"] =
                                "Adds two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["And"] =
                                "Computes the bitwise AND of two values and pushes the result onto the evaluation stack.",
                            ["Arglist"] =
                                "Returns an unmanaged pointer to the argument list of the current method.",
                            ["Beq"] = "Transfers control to a target instruction if two values are equal.",
                            ["Beq_S"] =
                                "Transfers control to a target instruction (short form) if two values are equal.",
                            ["Bge"] =
                                "Transfers control to a target instruction if the first value is greater than or equal to the second value.",
                            ["Bge_S"] =
                                "Transfers control to a target instruction (short form) if the first value is greater than or equal to the second value.",
                            ["Bge_Un"] =
                                "Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Bge_Un_S"] =
                                "Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Bgt"] =
                                "Transfers control to a target instruction if the first value is greater than the second value.",
                            ["Bgt_S"] =
                                "Transfers control to a target instruction (short form) if the first value is greater than the second value.",
                            ["Bgt_Un"] =
                                "Transfers control to a target instruction if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Bgt_Un_S"] =
                                "Transfers control to a target instruction (short form) if the first value is greater than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Ble"] =
                                "Transfers control to a target instruction if the first value is less than or equal to the second value.",
                            ["Ble_S"] =
                                "Transfers control to a target instruction (short form) if the first value is less than or equal to the second value.",
                            ["Ble_Un"] =
                                "Transfers control to a target instruction if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.",
                            ["Ble_Un_S"] =
                                "Transfers control to a target instruction (short form) if the first value is less than or equal to the second value, when comparing unsigned integer values or unordered float values.",
                            ["Blt"] =
                                "Transfers control to a target instruction if the first value is less than the second value.",
                            ["Blt_S"] =
                                "Transfers control to a target instruction (short form) if the first value is less than the second value.",
                            ["Blt_Un"] =
                                "Transfers control to a target instruction if the first value is less than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Blt_Un_S"] =
                                "Transfers control to a target instruction (short form) if the first value is less than the second value, when comparing unsigned integer values or unordered float values.",
                            ["Bne_Un"] =
                                "Transfers control to a target instruction when two unsigned integer values or unordered float values are not equal.",
                            ["Bne_Un_S"] =
                                "Transfers control to a target instruction (short form) when two unsigned integer values or unordered float values are not equal.",
                            ["Box"] = "Converts a value type to an object reference (type O).",
                            ["Br"] = "Unconditionally transfers control to a target instruction.",
                            ["Br_S"] = "Unconditionally transfers control to a target instruction (short form).",
                            ["Break"] =
                                "Signals the Common Language Infrastructure (CLI) to inform the debugger that a break point has been tripped.",
                            ["Brfalse"] =
                                "Transfers control to a target instruction if value is false, a null reference (Nothing in Visual Basic), or zero.",
                            ["Brfalse_S"] =
                                "Transfers control to a target instruction if value is false, a null reference, or zero.",
                            ["Brtrue"] =
                                "Transfers control to a target instruction if value is true, not null, or non-zero.",
                            ["Brtrue_S"] =
                                "Transfers control to a target instruction (short form) if value is true, not null, or non-zero.",
                            ["Call"] = "Calls the method indicated by the passed method descriptor.",
                            ["Calli"] =
                                "Calls the method indicated on the evaluation stack (as a pointer to an entry point) with arguments described by a calling convention.",
                            ["Callvirt"] =
                                "Calls a late-bound method on an object, pushing the return value onto the evaluation stack.",
                            ["Castclass"] =
                                "Attempts to cast an object passed by reference to the specified class.",
                            ["Ceq"] =
                                "Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.",
                            ["Cgt"] =
                                "Compares two values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.",
                            ["Cgt_Un"] =
                                "Compares two unsigned or unordered values. If the first value is greater than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.",
                            ["Ckfinite"] = "Throws ArithmeticException if value is not a finite number.",
                            ["Clt"] =
                                "Compares two values. If the first value is less than the second, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.",
                            ["Clt_Un"] =
                                "Compares the unsigned or unordered values value1 and value2. If value1 is less than value2, then the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.",
                            ["Constrained"] = "Constrains the type on which a virtual method call is made.",
                            ["Conv_I"] = "Converts the value on top of the evaluation stack to native int.",
                            ["Conv_I1"] =
                                "Converts the value on top of the evaluation stack to int8, then extends (pads) it to int32.",
                            ["Conv_I2"] =
                                "Converts the value on top of the evaluation stack to int16, then extends (pads) it to int32.",
                            ["Conv_I4"] = "Converts the value on top of the evaluation stack to int32.",
                            ["Conv_I8"] = "Converts the value on top of the evaluation stack to int64.",
                            ["Conv_Ovf_I"] =
                                "Converts the signed value on top of the evaluation stack to signed native int, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to signed native int, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I1"] =
                                "Converts the signed value on top of the evaluation stack to signed int8 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I1_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to signed int8 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I2"] =
                                "Converts the signed value on top of the evaluation stack to signed int16 and extending it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I2_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to signed int16 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I4"] =
                                "Converts the signed value on top of the evaluation stack to signed int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I4_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to signed int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I8"] =
                                "Converts the signed value on top of the evaluation stack to signed int64, throwing OverflowException on overflow.",
                            ["Conv_Ovf_I8_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to signed int64, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U"] =
                                "Converts the signed value on top of the evaluation stack to unsigned native int, throwing OverflowExceptionon overflow.",
                            ["Conv_Ovf_U_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to unsigned native int, throwing OverflowExceptionon overflow.",
                            ["Conv_Ovf_U1"] =
                                "Converts the signed value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U1_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to unsigned int8 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U2"] =
                                "Converts the signed value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U2_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to unsigned int16 and extends it to int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U4"] =
                                "Converts the signed value on top of the evaluation stack to unsigned int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U4_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to unsigned int32, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U8"] =
                                "Converts the signed value on top of the evaluation stack to unsigned int64, throwing OverflowException on overflow.",
                            ["Conv_Ovf_U8_Un"] =
                                "Converts the unsigned value on top of the evaluation stack to unsigned int64, throwing OverflowException on overflow.",
                            ["Conv_R_Un"] =
                                "Converts the unsigned integer value on top of the evaluation stack to float32.",
                            ["Conv_R4"] = "Converts the value on top of the evaluation stack to float32.",
                            ["Conv_R8"] = "Converts the value on top of the evaluation stack to float64.",
                            ["Conv_U"] =
                                "Converts the value on top of the evaluation stack to unsigned native int, and extends it to native int.",
                            ["Conv_U1"] =
                                "Converts the value on top of the evaluation stack to unsigned int8, and extends it to int32.",
                            ["Conv_U2"] =
                                "Converts the value on top of the evaluation stack to unsigned int16, and extends it to int32.",
                            ["Conv_U4"] =
                                "Converts the value on top of the evaluation stack to unsigned int32, and extends it to int32.",
                            ["Conv_U8"] =
                                "Converts the value on top of the evaluation stack to unsigned int64, and extends it to int64.",
                            ["Cpblk"] =
                                "Copies a specified number bytes from a source address to a destination address.",
                            ["Cpobj"] =
                                "Copies the value type located at the address of an object (type &, * or native int) to the address of the destination object (type &, * or native int).",
                            ["Div"] =
                                "Divides two values and pushes the result as a floating-point (type F) or quotient (type int32) onto the evaluation stack.",
                            ["Div_Un"] =
                                "Divides two unsigned integer values and pushes the result (int32) onto the evaluation stack.",
                            ["Dup"] =
                                "Copies the current topmost value on the evaluation stack, and then pushes the copy onto the evaluation stack.",
                            ["Endfilter"] =
                                "Transfers control from the filter clause of an exception back to the Common Language Infrastructure (CLI) exception handler.",
                            ["Endfinally"] =
                                "Transfers control from the fault or finally clause of an exception block back to the Common Language Infrastructure (CLI) exception handler.",
                            ["Initblk"] =
                                "Initializes a specified block of memory at a specific address to a given size and initial value.",
                            ["Initobj"] =
                                "Initializes each field of the value type at a specified address to a null reference or a 0 of the appropriate primitive type.",
                            ["Isinst"] =
                                "Tests whether an object reference (type O) is an instance of a particular class.",
                            ["Jmp"] = "Exits current method and jumps to specified method.",
                            ["Ldarg"] =
                                "Loads an argument (referenced by a specified index value) onto the stack.",
                            ["Ldarg_0"] = "Loads the argument at index 0 onto the evaluation stack.",
                            ["Ldarg_1"] = "Loads the argument at index 1 onto the evaluation stack.",
                            ["Ldarg_2"] = "Loads the argument at index 2 onto the evaluation stack.",
                            ["Ldarg_3"] = "Loads the argument at index 3 onto the evaluation stack.",
                            ["Ldarg_S"] =
                                "Loads the argument (referenced by a specified short form index) onto the evaluation stack.",
                            ["Ldarga"] = "Load an argument address onto the evaluation stack.",
                            ["Ldarga_S"] = "Load an argument address, in short form, onto the evaluation stack.",
                            ["Ldc_I4"] =
                                "Pushes a supplied value of type int32 onto the evaluation stack as an int32.",
                            ["Ldc_I4_0"] = "Pushes the integer value of 0 onto the evaluation stack as an int32.",
                            ["Ldc_I4_1"] = "Pushes the integer value of 1 onto the evaluation stack as an int32.",
                            ["Ldc_I4_2"] = "Pushes the integer value of 2 onto the evaluation stack as an int32.",
                            ["Ldc_I4_3"] = "Pushes the integer value of 3 onto the evaluation stack as an int32.",
                            ["Ldc_I4_4"] = "Pushes the integer value of 4 onto the evaluation stack as an int32.",
                            ["Ldc_I4_5"] = "Pushes the integer value of 5 onto the evaluation stack as an int32.",
                            ["Ldc_I4_6"] = "Pushes the integer value of 6 onto the evaluation stack as an int32.",
                            ["Ldc_I4_7"] = "Pushes the integer value of 7 onto the evaluation stack as an int32.",
                            ["Ldc_I4_8"] = "Pushes the integer value of 8 onto the evaluation stack as an int32.",
                            ["Ldc_I4_M1"] =
                                "Pushes the integer value of -1 onto the evaluation stack as an int32.",
                            ["Ldc_I4_S"] =
                                "Pushes the supplied int8 value onto the evaluation stack as an int32, short form.",
                            ["Ldc_I8"] =
                                "Pushes a supplied value of type int64 onto the evaluation stack as an int64.",
                            ["Ldc_R4"] =
                                "Pushes a supplied value of type float32 onto the evaluation stack as type F (float).",
                            ["Ldc_R8"] =
                                "Pushes a supplied value of type float64 onto the evaluation stack as type F (float).",
                            ["Ldelem"] =
                                "Loads the element at a specified array index onto the top of the evaluation stack as the type specified in the instruction.",
                            ["Ldelem_I"] =
                                "Loads the element with type native int at a specified array index onto the top of the evaluation stack as a native int.",
                            ["Ldelem_I1"] =
                                "Loads the element with type int8 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelem_I2"] =
                                "Loads the element with type int16 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelem_I4"] =
                                "Loads the element with type int32 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelem_I8"] =
                                "Loads the element with type int64 at a specified array index onto the top of the evaluation stack as an int64.",
                            ["Ldelem_R4"] =
                                "Loads the element with type float32 at a specified array index onto the top of the evaluation stack as type F(float).",
                            ["Ldelem_R8"] =
                                "Loads the element with type float64 at a specified array index onto the top of the evaluation stack as type F(float).",
                            ["Ldelem_Ref"] =
                                "Loads the element containing an object reference at a specified array index onto the top of the evaluation stack as type O (object reference).",
                            ["Ldelem_U1"] =
                                "Loads the element with type unsigned int8 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelem_U2"] =
                                "Loads the element with type unsigned int16 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelem_U4"] =
                                "Loads the element with type unsigned int32 at a specified array index onto the top of the evaluation stack as an int32.",
                            ["Ldelema"] =
                                "Loads the address of the array element at a specified array index onto the top of the evaluation stack as type &(managed pointer).",
                            ["Ldfld"] =
                                "Finds the value of a field in the object whose reference is currently on the evaluation stack.",
                            ["Ldflda"] =
                                "Finds the address of a field in the object whose reference is currently on the evaluation stack.",
                            ["Ldftn"] =
                                "Pushes an unmanaged pointer (type native int) to the native code implementing a specific method onto the evaluation stack.",
                            ["Ldind_I"] =
                                "Loads a value of type native int as a native int onto the evaluation stack indirectly.",
                            ["Ldind_I1"] =
                                "Loads a value of type int8 as an int32 onto the evaluation stack indirectly.",
                            ["Ldind_I2"] =
                                "Loads a value of type int16 as an int32 onto the evaluation stack indirectly.",
                            ["Ldind_I4"] =
                                "Loads a value of type int32 as an int32 onto the evaluation stack indirectly.",
                            ["Ldind_I8"] =
                                "Loads a value of type int64 as an int64 onto the evaluation stack indirectly.",
                            ["Ldind_R4"] =
                                "Loads a value of type float32 as a type F (float) onto the evaluation stack indirectly.",
                            ["Ldind_R8"] =
                                "Loads a value of type float64 as a type F (float) onto the evaluation stack indirectly.",
                            ["Ldind_Ref"] =
                                "Loads an object reference as a type O (object reference) onto the evaluation stack indirectly.",
                            ["Ldind_U1"] =
                                "Loads a value of type unsigned int8 as an int32 onto the evaluation stack indirectly.",
                            ["Ldind_U2"] =
                                "Loads a value of type unsigned int16 as an int32 onto the evaluation stack indirectly.",
                            ["Ldind_U4"] =
                                "Loads a value of type unsigned int32 as an int32 onto the evaluation stack indirectly.",
                            ["Ldlen"] =
                                "Pushes the number of elements of a zero-based, one-dimensional array onto the evaluation stack.",
                            ["Ldloc"] = "Loads the local variable at a specific index onto the evaluation stack.",
                            ["Ldloc_0"] = "Loads the local variable at index 0 onto the evaluation stack.",
                            ["Ldloc_1"] = "Loads the local variable at index 1 onto the evaluation stack.",
                            ["Ldloc_2"] = "Loads the local variable at index 2 onto the evaluation stack.",
                            ["Ldloc_3"] = "Loads the local variable at index 3 onto the evaluation stack.",
                            ["Ldloc_S"] =
                                "Loads the local variable at a specific index onto the evaluation stack, short form.",
                            ["Ldloca"] =
                                "Loads the address of the local variable at a specific index onto the evaluation stack.",
                            ["Ldloca_S"] =
                                "Loads the address of the local variable at a specific index onto the evaluation stack, short form.",
                            ["Ldnull"] = "Pushes a null reference (type O) onto the evaluation stack.",
                            ["Ldobj"] =
                                "Copies the value type object pointed to by an address to the top of the evaluation stack.",
                            ["Ldsfld"] = "Pushes the value of a static field onto the evaluation stack.",
                            ["Ldsflda"] = "Pushes the address of a static field onto the evaluation stack.",
                            ["Ldstr"] =
                                "Pushes a new object reference to a string literal stored in the metadata.",
                            ["Ldtoken"] =
                                "Converts a metadata token to its runtime representation, pushing it onto the evaluation stack.",
                            ["Ldvirtftn"] =
                                "Pushes an unmanaged pointer (type native int) to the native code implementing a particular virtual method associated with a specified object onto the evaluation stack.",
                            ["Leave"] =
                                "Exits a protected region of code, unconditionally transferring control to a specific target instruction.",
                            ["Leave_S"] =
                                "Exits a protected region of code, unconditionally transferring control to a target instruction (short form).",
                            ["Localloc"] =
                                "Allocates a certain number of bytes from the local dynamic memory pool and pushes the address (a transient pointer, type *) of the first allocated byte onto the evaluation stack.",
                            ["Mkrefany"] =
                                "Pushes a typed reference to an instance of a specific type onto the evaluation stack.",
                            ["Mul"] = "Multiplies two values and pushes the result on the evaluation stack.",
                            ["Mul_Ovf"] =
                                "Multiplies two integer values, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["Mul_Ovf_Un"] =
                                "Multiplies two unsigned integer values, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["Neg"] = "Negates a value and pushes the result onto the evaluation stack.",
                            ["Newarr"] =
                                "Pushes an object reference to a new zero-based, one-dimensional array whose elements are of a specific type onto the evaluation stack.",
                            ["Newobj"] =
                                "Creates a new object or a new instance of a value type, pushing an object reference (type O) onto the evaluation stack.",
                            ["Nop"] =
                                "Fills space if opcodes are patched. No meaningful operation is performed although a processing cycle can be consumed.",
                            ["Not"] =
                                "Computes the bitwise complement of the integer value on top of the stack and pushes the result onto the evaluation stack as the same type.",
                            ["Or"] =
                                "Compute the bitwise complement of the two integer values on top of the stack and pushes the result onto the evaluation stack.",
                            ["Pop"] = "Removes the value currently on top of the evaluation stack.",
                            ["Prefix1"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix2"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix3"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix4"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix5"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix6"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefix7"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Prefixref"] =
                                "This API supports the product infrastructure and is not intended to be used directly from your code. This is a reserved instruction.",
                            ["Readonly"] =
                                "Specifies that the subsequent array address operation performs no type check at run time, and that it returns a managed pointer whose mutability is restricted.",
                            ["Refanytype"] = "Retrieves the type token embedded in a typed reference.",
                            ["Refanyval"] = "Retrieves the address (type &) embedded in a typed reference.",
                            ["Rem"] = "Divides two values and pushes the remainder onto the evaluation stack.",
                            ["Rem_Un"] =
                                "Divides two unsigned values and pushes the remainder onto the evaluation stack.",
                            ["Ret"] =
                                "Returns from the current method, pushing a return value (if present) from the callee's evaluation stack onto the caller's evaluation stack.",
                            ["Rethrow"] = "Rethrows the current exception.",
                            ["Shl"] =
                                "Shifts an integer value to the left (in zeroes) by a specified number of bits, pushing the result onto the evaluation stack.",
                            ["Shr"] =
                                "Shifts an integer value (in sign) to the right by a specified number of bits, pushing the result onto the evaluation stack.",
                            ["Shr_Un"] =
                                "Shifts an unsigned integer value (in zeroes) to the right by a specified number of bits, pushing the result onto the evaluation stack.",
                            ["Sizeof"] =
                                "Pushes the size, in bytes, of a supplied value type onto the evaluation stack.",
                            ["Starg"] =
                                "Stores the value on top of the evaluation stack in the argument slot at a specified index.",
                            ["Starg_S"] =
                                "Stores the value on top of the evaluation stack in the argument slot at a specified index, short form.",
                            ["Stelem"] =
                                "Replaces the array element at a given index with the value on the evaluation stack, whose type is specified in the instruction.",
                            ["Stelem_I"] =
                                "Replaces the array element at a given index with the native int value on the evaluation stack.",
                            ["Stelem_I1"] =
                                "Replaces the array element at a given index with the int8 value on the evaluation stack.",
                            ["Stelem_I2"] =
                                "Replaces the array element at a given index with the int16 value on the evaluation stack.",
                            ["Stelem_I4"] =
                                "Replaces the array element at a given index with the int32 value on the evaluation stack.",
                            ["Stelem_I8"] =
                                "Replaces the array element at a given index with the int64 value on the evaluation stack.",
                            ["Stelem_R4"] =
                                "Replaces the array element at a given index with the float32 value on the evaluation stack.",
                            ["Stelem_R8"] =
                                "Replaces the array element at a given index with the float64 value on the evaluation stack.",
                            ["Stelem_Ref"] =
                                "Replaces the array element at a given index with the object ref value (type O) on the evaluation stack.",
                            ["Stfld"] =
                                "Replaces the value stored in the field of an object reference or pointer with a new value.",
                            ["Stind_I"] = "Stores a value of type native int at a supplied address.",
                            ["Stind_I1"] = "Stores a value of type int8 at a supplied address.",
                            ["Stind_I2"] = "Stores a value of type int16 at a supplied address.",
                            ["Stind_I4"] = "Stores a value of type int32 at a supplied address.",
                            ["Stind_I8"] = "Stores a value of type int64 at a supplied address.",
                            ["Stind_R4"] = "Stores a value of type float32 at a supplied address.",
                            ["Stind_R8"] = "Stores a value of type float64 at a supplied address.",
                            ["Stind_Ref"] = "Stores a object reference value at a supplied address.",
                            ["Stloc"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at a specified index.",
                            ["Stloc_0"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 0.",
                            ["Stloc_1"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 1.",
                            ["Stloc_2"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 2.",
                            ["Stloc_3"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index 3.",
                            ["Stloc_S"] =
                                "Pops the current value from the top of the evaluation stack and stores it in a the local variable list at index (short form).",
                            ["Stobj"] =
                                "Copies a value of a specified type from the evaluation stack into a supplied memory address.",
                            ["Stsfld"] =
                                "Replaces the value of a static field with a value from the evaluation stack.",
                            ["Sub"] =
                                "Subtracts one value from another and pushes the result onto the evaluation stack.",
                            ["Sub_Ovf"] =
                                "Subtracts one integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["Sub_Ovf_Un"] =
                                "Subtracts one unsigned integer value from another, performs an overflow check, and pushes the result onto the evaluation stack.",
                            ["Switch"] = "Implements a jump table.",
                            ["Tailcall"] =
                                "Performs a postfixed method call instruction such that the current method's stack frame is removed before the actual call instruction is executed.",
                            ["Throw"] = "Throws the exception object currently on the evaluation stack.",
                            ["Unaligned"] =
                                "Indicates that an address currently atop the evaluation stack might not be aligned to the natural size of the immediately following ldind, stind, ldfld, stfld, ldobj, stobj, initblk, or cpblk instruction.",
                            ["Unbox"] = "Converts the boxed representation of a value type to its unboxed form.",
                            ["Unbox_Any"] =
                                "Converts the boxed representation of a type specified in the instruction to its unboxed form.",
                            ["Volatile"] =
                                "Specifies that an address currently atop the evaluation stack might be volatile, and the results of reading that location cannot be cached or that multiple stores to that location cannot be suppressed.",
                            ["Xor"] =
                                "Computes the bitwise XOR of the top two values on the evaluation stack, pushing the result onto the evaluation stack."
                        };
        }

        public static IReadOnlyDictionary<string, string> Texts { get; }
    }
}