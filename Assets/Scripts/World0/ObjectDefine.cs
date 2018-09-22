using UnityEngine;
using System.Collections;


public enum TypeID
{
    Food = 0,
    Sword = 1,
    Rabbit = 2,
    Wolf = 3,
}

public static class Rule
{
    public static float GetDangerValue(TypeID local, TypeID other)
    {
        if (other == TypeID.Wolf && local == TypeID.Rabbit) {
            return 1;
        }
        return 0;
    }
}