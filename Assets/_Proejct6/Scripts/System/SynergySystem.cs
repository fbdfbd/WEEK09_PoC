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
                context.State.Log.Add("시너지 발동: " + synergy.DisplayName);
                context.State.PendingSynergyPopups.Add(new SynergyPopupRequest(synergy.DisplayName, synergy.Description));
                synergy.Apply(context);
            }
        }
    }
}
