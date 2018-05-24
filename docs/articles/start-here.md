## What does it do

Much like in python, Yaap can make .NET loops, `IEnumerable`s  and more show a smart progress meter.



The most dead simple way of starting with Yaap is to add it via the nuget package and 

```c#
using Yaap;

foreach (var i in Enumerable.Range(0, 1000).Yaap()) {
    Thread.Sleep(10);
}
```

Will display a continuously updating progress bar like this, on Mac/Linux:

```
76%|████████████████████████████         | 7568/10000 [00:07s<00:10s, 229.00it/s]
```

Unfortunately, getting nice looking progress to show up on Windows is not so straight forward, unless you follow [these instructions](Windows.md)...

Out of the box, On Windows, the progress bar will be less visually appeaking and use ASCII characters:

```
76%|############################         | 7568/10000 [00:07s<00:10s, 229.00it/s]
```

