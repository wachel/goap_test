using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Goap
{
    public class Pool<T>where T: new()
    {
        Stack<T> unusedPool= new Stack<T>();
        public T New()
        {
            if(unusedPool.Count > 0) {
                return unusedPool.Pop();
            }
            else {
                return new T();
            }
        }
        public void Recycle(T obj)
        {
            if (obj != null) {
                unusedPool.Push(obj);
            }
        }
    }
}
