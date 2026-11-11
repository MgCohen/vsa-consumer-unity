using VSA.Contracts.Commands;

namespace Counter.Contracts.Commands {
    public sealed record ResetCommand : ICommand<ResetResult>;
}
