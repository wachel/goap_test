using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Goap
{
    public static class Planner
    {
        static Pool<Path2> pathPool = new Pool<Path2>();
        public static Path2 GetPlanning(List<ActionBase> actions, Agent start, ActionBase goal, int maxCost)
        {
            if (goal.CheckCondition(start)) {
                Path2 path = pathPool.New();
                path.Reset();
                path.Append(goal);
                return path;
            }
            else {
                int minCost = 99999;
                ActionBase bestAction = null;
                Path2 bestSubPath = null;
                foreach (ActionBase action in actions) {
                    if (action.CheckCondition(start)) {
                        if (action.GetCost() <= maxCost) {
                            ActionBase.FunRevert funRevert = action.PerformEffect(start);
                            Path2 subPath = GetPlanning(actions, start, goal, maxCost - action.GetCost());
                            if (subPath != null) {
                                int cost = action.GetCost() + subPath.Cost;
                                if (cost < minCost) {
                                    if (bestSubPath != null) {
                                        pathPool.Recycle(bestSubPath);
                                    }
                                    minCost = cost;
                                    maxCost = minCost;
                                    bestAction = action;
                                    bestSubPath = subPath;
                                }
                                else {
                                    pathPool.Recycle(subPath);
                                }
                            }
                            funRevert(start);
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

}