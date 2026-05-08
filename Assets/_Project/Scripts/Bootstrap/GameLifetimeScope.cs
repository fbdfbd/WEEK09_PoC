using App.Gameplay.Conditions;
using App.Gameplay.Effects;
using App.Gameplay.Environment;
using App.Gameplay.Ending;
using App.Gameplay.Loop;
using App.Gameplay.Phases;
using App.Gameplay.Runtime;
using App.Gameplay.Session;
using VContainer;
using VContainer.Unity;

namespace App.Bootstrap
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameRuntimeState>(Lifetime.Scoped);
            builder.Register<GameSession>(Lifetime.Scoped);

            builder.Register<EffectProcessor>(Lifetime.Scoped);
            builder.Register<EnvironmentControlProcessor>(Lifetime.Scoped);
            builder.Register<ConditionEvaluator>(Lifetime.Scoped);
            builder.Register<ContentSelector>(Lifetime.Scoped);
            builder.Register<DialoguePlayer>(Lifetime.Transient);
            builder.Register<IntroPhase>(Lifetime.Scoped);
            builder.Register<MorningPhase>(Lifetime.Scoped);
            builder.Register<NoonPhase>(Lifetime.Scoped);
            builder.Register<EveningPhase>(Lifetime.Scoped);
            builder.Register<EndingResolver>(Lifetime.Scoped);
            builder.Register<WeekLoop>(Lifetime.Scoped);
        }
    }
}
