using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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
    bool puttingDownFactory = true;

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
            //SimulationStep();
            nextSimulationStepTimer = nextSimulationStepRate;
        }

        // Handle mouse hover detection
        bool puttingDownAnything = puttingDownFactory /* or any other ones of the same bool */;
        if (!puttingDownAnything) {
            mouseHoverDetection();
        }
        if (puttingDownFactory) {
            placeStructure("factory", 1, 2);
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
                    handlePlacements(structName);
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                //currentHoverCell.RightClicked();
            }
        }
    }

    void handlePlacements(string structName) {
        if (structName == "factory") {
            placeFactory();
            //add to a list of game objects too
        } //else every other structure i wanna add



    }

    void placeFactory() { 
        
    }

    void setOccupy(List<CellScript> cells, bool occupied) {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].occupied = occupied;
        }
    }

    bool isClear(List<CellScript> cells)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].occupied) {
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
                currentHoverCell.Clicked();
            }
            if (Input.GetMouseButtonDown(1)) {
                currentHoverCell.RightClicked();
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
        for (int x = 0; x < gridW; x++) {
            for (int y = 0; y < gridH; y++) {
                nextState[x,y] = grid[x,y].GenereateNextSimulationStep();
            }
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

                // Initialize cell state with Perlin noise for height variation
                cs.State.height = 1f;
                cs.State.x = x;
                cs.State.y = y;
                
                // Set cell size and parent
                cell.transform.localScale = new Vector3(cellWidth, 1, cellHeight);
                cell.transform.SetParent(transform);
                
                // Store reference in the grid array
                grid[x,y] = cell.GetComponent<CellScript>();
            }
        }
    }
}
