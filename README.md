# dynamodb-geo-dotnetcore
=========================

[![.NET](https://github.com/ReddragonLR/dynamodb-geo-dotnetcore/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/ReddragonLR/dynamodb-geo-dotnetcore/actions/workflows/dotnet.yml)

DynamoDB.GeoSpatial.Contract [![NuGet Version](http://img.shields.io/nuget/v/DynamoDB.GeoSpatial.Contract.svg?style=flat)](https://www.nuget.org/packages/DynamoDB.GeoSpatial.Contract/) 

DynamoDB.GeoSpatial [![NuGet Version](http://img.shields.io/nuget/v/DynamoDB.GeoSpatial.svg?style=flat)](https://www.nuget.org/packages/DynamoDB.GeoSpatial/) 

C# Adaptation of the AWS DynamoDB GeoSpatial library inspired by https://github.com/novotnyllc/dynamodb-geo-csharp

##Features
* **Box Queries:** Return all of the items that fall within a pair of geo points that define a rectangle as projected onto a sphere.
* **Radius Queries:** Return all of the items that are within a given radius of a geo point.
* **Basic CRUD Operations:** Create, retrieve, update, and delete geospatial data items.
* **Easy Integration:** Adds functionality to the AWS SDK for .NET in your server application.
* **Customizable:** Access to raw request and result objects from the AWS SDK for .NET.
* **DotnetCore DI ready:** GeoClientOptions and GeoClientExtension have been added to make it easier to incorporate via DI.

##Installation
Install via Nuget
```Install-Package DynamoDB.GeoSpatial.Contract```

Implementation of Contract
```Install-Package DynamoDB.GeoSpatial```

##Usage
```
TBC...
```

##Limitations

###No composite key support
Currently, the library does not support composite keys. You may want to add tags such as restaurant, bar, and coffee shop, and search locations of a specific category; however, it is currently not possible. You need to create a table for each tag and store the items separately.

###Queries retrieve all paginated data
Although low level [DynamoDB Query][dynamodb-query] requests return paginated results, this library automatically pages through the entire result set. When querying a large area with many points, a lot of Read Capacity Units may be consumed.

###More Read Capacity Units
The library retrieves candidate Geo points from the cells that intersect the requested bounds. The library then post-processes the candidate data, filtering out the specific points that are outside the requested bounds. Therefore, the consumed Read Capacity Units will be higher than the final results dataset.

###High memory consumption
Because all paginated `Query` results are loaded into memory and processed, it may consume substantial amounts of memory for large datasets.

###The server is essential
Because Geo Library calls multiple DynamoDB `Query` requests and processes the results in memory, it is not suitable for mobile device use. You should maintain a .NET server/Website, and use the library on the server.

###Dataset density limitation
The Geohash used in this library is roughly centimeter precision. Therefore, the library is not suitable if your dataset has much higher density.

##Reference

###Amazon DynamoDB
* [Amazon DynamoDB][dynamodb]
* [Amazon DynamoDB Forum][dynamodb-forum]


[dynamodb]: http://aws.amazon.com/dynamodb
[dynamodb-query]: http://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_Query.html
[dynamodb-forum]: https://forums.aws.amazon.com/forum.jspa?forumID=131
