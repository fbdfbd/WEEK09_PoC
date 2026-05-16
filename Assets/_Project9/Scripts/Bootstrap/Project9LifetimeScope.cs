using VContainer;
using VContainer.Unity;
using Project9.Runtime;
using Project9.Systems;
using Project9.Presentation;

namespace Project9.Bootstrap
{
    public sealed class Project9LifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ReportSessionFactory>(Lifetime.Scoped);
            builder.Register<SubmissionScoringSystem>(Lifetime.Scoped);
            builder.Register<ReputationSystem>(Lifetime.Scoped);
            builder.Register<Project9Presenter>(Lifetime.Scoped);
        }
    }
}
