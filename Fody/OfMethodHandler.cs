using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    void HandleOfMethod(Instruction instruction, ILProcessor ilProcessor, MethodReference ofMethodReference)
    {
        //Info.OfMethod("AssemblyToProcess","MethodClass","InstanceMethod");

        Instruction methodNameInstruction;
        List<string> parameters;
        Instruction parametersInstruction = null;
        if (ofMethodReference.Parameters.Count == 4)
        {
            parametersInstruction = instruction.Previous;
            parameters = GetLdString(parametersInstruction)
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();
            methodNameInstruction = parametersInstruction.Previous;
        }
        else
        {
            methodNameInstruction = instruction.Previous;
            parameters = new List<string>();
        }

        var methodName = GetLdString(methodNameInstruction);

        var typeNameInstruction = methodNameInstruction.Previous;
        var typeName = GetLdString(typeNameInstruction);

        var assemblyNameInstruction = typeNameInstruction.Previous;
        var assemblyName = GetLdString(assemblyNameInstruction);

        var typeDefinition = GetTypeDefinition(assemblyName, typeName);

        var methodDefinitions = typeDefinition.FindMethodDefinitions(methodName, parameters);

        if (methodDefinitions.Count == 0)
        {
            throw new WeavingException($"Could not find method named '{methodName}'.")
            {
                SequencePoint = instruction.SequencePoint
            };
        }
        if (methodDefinitions.Count > 1)
        {
            throw new WeavingException($"More than one method named '{methodName}' found.")
            {
                SequencePoint = instruction.SequencePoint
            };
        }

        var methodReference = ModuleDefinition.ImportReference(methodDefinitions[0]);

        ilProcessor.Remove(typeNameInstruction);
        ilProcessor.Remove(methodNameInstruction);
        if (parametersInstruction != null)
        {
            ilProcessor.Remove(parametersInstruction);
        }

        assemblyNameInstruction.OpCode = OpCodes.Ldtoken;
        assemblyNameInstruction.Operand = methodReference;

        if (typeDefinition.HasGenericParameters)
        {
            var typeReference = ModuleDefinition.ImportReference(typeDefinition);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldtoken, typeReference));
            instruction.Operand = getMethodFromHandleGeneric;
        }
        else
        {
            instruction.Operand = getMethodFromHandle;
        }

        ilProcessor.InsertAfter(instruction, Instruction.Create(OpCodes.Castclass, methodInfoType));
    }
}