using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

using Jeevan.JsonUtils.Core;

namespace Jeevan.JsonUtils.SystemTextJson;

public sealed class JsonObjectModelBuilder : IJsonObjectModelBuilder
{
    public JsonObjectModel BuildModel(TextReader reader)
    {
        string json = reader.ReadToEnd();

        var document = JsonDocument.Parse(json);
        object model = document.RootElement.ValueKind switch
        {
            JsonValueKind.Object => new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase),
            JsonValueKind.Array => new List<object?>(),
            _ => throw new InvalidOperationException(
                $"Invalid JSON root element type {document.RootElement.ValueKind}."),
        };

        BuildModelDecider(document.RootElement, model);

        return new JsonObjectModel(model);
    }

    private static void BuildModelDecider(JsonElement element, object model)
    {
        if (element.ValueKind == JsonValueKind.Object && model is IDictionary<string, object?> dictionary)
        {
            foreach (JsonProperty property in element.EnumerateObject())
                BuildModel(property.Value, dictionary, (dict, value, name) => dict.Add(name!, value), property.Name);
        }
        else if (element.ValueKind == JsonValueKind.Array && model is IList<object?> list)
        {
            foreach (JsonElement itemElement in element.EnumerateArray())
                BuildModel(itemElement, list, (lst, value, _) => lst.Add(value));
        }
        else
        {
            throw new InvalidOperationException(
                $"Invalid JSON element/object model type combination - {element.ValueKind} / {model.GetType()}");
        }
    }

    private static void BuildModel<TModel>(JsonElement element, TModel model,
        Action<TModel, object?, string?> modelAppender, string? name = null)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Undefined:
            case JsonValueKind.Null:
                modelAppender(model, null, name);
                break;
            case JsonValueKind.Object:
                Dictionary<string, object?> objectModel = new(StringComparer.OrdinalIgnoreCase);
                modelAppender(model, objectModel, name);
                BuildModelDecider(element, objectModel);
                break;
            case JsonValueKind.Array:
                List<object?> listModel = new();
                modelAppender(model, listModel, name);
                BuildModelDecider(element, listModel);
                break;
            case JsonValueKind.String:
                modelAppender(model, element.GetString(), name);
                break;
            case JsonValueKind.Number:
                if (element.TryGetInt64(out long longValue))
                    modelAppender(model, longValue, name);
                else if (element.TryGetDouble(out double doubleValue))
                    modelAppender(model, doubleValue, name);
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                modelAppender(model, element.GetBoolean(), name);
                break;
            default:
                throw new InvalidOperationException($"Unrecognized JSON element type {element.ValueKind}.");
        }
    }
}
