using App.Foundation.Data;
using VContainer;
using VContainer.Unity;

namespace App.Bootstrap
{
    public sealed class ProjectLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);

            builder.Register<IDataRegistry, ScriptableObjectDataRegistry>(Lifetime.Singleton);
        }
    }
}
