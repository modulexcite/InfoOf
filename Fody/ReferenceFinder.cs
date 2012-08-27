using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public void FindReferences()
    {
        var mscorlib = AssemblyResolver.Resolve("mscorlib");
        var mscorlibTypes = mscorlib.MainModule.Types;

        var methodBaseType = mscorlibTypes.First(x => x.Name == "MethodBase");
        getMethodFromHandle = methodBaseType.Methods
            .First(x => x.Name == "GetMethodFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeMethodHandle");
        getMethodFromHandle = ModuleDefinition.Import(getMethodFromHandle);
        getMethodFromHandleGeneric = methodBaseType.Methods
            .First(x => x.Name == "GetMethodFromHandle" &&
                        x.Parameters.Count == 2 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeMethodHandle" &&
                        x.Parameters[1].ParameterType.Name == "RuntimeTypeHandle");
        getMethodFromHandleGeneric = ModuleDefinition.Import(getMethodFromHandleGeneric);

        var fieldInfoType = mscorlibTypes.First(x => x.Name == "FieldInfo");
        getFieldFromHandle = fieldInfoType.Methods
            .First(x => x.Name == "GetFieldFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeFieldHandle");
        getFieldFromHandle = ModuleDefinition.Import(getFieldFromHandle);
        getFieldFromHandleGeneric = fieldInfoType.Methods
            .First(x => x.Name == "GetFieldFromHandle" &&
                        x.Parameters.Count == 2 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeFieldHandle" &&
                        x.Parameters[1].ParameterType.Name == "RuntimeTypeHandle");
        getFieldFromHandleGeneric = ModuleDefinition.Import(getFieldFromHandleGeneric);

        methodInfoType = mscorlibTypes.First(x => x.Name == "MethodInfo");
        methodInfoType = ModuleDefinition.Import(methodInfoType);

        propertyInfoType = mscorlibTypes.First(x => x.Name == "PropertyInfo");
        propertyInfoType = ModuleDefinition.Import(propertyInfoType);

        var typeType = mscorlibTypes.First(x => x.Name == "Type");
        getTypeFromHandle = typeType.Methods
            .First(x => x.Name == "GetTypeFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        getTypeFromHandle = ModuleDefinition.Import(getTypeFromHandle);
    }

    MethodReference getMethodFromHandle;
    MethodReference getTypeFromHandle;
    TypeReference methodInfoType;
    TypeReference propertyInfoType;
    MethodReference getFieldFromHandle;
    MethodReference getFieldFromHandleGeneric;
    MethodReference getMethodFromHandleGeneric;
}