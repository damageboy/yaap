# FAQ and Known Issues

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
    For more information on what works/doesn't, consult the specific [unit tests](https://github.com/damageboy/yaap/blob/dev/Yaap.Tests/CountHackTests.cs).
    Finally, when all else fails, Yaap will throw an exception, explaining that the `total` progress value has to be provided by the user, for Yaap to work.
* *Can I write to the console while the progress bars are being updated?*
  * Yes, but not directly though `Console.Write*()` but rather going through `YaapConsole.Write*()` wrapper functions, which keep everything nice and tidy
* *Does dynamic resizing work?*
  * Kind of: When enlarging the width everything should be OK, when reducing the width, the screen could get momentarily garbled, but everything should look ok, "eventually"...
