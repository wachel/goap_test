using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Goap
{
    public struct Agent4
    {
        public bool hasWeapon;
        public int bulletNum;
        public int killedNum;
    }

    public abstract class ActionBase4<T>
    {
        public abstract int GetValue();
        public abstract bool CheckCondition(T agent);
        public abstract T PerformEffect(T agent);
    }

    public class ActionPickWeapon4 : ActionBase4<Agent4>
    {
        public override int GetValue()
        {
            return -30;
        }
        public override bool CheckCondition(Agent4 agent)
        {
            return agent.hasWeapon == false;
        }
        public override Agent4 PerformEffect(Agent4 agent)
        {
            Agent4 newAgent = agent;
            newAgent.hasWeapon = true;
            newAgent.bulletNum = 10;
            return newAgent;
        }
    }

    public class ActionBuyWeapon4 : ActionBase4<Agent4>
    {
        public override int GetValue()
        {
            return -4;
        }
        public override bool CheckCondition(Agent4 agent)
        {
            return agent.hasWeapon == false;
        }
        public override Agent4 PerformEffect(Agent4 agent)
        {
            Agent4 newAgent = agent;
            newAgent.hasWeapon = true;
            return newAgent;
        }
    }

    public class ActionBuyBullet4 : ActionBase4<Agent4>
    {
        public override int GetValue()
        {
            return -3;
        }
        public override bool CheckCondition(Agent4 agent)
        {
            return true;
        }
        public override Agent4 PerformEffect(Agent4 agent)
        {
            Agent4 newAgent = agent;
            newAgent.bulletNum += 1;
            return newAgent;
        }
    }

    public class ActionFire4 : ActionBase4<Agent4>
    {
        public override int GetValue()
        {
            return -10;
        }
        public override bool CheckCondition(Agent4 agent)
        {
            return agent.hasWeapon && agent.bulletNum > 0;
        }
        public override Agent4 PerformEffect(Agent4 agent)
        {
            Agent4 newAgent = agent;
            newAgent.bulletNum -= 1;
            newAgent.killedNum += 1;
            return newAgent;
        }
    }

    public class ActionGoal4 : ActionBase4<Agent4>
    {
        public override int GetValue()
        {
            return 0;
        }
        public delegate bool FunGoalCondition(Agent4 agent);
        public FunGoalCondition condition;
        public override bool CheckCondition(Agent4 agent)
        {
            return condition(agent);
        }
        public override Agent4 PerformEffect(Agent4 agent)
        {
            return agent;
        }
    }

    public class Path4<T>
    {
        public List<ActionBase4<T>> actions = new List<ActionBase4<T>>(20);
        public int Value { get { return value; } }
        private int value;
        public void Append(ActionBase4<T> action)
        {
            actions.Add(action);
            value += action.GetValue();
        }
        public void Append(Path4<T> path)
        {
            actions.AddRange(path.actions);
            foreach (ActionBase4<T> action in path.actions) {
                value += action.GetValue();
            }
        }
        public void AddHead(ActionBase4<T> action)
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

    public static class Planner4<T>
    {
        public static Pool<Path4<T>> pathPool = new Pool<Path4<T>>();
        public static Path4<T> GetPlanning(List<ActionBase4<T>> actions, T agent, ActionBase4<T> goal,int minValue)
        {
            if (goal.CheckCondition(agent)) {
                Path4<T> path = pathPool.New();
                path.Reset();
                path.Append(goal);
                return path;
            } else {
                int maxValue = -99999;
                ActionBase4<T> bestAction = null;
                Path4<T> bestSubPath = null;
                foreach (ActionBase4<T> action in actions) {
                    if (action.CheckCondition(agent)) {
                        if (action.GetValue() >= minValue) {
                            T newAgent = action.PerformEffect(agent);
                            Path4<T> subPath = GetPlanning(actions, newAgent, goal, minValue - action.GetValue());
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

    public class ActionTest4 : MonoBehaviour
    {
        Agent4 agent = new Agent4();
        List<ActionBase4<Agent4>> actions = new List<ActionBase4<Agent4>>();
        ActionGoal4 actionGoal = null;
        void Start()
        {
            agent.hasWeapon = false;
            agent.bulletNum = 0;
            agent.killedNum = 0;

            actions.Add(new ActionPickWeapon4());
            actions.Add(new ActionBuyWeapon4());
            actions.Add(new ActionBuyBullet4());
            actions.Add(new ActionFire4());

            actionGoal = new ActionGoal4();
            actionGoal.condition = (Agent4 a) => { return a.killedNum == 2; };

            Path4<Agent4> path = Planner4<Agent4>.GetPlanning(actions, agent, actionGoal, -50);
            string pathString = "";
            foreach (ActionBase4<Agent4> action in path.actions) {
                pathString += "->" + action.GetType().Name;
            }
            Debug.Log("start" + pathString);
        }

        void Update()
        {
            Path4<Agent4> path = Planner4<Agent4>.GetPlanning(actions, agent, actionGoal, -50);
            Planner4<Agent4>.pathPool.Recycle(path);
            //string pathString = "";
            //foreach (ActionBase action in path.actions) {
            //    pathString += "->" + action.GetType().Name;
            //}
            //Debug.Log("start" + pathString);
        }
    }
}