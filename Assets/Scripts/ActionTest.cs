using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Goap
{
    public class ActionTest : MonoBehaviour
    {
        Agent agent = new Agent();
        List<Action> actions = new List<Action>();
        Action actionGoal = null;
        void Start()
        {
            agent.SetValue("has_weapon", false);
            agent.SetValue("bullet_num", 0);
            agent.SetValue("kill_enemy", 0);

            Action actionPickWeapon = new Action("pick weapon",30);
            actionPickWeapon.AddCondition("has_weapon", (object val) => (bool)val == false);
            actionPickWeapon.AddEffect("has_weapon",(ref object val) => val = true);
            actionPickWeapon.AddEffect("bullet_num", (ref object val) => val = 10);
            actions.Add(actionPickWeapon);

            Action actionBuyWeapon = new Action("buy weapon", 4);
            actionBuyWeapon.AddCondition("has_weapon", (object val) => (bool)val == false);
            actionBuyWeapon.AddEffect("has_weapon", (ref object val) => val = true);
            actions.Add(actionBuyWeapon);

            Action actionBuyBullet = new Action("buy bullet", 3);
            actionBuyBullet.AddEffect("bullet_num", (ref object val) => val = (int)val + 1);
            actions.Add(actionBuyBullet);

            Action actionFire = new Action("fire", 10);
            actionFire.AddCondition("has_weapon", (object val) => (bool)val == true);
            actionFire.AddCondition("bullet_num", (object val) => (int)val > 0);
            actionFire.AddEffect("bullet_num", (ref object val) => val = (int)val - 1);
            actionFire.AddEffect("kill_enemy", (ref object val) => val = (int)val + 1);
            actions.Add(actionFire);


            actionGoal = new Action("done",0);
            actionGoal.AddCondition("kill_enemy", (object val) => (int)val == 2);

            Path path = GetPlanning(actions, agent, actionGoal,50);
            string pathString = "";
            foreach(Action action in path.actions) {
                pathString += "->" + action.name;
            }
            Debug.Log("start" + pathString);
        }

        Path GetPlanning(List<Action>actions,Agent start,Action goal,int maxCost)
        {
            if (goal.CheckCondition(start)) {
                Path path = new Path();
                path.Append(goal);
                return path;
            }
            else{
                List<Action> result = new List<Action>();
                int minCost = 99999;
                Action bestAction = null;
                Path bestSubPath = null;
                foreach(Action action in actions){
                    if (action.CheckCondition(start)) {
                        if(action.cost <= maxCost) {
                            List<KeyValuePair<string, object>> oldValues = action.PerformEffects(start);
                            Path subPath = GetPlanning(actions, start, goal, maxCost - action.cost);
                            if (subPath != null) {
                                int cost = action.cost + subPath.Cost;
                                if (cost < minCost) {
                                    minCost = cost;
                                    bestAction = action;
                                    bestSubPath = subPath;
                                }
                            }
                            action.RevertValue(agent, oldValues);
                        }
                    }
                }
                if(bestAction != null) {
                    Path path = new Path();
                    path.Append(bestAction);
                    path.Append(bestSubPath);
                    return path;
                }
            }
            return null;
        }

        void Update()
        {
            Path path = GetPlanning(actions, agent, actionGoal, 50);
            //string pathString = "";
            //foreach (Action action in path.actions) {
            //    pathString += "->" + action.name;
            //}
            //Debug.Log("start" + pathString);
        }
    }
}