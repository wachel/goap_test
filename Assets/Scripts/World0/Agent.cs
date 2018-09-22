using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Agent
{
    public void AddComponent<T>()where T:ComponentBase,new()
    {
        ComponentBase comp = new T();
        comp.agent = this;
        comp.Init();
        components[typeof(T)] = comp;
    }
    //public T GetComponent<T>()
    //{
    //    if (components.ContainsKey(typeof(T))) {
    //        return components[typeof(T)];
    //    }
    //    return null;
    //}
    public Dictionary<Type, ComponentBase> components;
    public Vector3 position;
    public World world;
    public void Update()
    {
        foreach (ComponentBase comp in components.Values) {
            comp.Update();
        }
    }
}
