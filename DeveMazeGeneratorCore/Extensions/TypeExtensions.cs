using System.Reflection;

namespace DeveMazeGeneratorCore.Extensions;

public static class TypeExtensions
{
    public static FieldInfo GetRequiredField(this Type type, string name)
    {
        return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new MissingFieldException(type.FullName, name);
    }

    public static object GetRequiredValue(this FieldInfo field, object receiver)
    {
        return field.GetValue(receiver) ?? throw new NullReferenceException($"receiver.{field.Name} value is null");
    }

    public static T? GetValue<T>(this FieldInfo field, object receiver)
    {
        var result = field.GetValue(receiver);
        if(result == null) return default;
        if(result is T t) return t;
        else throw new InvalidCastException($"Expected receiver.{field.Name} to contain a value of type {nameof(T)}");
    }

    public static T GetRequiredValue<T>(this FieldInfo field, object receiver)
    {
        var result = field.GetValue<T>(receiver);
        return result ?? throw new NullReferenceException($"receiver.{field.Name} value is null");
    }

    public static object? GetFieldValue(this object receiver, string name)
    {
        var field = receiver.GetType().GetRequiredField(name);
        return field.GetValue(receiver);
    }

    public static T? GetFieldValue<T>(this object receiver, string name)
    {
        var field = receiver.GetType().GetRequiredField(name);
        return field.GetValue<T>(receiver);
    }

    public static object GetRequiredFieldValue(this object receiver, string name)
    {
        var field = receiver.GetType().GetRequiredField(name);
        return field.GetRequiredValue(receiver);
    }

    public static T GetRequiredFieldValue<T>(this object receiver, string name)
    {
        var field = receiver.GetType().GetRequiredField(name);
        return field.GetRequiredValue<T>(receiver);
    }
}
