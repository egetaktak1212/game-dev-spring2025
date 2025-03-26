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

    //1-100, 0 - 33 is treeState 1, 34 - 66 is tree state 2, 67 - 100 is tree state 3.
    int treeState;
    public TreeStateSwitcher treeScript;


    public void setTreeState(int setTo) { 
        treeState = setTo;
        treeScript.setTreeVisual(setTo);
        Debug.Log("A");
    }

    public void debugTreeSwitcherDown() {
        setTreeState(treeState - 10);
    }
    public void debugTreeSwitcherUp() { 
        setTreeState(treeState + 10);
    }

    


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
