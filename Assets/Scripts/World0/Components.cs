using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComponentBase {
    public Agent agent;
    public virtual void Init() { }
    public virtual void Update() { }
}

public class FieldOfView : ComponentBase
{
    public Dictionary<int, Agent> agents;
    public override void Update()
    {
        agents = agent.world.agents;
    }
}

public class DangerDetecter:ComponentBase
{
    float value = 0;
    FieldOfView field;
    public void Init()
    {
        //field = agent.GetComponent<FieldOfView>();
    }
    void Update()
    {
        foreach (Agent item in field.agents.Values) {
            if (item != agent) {
                //if()
            }
        }
    }
}