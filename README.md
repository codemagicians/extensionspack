# What is ExtensionsPack ?
It is essentially a package of useful and multi-puprose extension methods that you can use here and there while developing enterprise applications (especially web apis).
Please feel free to use it for whatever use and recommend us your own extension methods that you find useful. However, please make sure that those extensions do not have any references to 3rd party libraries and only reference native .NET libraries.

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
