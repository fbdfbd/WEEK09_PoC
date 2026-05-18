using System;
using R3;
using VContainer.Unity;

public sealed class PlayerTrustPresenter : IStartable, IDisposable
{
    private readonly AgencyRelationView _view;
    private readonly PlayerTrustStore _playerTrustStore;

    private readonly CompositeDisposable _disposables = new();

    public PlayerTrustPresenter(
        AgencyRelationView view,
        PlayerTrustStore playerTrustStore)
    {
        _view = view;
        _playerTrustStore = playerTrustStore;
    }

    public void Start()
    {
        _playerTrustStore.Trust
            .Subscribe(_view.SetPlayerTrust)
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
