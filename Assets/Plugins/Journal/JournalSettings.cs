using System.ComponentModel.DataAnnotations;

namespace Journal {
    public sealed class JournalSettings {
        [Range(1, 10000)]
        public int MaxLines { get; set; } = 100;
    }
}
