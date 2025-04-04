using UnityEngine;
using System.Collections.Generic;

// Represents a single cell in the simulation grid
// Handles the cell's state and visual representation
public class CellScript : MonoBehaviour
{
    // References to visual components
    [SerializeField] GameObject selectionPlane;
    [SerializeField] GameObject heightCube;
    [SerializeField] TreeStateSwitcher treeScript;
    private Material heightCubeMaterial;

    // Cell state with property to update visuals when changed
    private CellState _state = new CellState();


    float growthSpeed = 500000f;

    bool debugBool = true;


    private void OnEnable()
    {
        _state.treeScript = treeScript;

    }


    public CellState State
    {
        get
        {
            return _state;
        }
        set
        {
            _state = value;
            UpdateVisuals();
        }
    }

    void Start()
    {

        if (State.x == 1 && State.y == 1)
        {
            debugBool = true;
        }
        // Cache the material for performance and initialize visuals
        heightCubeMaterial = heightCube.GetComponentInChildren<Renderer>().material;
        UpdateVisuals();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            State.debugTreeSwitcherUp();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            State.debugTreeSwitcherDown();
        }
    }

    public void Hover() {
        selectionPlane.SetActive(true);
        // Update the selection plane's position to match the state's height
        float height = transform.position.y + State.height + 0.1f;
        selectionPlane.transform.position = new Vector3(selectionPlane.transform.position.x, height, selectionPlane.transform.position.z);
    }

    public void Unhover() {
        selectionPlane.SetActive(false);
    }

    public void Clicked() {
        State.height += 5;
        State.height = Mathf.Clamp(State.height, 0, GridManager.Instance.maxHeight);
        UpdateVisuals();
    }

    public void RightClicked() {
        State.height -= 5;
        State.height = Mathf.Clamp(State.height, 0, GridManager.Instance.maxHeight);
        UpdateVisuals();
    }   

    // Calculates the next state of this cell for the simulation
    public CellState GenereateNextSimulationStep(CellState nextState)
    {
        ApplyTreeGrowth(nextState);

        return nextState;
        
    }


    void ApplyTreeGrowth(CellState cellState) {

        bool weCantGrow = State.occupied || State.being_harvested;

        if (!weCantGrow)
        {
            // Get all neighboring cells (excluding the current cell)
            List<CellState> neighborStates = GridManager.Instance.GetCellStatesInRange(State.x, State.y, 1, 1);

            float amountToGrow = 0;

            for (int i = 0; i < neighborStates.Count; i++)
            {
                if (neighborStates[i].treeState > -1)
                {
                    amountToGrow += neighborStates[i].treeState * 0.05f;
                }
            }

            if (debugBool)
            {
                //Debug.Log("Amount to grow before mult = " + amountToGrow);
                amountToGrow *= cellState.treeState / growthSpeed;
                //Debug.Log("Amount to grow after mult = " + amountToGrow);
                //Debug.Log("Treestate before = " + cellState.treeState);
                float temp = cellState.treeState;
                cellState.treeState = temp + amountToGrow;
                if (temp + amountToGrow == -1) {
                    //Debug.Log("-1 being passed here");
                }
                //Debug.Log("Treestate after = " + cellState.treeState);
            }

        }
        
    }

    // Updates the visual representation of the cell based on its state
    void UpdateVisuals()
    {
        // Adjust the height cube to match the cell's height value
        heightCube.transform.localScale = new Vector3(1, State.height, 1);
        
        // Update the material with normalized height value (0-1 range)
        // This controls the color based on height (based on the material shader - see the CellShader for more details, or ignore this if this is new to you)
        heightCubeMaterial.SetFloat("_height", State.height/GridManager.Instance.maxHeight);
    }
}
