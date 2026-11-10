using System.Collections.Generic;
using System.Linq;

namespace Journal.Handlers.Requests {
    public sealed class JournalLog {
        readonly int maxLines;
        readonly Queue<string> lines = new Queue<string>();

        public JournalLog(int maxLines) => this.maxLines = maxLines;

        public IReadOnlyList<string> Lines {
            get {
                lock (lines) {
                    return lines.ToList();
                }
            }
        }

        public void Write(string line) {
            lock (lines) {
                lines.Enqueue(line);
                while (lines.Count > maxLines) {
                    lines.Dequeue();
                }
            }
        }
    }
}
