// using UnityEngine;

public class CellState
{
    public int x;
    public int y;
    public float height;

    public CellState Clone()
    {
        return new CellState
        {
            x = this.x,
            y = this.y,
            height = this.height
        };
    }
}
