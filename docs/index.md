<img src="yaap.svg" width="400px" /> 

# Yaap

Yaap is a straight up port of python venerable [tqdm](https://github.com/tqdm/tqdm) to .NET / CLR

Yaap stands for **Y**et **A**nother **A**NSI **P**rogressbar

Feel free to browse some of the articles like the [Getting Started](articles/start-here.md) page, [FAQ](articles/FAQ.md) or figure out how to get the best looking progress bar under [Windows](articles/windows.md), alternatively consule the [API](api/) docs

## What does it do

Much like in python, Yaap can make .NET loops, `IEnumerable`s  and more show a smart progress meter.

Here's what Yaap's own Demo looks like:



















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
