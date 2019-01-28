using System;
using System.Collections.Generic;

namespace Yaap.Backends
{
    internal interface IYaapBackend : IDisposable
    {
        void UpdateAllYaaps(ICollection<Yaap> instances);
        void UpdateSingleYaap(Yaap yaap);
        void ClearSingleYaap(Yaap yaap);
    }
}
