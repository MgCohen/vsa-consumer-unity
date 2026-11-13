using VSA.Contracts.Commands;

namespace Counter.Slices.Reset;

public sealed record ResetCommand : ICommand<ResetResult>;
