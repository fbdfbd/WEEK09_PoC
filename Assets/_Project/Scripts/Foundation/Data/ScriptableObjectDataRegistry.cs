using System;
using System.Collections.Generic;
using App.Gameplay.Definitions;
using UnityEngine;

namespace App.Foundation.Data
{
    public sealed class ScriptableObjectDataRegistry : IDataRegistry
    {
        private const string EnvironmentControlPath = "Data/EnvironmentControls";
        private const string WeekPath = "Data/Weeks";
        private const string EndingPath = "Data/Endings";

        private readonly Dictionary<string, EnvironmentControlDefinition> _environmentControls = new();
        private readonly Dictionary<int, WeekDefinition> _weeks = new();
        private readonly List<EndingDefinition> _endings = new();
        private bool _isLoaded;

        public EnvironmentControlDefinition GetEnvironmentControl(string id)
        {
            LoadIfNeeded();

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            _environmentControls.TryGetValue(id, out var definition);
            return definition;
        }

        public IReadOnlyList<EnvironmentControlDefinition> GetEnvironmentControls()
        {
            LoadIfNeeded();
            return new List<EnvironmentControlDefinition>(_environmentControls.Values);
        }

        public WeekDefinition GetWeek(int weekIndex)
        {
            LoadIfNeeded();
            _weeks.TryGetValue(weekIndex, out var definition);
            return definition;
        }

        public IReadOnlyList<WeekDefinition> GetWeeks()
        {
            LoadIfNeeded();
            return new List<WeekDefinition>(_weeks.Values);
        }

        public IReadOnlyList<EndingDefinition> GetEndings()
        {
            LoadIfNeeded();
            return _endings;
        }

        private void LoadIfNeeded()
        {
            if (_isLoaded)
            {
                return;
            }

            _isLoaded = true;
            var definitions = Resources.LoadAll<EnvironmentControlDefinition>(EnvironmentControlPath);

            foreach (var definition in definitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                {
                    continue;
                }

                if (_environmentControls.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException($"Duplicated environment control id: {definition.Id}");
                }

                _environmentControls.Add(definition.Id, definition);
            }

            LoadWeeks();
            LoadEndings();
        }

        private void LoadWeeks()
        {
            var definitions = Resources.LoadAll<WeekDefinition>(WeekPath);
            foreach (var definition in definitions)
            {
                if (definition == null)
                {
                    continue;
                }

                if (_weeks.ContainsKey(definition.WeekIndex))
                {
                    throw new InvalidOperationException($"Duplicated week index: {definition.WeekIndex}");
                }

                _weeks.Add(definition.WeekIndex, definition);
            }
        }

        private void LoadEndings()
        {
            _endings.AddRange(Resources.LoadAll<EndingDefinition>(EndingPath));
        }
    }
}
