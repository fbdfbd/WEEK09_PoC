using App.Gameplay.Effects;
using App.Gameplay.Environment;
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
        }
    }
}
