using UnityEngine;
using System.Collections.Generic;

// Represents a single cell in the simulation grid
// Handles the cell's state and visual representation
public class CellScript : MonoBehaviour
{
    // References to visual components
    [SerializeField] GameObject selectionPlane;
    [SerializeField] GameObject heightCube;
    private Material heightCubeMaterial;

    // Cell state with property to update visuals when changed
    private CellState _state = new CellState();

    // Is the cell occupied by smth
    public bool occupied = false;


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
        // Cache the material for performance and initialize visuals
        heightCubeMaterial = heightCube.GetComponentInChildren<Renderer>().material;
        UpdateVisuals();
    }

    void Update()
    {
        
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
    public CellState GenereateNextSimulationStep()
    {
        // Create a copy of the current state to modify
        CellState nextState = this.State.Clone();
        // This is just an example
        ApplyMountainSmoothing(nextState);

        return nextState;
    }

    void ApplyMountainSmoothing(CellState cellState) {
        // Get all neighboring cells (excluding the current cell)
        List<CellState> neighborStates = GridManager.Instance.GetCellStatesInRange(State.x,State.y,1,1);
        
        // Calculate the average height of all neighboring cells
        float totalHeight = 0;
        foreach (CellState neighborState in neighborStates) {
            totalHeight += neighborState.height;
        }
        
        // Set the next height to be the average of all neighbors
        // This creates a smoothing/diffusion effect across the grid
        cellState.height = totalHeight / neighborStates.Count;
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
