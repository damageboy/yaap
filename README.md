[![Build status](https://ci.appveyor.com/api/projects/status/o92rqmfl93rajety?svg=true)](https://ci.appveyor.com/project/damageboy/yaap)

# yaap

This is a straight up port of python venerable [tqdm](https://github.com/tqdm/tqdm) to .NET / CLR

### Name

Simply: **Y**et **A**nother **A**NSI **P**rogressbar

### Origin

From the python project README:

> ```
> `tqdm` means "progress" in Arabic (taqadum, تقدّم)
> and an abbreviation for "I love you so much" in Spanish (te quiero demasiado).
>
> Instantly make your loops show a smart progress meter - just wrap any
> iterable with ``tqdm(iterable)``, and you're done!
> ```

Much like in python, yaap makes .NET loops, `IEnumerable`s  and more show a smart progress meter.

```c#
using Yaap;

foreach (var i in Yaap.Wrap(Enumerable.Range(10000))) {
    ...
}
```

Will display a progress bar like this:

```
76%|████████████████████████████         | 7568/10000 [00:33<00:10, 229.00it/s]
```
