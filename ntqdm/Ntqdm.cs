using System;
using System.Collections.Concurrent;

namespace ntqdm
{
    public class Ntqdm : IDisposable
    {
      static object _consoleLock = new object();
      static ConcurrentDictionary<Ntqdm, bool> _instances = new ConcurrentDictionary<Ntqdm, bool>();

      static void AddInstance(Ntqdm ntqdm) => _instances.TryAdd(ntqdm, true);
      static void RemoveInstance(Ntqdm ntqdm) => _instances.TryRemove(ntqdm, out var _);

      public Ntqdm()
      {
        AddInstance(this);
      }


      public void Dispose()
      {
        RemoveInstance(this);
      }
    }
}
