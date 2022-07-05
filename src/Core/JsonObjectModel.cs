using System;

namespace Jeevan.JsonUtils.Core;

public sealed class JsonObjectModel
{
    public JsonObjectModel(object model)
    {
        Model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public object Model { get; }
}
