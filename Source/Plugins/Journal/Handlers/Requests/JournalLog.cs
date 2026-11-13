namespace Journal.Handlers.Requests;

public sealed class JournalLog(int maxLines)
{
    private readonly Queue<string> lines = new();

    public IReadOnlyList<string> Lines
    {
        get
        {
            lock (lines)
            {
                return [.. lines];
            }
        }
    }

    public void Write(string line)
    {
        lock (lines)
        {
            lines.Enqueue(line);
            while (lines.Count > maxLines)
            {
                lines.Dequeue();
            }
        }
    }
}
