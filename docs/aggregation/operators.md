# ✨ Operators

## _Slice_ operator - _$slice_

The _$slice_ operator is used to return a subset of an **array**. The `Enumerable.Take` method can be used to create a _$slice_ operation on a array field.

| Method call | Description |
| :--- | :--- |
| **`Take(1)`** | Returns the **first** element |
| **`Take(N)`** | Returns the **first N** elements |
| **`Take(-1)`** | Returns the **last** element |
| **`Take(-N)`** | Returns the **last N** elements |

### Get first N elements

The sample returns `Traveler` documents but only include the **first** element of their _VisitedCountries_ array field. 

{% tabs %}
{% tab title="C\#" %}
{% code title="Slice.cs" %}
```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(Constants.TravelersCollection)
    .AsQueryable();

var sliceQuery = from t in travelersQueryableCollection
    select new {
        t.Name, visitedCountries = 
        t.VisitedCountries.Take(1) // slice here
    };

var sliceQueryResults = await sliceQuery.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate()
.project(
    {   
        name: 1, 
        visitedCountries : { 
            $slice: ["$visitedCountries", 1] 
        }
    })

-------------------

// sample result
{
	"_id" : ObjectId("5e9d705b45359358b426065f"),
	"name" : "Leopoldo Lueilwitz",
	"visitedCountries" : [ // only one item
		{
			"name" : "Malta",
			"timesVisited" : 9,
			"lastDateVisited" : ISODate("2017-12-19T21:22:35.607+02:00"),
			"coordinates" : {
				"latitude" : 79.2858,
				"longitude" : 13.7049
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

The same result can be achieved using a `ProjectionDefinition`.

```csharp
var sliceProjection = Builders<Traveler>.Projection
    .Expression(u =>
    new
    {
        name = u.Name,
        visitedCountries = u.VisitedCountries.Take(1)
    });

var sliceProjectionResults = await travelersCollection
    .Find(Builders<Traveler>.Filter.Empty)
    .Project(sliceProjection)
    .ToListAsync();
```

### Get last _N_ elements

The following sample returns the Traveler documents but only **the last 2** visited countries included.

{% tabs %}
{% tab title="C\#" %}
{% code title="Slice.cs" %}
```csharp
var sliceQueryTwoLastCountries = 
   from t in travelersQueryableCollection
   select new 
   { 
      t.Name, 
      visitedCountries = t.VisitedCountries.Take(-2) 
   };

var sliceQueryTwoLastCountriesResults = 
   await sliceQueryTwoLastCountries
   .ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers
  .aggregate()
  .project(
  { 
    name: 1, 
    visitedCountries : 
        { $slice: ["$visitedCountries", -2] } 
  })
  
-----------------------

// sample result

{
	"_id" : ObjectId("5e9d96f37db33d7ec8aed9ee"),
	"name" : "Emmitt Wuckert",
	"visitedCountries" : [ // last two
		{
			"name" : "Greece",
			"timesVisited" : 1,
			"lastDateVisited" : ISODate("2019-05-30T15:26:04.146+03:00"),
			"coordinates" : {
				"latitude" : 23.1572,
				"longitude" : 36.6096
			}
		},
		{
			"name" : "Malta",
			"timesVisited" : 5,
			"lastDateVisited" : ISODate("2017-12-08T05:52:58.182+02:00"),
			"coordinates" : {
				"latitude" : -75.3477,
				"longitude" : 97.188
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

### Pagination

_Slice_ can be combined with the _Skip_ method and provide full pagination functionality. The following sample skips the first 2 _VisitedCountries_ array elements and returns the next 3.

{% tabs %}
{% tab title="C\#" %}
```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(Constants.TravelersCollection)
    .AsQueryable();
    
var sliceWithSkipQuery = 
    from t in travelersQueryableCollection
    select new { 
        t.Name, 
        visitedCountries = t.VisitedCountries
                            .Skip(2).Take(3) };

var sliceWithSkipQueryResults = 
    await sliceQuery.ToListAsync();
```
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate([
   {
      "$project":{
         "Name":"$name",
         "visitedCountries":{
            "$slice":[
               "$visitedCountries", 2, 3
            ]
         },
         "_id":0
      }
   }
])

-----------------------------
// sample result

{
	"Name" : "Kristofer Gutkowski",
	"visitedCountries" : [
		{ // 3rd
			"name" : "Austria",
			"timesVisited" : 1,
			"lastDateVisited" : ISODate("2016-05-22T17:30:10.643+03:00"),
			"coordinates" : {
				"latitude" : -25.3767,
				"longitude" : -114.1358
			}
		},
		{ // 4th
			"name" : "Tokelau",
			"timesVisited" : 5,
			"lastDateVisited" : ISODate("2019-02-15T16:33:06.861+02:00"),
			"coordinates" : {
				"latitude" : -83.42,
				"longitude" : 65.0535
			}
		},
		{ // 5th
			"name" : "Libyan Arab Jamahiriya",
			"timesVisited" : 2,
			"lastDateVisited" : ISODate("2015-08-31T14:34:19.979+03:00"),
			"coordinates" : {
				"latitude" : -65.3698,
				"longitude" : -93.8438
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

## _Filter_ operator - _$filter_

The $_filter_ operator is used to match and return array elements that fulfill the specified condition. The `Enumerable.Where` method can be used to create the condition.

The sample returns `Traveler` documents with their _VisitedCountries_ array field containing only the countries that have been visited once.

{% tabs %}
{% tab title="C\#" %}
{% code title="ComparisonOperators.cs" %}
```csharp
var travelersQueryableCollection = tripsDatabase
    .GetCollection<Traveler>(Constants.TravelersCollection)
    .AsQueryable();

var filterQuery = 
    from t in travelersQueryableCollection
    select new
    {
         t.Name,
         visitedCountries = t.VisitedCountries
                 .Where(c => c.TimesVisited == 1)
    };

var filterQueryResults = await filterQuery.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.travelers.aggregate([
   {
      "$project":{
         "name":"$name",
         "visitedCountries":{
            "$filter":{
               "input":"$visitedCountries",
               "as":"c",
               "cond":{
                  "$eq":[
                     "$$c.timesVisited",
                     1
                  ]
               }
            }
         },
         "_id":0
      }
   }
])

------------------------------

// sample result

/* 1 */
{
	"name" : "Emmitt Wuckert",
	"visitedCountries" : [
		{
			"name" : "Djibouti",
			"timesVisited" : 1, // matched
			"lastDateVisited" : ISODate("2019-03-10T04:36:32.678+02:00"),
			"coordinates" : {
				"latitude" : -45.7154,
				"longitude" : 60.261
			}
		},
		{
			"name" : "Greece",
			"timesVisited" : 1, // matched
			"lastDateVisited" : ISODate("2019-05-30T15:26:04.146+03:00"),
			"coordinates" : {
				"latitude" : 23.1572,
				"longitude" : 36.6096
			}
		}
	]
}
```
{% endtab %}

{% tab title="Traveler" %}
```csharp
public class Traveler
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Activities { get; set; }
    public List<VisitedCountry> VisitedCountries { get; set; }
}
```
{% endtab %}
{% endtabs %}

## Multiply operator - _$multiply_

The **$multiply** operator is used to multiply numbers and return the result. The operator can be used with both raw and field values.

Assuming a collection contains `Order` documents in the following format...

```javascript
{
    "_id" : 0,
    "item" : "Handmade Steel Shoes",
    "price" : 172,
    "quantity" : 3,
    "shipmentDetails" : {
        "shipAddress" : "547 Kris Hill, Dooleyville, Niger",
        "city" : "Port Caryport",
        "country" : "Northern Mariana Islands",
        "contactName" : "Keenan McDermott",
        "contactPhone" : "1-956-915-2404"
    }
}
```

The sample creates a **projectio**n stage to return the total for each order, with $$total = price * quantity$$ 

{% tabs %}
{% tab title="C\#" %}
{% code title="Multiply.cs" %}
```csharp
var ordersCollection = tripsDatabase
    .GetCollection<Order>(Constants.OrdersCollection);

var multiplyQuery = 
    from o in ordersCollection.AsQueryable()
    select new // creates a projection stage
    {
      o.OrderId,
      total = o.Quantity * o.Price // $multiply field
    };

var multiplyQueryResults = 
    await multiplyQuery.ToListAsync();
```
{% endcode %}
{% endtab %}

{% tab title="Shell" %}
```javascript
db.orders.aggregate([
  {
    $project: {
      OrderId: '$_id',
      total: { $multiply: ['$quantity', '$price'] },
      _id: 0
    }
  }
]);

----------------------------

// sample results

{ "OrderId" : 0, "total" : 516 }
{ "OrderId" : 1, "total" : 450 }
{ "OrderId" : 2, "total" : 752 }
{ "OrderId" : 3, "total" : 1848 }
{ "OrderId" : 4, "total" : 870 }
{ "OrderId" : 5, "total" : 544 }
{ "OrderId" : 6, "total" : 752 }

```
{% endtab %}

{% tab title="Order" %}
```csharp
public class Order
{
    [BsonId]
    public int OrderId { get; set; }
    public string Item { get; set; }
    public  int Price { get; set; }
    public int Quantity { get; set; }

    [BsonIgnoreIfDefault]
    public int? LotNumber { get; set; }

    public ShipmentDetails ShipmentDetails { get; set; }
}
```
{% endtab %}
{% endtabs %}

