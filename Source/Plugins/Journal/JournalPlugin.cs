using Journal.Contracts.Requests;
using Journal.Handlers.Requests;
using Microsoft.Extensions.DependencyInjection;
using VSA.Handlers;
using VSA.Handlers.Requests;
using VSA.Module;

namespace Journal;

public sealed class JournalPlugin : Plugin<JournalSettings>
{
    public override void Configure(ModuleBuilder module, JournalSettings settings)
    {
        module.Services.AddSingleton(new JournalLog(settings.MaxLines));
        module.Services.AddPipeline(new PipelineBuilder<Journaled>()
            .HandlersOf(typeof(RequestHandler<,>))
            .AfterStep<WriteJournal>());
    }
}
