using UnityEngine;
using System.Collections;

namespace World0
{
    public class GoalEscape:Goap.ActionBase,Goap.GoalInterface
    {
        private Goap.Agent agent;
        public GoalEscape(Goap.Agent agent)
        {
        }
        public override bool CheckCondition(Goap.Agent agent)
        {
            return true;
        }
        public override int GetCost()
        {
            return 0;
        }
        public int GetValue()
        {
            return 10;
        }
        public override FunRevert PerformEffect(Goap.Agent agent)
        {
            return null;
        }
    }

    public class GoalPickObject
    {

    }

}
