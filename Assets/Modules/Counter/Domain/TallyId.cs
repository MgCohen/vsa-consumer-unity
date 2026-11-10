using System;

namespace Counter.Domain {
    public sealed record TallyId(Guid Value) {
        public static TallyId Main { get; } = new TallyId(Guid.Empty);
    }
}
