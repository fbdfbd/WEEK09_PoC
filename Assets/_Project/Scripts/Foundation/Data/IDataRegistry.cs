using System.Collections.Generic;
using App.Gameplay.Definitions;

namespace App.Foundation.Data
{
    public interface IDataRegistry
    {
        EnvironmentControlDefinition GetEnvironmentControl(string id);
        IReadOnlyList<EnvironmentControlDefinition> GetEnvironmentControls();
        WeekDefinition GetWeek(int weekIndex);
        IReadOnlyList<WeekDefinition> GetWeeks();
        IReadOnlyList<EndingDefinition> GetEndings();
    }
}
