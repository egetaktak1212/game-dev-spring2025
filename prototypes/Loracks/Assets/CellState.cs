// using UnityEngine;

using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class CellState
{
    public int x;
    public int y;
    public float height;
    public bool being_harvested;
    public bool occupied = false;

    //1-100, 0 - 33 is treeState 1, 34 - 66 is tree state 2, 67 - 100 is tree state 3.
    float _treeState;
    public TreeStateSwitcher treeScript;
    public float treeState
    {
        get
        {
            return _treeState;
        }
        set
        {
            _treeState = value;
            _treeState = Mathf.Clamp(_treeState, -1, 100);
            treeScript.setTreeVisual(_treeState);
        }
    }

    public void debugTreeSwitcherDown() {
        treeState = (treeState - 10);
    }
    public void debugTreeSwitcherUp() {
        treeState = (treeState + 10);
    }

    


    public CellState Clone()
    {
        return new CellState
        {
            x = this.x,
            y = this.y,
            height = this.height,
            _treeState = this._treeState,
            treeScript = this.treeScript,
            being_harvested = this.being_harvested
        };
    }
}
