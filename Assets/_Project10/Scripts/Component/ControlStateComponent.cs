using System;

[Serializable]
public class ControlStateComponent
{
    private ActionType _currentAction = ActionType.Allow;

    public ActionType CurrentAction => _currentAction;

    public bool IsAllowed => _currentAction == ActionType.Allow;
    public bool IsBlocked => _currentAction == ActionType.Block;
    public bool IsModified => _currentAction == ActionType.Modify;

    public void Set(ActionType actionType)
    {
        _currentAction = actionType;
    }

    public void ResetToDefault()
    {
        _currentAction = ActionType.Allow;
    }
}