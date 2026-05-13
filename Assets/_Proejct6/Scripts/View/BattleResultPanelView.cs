using System;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPanelView : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    public event Action RestartClicked;
    public event Action QuitClicked;

    private void Awake()
    {
        EnsureRoot();

        if (_restartButton != null)
        {
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(OnQuitClicked);
        }
    }

    private void OnDestroy()
    {
        if (_restartButton != null)
        {
            _restartButton.onClick.RemoveListener(OnRestartClicked);
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }

    public void Show(BattleResult result)
    {
        EnsureRoot();

        if (_root != null)
        {
            _root.SetActive(result != BattleResult.None);
        }
    }

    public void Hide()
    {
        EnsureRoot();

        if (_root != null)
        {
            _root.SetActive(false);
        }
    }

    private void EnsureRoot()
    {
        if (_root == null)
        {
            _root = gameObject;
        }
    }

    private void OnRestartClicked()
    {
        if (RestartClicked != null)
        {
            RestartClicked();
        }
    }

    private void OnQuitClicked()
    {
        if (QuitClicked != null)
        {
            QuitClicked();
        }
    }
}
