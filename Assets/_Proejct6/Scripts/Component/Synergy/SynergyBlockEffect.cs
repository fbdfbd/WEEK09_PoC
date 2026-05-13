using UnityEngine;

[CreateAssetMenu(fileName = "SynergyBlockEffect", menuName = "Synergy/Effect/Block")]
public class SynergyBlockEffect : SynergyEffect
{
    [SerializeField] private int _block = 6;

    public override void Apply(BattleContext context)
    {
        context.Player.AddBlock(_block);
    }
}
