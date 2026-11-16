using System;
using System.Collections.Generic;
using System.Text;

namespace AILab.Core.BehaviourTree
{
    public class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        public void SetValue<T>(string key, T value)
        {
            _data[key] = value!;
        }

        public T? GetValue<T>(string key)
        {
            if (_data.TryGetValue(key, out object? value) &&
                value is T typedValue)
            {
                return typedValue;
            }

            return default;
        }

        public bool HasValue(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
