using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using TreeEditor;
using Unity.VisualScripting;


// Manages the grid of cells for the simulation.
// Handles grid creation, cell updates, and provides utility functions for cell access.
public class GridManager : MonoBehaviour
{
    // Singleton instance for easy access from other scripts
    public static GridManager Instance { get; private set; }

    // Reference to the prefab used to create each cell
    [SerializeField]
    GameObject cellPrefab;

    // UI text element to display grid coordinates
    [SerializeField]
    TMP_Text indeceseText;

    // Grid dimensions
    [SerializeField]
    int gridW = 10;
    [SerializeField]
    int gridH = 5;

    [SerializeField]
    GameObject factoryPrefab;

    // Materials for visual feedback when hovering over cells
    [SerializeField]
    Material hoverMaterial;
    [SerializeField]
    Material defaultMaterial;
    [SerializeField]
    Material hoveringClearMaterial;
    [SerializeField]
    Material hoveringOccupiedMaterial;

    // Cell size parameters
    float cellWidth = 1;
    float cellHeight = 1;
    float spacing = 0.0f;

    // Maximum height value for cells (used for normalization)
    public float maxHeight = 5;

    // Timing control for simulation updates
    float nextSimulationStepTimer = 0;
    float nextSimulationStepRate = 0.25f;

    // Are we trying to put down an object
    bool puttingDownFactory = false;

    List<GameObject> factoryList = new List<GameObject>();
    /*
    List<GameObject> homeList = new List<GameObject>(); 
    List<GameObject> beekeeperList = new List<GameObject>(); 
    */


    // The 2D array that stores all cell references
    public CellScript[,] grid;
    
    // Tracks which cell the mouse is currently hovering over
    public CellScript currentHoverCell;

    // Initializes the singleton instance
    private void Awake()
    {
        // Standard singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Prevent duplicate instances
            return;
        }

        Instance = this;
    }

    // Cleans up the singleton reference when destroyed
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Called once when the script is enabled
    void Start()
    {
        GenereateGrid();
    }

    // Called every frame
    void Update()
    {
        // Handle simulation timing
        nextSimulationStepTimer -= Time.deltaTime;
        if (nextSimulationStepTimer < 0)
        {
            SimulationStep();
            nextSimulationStepTimer = nextSimulationStepRate;
        }

        if (Input.GetKeyDown(KeyCode.F)) {
            puttingDownFactory = !puttingDownFactory;
        }



        // Handle mouse hover detection
        bool puttingDownAnything = puttingDownFactory /* or any other ones of the same bool */;
        if (!puttingDownAnything)
        {
            //mouseHoverDetection();
            unhideTrees();
            cleanCellsAfterPlacingStructure();
        }
        else {
            hideTrees();
        }
        if (puttingDownFactory) {
            placeStructure("factory", 1, 2);
        }

    }


    void hideTrees() {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i,j].State.treeScript.temporarlyTurnOff();
            }
        }
    }

    void unhideTrees() {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].State.treeScript.temporarlyTurnOn();
            }
        }
    }

    void cleanCellsAfterPlacingStructure() {
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j].gameObject.GetComponentInChildren<Renderer>().material = defaultMaterial;
            }
        }
    }


    void placeStructure(string structName, int sizeX, int sizeY) {
        int structureX = sizeX;
        int structureY = sizeY;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool enoughSpace = true;
        if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("cell")))
        {
            // Get the cell that was hit
            CellScript cs = hit.collider.gameObject.GetComponentInParent<CellScript>();
            Vector2Int gridPosition = new Vector2Int(cs.State.x, cs.State.y);

            // Update UI with current position
            indeceseText.text = gridPosition.ToString();

            // Reset previous hover cell's material if we've moved to a new cell
            if (currentHoverCell != null && currentHoverCell != grid[gridPosition.x, gridPosition.y])
            {

                var tuple = getGridWithCenter(currentHoverCell.State.x, currentHoverCell.State.y, structureX, structureY);
                List<CellScript> surroundingCells = tuple.Item1;



                for (int i = 0; i < surroundingCells.Count; i++)
                {
                    surroundingCells[i].gameObject.GetComponentInChildren<Renderer>().material = defaultMaterial;
                }
            }

            // Update current hover cell and change its material
            currentHoverCell = grid[gridPosition.x, gridPosition.y];

            var tupleHover = getGridWithCenter(currentHoverCell.State.x, currentHoverCell.State.y, structureX, structureY);
            List<CellScript> surroundingCellsHover = tupleHover.Item1;
            enoughSpace = tupleHover.Item2;


            bool cellsAreClear = enoughSpace && isClear(surroundingCellsHover);

            for (int i = 0; i < surroundingCellsHover.Count; i++)
            {
                if (cellsAreClear)
                {
                    surroundingCellsHover[i].gameObject.GetComponentInChildren<Renderer>().material = hoveringClearMaterial;
                }
                else {
                    surroundingCellsHover[i].gameObject.GetComponentInChildren<Renderer>().material = hoveringOccupiedMaterial;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (cellsAreClear)
                {
                    handlePlacements(structName, currentHoverCell.State.x, currentHoverCell.State.y, surroundingCellsHover);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                //currentHoverCell.RightClicked();
            }
        }
    }

    void handlePlacements(string structName, int x, int y, List<CellScript> grid) {
        if (structName == "factory") {
            placeFactory(x, y, grid);
            //add to a list of game objects too
        } //else every other structure i wanna add

        for (int i = 0; i < grid.Count; i++)
        {
            grid[i].State.occupied = true;
            grid[i].State.treeState = -1;
        }
        unhideTrees();

    }

    void placeFactory(int x, int y, List<CellScript> grid) {
        Vector3 pos = new Vector3(x, 1, y);
        GameObject factory = Instantiate(factoryPrefab, pos, Quaternion.identity);
        factoryList.Add(factory);

        //done putting down factory
        puttingDownFactory = false;


        //set the local variables of the factory
        FactoryScript factoryScript = factory.GetComponent<FactoryScript>();
        factoryScript.setGrid(grid);
        factoryScript.centerX = x;
        factoryScript.centerY = y;

    }

    void setOccupy(List<CellScript> cells, bool occupied) {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].State.occupied = occupied;
        }
    }

    bool isClear(List<CellScript> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].State.occupied) {
                return false;
            }
        }
        return true;

    }


    (List<CellScript>, bool) getGridWithCenter(int centerX, int centerY, int rangeX, int rangeY) {
        // identical to get grid but I need the middle element as well
        List<CellScript> cellStates = new List<CellScript>();
        bool enoughSpace = true;
        // Loop through all cells in the rectangular range around the center cell
        for (int x = centerX - rangeX; x <= centerX + rangeX; x++)
        {
            for (int y = centerY - rangeY; y <= centerY + rangeY; y++)
            {
                // Skip cells that are outside the grid boundaries
                if (x < 0 || x >= gridW || y < 0 || y >= gridH)
                {
                    if (enoughSpace) {
                        enoughSpace = false;
                    }
                    continue;
                }

                // Add the neighbor's state to our collection
                cellStates.Add(grid[x, y]);
            }
        }

        return (cellStates, enoughSpace);
    }


    void mouseHoverDetection() { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask("cell"))) {
            // Get the cell that was hit
            CellScript cs = hit.collider.gameObject.GetComponentInParent<CellScript>();
            Vector2Int gridPosition = new Vector2Int(cs.State.x, cs.State.y);
            
            // Update UI with current position
            indeceseText.text = gridPosition.ToString();
            
            // Reset previous hover cell's material if we've moved to a new cell
            if (currentHoverCell != null && currentHoverCell != grid[gridPosition.x, gridPosition.y]) {
                currentHoverCell.gameObject.GetComponentInChildren<Renderer>().material = defaultMaterial;
                currentHoverCell.Unhover();
            }
            
            // Update current hover cell and change its material
            currentHoverCell = grid[gridPosition.x, gridPosition.y];
            currentHoverCell.Hover();

            if (Input.GetMouseButtonDown(0)) {
                //currentHoverCell.Clicked();
            }
            if (Input.GetMouseButtonDown(1)) {
                //currentHoverCell.RightClicked();
            }
        }
    }

    // Advances the simulation by one step
    void SimulationStep() 
    {
        // Calculate the next state for all cells
        // Store all of the updated cells in a new array so that we don't "contaminate" the cells
        // in state "time" with the cells in state "time + 1".
        CellState[,] nextState = new CellState[gridW, gridH];

        // make it a copy
        for (int x = 0; x < gridW; x++)
        {
            for (int y = 0; y < gridH; y++)
            {
                nextState[x, y] = grid[x, y].State.Clone();
            }
        }

        for (int x = 0; x < gridW; x++)
        {
            for (int y = 0; y < gridH; y++)
            {
                nextState[x, y] = grid[x, y].GenereateNextSimulationStep();
            }
        }

        for (int i = 0; i < factoryList.Count; i++) {
            factoryList[i].GetComponent<FactoryScript>().handleSimulation(nextState);
        }



        // Apply the new states (now that we are done updating all the cells)
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                grid[x,y].State = nextState[x,y];
            }
        }
    }

    // Gets a cell state with wrapping at grid boundaries
    public CellState GetCellStateByIndexWithWrap(int x, int y) {
        // Wrap coordinates to stay within grid bounds
        x = (x + gridW) % gridW;
        y = (y + gridH) % gridH;
        return grid[x,y].State;
    }

    // Returns null if it is out of bounds
    public CellState GetCellStateByIndex(int x, int y) {
        if (x < gridW && x >= 0 && y < gridH && y >= 0) {
            return grid[x,y].State;
        }
        return null;
    }

    public List<CellState> GetBorderingCellsByOffset(int centerX, int centerY, int length, int width,  int offset, CellState[,] grid) {
        List<CellState> borderingCells = new List<CellState>();

        int leftX = centerX - length - offset;
        int rightX = centerX + length + offset;

        int upY = centerY + width + offset;
        int downY = centerY - width - offset;
        for (int i = leftX+1; i < rightX; i++) {
            if (!(i < 0 || i >= gridW || upY < 0 || upY >= gridH))
            {
                borderingCells.Add(grid[i, upY]);
            }
            if (!(i < 0 || i >= gridW || downY < 0 || downY >= gridH))
            {
                borderingCells.Add(grid[i, downY]);
            }      
        }
        for (int i = downY; i <= upY; i++) {
            if (!(leftX < 0 || leftX >= gridW || i < 0 || i >= gridH))
            {
                borderingCells.Add(grid[leftX, i]);
            }
            if (!(rightX < 0 || rightX >= gridW || i < 0 || i >= gridH))
            {
                borderingCells.Add(grid[rightX, i]);
            }

        }


        return borderingCells;
    }

    public List<CellScript> GetBorderingCellsByOffsetCellScript(int centerX, int centerY, int length, int width, int offset, CellScript[,] grid)
    {
        List<CellScript> borderingCells = new List<CellScript>();

        int leftX = centerX - length - offset;
        int rightX = centerX + length + offset;

        int upY = centerY + width + offset;
        int downY = centerY - width - offset;
        for (int i = leftX + 1; i < rightX; i++)
        {
            if (!(i < 0 || i >= gridW || upY < 0 || upY >= gridH))
            {
                borderingCells.Add(grid[i, upY]);
            }
            if (!(i < 0 || i >= gridW || downY < 0 || downY >= gridH))
            {
                borderingCells.Add(grid[i, downY]);
            }
        }
        for (int i = downY; i <= upY; i++)
        {
            if (!(leftX < 0 || leftX >= gridW || i < 0 || i >= gridH))
            {
                borderingCells.Add(grid[leftX, i]);
            }
            if (!(rightX < 0 || rightX >= gridW || i < 0 || i >= gridH))
            {
                borderingCells.Add(grid[rightX, i]);
            }

        }


        return borderingCells;
    }




    // Gets all cell states within a specified range around a center cell.
    public List<CellState> GetCellStatesInRange(int centerX, int centerY, int rangeX, int rangeY) {
        List<CellState> cellStates = new List<CellState>();
        
        // Loop through all cells in the rectangular range around the center cell
        for (int x = centerX - rangeX; x <= centerX + rangeX; x++) {
            for (int y = centerY - rangeY; y <= centerY + rangeY; y++) {
                // Skip cells that are outside the grid boundaries
                if (x < 0 || x >= gridW || y < 0 || y >= gridH) {
                    continue;
                }
                
                // Skip the center cell since we only want neighbors
                if (x == centerX && y == centerY) continue;
                
                // Add the neighbor's state to our collection
                cellStates.Add(grid[x, y].State);
            }
        }
        
        return cellStates;
    }

    // Converts a world position to grid indices
    Vector2Int WorldPointToGridIndices(Vector3 worldPoint) {
        Vector2Int gridPosition = new Vector2Int();
        gridPosition.x = Mathf.FloorToInt(worldPoint.x / (cellWidth + spacing));
        gridPosition.y = Mathf.FloorToInt(worldPoint.z / (cellHeight + spacing));
        return gridPosition;
    }

    // Creates the grid of cells
    public void GenereateGrid() {
        // Clear any existing cells
        for (int i = transform.childCount-1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        // Initialize the grid array
        grid = new CellScript[gridW, gridH];
        
        // Create each cell in the grid
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                // Calculate position based on cell size and spacing
                Vector3 pos = new Vector3((cellWidth+spacing) * x, 0, (cellHeight+spacing) * y);
                
                // Instantiate the cell and get its script component
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
                CellScript cs = cell.GetComponent<CellScript>();

                // Make the tree start at 100




                
                cs.State.height = 1f;
                cs.State.x = x;
                cs.State.y = y;
                

                // Set cell size and parent
                cell.transform.localScale = new Vector3(cellWidth, 1, cellHeight);
                cell.transform.SetParent(transform);


                int treeStateNumber = (int)(Mathf.PerlinNoise((Time.time + x) / 25f, (Time.time + y) / 25f) * 200f);
                treeStateNumber = Mathf.Clamp(treeStateNumber, 0, 100);
                if (treeStateNumber < 50) {
                    treeStateNumber = -1;
                }


                cs.State.treeState = treeStateNumber;

                // Store reference in the grid array
                grid[x,y] = cell.GetComponent<CellScript>();
            }
        }
    }
}
