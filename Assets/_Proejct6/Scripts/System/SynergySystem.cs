public class SynergySystem
{
    public void Resolve(BattleContext context)
    {
        for (int i = 0; i < context.State.EquippedSynergies.Count; i++)
        {
            SynergyDefinition synergy = context.State.EquippedSynergies[i];

            if (synergy == null)
            {
                continue;
            }

            if (synergy.CanActivate(context))
            {
                synergy.Apply(context);
            }
        }
    }
}
