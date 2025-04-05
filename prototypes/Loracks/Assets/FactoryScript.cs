using System.Collections.Generic;
using UnityEngine;

public class FactoryScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<CellScript> gridPlacedOn = new List<CellScript>();

    public int centerX;
    public int centerY;
    float harvestAmount = 0.1f;
    public float storage = 0;

    void Start()
    {
        setNearbyAsHarvested(true);
    }

    public void setGrid(List<CellScript> grid) {
        gridPlacedOn = grid;
    }

    public void removeFactory() {
        for (int i = 0; i < gridPlacedOn.Count; i++) {
            gridPlacedOn[i].State.occupied = false;
        }

        setNearbyAsHarvested(false);


        //make each grid dead tree
    }

    void setNearbyAsHarvested(bool yesorno) {
        List<CellScript> level1 = GridManager.Instance.GetBorderingCellsByOffsetCellScript(centerX, centerY, 1, 2, 1, GridManager.Instance.grid);
        List<CellScript> level2 = GridManager.Instance.GetBorderingCellsByOffsetCellScript(centerX, centerY, 1, 2, 2, GridManager.Instance.grid);
        List<CellScript> level3 = GridManager.Instance.GetBorderingCellsByOffsetCellScript(centerX, centerY, 1, 2, 3, GridManager.Instance.grid);

        for (int i = 0; i < level1.Count; i++) { 
            level1[i].State.being_harvested = yesorno;
        }
        for (int i = 0; i < level2.Count; i++)
        {
            level2[i].State.being_harvested = yesorno;
        }
        for (int i = 0; i < level3.Count; i++) 
        {
            level3[i].State.being_harvested = yesorno;
        }

    }



    // Update is called once per frame
    void Update()
    {
        

        
    }

    public void handleSimulation(CellState[,] grid) {
        List<CellState> level1 = GridManager.Instance.GetBorderingCellsByOffset(centerX, centerY, 1, 2, 1, grid);
        for (int i = 0; i < level1.Count; i++) {
            float prev = level1[i].treeState;
            level1[i].treeState = level1[i].treeState - harvestAmount;
            float harvested = prev - level1[i].treeState;
            storage += harvested;
        }

        List<CellState> level2 = GridManager.Instance.GetBorderingCellsByOffset(centerX, centerY, 1, 2, 2, grid);
        for (int i = 0; i < level2.Count; i++)
        {
            float prev = level2[i].treeState;
            level2[i].treeState = level2[i].treeState - harvestAmount/2;
            float harvested = prev - level2[i].treeState;
            storage += harvested;
        }

        List<CellState> level3 = GridManager.Instance.GetBorderingCellsByOffset(centerX, centerY, 1, 2, 2, grid);
        for (int i = 0; i < level3.Count; i++)
        {
            float prev = level3[i].treeState;
            level3[i].treeState = level3[i].treeState - harvestAmount / 4;
            float harvested = prev - level3[i].treeState;
            storage += harvested;
        }

    }



}
