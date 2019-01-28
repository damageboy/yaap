using System.Collections.Generic;

namespace Yaap.Backends
{
    internal class NullBackend : IYaapBackend
    {
        public void Dispose()
        {
        }

        public void UpdateAllYaaps(ICollection<Yaap> instances)
        {
        }

        public void UpdateSingleYaap(Yaap yaap) { }

        public void ClearSingleYaap(Yaap yaap)
        {
        }
    }
}
