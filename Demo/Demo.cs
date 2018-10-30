﻿using System;
using System.Linq;
using System.Threading;
using Yaap;
using static Yaap.YaapConsole;
using static System.Linq.Enumerable;

namespace Demo
{
    static class Demo
    {
        static void Main(string[] args)
        {
            var startDemo = args.Length > 0
                ? (int.TryParse(args[0], out var tmp) ? tmp : 1)
                : 1;
            var lastDemo = args.Length > 0 ? startDemo : Int32.MaxValue;

            switch (startDemo)
            {
                case 1: goto demo1;
                case 2: goto demo2;
                case 3: goto demo3;
                case 4: goto demo4;
                case 5: goto demo5;
                case 6: goto demo6;
            }


            demo1:
            WriteLine("Here's a plain vanilla progress bar (_can_ be with nice smooth unicode even on windows*)");
            WriteLine("It's width is constrainted to 100 charchters in total");
            WriteLine("* for more on Windows, go to http://xxxxx");
            WriteLine();

            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings {Description = "regular", Width = 100})) {
                Thread.Sleep(100);
                switch (i) {
                    case 50:
                        WriteLine("The (re)drawing of the progress bar, happens in the background");
                        break;
                    case 100:
                        Write("As long as you use YaapConsole.Write* methods....");
                        break;
                    case 150:
                        WriteLine(" ... you can continue writing to the terminal");
                        break;
                }
            }

            if (++startDemo > lastDemo) return;

            demo2:
            WriteLine("Here's the same demo as before, but this time with colors");
            WriteLine();

            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings { Description = "regular", Width = 100, ColorScheme = YaapColorScheme.Bright}))
                Thread.Sleep(100);

            if (++startDemo > lastDemo) return;

            demo3:
            WriteLine("When in color mode, Yaap can also express pauses and detect stalls");
            WriteLine();

            var yaap = Range(0, 2000).Yaap(settings: new YaapSettings {
                Description = "regular",
                Width = 100,
                ColorScheme = YaapColorScheme.Bright,
                SmoothingFactor = 0.5,
            });

            foreach (var i in yaap) {
                if (i == 900) {
                    yaap.State = YaapState.Paused;
                    Thread.Sleep(5000);
                    yaap.State = YaapState.Running;
                    continue;
                }

                if (i == 1900) {
                    yaap.State = YaapState.Stalled;
                    Thread.Sleep(5000);
                    yaap.State = YaapState.Running;
                    continue;
                }

                Thread.Sleep(10);
            }

            if (++startDemo > lastDemo) return;


            demo4:
            WriteLine("Here's a progress bar that adapts to the width of the terminal");
            WriteLine("It's pre-configured to slow down, and the rate/time estimation uses EMA to adapt more quickly");
            WriteLine();

            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings { Description = "smoothing", SmoothingFactor = 0.1, UseMetricAbbreviations = true }))
                Thread.Sleep(i / 2);

            if (++startDemo > lastDemo) return;

            demo5:
            WriteLine("You can even have nested loops, each with its own progress bar");
            WriteLine("These bars also use metric abbreviation(s) for the progress/rate/total counts");
            WriteLine();

            foreach (var i in Range(0, 3).Yaap(settings: new YaapSettings { Description = "nested1", UseMetricAbbreviations = true }))
                foreach (var j in Range(0, 10).Yaap(settings: new YaapSettings { Description = "nested2", UseMetricAbbreviations = true }))
                    foreach (var k in Range(0, 100_000_000).Yaap(settings: new YaapSettings { Description = "nested3", UseMetricAbbreviations = true }))
                    ;
            if (++startDemo > lastDemo) return;

            demo6:
            WriteLine("You can also launch multiple threads and have them progress independently");
            WriteLine("While still updating the progress bars in a coherent way...");
            WriteLine();

            var mre = new ManualResetEvent(false);
            var allReady = new Semaphore(0, 10);

            var threads = Range(0, 10).Select(ti => new Thread(() =>
            {
                var r = new Random((int) (DateTime.Now.Ticks % int.MaxValue));
                var y = Range(0, 200).Yaap(settings: new YaapSettings { Description = $"thread{ti}", VerticalPosition = ti });
                allReady.Release();
                mre.WaitOne();
                foreach (var i in y)
                    Thread.Sleep(r.Next(90, 110) / (ti + 1));
            })).ToList();

            foreach (var t in threads) t.Start();
            foreach (var t in threads) allReady.WaitOne();
            mre.Set();
            foreach (var t in threads) t.Join();

            if (++startDemo > lastDemo) return;
        }
    }
}