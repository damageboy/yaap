using System;
using System.Linq;
using System.Threading;
using Yaap;
using static Yaap.YaapConsole;
using static System.Linq.Enumerable;

namespace Demo
{
    static class Demo
    {
        static void ResetCursor()
        {
            Console.CursorTop = 0;
            Console.CursorLeft = 0;
        }

        static void Main(string[] args)
        {
            var demos = new Action[] {Demo1, Demo2, Demo3, Demo4, Demo5, Demo6, Demo7};
            var startDemo = args.Length > 0
                ? (int.TryParse(args[0], out var tmp) ? tmp : 1)
                : 1;
            var lastDemo = args.Length > 0 ? startDemo : demos.Length;

            for (var i = startDemo - 1; i < lastDemo; i++) {
                ResetCursor();
                ClearScreen();
                demos[i]();
            }
        }

        static void Demo1()
        {
            WriteLine("Here's a plain vanilla progress bar (_can_ be with nice smooth unicode even on windows*)");
            WriteLine("It's width is constrained to 100 characters in total");
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
        }

        static void Demo2()
        {
            WriteLine("Here's the same demo as before, but this time with colors");
            WriteLine();

            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings
                {Description = "regular", Width = 100, ColorScheme = YaapColorScheme.Bright}))
                Thread.Sleep(100);
        }

        static void Demo3()
        {
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
        }

        static void Demo4()
        {
            WriteLine("Here's a progress bar that adapts to the width of the terminal");
            WriteLine("It's pre-configured to slow down, and the rate/time estimation uses EMA to adapt more quickly");
            WriteLine();

            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings
                {Description = "smoothing", SmoothingFactor = 0.1, MetricAbbreviations = true}))
                Thread.Sleep(i / 2);
        }

        static void Demo5()
        {
            WriteLine("You can even have nested loops, each with its own progress bar");
            WriteLine("These bars also use metric abbreviation(s) for the progress/rate/total counts");
            WriteLine();

            foreach (var i in Range(0, 3).Yaap(settings: new YaapSettings { Description = "nested1", MetricAbbreviations = true})) {
                foreach (var j in Range(0, 10).Yaap(settings: new YaapSettings { Description = "nested2", MetricAbbreviations = true})) {
                    foreach (var k in Range(0, 100_000_000).Yaap(settings: new YaapSettings { Description = "nested3", MetricAbbreviations = true})) {
                        ;
                    }
                }
            }
        }

        static void Demo6()
        {
            WriteLine("You can also launch multiple threads and have them progress independently");
            WriteLine("While still updating the progress bars in a coherent way...");
            WriteLine();

            var mre = new ManualResetEvent(false);
            var allReady = new Semaphore(0, 10);

            var threads = Range(0, 10).Select(ti => new Thread(() => {
                var r = new Random((int) (DateTime.Now.Ticks % int.MaxValue));
                var y = Range(0, 200).Yaap(settings: new YaapSettings {Description = $"thread{ti}", VerticalPosition = ti});
                allReady.Release();
                mre.WaitOne();
                foreach (var i in y) {
                    Thread.Sleep(r.Next(90, 110) / (ti + 1));
                }
            })).ToList();

            foreach (var t in threads) {
                t.Start();
            }

            foreach (var t in threads) {
                allReady.WaitOne();
            }
            mre.Set();
            foreach (var t in threads) {
                t.Join();
            }
        }

        static void Demo7()
        {
            foreach (var i in Range(0, 200).Yaap(settings: new YaapSettings {
                Description = "regular", Width = 100, Positioning = YaapPositioning.FixToBottom
            })) {
                Thread.Sleep(100);
                WriteLine($"Scrolling is fun! ({i}/200)");
            }
        }
    }
}
