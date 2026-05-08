using App.Foundation.Data;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;

namespace App.Gameplay.Ending
{
    public sealed class EndingResolver
    {
        private readonly IDataRegistry _dataRegistry;
        private readonly ContentSelector _contentSelector;

        public EndingResolver(
            IDataRegistry dataRegistry,
            ContentSelector contentSelector)
        {
            _dataRegistry = dataRegistry;
            _contentSelector = contentSelector;
        }

        public EndingDefinition Resolve()
        {
            var endings = _dataRegistry.GetEndings();
            if (endings == null)
            {
                return null;
            }

            var matched = _contentSelector.SelectHighestPriority(
                endings,
                ending => ending.IsFallback ? null : ending.Conditions,
                ending => ending.IsFallback ? int.MinValue : ending.Priority);

            if (matched != null && !matched.IsFallback)
            {
                return matched;
            }

            foreach (var ending in endings)
            {
                if (ending != null && ending.IsFallback)
                {
                    return ending;
                }
            }

            return null;
        }
    }
}
