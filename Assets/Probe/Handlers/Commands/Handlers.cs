using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Probe.Contracts.Commands;
using Probe.Domain;
using VSA.Analyzers.Markers;
using VSA.Contracts;
using VSA.Handlers;
using VSA.Handlers.Commands;

namespace Probe.Handlers.Commands {
    static class Work {
        public static string Use<T>(int amount) {
            var list = new List<T> { default!, default! };
            var counts = new Dictionary<T, int>();
            if (list[0] != null) {
                counts[list[0]] = amount;
            }
            var same = EqualityComparer<T>.Default.Equals(list[0], list[1]);
            var boxed = (object)list[1];
            var array = new T[amount < 1 ? 1 : amount];
            return $"{typeof(T).Name} {amount} {counts.Count} {same} {boxed != null} {array.Length} {Echo(list[0]) is T}";
        }

        static T Echo<T>(T value) => value;
    }

    public sealed class StructHandler<T> : CommandHandler<StructCommand<T>, string> where T : struct {
        T last;

        public override Task<Result<string>> HandleAsync(Envelope<StructCommand<T>> envelope) {
            last = default;
            return Task.FromResult<Result<string>>(Work.Use<T>(envelope.Message.Amount) + $" {last.GetHashCode() >= int.MinValue}");
        }
    }

    public sealed class StructValidator<T> : Validator<StructCommand<T>> where T : struct {
        static readonly Error Zero = new Error("probe.zero", "The amount is zero.");

        public override Error Validate(IEnvelope<StructCommand<T>> envelope) =>
            envelope.Message.Amount == 0 ? Zero : null;
    }

    public sealed class ClassHandler<T> : CommandHandler<ClassCommand<T>, string> where T : class {
        public override Task<Result<string>> HandleAsync(Envelope<ClassCommand<T>> envelope) =>
            Task.FromResult<Result<string>>(Work.Use<T>(envelope.Message.Amount));
    }

    public sealed class UnmanagedHandler<T> : CommandHandler<UnmanagedCommand<T>, string> where T : unmanaged {
        public override Task<Result<string>> HandleAsync(Envelope<UnmanagedCommand<T>> envelope) {
            Span<T> span = stackalloc T[2];
            return Task.FromResult<Result<string>>(Work.Use<T>(envelope.Message.Amount) + $" {span.Length}");
        }
    }

    [Preserve]
    public sealed class PreservedHandler<T> : CommandHandler<PreservedCommand<T>, string> where T : struct {
        public override Task<Result<string>> HandleAsync(Envelope<PreservedCommand<T>> envelope) =>
            Task.FromResult<Result<string>>(Work.Use<T>(envelope.Message.Amount));
    }

    public sealed class PlainHandler : CommandHandler<PlainCommand, string> {
        public override Task<Result<string>> HandleAsync(Envelope<PlainCommand> envelope) =>
            Task.FromResult<Result<string>>("plain");
    }

    [Preserve]
    public sealed class PreservedPlainHandler : CommandHandler<PreservedPlainCommand, string> {
        public override Task<Result<string>> HandleAsync(Envelope<PreservedPlainCommand> envelope) =>
            Task.FromResult<Result<string>>("preserved plain");
    }

    public interface IGaugeService : IAutoRegister {
        string Gauge { get; }
    }

    public sealed class GaugeService<T> : IGaugeService where T : struct, IGauge {
        public string Gauge => typeof(T).Name;
    }

    [AutoRegister(Lifetime.Singleton)]
    public sealed class KeyCache<T> where T : struct {
        public readonly Guid Id = Guid.NewGuid();
        public T Value;
    }

    public sealed class Unmarked<T> where T : struct { }

    public sealed class OnlyClass<T> where T : class { }

    static class AotHints {
        static void Use() {
            _ = new StructHandler<Named>();
            _ = new StructValidator<Named>();
            _ = Work.Use<Named>(0);
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    sealed class PreserveAttribute : Attribute { }
}
