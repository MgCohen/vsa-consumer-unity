using System.ComponentModel.DataAnnotations;

namespace Counter {
    public sealed class CounterSettings {
        [Range(1, 1000)]
        public int Max { get; set; } = 10;
    }
}
