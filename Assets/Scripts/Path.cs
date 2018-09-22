using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Goap {
    public class Path {
        public List<Action> actions = new List<Action>(10);
        public int Cost { get { return cost; } }
        private int cost;
        public void Append(Action action)
        {
            actions.Add(action);
            cost += action.cost;
        }
        public void Append(Path path)
        {
            actions.AddRange(path.actions);
            foreach(Action action in path.actions) {
                cost += action.cost;
            }
        }
    }


    public class Path2
    {
        public List<ActionBase> actions = new List<ActionBase>(0);
        public int Cost { get { return cost; } }
        private int cost;
        public void Append(ActionBase action)
        {
            actions.Add(action);
            cost += action.GetCost();
        }
        public void Append(Path2 path)
        {
            actions.AddRange(path.actions);
            foreach (ActionBase action in path.actions) {
                cost += action.GetCost();
            }
        }
        public void AddHead(ActionBase action)
        {
            actions.Insert(0, action);
            cost += action.GetCost();
        }

        public void Reset()
        {
            actions.Clear();
            cost = 0;
        }
    }
}
