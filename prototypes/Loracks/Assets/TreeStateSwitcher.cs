using System.Collections.Generic;
using UnityEngine;

public class TreeStateSwitcher : MonoBehaviour
{
    List<GameObject> treeModels = new List<GameObject>();

    [SerializeField]
    GameObject treeState5;
    [SerializeField]
    GameObject treeState4;
    [SerializeField]
    GameObject treeState3;
    [SerializeField]
    GameObject treeState2;
    [SerializeField]
    GameObject treeState1;

    int currentIndex;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        treeModels.Add(treeState1);

        treeModels.Add(treeState2);
        
        treeModels.Add(treeState3);
        
        treeModels.Add(treeState4);
        
        treeModels.Add(treeState5);
        
    }



    public void setTreeVisual(float level) {
        if (level > 0)
        {
            float levelIndex = (level - 1) / 20;
            levelIndex = (int)(levelIndex);
            currentIndex = (int) levelIndex;
            for (int i = 0; i < treeModels.Count; i++)
            {
                treeModels[i].SetActive(i == levelIndex);
            }
        }
        else if (level == -1) {
            currentIndex = -1;
            for (int i = 0; i < treeModels.Count; i++)
            {
                treeModels[i].SetActive(false);
            }
        }

    }

    public void temporarlyTurnOff() {
        for (int i = 0; i < treeModels.Count; i++)
        {
            Debug.Log("TURNED OFF");
            treeModels[i].SetActive(false);
        }
    }

    public void temporarlyTurnOn() {
        for (int i = 0; i < treeModels.Count; i++)
        {
            
            treeModels[i].SetActive(i == currentIndex);
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
