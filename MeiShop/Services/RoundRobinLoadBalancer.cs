namespace MeiShop.Services
{
    /// <summary>
    /// Thread-safe Round-Robin Load Balancer fuer Service-Instanzen.
    /// Verwaltet eine Liste von Service-URLs und verteilt Anfragen gleichmaessig.
    /// Muss als Singleton registriert werden, damit der Zustand (Index) ueber Requests erhalten bleibt.
    /// </summary>
    public class RoundRobinLoadBalancer
    {
        private readonly List<string> _instances;
        private int _currentIndex = -1;

        public RoundRobinLoadBalancer(List<string> instances)
        {
            if (instances == null || instances.Count == 0)
            {
                throw new ArgumentException("Mindestens eine Service-Instanz muss konfiguriert sein.", nameof(instances));
            }

            _instances = instances;
        }

        /// <summary>
        /// Gibt die naechste Service-Instanz im Round-Robin-Verfahren zurueck.
        /// Thread-safe durch Interlocked.Increment.
        /// </summary>
        public string GetNextInstance()
        {
            var index = Interlocked.Increment(ref _currentIndex);
            return _instances[Math.Abs(index) % _instances.Count];
        }

        /// <summary>
        /// Gibt eine andere Instanz zurueck (fuer Failover nach gescheiterten Retries).
        /// </summary>
        public string GetNextInstanceAfter(string currentInstance)
        {
            var currentIdx = _instances.IndexOf(currentInstance);
            if (currentIdx == -1)
            {
                return GetNextInstance();
            }

            var nextIdx = (currentIdx + 1) % _instances.Count;
            return _instances[nextIdx];
        }

        /// <summary>
        /// Anzahl der konfigurierten Instanzen.
        /// </summary>
        public int InstanceCount => _instances.Count;

        /// <summary>
        /// Alle konfigurierten Instanz-URLs (read-only).
        /// </summary>
        public IReadOnlyList<string> Instances => _instances.AsReadOnly();
    }
}
