using System.Reflection;

namespace CoreService.Common.Architecture;

public static class ServiceContractGuard
{
    public static void ValidateNoTupleReturns(Assembly assembly)
    {
        var offenders = assembly
            .GetTypes()
            .Where(t => t.IsInterface && t.Name.StartsWith("I", StringComparison.Ordinal) && t.Namespace?.Contains(".Services", StringComparison.Ordinal) == true)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => new { InterfaceType = t, Method = m, ReturnType = UnwrapTaskType(m.ReturnType) }))
            .Where(x => x.ReturnType is not null && IsTupleLike(x.ReturnType))
            .Select(x => $"{x.InterfaceType.FullName}.{x.Method.Name} -> {x.ReturnType!.Name}")
            .ToList();

        if (offenders.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Service interfaces must not return tuple types. Use DTO/result models instead. Offenders: " +
            string.Join("; ", offenders));
    }

    private static Type? UnwrapTaskType(Type returnType)
    {
        if (!returnType.IsGenericType)
        {
            return returnType;
        }

        var def = returnType.GetGenericTypeDefinition();
        if (def == typeof(Task<>) || def == typeof(ValueTask<>))
        {
            return returnType.GetGenericArguments()[0];
        }

        return returnType;
    }

    private static bool IsTupleLike(Type type)
    {
        if (!type.IsGenericType)
        {
            return false;
        }

        var def = type.GetGenericTypeDefinition();
        return def == typeof(Tuple<>) ||
               def == typeof(Tuple<,>) ||
               def == typeof(Tuple<,,>) ||
               def == typeof(Tuple<,,,>) ||
               def == typeof(Tuple<,,,,>) ||
               def == typeof(Tuple<,,,,,>) ||
               def == typeof(Tuple<,,,,,,>) ||
               def == typeof(Tuple<,,,,,,,>) ||
               def == typeof(ValueTuple<>) ||
               def == typeof(ValueTuple<,>) ||
               def == typeof(ValueTuple<,,>) ||
               def == typeof(ValueTuple<,,,>) ||
               def == typeof(ValueTuple<,,,,>) ||
               def == typeof(ValueTuple<,,,,,>) ||
               def == typeof(ValueTuple<,,,,,,>) ||
               def == typeof(ValueTuple<,,,,,,,>);
    }
}

