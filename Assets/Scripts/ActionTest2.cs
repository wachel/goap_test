using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

namespace Goap
{
    public interface GoalInterface
    {
        int GetCost();
        int GetValue();
    }

    public abstract class ActionBase
    {
        public abstract int GetCost();
        public delegate void FunRevert(Agent agent);
        public abstract bool CheckCondition(Agent agent);
        public abstract FunRevert PerformEffect(Agent agent);
    }

    public class ActionPickWeapon:ActionBase
    {
        public override int GetCost()
        {
            return 30;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.GetBoolValue("has_weapon", false) == false;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            bool old_has_weapon = agent.GetBoolValue("has_weapon");
            int old_bullet_num = agent.GetIntValue("bullet_num");
            agent.SetBoolValue("has_weapon", true);
            agent.SetIntValue("bullet_num", 10);
            return (Agent a) => {
                a.SetBoolValue("has_weapon", old_has_weapon);
                a.SetIntValue("bullet_num", old_bullet_num);
            };
        }
    }

    public class ActionBuyWeapon : ActionBase
    {
        public override int GetCost()
        {
            return 4;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.GetBoolValue("has_weapon", false) == false;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            bool old_has_weapon = agent.GetBoolValue("has_weapon");
            agent.SetBoolValue("has_weapon", true);
            return (Agent a) => {
                a.SetBoolValue("has_weapon", old_has_weapon);
            };
        }
    }

    public class ActionBuyBullet : ActionBase
    {
        public override int GetCost()
        {
            return 3;
        }
        public override bool CheckCondition(Agent agent)
        {
            return true;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            int old_bullet_num = agent.GetIntValue("bullet_num");
            agent.SetIntValue("bullet_num", old_bullet_num + 1);
            return (Agent a) => {
                a.SetIntValue("bullet_num", old_bullet_num);
            };
        }
    }

    public class ActionFire : ActionBase
    {
        public override int GetCost()
        {
            return 10;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.GetBoolValue("has_weapon") && agent.GetIntValue("bullet_num") > 0;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            int old_bullet_num = agent.GetIntValue("bullet_num");
            int old_kill_enemy = agent.GetIntValue("kill_enemy");
            agent.SetIntValue("bullet_num", old_bullet_num - 1);
            agent.SetIntValue("kill_enemy", old_kill_enemy + 1);
            return (Agent a) => {
                a.SetIntValue("bullet_num", old_bullet_num);
                a.SetIntValue("kill_enemy", old_kill_enemy);
            };
        }
    }

    public class ActionGoal:ActionBase
    {
        public override int GetCost()
        {
            return 0;
        }
        public delegate bool FunGoalCondition(Agent agent);
        public FunGoalCondition condition;
        public override bool CheckCondition(Agent agent)
        {
            return condition(agent);
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            return null;
        }
    }

    public class ActionTest2 : MonoBehaviour
    {
        Agent agent = new Agent();
        List<ActionBase> actions = new List<ActionBase>();
        ActionGoal actionGoal = null;
        void Start()
        {
            agent.SetValue("has_weapon", false);
            agent.SetValue("bullet_num", 0);
            agent.SetValue("kill_enemy", 0);

            actions.Add(new ActionPickWeapon());
            actions.Add(new ActionBuyWeapon());
            actions.Add(new ActionBuyBullet());
            actions.Add(new ActionFire());

            actionGoal = new ActionGoal();
            actionGoal.condition = (Agent a) => { return a.GetIntValue("kill_enemy") == 2; };

            Path2 path = Planner.GetPlanning(actions, agent, actionGoal,50);
            string pathString = "";
            foreach(ActionBase action in path.actions) {
                pathString += "->" + action.GetType().Name;
            }
            Debug.Log("start" + pathString);
        }

        void Update()
        {
            Path2 path = Planner.GetPlanning(actions, agent, actionGoal, 50);
            //string pathString = "";
            //foreach (ActionBase action in path.actions) {
            //    pathString += "->" + action.GetType().Name;
            //}
            //Debug.Log("start" + pathString);
        }
    }
}