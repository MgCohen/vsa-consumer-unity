using System.ComponentModel.DataAnnotations;

namespace Journal;

public sealed class JournalSettings
{
    [Range(1, 10_000)]
    public int MaxLines { get; set; } = 100;
}
