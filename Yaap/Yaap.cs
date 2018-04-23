using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Yaap
{
    public class Yaap : IDisposable
    {
      static object _consoleLock = new object();
      static ConcurrentDictionary<Yaap, bool> _instances = new ConcurrentDictionary<Yaap, bool>();

      static void AddInstance(Yaap ntqdm) => _instances.TryAdd(ntqdm, true);
      static void RemoveInstance(Yaap ntqdm) => _instances.TryRemove(ntqdm, out var _);

      public Yaap()
      {
        AddInstance(this);
      }

      public void Dispose()
      {
        RemoveInstance(this);
      }
    }

  public static class EnumerableExtensions
  {
    public static IEnumerable<T> Yaap<T>(this IEnumerable<T> e)
    {
      Console.WriteLine(e.Count());
      return e;
    }
  }
}
