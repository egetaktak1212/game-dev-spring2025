using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class VehicleScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    List<CellScript> gridPlacedOn = new List<CellScript>();

    List<CellScript> pathToFollow = new List<CellScript>();

    public int centerX;
    public int centerY;
    float harvestAmount = 0.1f;
    public float storage = 0;
    int range = 5;


    CellScript[,] cellGrid;


    void Start()
    {
        cellGrid = GridManager.Instance.grid;
        setNearbyAsHarvested(true);
    }

    public void setGrid(List<CellScript> grid) {
        gridPlacedOn = grid;
    }

    void setNearbyAsHarvested(bool yesorno) {
        cellGrid[centerX, centerY].State.being_harvested = yesorno;
    }



    // Update is called once per frame
    void Update()
    {
        

        
    }

    public void handleSimulation(CellState[,] grid) {
        if (pathToFollow.Count == 0) {
            CellScript startCell = cellGrid[centerX, centerY];
            CellScript endCell = getEndCell(range);
            if (endCell == null) {
                int i = 0;
                while (getEndCell(range + i) == null) {
                    i++;
                }
                endCell = getEndCell(range + i);
            }
        
            pathToFollow = GridManager.Instance.AStarPath(startCell, endCell);
        }

        CellState currentlyOn = grid[centerX, centerY];

        //if the current tree isnt wiped, eat it
        if (currentlyOn.treeState > -1f) {
            storage += currentlyOn.treeState/10f;
            currentlyOn.treeState = -10f;
        }

        //move to the next tree
        moveTo(pathToFollow[0]);
        pathToFollow.RemoveAt(0);

    }

    void moveTo(CellScript nextCell) {
        int nextX = nextCell.State.x;
        int nextY = nextCell.State.y;

        centerX = nextX;
        centerY = nextY;

        transform.position = new Vector3(nextX, transform.position.y, nextY);

    }

    CellScript getEndCell(int range) { 
        List<CellScript> rangeCells = GridManager.Instance.GetCellScriptsInRange(centerX, centerY, range, range);

        float maxTreeState = rangeCells.Max(cell => cell.State.treeState);
        List<CellScript> nearTopCells = rangeCells
            //i got this from gpt, no clue what it means. it picks cells that are 10f close to the max, makes sure they arent occupied and harvested
            .Where(cell =>
                (maxTreeState - cell.State.treeState <= 10.0f) &&
                !cell.State.being_harvested &&
                !cell.State.occupied)
            .ToList();
        if (nearTopCells.Count == 0)
        {
            return null;
        }
        CellScript chosenCell = nearTopCells[UnityEngine.Random.Range(0, nearTopCells.Count)];
        return chosenCell;

    }



}
