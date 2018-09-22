using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Goap
{
    public delegate bool FunCondition(object val);
    public delegate void FunEffect(ref object val);



    public class Action {
        public List<KeyValuePair<string, FunCondition>> conditions = new List<KeyValuePair<string,FunCondition>>();
        public List<KeyValuePair<string, FunEffect>> effects = new List<KeyValuePair<string,FunEffect>>();
        public int cost;
        public string name;

        public Action(string name,int cost)
        {
            this.name = name;
            this.cost = cost;
        }

        public void AddCondition(string key, FunCondition condition)
        {
            conditions.Add(new KeyValuePair<string, FunCondition>(key,condition));
        }
        public bool CheckCondition(Agent agent)
        {
            for(int i = 0; i<conditions.Count; i++) {
                object val = agent.GetValue(conditions[i].Key);
                if (!conditions[i].Value(val)) {
                    return false;
                }
            }
            return true;
        }
        public void AddEffect(string key, FunEffect effect)
        {
            effects.Add(new KeyValuePair<string, FunEffect>(key, effect));
        }
        public List<KeyValuePair<string,object>> PerformEffects(Agent agent)
        {
            List<KeyValuePair<string, object>> oldValues = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < effects.Count; i++) {
                string key = effects[i].Key;
                object val = agent.GetValue(key);
                oldValues.Add(new KeyValuePair<string,object>(key,val));
                effects[i].Value(ref val);
                agent.SetValue(key,val);
            }
            return oldValues;
        }
        public void RevertValue(Agent agent,List<KeyValuePair<string,object>>oldValues)
        {
            for (int i = 0; i < oldValues.Count; i++) {
                agent.SetValue(oldValues[i].Key,oldValues[i].Value);
            }
        }
    }

    
}

