using System;
using R3;
using VContainer.Unity;

public sealed class RequestListPresenter : IStartable, IDisposable
{
    private readonly IndexSelectorView _indexSelectorView;
    private readonly RequestStore _requestStore;
    private readonly GameFlowState _flowState;

    private readonly CompositeDisposable _disposables = new();

    public RequestListPresenter(
        IndexSelectorView indexSelectorView,
        RequestStore requestStore,
        GameFlowState flowState)
    {
        _indexSelectorView = indexSelectorView;
        _requestStore = requestStore;
        _flowState = flowState;
    }

    public void Start()
    {
        _indexSelectorView.OnIndexClicked += HandleIndexClicked;

        _flowState.SelectedRequestIndex
            .Subscribe(OnSelectedIndexChanged)
            .AddTo(_disposables);

        _flowState.CurrentPhase
            .Subscribe(_ => Refresh())
            .AddTo(_disposables);
    }

    private void HandleIndexClicked(int index)
    {
        var count = GetCurrentCount();
        if (index < 0 || index >= count)
            return;

        _flowState.SelectedRequestIndex.Value = index;
    }

    private void OnSelectedIndexChanged(int index)
    {
        var count = GetCurrentCount();
        if (index < 0 || index >= count)
            return;

        _indexSelectorView.SetCount(count);
        _indexSelectorView.SetSelectedIndex(index);
        _flowState.SelectedRequestId.Value = GetCurrentRequestId(index);
    }

    private void Refresh()
    {
        var count = GetCurrentCount();
        _indexSelectorView.SetCount(count);

        if (count == 0)
            return;

        var index = _flowState.SelectedRequestIndex.Value;
        if (index < 0 || index >= count)
            index = 0;

        OnSelectedIndexChanged(index);
    }

    private int GetCurrentCount()
    {
        return _flowState.CurrentPhase.Value == GamePhase.AgencyAssignment
            ? _requestStore.AssignmentTargetCount
            : _requestStore.ActiveCount;
    }

    private string GetCurrentRequestId(int index)
    {
        return _flowState.CurrentPhase.Value == GamePhase.AgencyAssignment
            ? _requestStore.GetAssignmentTarget(index).Id
            : _requestStore.GetActive(index).Id;
    }

    public void Dispose()
    {
        _indexSelectorView.OnIndexClicked -= HandleIndexClicked;
        _disposables.Dispose();
    }
}
