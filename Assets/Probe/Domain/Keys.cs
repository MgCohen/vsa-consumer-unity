using System;
using VSA.Analyzers.Markers;

[assembly: TypeArgument(typeof(int))]
[assembly: TypeArgument(typeof(decimal))]

namespace Probe.Domain {
    [TypeArgument]
    public struct Heat {
        public int Level;
    }

    public interface IGauge : ITypeArgument { }

    public struct Charge : IGauge {
        public long Amount;
        public string Label;
    }

    [TypeArgument]
    public struct Named {
        public Guid Id;
    }

    public sealed class Mana : ITypeArgument { }

    public struct Weight { }
}
