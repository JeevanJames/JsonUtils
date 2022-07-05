using System;

using Jeevan.JsonUtils.Core;

using Shouldly;

using Xunit;
using Xunit.DataAttributes;

namespace Jeevan.JsonUtils.SystemTextJson.UnitTests;

public sealed class JsonObjectModelBuilderTests
{
    private readonly IJsonObjectModelBuilder _builder = new JsonObjectModelBuilder();

    [Theory]
    [EmbeddedResourceContent("Jeevan.JsonUtils.SystemTextJson.UnitTests.object.json")]
    [EmbeddedResourceContent("Jeevan.JsonUtils.SystemTextJson.UnitTests.array.json")]
    public void Can_build_model_from_json(string json)
    {
        Should.NotThrow(() => _builder.BuildModel(json));
    }

    [Theory]
    [EmbeddedResourceContent("Jeevan.JsonUtils.SystemTextJson.UnitTests.object.json")]
    public void Can_read_property_in_object_model(string json)
    {
        JsonObjectModel model = _builder.BuildModel(json);

        string? str = model.GetValue<string>("string");
        str.ShouldBe("A string value");

        long longValue = model.GetValue<long>("integer");
        longValue.ShouldBe(50);

        longValue = model.GetValue<long>("simpleArray", 1);
        longValue.ShouldBe(2);
    }

    [Theory]
    [EmbeddedResourceContent("Jeevan.JsonUtils.SystemTextJson.UnitTests.array.json")]
    public void Can_read_property_in_array_model(string json)
    {
        JsonObjectModel model = _builder.BuildModel(json);

        string? str = model.GetValue<string>(0);
        str.ShouldBe("Jeevan");

        str = model.GetValue<string>(1);
        str.ShouldBe("James");
    }

    [Theory]
    [EmbeddedResourceContent("Jeevan.JsonUtils.SystemTextJson.UnitTests.plain_string.json")]
    public void Throws_on_loading_plain_json(string json)
    {
        Should.Throw<InvalidOperationException>(() => _builder.BuildModel(json));
    }
}
