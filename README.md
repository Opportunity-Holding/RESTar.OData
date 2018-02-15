_By Erik von Krusenstierna (erik.von.krusenstierna@mopedo.com)_

# What is RESTar.OData?

RESTar.OData is a free to use open-source protocol provider for [RESTar](https://github.com/Mopedo/Home/tree/master/RESTar) makes it possible to interact with a RESTar API using the [OData 4.0 protocol](http://www.odata.org/).

This documentation will cover the basics of RESTar.OData and how to set it up in a Visual Studio project.

## Getting started

RESTar.OData is, like RESTar, distributed as a [package](https://www.nuget.org/packages/RESTar.OData) on the NuGet Gallery, and an easy way to install it in an active Visual Studio project is by entering the following into the NuGet Package Manager console:

```
Install-Package RESTar.OData
```

## Using RESTar.OData

RESTar.SQLite defines a **protocol provider** for RESTar, which should be included in the call to `RESTarConfig.Init()` in applications that wish to use it. Protocol providers are essentially add-ons for RESTar, enabling – for example – API protocols like OData to work as a native protocol for RESTar – and interact with RESTar resources just like the built-in protocol. For more on protocol providers, see the [RESTar Specification](https://github.com/Mopedo/Home/blob/master/RESTar/Developing%20a%20RESTar%20API/Protocol%20providers.md).

For information about the OData protocol, see the [OData documentation](http://www.odata.org/documentation/), and take note the restrictions outlined below.

### Example requests

To specify the protocol for a RESTar request, we add a dash `-` and the protocol ID directly after the root URI of the service. In RESTar.OData's case, the ID is `odata`. Here are some OData requests URIs, and their equivalent RESTar protocol URIs.

```
OData:  GET http://localhost:8282/api-odata/superhero
RESTar: GET http://localhost:8282/api/superhero
```

```
OData:  GET http://localhost:8282/api-odata/superhero?$filter=hassecretidentity%20eq%20true&$top=5
RESTar: GET http://localhost:8282/api/superhero/hassecretidentity=true/limit=5
```

```
OData:  GET http://localhost:8282/api-odata/superhero?$orderby=name%20asc
RESTar: GET http://localhost:8282/api/superhero//order_asc=name
```

## Supported operations

### URIs and operations

RESTar.OData supports the following OData query options:

- `$filter` – equivalent to URI conditions in RESTar protocol
- `$orderby` – equivalent to the `order_desc` and `order_asc` meta-conditions
- `$select` – equivalent to the `select` meta-condition
- `$skip` – equivalent to the `offset` meta-condition
- `$top` – equivalent to the `limit` meta-condition

#### `$filter`

RESTar.OData supports the following operators in `$filter` query option conditions:

- `"eq"`– equivalent to `"="` (equals)
- `"ne"`– equivalent to `"!="` (not equals)
- `"lt"`– equivalent to `"<"` (less than)
- `"gt"`– equivalent to `">"` (greater than)
- `"le"`– equivalent to `"<="` (less than or equals)
- `"ge"`– equivalent to `">="` (greater than or equals)

#### `$orderby`

RESTar.OData supports only one argument per `$orderby` query option, and only one `$orderby` query option per URI.

### Content types

RESTar.OData has support for only `application/xml` when writing the Metadata document, and only `application/json` when writing all other resources. RESTar.OData only accepts `application/json` as input format.

### Methods

RESTar.OData supports all [methods]() that are enabled for the resource, except `REPORT`.

### Authentication/authorization

RESTar.OData supports [basic authentication]() and [`apikey` authentication](), just like in the RESTar protocol.

### Metadata

RESTar.OData publishes an OData metadata document at `/$metadata`, with the following notes:

- Navigation properties are not currently used. Instead all properties have `<Property>` tags.
