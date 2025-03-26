using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<CellScript> gridPlacedOn = new List<CellScript>();

    public int centerX;
    public int centerY;

    void Start()
    {
        
    }

    public void setGrid(List<CellScript> grid) {
        gridPlacedOn = grid;
    }

    public void removeFactory() {
        for (int i = 0; i < gridPlacedOn.Count; i++) {
            gridPlacedOn[i].occupied = false;
        }
        //make each grid dead tree
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
