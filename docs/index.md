<p align="center"><img src="images/yaap.svg" width="500px" align="center"/> </p>

# Yaap

Yaap is a straight up port of python venerable [tqdm](https://github.com/tqdm/tqdm) to .NET / CLR

Yaap stands for **Y**et **A**nother **A**NSI **P**rogressbar

Feel free to browse some of the articles like the [Getting Started](articles/start-here.md) page, [FAQ](articles/FAQ.md) or figure out how to get the best looking progress bar under [Windows](articles/Windows.md), alternatively consule the [API](api/index.md) docs

## What does it do

Much like in python, Yaap can make .NET loops, `IEnumerable`s  and more show a smart progress meter.

Here's what Yaap's own Demo looks like:

### [Simple Yaap](#tab/demo1)
<img src="images/demo1.cast" rows=20 cols=95 loop="loop" poster="npt:0:10" class="asciinema" >

### [Colored Yaap](#tab/demo2)
<img src="images/demo2.cast" rows=20 cols=95 loop="loop" poster="npt:0:10" class="asciinema" >

### [Paused/Stalled](#tab/demo3)
<img src="images/demo3.cast" rows=20 cols=95 loop="loop" poster="npt:0:10" class="asciinema" >

### [Nested](#tab/demo5)
<img src="images/demo5.cast" rows=20 cols=95 loop="loop" poster="npt:0:10" class="asciinema" >

### [Multi-threaded](#tab/demo6)
<img src="images/demo6.cast" rows=20 cols=95 loop="loop" poster="npt:0:02" class="asciinema" >

***

### [Simple Yaap](#tab/demo1)
```csharp
using static Yaap.YaapConsole;
using static Enumerable;
...
foreach (var i in Range(0, 200).Yaap(settings: 
    new YaapSettings {Description = "regular", Width = 100})) {
    Thread.Sleep(100);
    switch (i) {
        case 50: WriteLine("The (re)drawing of the progress bar, happens in the background"); break;
        case 100: Write("As long as you use YaapConsole.Write* methods...."); break;
        case 150: WriteLine(" ... you can continue writing to the terminal"); break;
    }
}
```

### [Colored Yaap](#tab/demo2)
```csharp
using static Enumerable;
...
foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings { 
    Description = "regular", 
    Width = 100, 
    ColorScheme = YaapColorScheme.Bright}))
    Thread.Sleep(100);
```



### [Paused/Stalled](#tab/demo3)
```csharp
using static Enumerable;

var yaap = Range(0, 2000).Yaap(settings: new YaapSettings {
    Description = "regular",
    Width = 100,
    ColorScheme = YaapColorScheme.Bright,
    SmoothingFactor = 0.5,
});

foreach (var i in yaap) {
    ...
    yaap.State = YaapState.Paused;
    ...
    yaap.State = YaapState.Running;
    ...
    yaap.State = YaapState.Stalled;
    ...
    yaap.State = YaapState.Running;
}
```



### [Nested](#tab/demo5)
```csharp
foreach (var i in Range(0, 3).Yaap(settings: new YaapSettings { 
    Description = "nested1", UseMetricAbbreviations = true }))
    foreach (var j in Range(0, 10).Yaap(settings: new YaapSettings { 
        Description = "nested2", UseMetricAbbreviations = true }))
        foreach (var k in Range(0, 100_000_000).Yaap(settings: new YaapSettings {
            Description = "nested3", UseMetricAbbreviations = true }))
            ; // Oh, just do nothing here

```



### [Multi-threaded](#tab/demo6)
```csharp
using static Enumerable;

var threads = Range(0, 10).Select(ti => new Thread(() => {
    var r = new Random((int) (DateTime.Now.Ticks % int.MaxValue));
    
    foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings {
      Description = $"thread{ti}", VerticalPosition = ti }) {
      Thread.Sleep(r.Next(90, 110) / (ti + 1));
})).ToList();
    
foreach (var t in threads) t.Start();
```
---

## What is a Yaap made of

![](images/progressbar.png)

## What Else

Yaap has the following features:

| Feature                                                      | Blurb                                                        |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Easy wrapping of `IEnumerable<T>` with a Yaap progress bar   | `foreach (var i in Enumerable.Range(0, 1000).Yaap()) { }`    |
| Manual (non `IEnumerable<T>`) progress updates               | `var y = new Yaap(100); y.progress = 99;`                    |
| Low latency (~30ns) on enumeration                           | Generally speaking, everything happens in background thread, and the enumeration is un-hindered<br /> Checkout the [benchmarks](https://github.com/damageboy/yaap/Yaap.Bench) |
| Zero allocation (post construction)                          | What can I say, I just really hate alloactions<br />Checkout the [benchmarks](https://github.com/damageboy/yaap/Yaap.Bench) |
| Full progress bar with smooth unicode and numeric progress, time and rate | Yaap will detect unicode support and will opt to use the default unicode block charchter based theme, but you can use additional unicode themes using the provided `BarStyle` enumeration if you are a special snowflake<br />When unicode is not supported by the terminal, we us the plain ole' `#` char |
| Elapsed time tracking / Total Time Prediction                | Yaap will time the elapsed time and predict the remaining time |
| No-Embarrassment guarantee<br /><br /><br /><sub>or your money back**</sub> | Yaap will never make you lose face with showing embarrassing things like 101% completion, or having the elapsed time go past the predicted time (Instead we'll keep changing the predicted time!) |
| Rate Prediction                                              | Uses advanced machine learning to predict total estimated time (team available for acquihire)<sup>1</sup> <br /><br /><br /> <sup>1</sup> OK, I lied, Just using Kalman filters effectively instead |
| Metric Abbreviation for counts (K/M/G...)                    | Yaap can automatically use metric units (`K`/`M`/`G`)        |
| Nested / Multiple concurrent progress bars                   | Yaap will automatically detect nested progress bars with no extra effort on your part |
| Colors                                                       | Yaap can do normal 16 color palette or force TrueColor in case someone redifined yellow in their infinite wisdom |
| Butter Smooth Progress bars                                  | Yaap can predict progress from rate, allowing smooth progress bars, even on slow/choppy enumeration |
| Butter Smooth **Nested** Progress bars                       | Yaap can estimate progress in higher level loops from the progress of their sub-loops(!) |
| Yaap will **NEVER** `.Count()` your `IEnumerable`            | Yaap never calls `.Count()`. It does other ungodly things to try and get your `IEnumerable` "length" in weird/undocumented ways, and when all fails, it will miserably fail, asking you to provide the `Total` value anagrammatically |
| Turn elements on/off                                         |                                                              |
| Works on Windows                                             | [But you have to work for it, at least bit](articles/Windows.md) |
| Dynamic Resizing                                             |                                                              |
| Constant Width                                               |                                                              |
