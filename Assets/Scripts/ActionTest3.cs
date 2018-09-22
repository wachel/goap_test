using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Goap
{
    public class ActionPickWeapon3:ActionBase
    {
        public override int GetCost()
        {
            return 30;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.hasWeapon == false;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            bool old_has_weapon = agent.hasWeapon;
            int old_bullet_num = agent.bulletNum;
            agent.hasWeapon = true;
            agent.bulletNum = 10;
            return (Agent a) => {
                a.hasWeapon = old_has_weapon;
                a.bulletNum = old_bullet_num;
            };
        }
    }

    public class ActionBuyWeapon3 : ActionBase
    {
        public override int GetCost()
        {
            return 4;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.hasWeapon == false;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            bool old_has_weapon = agent.hasWeapon;
            agent.hasWeapon = true;
            return (Agent a) => {
                a.hasWeapon = old_has_weapon;
            };
        }
    }

    public class ActionBuyBullet3 : ActionBase
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
            int old_bullet_num = agent.bulletNum;
            agent.bulletNum += 1;
            return (Agent a) => {
                a.bulletNum = old_bullet_num;
            };
        }
    }

    public class ActionFire3 : ActionBase
    {
        public override int GetCost()
        {
            return 10;
        }
        public override bool CheckCondition(Agent agent)
        {
            return agent.hasWeapon && agent.bulletNum > 0;
        }
        public override FunRevert PerformEffect(Agent agent)
        {
            int old_bullet_num = agent.bulletNum;
            int old_kill_enemy = agent.killedNum;
            agent.bulletNum -= 1;
            agent.killedNum += 1;
            return (Agent a) => {
                a.bulletNum = old_bullet_num;
                a.killedNum = old_kill_enemy;
            };
        }
    }

    public class ActionGoal3:ActionBase
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

    public class ActionTest3 : MonoBehaviour
    {
        Agent agent = new Agent();
        List<ActionBase> actions = new List<ActionBase>();
        ActionGoal actionGoal = null;
        void Start()
        {
            agent.hasWeapon = false;
            agent.bulletNum = 0;
            agent.killedNum = 0;

            actions.Add(new ActionPickWeapon3());
            actions.Add(new ActionBuyWeapon3());
            actions.Add(new ActionBuyBullet3());
            actions.Add(new ActionFire3());

            actionGoal = new ActionGoal();
            actionGoal.condition = (Agent a) => { return a.killedNum == 2; };

            Path2 path = Planner.GetPlanning(actions, agent, actionGoal, 50);
            string pathString = "";
            foreach (ActionBase action in path.actions) {
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