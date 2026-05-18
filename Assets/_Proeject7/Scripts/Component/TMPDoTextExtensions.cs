using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;

public static class TMPDoTextExtensions
{
    public static TweenerCore<string, string, StringOptions> DOText(
        this TMP_Text target,
        string endValue,
        float duration)
    {
        return DOTween
            .To(() => target.text, value => target.text = value, endValue, duration)
            .SetTarget(target);
    }
}
