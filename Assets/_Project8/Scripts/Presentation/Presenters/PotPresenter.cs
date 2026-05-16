using System;
using Project8.Application.Events;
using Project8.Presentation.Views;
using VContainer.Unity;

namespace Project8.Presentation.Presenters
{
    public sealed class PotPresenter :
        IStartable,
        IDisposable,
        IEventHandler<PotStateChangedEvent>
    {
        private readonly IPotView _view;
        private readonly IEventBus _eventBus;

        public PotPresenter(
            IPotView view,
            IEventBus eventBus)
        {
            _view = view;
            _eventBus = eventBus;
        }

        public void Start()
        {
            _eventBus.Register(this);
        }

        public void Handle(PotStateChangedEvent gameEvent)
        {
            _view.SetPot(gameEvent.Pot);
        }

        public void Dispose()
        {
            _eventBus.Unregister<PotStateChangedEvent>(this);
        }
    }
}
