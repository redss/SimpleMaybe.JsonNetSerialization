# SimpleMaybe.JsonNetSerialization

[![AppVeyor](https://img.shields.io/appveyor/ci/redss/simplemaybe-jsonnetserialization.svg)](https://ci.appveyor.com/project/redss/simplemaybe-jsonnetserialization)
[![NuGet](https://img.shields.io/nuget/v/SimpleMaybe.JsonNetSerialization.svg)](https://www.nuget.org/packages/SimpleMaybe.JsonNetSerialization/)

_This library is under development!_

**How about you didn't have to worry about random nulls in your JSON contracts?**

## Quickstart

Let's say you've created an API for selling concert tickets.  You're using [Json.NET](https://www.newtonsoft.com/json) for serialization/deserialization and one of your contracts looks like this:

```cs
public class BuyTicketRequest
{
    public string EventId { get; set; }
    public string FullName { get; set; }
    public string DiscountCode { get; set; }
}
```

Not all fields are required by your contract, though. Let's say that `FullName` and `DiscountCode` are not necessary to buy a ticket. This is hardly visible – in the end, every `string` can be null.

There are a few ways to deal with it, including naming conventions and fancy validators. Personally, I prefer to take advantage of type systems, and more precisely of [option/maybe types](https://en.wikipedia.org/wiki/Option_type).

In this example, let's make our contract more explicit by using [SimpleMaybe](https://github.com/redss/SimpleMaybe) – a library implementing [option/maybe type](https://en.wikipedia.org/wiki/Option_type):

```
PM> Install-Package SimpleMaybe -IncludePrerelease
```

Our refined contract looks like this:

``` cs
using SimpleMaybe;

public class BuyTicketRequest
{
    public string EventId { get; set; }
    public Maybe<string> FullName { get; set; }
    public Maybe<string> DiscountCode { get; set; }
}
```

It's definitely more clear, but it won't work by default with Json.NET.

Also, I would like for two things to happen during deserialization:

1. for properties of `Maybe<T>` type, nulls should map to `Maybe.None<T>()` and values to `Maybe.Some(value)`,
1. every other property should never deserialize into null.

We can achieve this by installing `SimpleMaybe.JsonNetSerialization`  - a library integrating Json.NET with [SimpleMaybe](https://github.com/redss/SimpleMaybe):

```
PM> Install-Package SimpleMaybe.JsonNetSerialization -IncludePrerelease
```

Json.NET configuration should look like this:

``` cs
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleMaybe.JsonNetSerialization;

// ...

var settings = new JsonSerializerSettings
{
    ContractResolver = new RequireMaybePropertiesContractResolver(
        new DefaultContractResolver()
    ),
    Converters = new JsonConverter[]
    {
        new MaybeJsonConverter()
    }
};
```

Let's test how these maybes map!

Given following JSON:

``` json
{
    "EventId": "modal-nodes-05-1977",
    "FullName": "Han Solo",
    "DiscountCode": null
}
```

Deserialization should have following effect:

``` cs
var request = JsonConvert.DeserializeObject<BuyTicketRequest>(json, settings);

request.EventId.Should().Be("modal-nodes-05-1977");
request.FullName.Should().Be(Maybe.Some("Han Solo"));
request.DiscountCode.Should().Be(Maybe.None<string>());
```

Great! But what if we try to _smuggle_ a null through some non-maybe property?

``` json
{
    "EventId": null,
    "FullName": "Han Solo",
    "DiscountCode": null
}
```

If we try to do that, we'll get the following exception:

```
Required property 'EventId' expects a value but got null. (...)
```

One more thing - the same principles will apply to serialization. It means, that the only way to get null in your JSON, will be by using `Maybe.None<T>()`.

Happy coding with fewer nulls!

**Make sure to read the library's [current limitations](#current-limitations)!**

## Documentation

SimpleMaybe was created to fight nulls and make handling optional values more explicit. And since JSON contracts are the major source of unexpected nulls, why not create a tool to handle them more explicitly?

By using SimpleMaybe along with Json.NET:

1. you make it very explicit which properties are required, and which are optional,
1. you make sure that required properties are actually set and not null.

Also, you don't have to check nulls everywhere in your code.

This library consists of two classes: `MaybeJsonConverter` and `RequireMaybePropertiesContractResolver`. They were designed to work together, but can be used separately if necessary.

### `MaybeJsonConverter`

`MaybeJsonConverter` enables Json.NET to serialize and deserialize `Maybe<T>` objects.

When deserializing:

1. null is deserialized into `Maybe.None<T>`,
1. non-null value is deserialized into `Maybe.Some<T>`.

When serializing:

1. `Maybe.None<T>` is serialized into null,
1. `Maybe.Some<T>` is serialized into contained value.

### `RequireMaybePropertiesContractResolver`

`RequireMaybePropertiesContractResolver` will make sure, that _only_ properties of `Maybe<T>` type can be null or absent; if any other property is null, serialization/deserialization will throw an exception.

Of course, using it will only make sense in combination with `MaybeJsonConverter`.

Use it by assigning `RequireMaybePropertiesContractResolver` instance to `JsonSerializerSettings.ContractResolver`:

``` cs
var serializerSettings = new JsonSerializerSettings
{
    ContractResolver = new RequireMaybePropertiesContractResolver(
        new DefaultContractResolver()
    )
};
```

Since `RequireMaybePropertiesContractResolver` is a decorator for `IContractResolver`, you can compose it with any other contract resolver:

``` cs
new RequireMaybePropertiesContractResolver(
    new DefaultContractResolver()
);

new RequireMaybePropertiesContractResolver(
    new CamelCasePropertyNamesContractResolver()
);

new RequireMaybePropertiesContractResolver(
    new YourFancyContractResolver()
);
```

#### Current limitations

At the moment, `RequireMaybePropertiesContractResolver` will only work with objects' properties. **It means, that other containers such as arrays or dictionaries will still be able to contain nulls!** Be sure to check them by hand if necessary.

The reason for this is that I have no idea how to implement it in Json.NET. If you know how, let me know!