using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap5
{
    public class WorldState
    {
        Stack<KeyValuePair<string, int>> changeStack = new Stack<KeyValuePair<string, int>>(20);
        Dictionary<string, int> values = new Dictionary<string, int>();
        
        public int SetValue(string key,int value)
        {
            return values[key] = value;
        }
        public int GetValue(string key)
        {
            return values[key];
        }
        public void Perform(string key, int val)
        {
            changeStack.Push(new KeyValuePair<string, int>(key, values[key]));
            values[key] = val;
        }
        public void Revert()
        {
            KeyValuePair<string,int> keyVal = changeStack.Pop();
            values[keyVal.Key] = keyVal.Value;
        }
        public int StackLenght {
            get { return changeStack.Count; }
        }
    }

    public abstract class ActionBase
    {
        public abstract int GetValue();
        public abstract bool CheckCondition(WorldState state);
        public abstract void PerformEffect(WorldState state);
    }

    public class ActionPickWeapon4 : ActionBase
    {
        public override int GetValue()
        {
            return -30;
        }
        public override bool CheckCondition(WorldState state)
        {
            return state.GetValue("has_weapon") == 0;
        }
        public override void PerformEffect(WorldState state)
        {
            state.Perform("has_weapon", 1);
            state.Perform("bullet_num", 10);
        }
    }

    public class ActionBuyWeapon4 : ActionBase
    {
        public override int GetValue()
        {
            return -4;
        }
        public override bool CheckCondition(WorldState state)
        {
            return state.GetValue("has_weapon") == 0;
        }
        public override void PerformEffect(WorldState state)
        {
            state.Perform("has_weapon", 1);
        }
    }

    public class ActionBuyBullet4 : ActionBase
    {
        public override int GetValue()
        {
            return -3;
        }
        public override bool CheckCondition(WorldState state)
        {
            return true;
        }
        public override void PerformEffect(WorldState state)
        {
            state.Perform("bullet_num", state.GetValue("bullet_num") + 1);
        }
    }

    public class ActionFire4 : ActionBase
    {
        public override int GetValue()
        {
            return -10;
        }
        public override bool CheckCondition(WorldState state)
        {
            return state.GetValue("has_weapon") == 1 && state.GetValue("bullet_num") > 0;
        }
        public override void PerformEffect(WorldState state)
        {
            state.Perform("bullet_num", state.GetValue("bullet_num") - 1);
            state.Perform("kill_num", state.GetValue("kill_num") + 1);
        }
    }

    public class ActionGoal4 : ActionBase
    {
        public override int GetValue()
        {
            return 0;
        }
        public delegate bool FunGoalCondition(WorldState state);
        public FunGoalCondition condition;
        public override bool CheckCondition(WorldState state)
        {
            return condition(state);
        }
        public override void PerformEffect(WorldState state)
        {

        }
    }

    public class Path
    {
        public List<ActionBase> actions = new List<ActionBase>(20);
        public int Value { get { return value; } }
        private int value;
        public void Append(ActionBase action)
        {
            actions.Add(action);
            value += action.GetValue();
        }
        public void Append(Path path)
        {
            actions.AddRange(path.actions);
            foreach (ActionBase action in path.actions) {
                value += action.GetValue();
            }
        }
        public void AddHead(ActionBase action)
        {
            actions.Insert(0, action);
            value += action.GetValue();
        }

        public void Reset()
        {
            actions.Clear();
            value = 0;
        }
    }
    public static class Planner
    {
        static int gindex = 0;
        public static Goap.Pool<Path> pathPool = new Goap.Pool<Path>();
        public static Path GetPlanning(List<ActionBase> actions, WorldState agent, ActionBase goal,int minValue)
        {
            if (goal.CheckCondition(agent)) {
                Path path = pathPool.New();
                path.Reset();
                path.Append(goal);
                return path;
            } else {
                int maxValue = -99999;
                ActionBase bestAction = null;
                Path bestSubPath = null;
                foreach (ActionBase action in actions) {
                    if (action.CheckCondition(agent)) {
                        if (action.GetValue() >= minValue) {
                            int stackLenght = agent.StackLenght;
                            action.PerformEffect(agent);
                            Path subPath = GetPlanning(actions, agent, goal, minValue - action.GetValue());
                            if (subPath != null) {
                                int value = action.GetValue() + subPath.Value;
                                if (value > maxValue) {
                                    if (bestSubPath != null) {
                                        pathPool.Recycle(bestSubPath);
                                    }
                                    maxValue = value;
                                    bestAction = action;
                                    bestSubPath = subPath;
                                } else {
                                    pathPool.Recycle(subPath);
                                }
                            }
                            int numToRevert = agent.StackLenght - stackLenght;
                            for (int i = 0; i < numToRevert; i++) {
                                agent.Revert();
                            }
                        }
                    }
                }
                if (bestAction != null) {
                    bestSubPath.AddHead(bestAction);
                    return bestSubPath;
                }
            }
            return null;
        }

    }

    public class ActionTest5 : MonoBehaviour
    {
        WorldState agent = new WorldState();
        List<ActionBase> actions = new List<ActionBase>();
        ActionGoal4 actionGoal = null;
        void Start()
        {
            agent.SetValue("has_weapon", 0);
            agent.SetValue("bullet_num", 0);
            agent.SetValue("kill_num", 0);

            actions.Add(new ActionPickWeapon4());
            actions.Add(new ActionBuyWeapon4());
            actions.Add(new ActionBuyBullet4());
            actions.Add(new ActionFire4());

            actionGoal = new ActionGoal4();
            actionGoal.condition = (WorldState a) => { return a.GetValue("kill_num") == 2; };

            Path path = Planner.GetPlanning(actions, agent, actionGoal, -50);
            string pathString = "";
            foreach (ActionBase action in path.actions) {
                pathString += "->" + action.GetType().Name;
            }
            Debug.Log("start" + pathString);
        }

        void Update()
        {
            Path path = Planner.GetPlanning(actions, agent, actionGoal, -50);
            Planner.pathPool.Recycle(path);
            //string pathString = "";
            //foreach (ActionBase action in path.actions) {
            //    pathString += "->" + action.GetType().Name;
            //}
            //Debug.Log("start" + pathString);
        }
    }
}