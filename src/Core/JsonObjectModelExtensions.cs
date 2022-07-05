using System;
using System.Collections.Generic;

namespace Jeevan.JsonUtils.Core;

public static class JsonObjectModelExtensions
{
    public static T? GetValue<T>(this JsonObjectModel model, params JsonObjectModelPath[] path)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));
        if (path is null)
            throw new ArgumentNullException(nameof(path));
        if (path.Length == 0)
            throw new ArgumentException("The path is empty.", nameof(path));

        return GetValueDecider<T>(model.Model, path);
    }

    private static T? GetValueDecider<T>(object model, JsonObjectModelPath[] path)
    {
        if (model is IDictionary<string, object?> asObject)
            return GetValueFromObject<T>(asObject, path);

        if (model is IList<object?> asArray)
            return GetValueFromArray<T>(asArray, path);

        throw new InvalidOperationException($"Invalid model type: {model.GetType()}");
    }

    private static T? GetValueFromObject<T>(IDictionary<string, object?> dictionary, JsonObjectModelPath[] path)
    {
        JsonObjectModelPath part = path[0];
        if (part.Index is not null)
            throw new ArgumentException($"Cannot use index {part.Index.Value} in an object.", nameof(path));

        if (!dictionary.TryGetValue(part.Part!, out object? value))
            return default;

        if (value is null)
            return (T?)value;

        if (path.Length > 1)
            return GetValueDecider<T>(value, path[1..]);

        return (T)value;
    }

    private static T? GetValueFromArray<T>(IList<object?> list, JsonObjectModelPath[] path)
    {
        JsonObjectModelPath part = path[0];
        if (part.Part is not null)
            throw new ArgumentException($"Cannot use path part {part.Part} in an array.", nameof(path));

        int index = part.Index.GetValueOrDefault();
        if (index >= list.Count)
            throw new ArgumentOutOfRangeException($"List index {index} is out of range.", nameof(path));

        object? value = list[index];

        if (value is null)
            return (T?)value;

        if (path.Length > 1)
            return GetValueDecider<T>(value, path[1..]);

        return (T)value;
    }
}
