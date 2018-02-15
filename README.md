_By Erik von Krusenstierna (erik.von.krusenstierna@mopedo.com)_

# What is RESTar.OData?

RESTar.OData is a free to use open-source protocol provider for [RESTar](https://github.com/Mopedo/Home/tree/master/RESTar) that makes it possible to interact with a RESTar API using the [OData 4.0 protocol](http://www.odata.org/). Requests to RESTar.OData are compiled into a common format before evaluation, which means that things work the same from the resource's perspective.

RESTar.OData is meant to be a _community project_, and Mopedo will provide the initial help needed for further development. If you have any questions or issues, or wish to contribute to the project, [post an issue](https://github.com/Mopedo/RESTar.OData/issues) or contact Erik at erik.von.krusenstierna@mopedo.com.

## Getting started

RESTar.OData is, like RESTar, distributed as a [package](https://www.nuget.org/packages/RESTar.OData) on the NuGet Gallery, and an easy way to install it in an active Visual Studio project is by entering the following into the NuGet Package Manager console:

```
Install-Package RESTar.OData
```

## Using RESTar.OData

RESTar.SQLite defines a **protocol provider** for RESTar, which should be included in the call to [`RESTarConfig.Init()`](https://github.com/Mopedo/Home/blob/master/RESTar/Developing%20a%20RESTar%20API/RESTarConfig.Init.md) in applications that wish to use it. Protocol providers are essentially add-ons for RESTar, enabling – for example – API protocols like OData to work as a native protocol for RESTar – and interact with RESTar resources just like the built-in protocol. For more on protocol providers, see the [RESTar Specification](https://github.com/Mopedo/Home/blob/master/RESTar/Developing%20a%20RESTar%20API/Protocol%20providers.md).

For information about the OData protocol, see the [OData documentation](http://www.odata.org/documentation/).

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

- `$filter` – equivalent to [URI conditions](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Conditions.md) in RESTar protocol
- `$orderby` – equivalent to the [`order_asc`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Meta-conditions.md#order_asc) and [`order_desc`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Meta-conditions.md#order_desc) meta-conditions
- `$select` – equivalent to the [`select`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Meta-conditions.md#select) meta-condition
- `$skip` – equivalent to the [`offset`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Meta-conditions.md#offset) meta-condition
- `$top` – equivalent to the [`limit`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Meta-conditions.md#limit) meta-condition

#### `$filter`

RESTar.OData supports the following operators in `$filter` query option conditions:

- `"eq"`– equivalent to `"="` (equals)
- `"ne"`– equivalent to `"!="` (not equals)
- `"lt"`– equivalent to `"<"` (less than)
- `"gt"`– equivalent to `">"` (greater than)
- `"le"`– equivalent to `"<="` (less than or equals)
- `"ge"`– equivalent to `">="` (greater than or equals)

The operators available in the RESTar protocol are coverered [here](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/URI/Conditions.md#operators).

#### `$orderby`

RESTar.OData supports only one argument per `$orderby` query option, and only one `$orderby` query option per URI.

### Content types

RESTar.OData has support for only `application/xml` when writing the Metadata document, and only `application/json` when writing all other resources. RESTar.OData only accepts `application/json` as input format.

### Methods

RESTar.OData supports all [methods](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/Methods.md) that are enabled for the resource, except [`REPORT`](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/Methods.md#report). So OData clients can select, insert, update and delete entities just like with the RESTar protocol.

### Authentication/authorization

RESTar.OData supports [authentication and access control](https://github.com/Mopedo/Home/blob/master/RESTar/Consuming%20a%20RESTar%20API/Headers.md#authorization), just like in the RESTar protocol.

### Metadata

RESTar.OData publishes an OData metadata document at `/$metadata`, with a few things to mention:

- Metadata is generated by RESTar's own reflection, which is available at `RESTar.Metadata.Get()`, which means no further member reflection is needed to generate the metadata document.
- All Starcounter database resources have their `ObjectNo` as key.
- Navigation properties are not currently used. Instead all properties have `<Property>` tags.
