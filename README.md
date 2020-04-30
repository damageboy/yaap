|                                           |                                                              |                                                              |      |
| :---------------------------------------: | :----------------------------------------------------------: | ------------------------------------------------------------ | ---- |
| <img src="docs/images/yaap.svg" width="200px" /> | [![Build status](https://ci.appveyor.com/api/projects/status/o92rqmfl93rajety?svg=true)](https://ci.appveyor.com/project/damageboy/yaap) | [![NuGet](https://img.shields.io/nuget/v/Yaap.svg)](https://www.nuget.org/packages/Yaap/) |      |

# Yaap

This is a straight up port of python venerable [tqdm](https://github.com/tqdm/tqdm) to .NET / CLR

Yaap stands for **Y**et **A**nother **A**NSI **P**rogressbar

## Origin

From the python project README:

> ```
> `tqdm` means "progress" in Arabic (taqadum, تقدّم)
> and an abbreviation for "I love you so much" in Spanish (te quiero demasiado).
> 
> Instantly make your loops show a smart progress meter - just wrap any
> iterable with ``tqdm(iterable)``, and you're done!
> ```

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

Unfortunately, getting nice looking progress to show up on Windows is not so straight forward, unless you follow [these instructions](/docs/articles/Windows.md)...

Out of the box, On Windows, the progress bar will be less visually appeaking and use ASCII characters:

```
76%|############################         | 7568/10000 [00:07s<00:10s, 229.00it/s]
```

## What Else

Yaap has the following features:

* Easy wrapping of `IEnumerable<T>` with a Yaap progress bar
* Manual (non `IEnumetable<T>`) progress updates
* Low latency (~30ns) overhead imposed on the thread bumping the progress value
* Zero allocation (post construction) / Very little allocation during construction
* Elapsed time tracking
* Total Time Prediction
* Rate Prediction
* Metric Abbreviation for counts (K/M/G...)
* Nested / Multiple concurrent progress bars
* Butter Smooth Progress bars, by predicting the  progress from the rate
* Configurable Appearance:
  * Fancy Unicode / ASCII bars
  * Colors
  * Prefix text
  * Turn selected elements on/off
* Works on Windows(!) (Some features require Windows for advanced terminal emulation)
* Dynamic Resizing (tracking the terminal width)
* Constant Width Progress Bars

## Docs

Full documentation is provided here

## Examples

See the [Demo](Demo) project for a fancy demo that covers most of what Yaap can do and how it can be optimized

You can either run the demo project with `dotnet run` to run all the demos sequentially or invoke specific demos with `dotnet run <n>` where `<n>` is the number of the demo to run...

## FAQ and Known Issues

* *What sort of terminal support is required?*
  * Single progress bar at a time: only carriage-return (`'\r'`) is needed
  * Nested /Multi progress bars: require support for moving the cursor up/down in addition to carriage-return
* *What specific terminals actually work well with Yaap?*
  * On Linux/Mac all terminal have full vt100 support, and should therefore work flawlessly
  * On windows, things should work well on any version of windows, except for Unicode support, which has its own page on how to make that ok-ish as well. Generally speaking, CMD.exe and ConEmu have been known to work well
* *Will Yapp enumerate through the `IEnumerable<T>` to get the total count?*
  * <u>Absolutely not!</u> 
    Yaap goes through every possible known way of getting the `Count` "value" of the `IEnumerable<T>` **WITHOUT** actually enumerating it. This means that when an `ICollection`/`Array`/`IList` object is passed to Yaap, it will actually read the `Count` / `Length` property instead. 
    Finally, Yaap uses and undocumented internal .NET interface called `IIListProvider` which can, sometimes calculate the count value of the enumerable without consuming it.
    For more information on what works/doesn't, consult the specific [unit tests](Yaap.Tests/CountHackTests.cs).
    Finally, when all else fails, Yaap will throw an exception, explaining that the `total` progress value has to be provided by the user, for Yaap to work.
* *Can I write to the console while the progress bars are being updated?*
  * Yes, but not directly though `Console.Write*()` but rather going through `YaapConsole.Write*()` wrapper functions, which keep everything nice and tidy
* *Does dynamic resizing work?*
  * Kind of: When enlarging the width everything should be OK, when reducing the width, the screen could get momentarily garbled, but everything should look ok, "eventually"...
