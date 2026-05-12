public class SynergySystem
{
    public void ResolveSynergies(BattleContext context)
    {
        for (int i = 0; i < context.State.EquippedSynergies.Count; i++)
        {
            SynergyDefinition synergy = context.State.EquippedSynergies[i];
            if (synergy != null && synergy.CanActivate(context))
            {
                synergy.Apply(context);
            }
        }
    }
}
