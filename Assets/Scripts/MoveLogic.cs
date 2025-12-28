using System.Collections.Generic;
using static LevelManager;
using Unity.Mathematics;

public class MoveLogic
{
    public virtual List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        List<int2> options = new();
        if (pos.x == -1)
        {
            options.Add(new(0, pos.y));
        }
        return options;
    }
}

public class Linear : MoveLogic
{
    public override List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        var options = base.FindNextPos(pos, positions);

        if (options.Count == 0)
        {
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
        }
        return options;
    }
}

public class Diagonal : MoveLogic
{
    public override List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        var options = base.FindNextPos(pos, positions);

        if (options.Count == 0)
        {
            if (pos.x < positions.GetLength(0) - 1)
            {
                if (pos.y < positions.GetLength(1) - 1)
                {
                    options.Add(new(pos.x + 1, pos.y + 1));
                }
                if (pos.y > 0)
                {
                    options.Add(new(pos.x + 1, pos.y - 1));
                }
            }
        }
        return options;
    }
}

public class Combined : MoveLogic
{
    public override List<int2> FindNextPos(int2 pos, Position[,] positions)
    {
        var options = base.FindNextPos(pos, positions);

        if (options.Count == 0)
        {
            bool xValid = false;
            if (pos.x < positions.GetLength(0) - 1)
            {
                xValid = true;
                options.Add(new(pos.x + 1, pos.y));
            }
            if (pos.y < positions.GetLength(1) - 1)
            {
                options.Add(new(pos.x, pos.y + 1));
                if (xValid) options.Add(new(pos.x + 1, pos.y + 1));
            }
            if (pos.y > 0)
            {
                options.Add(new(pos.x, pos.y - 1));
                if (xValid) options.Add(new(pos.x + 1, pos.y - 1));
            }
        }
        return options;
    }
}

public static class MoveCont
{
    public static MoveLogic GetMoveLogic(string name)
    {
        return name switch
        {
            "linear" => new Linear(),
            "diagonal" => new Diagonal(),
            "combined" => new Combined(),
            _ => null,
        };
    }
}
