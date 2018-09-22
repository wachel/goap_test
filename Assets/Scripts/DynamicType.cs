using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Goap
{
    public class TypeValue
    {
        Dictionary<string, int> intValues = new Dictionary<string, int>();
        public int GetIntValue(string name, int defaultValue = 0)
        {
            int temp = defaultValue;
            intValues.TryGetValue(name, out temp);
            return temp;
        }
        public void SetIntValue(string name, int value)
        {
            intValues[name] = value;
        }

        Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        public bool GetBoolValue(string name, bool defaultValue = false)
        {
            bool temp = defaultValue;
            boolValues.TryGetValue(name, out temp);
            return temp;
        }
        public void SetBoolValue(string name, bool value)
        {
            boolValues[name] = value;
        }
    }
}