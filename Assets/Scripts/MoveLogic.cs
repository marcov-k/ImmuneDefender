using UnityEngine;
using System.Collections.Generic;
using System;
using static LevelManager;
using Unity.Mathematics;

public class MoveLogic
{
    public virtual List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        throw new NotImplementedException();
    }
}

public class Linear : MoveLogic
{
    public override List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        List<int2> options = new();

        if (pos.x < positions.GetLength(0) - 1)
        {
            options.Add(new(pos.x + 1, pos.y));
        }
        if (pos.y > 0)
        {
            options.Add(new(pos.x, pos.y - 1));
        }
        if (pos.y < positions.GetLength(1) - 1)
        {
            options.Add(new(pos.x, pos.y + 1));
        }
        return options;
    }
}

public static class MoveCont
{
    public static MoveLogic GetMoveLogic(string name)
    {
        switch (name)
        {
            case "linear":
                return new Linear();
            default:
                return null;
        }
    }
}
