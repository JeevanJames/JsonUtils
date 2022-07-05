using System.IO;

namespace Jeevan.JsonUtils.Core;

public interface IJsonObjectModelBuilder
{
    JsonObjectModel BuildModel(TextReader reader);
}

public static class JsonObjectModelBuildExtensions
{
    public static JsonObjectModel BuildModel(this IJsonObjectModelBuilder builder, string json)
    {
        TextReader reader = new StringReader(json);
        return builder.BuildModel(reader);
    }

    public static JsonObjectModel BuildModel(this IJsonObjectModelBuilder builder, Stream jsonStream)
    {
        TextReader reader = new StreamReader(jsonStream);
        return builder.BuildModel(reader);
    }
}
