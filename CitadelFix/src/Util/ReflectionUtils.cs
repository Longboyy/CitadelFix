using System;
using System.Reflection;

namespace CitadelFix.Util;

public static class ReflectionUtils
{

    public static T GetProperty<T>(this Type type, string name, BindingFlags flags = BindingFlags.Public | BindingFlags.Static)
    {
        return (T)type.GetProperty(name, flags)?.GetValue(null);
    }

}