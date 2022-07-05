using System;

namespace Jeevan.JsonUtils.Core;

public sealed class JsonObjectModelPath
{
    public JsonObjectModelPath(string part)
    {
        Part = part ?? throw new ArgumentNullException(nameof(part));
    }

    public JsonObjectModelPath(int index)
    {
        Index = index;
    }

    public string? Part { get; }

    public int? Index { get; }

    public static implicit operator JsonObjectModelPath(string part) => new(part);

    public static implicit operator JsonObjectModelPath(int index) => new(index);
}
