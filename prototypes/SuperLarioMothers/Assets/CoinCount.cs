using TMPro;
using UnityEngine;

public class CoinCount : MonoBehaviour
{
    public TMP_Text text;
    public PlayerControls player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    { 
        text.text = player.coins.ToString();
    }
}
