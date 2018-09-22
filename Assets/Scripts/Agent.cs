using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Goap
{

    public class Agent
    {
        Dictionary<string, object> values = new Dictionary<string,object>();
        TypeValue typeValues = new TypeValue();
        public object GetValue(string name, object defaultValue = null)
        {
            object temp = defaultValue;
            values.TryGetValue(name, out temp);
            return temp;
        }
        public void SetValue(string name, object value)
        {
            values[name] = value;
        }
        public int GetIntValue(string name, int defaultValue = 0)
        {
            return typeValues.GetIntValue(name, defaultValue);
        }
        public void SetIntValue(string name, int value)
        {
            typeValues.SetIntValue(name, value);
        }

        Dictionary<string, bool> boolValues = new Dictionary<string, bool>();
        public bool GetBoolValue(string name, bool defaultValue = false)
        {
            return typeValues.GetBoolValue(name, defaultValue);
        }
        public void SetBoolValue(string name, bool value)
        {
            typeValues.SetBoolValue(name, value);
        }

        public bool hasWeapon;
        public int bulletNum;
        public int killedNum;
    }



}