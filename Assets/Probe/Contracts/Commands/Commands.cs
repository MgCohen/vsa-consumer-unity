using VSA.Contracts.Commands;

namespace Probe.Contracts.Commands {
    public sealed class StructCommand<T> : ICommand<string> {
        public StructCommand(int amount) => Amount = amount;
        public int Amount { get; }
    }

    public sealed class ClassCommand<T> : ICommand<string> {
        public ClassCommand(int amount) => Amount = amount;
        public int Amount { get; }
    }

    public sealed class UnmanagedCommand<T> : ICommand<string> {
        public UnmanagedCommand(int amount) => Amount = amount;
        public int Amount { get; }
    }

    public sealed class PreservedCommand<T> : ICommand<string> {
        public PreservedCommand(int amount) => Amount = amount;
        public int Amount { get; }
    }

    public sealed class PlainCommand : ICommand<string> { }

    public sealed class PreservedPlainCommand : ICommand<string> { }
}
