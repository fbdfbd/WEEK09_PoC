using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleActorStatusView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private TMP_Text _blockText;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private string _displayName = "Actor";
    [SerializeField] private float _hpTweenDuration = 0.25f;

    public void Show(BattleActorState actor)
    {
        if (actor == null)
        {
            return;
        }

        RefreshName();
        RefreshHpText(actor);
        RefreshBlockText(actor);
        RefreshSlider(actor);
    }

    private void RefreshName()
    {
        if (_nameText != null)
        {
            _nameText.text = _displayName;
        }
    }

    private void RefreshHpText(BattleActorState actor)
    {
        if (_hpText != null)
        {
            _hpText.text = actor.Hp + " / " + actor.MaxHp;
        }
    }

    private void RefreshBlockText(BattleActorState actor)
    {
        if (_blockText == null)
        {
            return;
        }

        if (actor.Block <= 0)
        {
            _blockText.text = string.Empty;
            return;
        }

        _blockText.text = "방어 " + actor.Block;
    }

    private void RefreshSlider(BattleActorState actor)
    {
        if (_hpSlider == null)
        {
            return;
        }

        _hpSlider.minValue = 0;
        _hpSlider.maxValue = actor.MaxHp;
        _hpSlider.DOKill();
        _hpSlider.DOValue(actor.Hp, _hpTweenDuration).SetEase(Ease.OutCubic);
    }
}
