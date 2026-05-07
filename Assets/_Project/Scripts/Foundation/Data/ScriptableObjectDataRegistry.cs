using System;
using System.Collections.Generic;
using App.Gameplay.Definitions;
using UnityEngine;

namespace App.Foundation.Data
{
    public sealed class ScriptableObjectDataRegistry : IDataRegistry
    {
        private const string EnvironmentControlPath = "Data/EnvironmentControls";

        private readonly Dictionary<string, EnvironmentControlDefinition> environmentControls = new();
        private bool isLoaded;

        public EnvironmentControlDefinition GetEnvironmentControl(string id)
        {
            LoadIfNeeded();

            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            environmentControls.TryGetValue(id, out var definition);
            return definition;
        }

        public IReadOnlyList<EnvironmentControlDefinition> GetEnvironmentControls()
        {
            LoadIfNeeded();
            return new List<EnvironmentControlDefinition>(environmentControls.Values);
        }

        private void LoadIfNeeded()
        {
            if (isLoaded)
            {
                return;
            }

            isLoaded = true;
            var definitions = Resources.LoadAll<EnvironmentControlDefinition>(EnvironmentControlPath);

            foreach (var definition in definitions)
            {
                if (definition == null || string.IsNullOrWhiteSpace(definition.Id))
                {
                    continue;
                }

                if (environmentControls.ContainsKey(definition.Id))
                {
                    throw new InvalidOperationException($"Duplicated environment control id: {definition.Id}");
                }

                environmentControls.Add(definition.Id, definition);
            }
        }
    }
}
