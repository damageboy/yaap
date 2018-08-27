<p align="center"><img src="images/yaap.svg" width="500px" align="center"/> </p>

# Yaap

Yaap is a straight up port of python venerable [tqdm](https://github.com/tqdm/tqdm) to .NET / CLR

Yaap stands for **Y**et **A**nother **A**NSI **P**rogressbar

Feel free to browse some of the articles like the [Getting Started](articles/start-here.md) page, [FAQ](articles/FAQ.md) or figure out how to get the best looking progress bar under [Windows](articles/Windows.md), alternatively consule the [API](api/index.md) docs

## What does it do

Much like in python, Yaap can make .NET loops, `IEnumerable`s  and more show a smart progress meter.

Here's what Yaap's own Demo looks like:

### [Simple Yaap](#tab/demo1)
![demo1](images/demo1.svg)

### [Color Yaap](#tab/demo2)
![demo1](images/demo2.svg)

---

## What Else

Yaap has the following features:

| Programmatic                                                 | Visual                                                       |
| ------------------------------------------------------------ | ------------------------------------------------------------ |
| Easy wrapping of `IEnumerable<T>` with a Yaap progress bar   | `foreach (var i in Enumerable.Range(0, 1000).Yaap()) { }`    |
| Manual (non `IEnumetable<T>`) progress updates               | `var y = new Yaap(100); y.progress = 99;`                    |
| Low latency (~30ns) on enumeration                           | Checkout the [benchmarks](https://github.com/damageboy/yaap/Yaap.Bench) |
| Zero allocation (post construction)                          | Checkout the [benchmarks](https://github.com/damageboy/yaap/Yaap.Bench) |
| Full progress bar with smooth unicode and numeric progress, time and rate | ![progressbar](images/progressbar.png)                       |
| Elapsed time tracking                                        | ![progressbar](images/progressbar-elapsed.png)               |
| Total Time Prediction                                        | ![progressbar](images/progressbar-total.png)                 |
| Rate Prediction                                              | ![progressbar](images/progressbar-rate.png)                  |
| Metric Abbreviation for counts (K/M/G...)                    | ![progressbar](images/progressbar-metric.png)                |
| Nested / Multiple concurrent progress bars                   | ![progressbar](images/progressbar-nested.png)                |
| Colors                                                       |                                                              |
| Butter Smooth Progress bars                                  | Yaap can predict progress from rate, allowing smooth progress bars, even on slow/choppy enumeration |
| Turn elements on/off                                         |                                                              |
| Works on Windows                                             | [But you have to work for it, at least bit](articles/Windows.md) |
| Dynamic Resizing                                             |                                                              |
| Constant Width                                               |                                                              |
| Stall Detection                                              |                                                              |
