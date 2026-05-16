using System;
using Project8.Application.Events;
using Project8.Presentation.Views;
using VContainer.Unity;

namespace Project8.Presentation.Presenters
{
    public sealed class HudPresenter :
        IStartable,
        IDisposable,
        IEventHandler<ScoreChangedEvent>,
        IEventHandler<TimeChangedEvent>,
        IEventHandler<GameEndedEvent>
    {
        private readonly IHudView _view;
        private readonly IEventBus _eventBus;

        public HudPresenter(
            IHudView view,
            IEventBus eventBus)
        {
            _view = view;
            _eventBus = eventBus;
        }

        public void Start()
        {
            _eventBus.Register<ScoreChangedEvent>(this);
            _eventBus.Register<TimeChangedEvent>(this);
            _eventBus.Register<GameEndedEvent>(this);
        }

        public void Handle(ScoreChangedEvent gameEvent)
        {
            _view.SetScore(gameEvent.Score);
        }

        public void Handle(TimeChangedEvent gameEvent)
        {
            _view.SetTime(gameEvent.RemainingSeconds);
        }

        public void Handle(GameEndedEvent gameEvent)
        {
            _view.ShowGameEnded(gameEvent.FinalScore);
        }

        public void Dispose()
        {
            _eventBus.Unregister<ScoreChangedEvent>(this);
            _eventBus.Unregister<TimeChangedEvent>(this);
            _eventBus.Unregister<GameEndedEvent>(this);
        }
    }
}
