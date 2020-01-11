# What is ExtensionsPack ?
It is essentially a package of useful and multi-puprose extension methods that I was getting tired of duplicating.
This is an open-source library so feel free everyone to use it for whatever use and feel free to recommend new extension methods
that you find useful. However make sure that those extensions don't have any 3rd party references and libraries and only reference native .NET libraries.

# A few example of usages:

**Check whether class property is decorated with an attribute of type T:**
```
if(emp.FirstName.GetAttributeOrDefault<RequiredAttribute>() != null)
{
   // do something  
}
```

**Convert DateTime to unix timestamp and back:**<br>

```
var date = DateTime.Now;
long unixUtcTimeStamp = date.ToUnixUtcTimeStamp();
DateTime newDate = unixUtcTimeStamp.ToDateTime();
```

**A few examples for IEnumerable:**<br>

Enumerable extensions:
```
if (myAwesomeCollection.IsNullOrEmpty())
{
   return ErrorResult;
}
var collectionAsList = myAwesomeCollection.CastToList();

if (collectionAsList.Count > index)
{
   collectionAsList[index] = ...;
}

```
