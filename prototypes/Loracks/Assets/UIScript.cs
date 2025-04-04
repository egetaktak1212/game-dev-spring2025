using UnityEngine;
using TMPro;

public class UIScript : MonoBehaviour
{

    public TextMeshProUGUI WoodCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        WoodCount.SetText(getFormattedWoodCount());
    }

    string getFormattedWoodCount() {
        float totalWood = GridManager.Instance.getTotalWoodHarvested();

        totalWood = Mathf.Clamp(Mathf.RoundToInt(totalWood), 0, 99999);
        

        return totalWood.ToString();
    }

}
