using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GM : MonoBehaviour
{
    public static GM instance;

    public static Action goBigMode;

    public int lives = 3;

    public List<BrickExplode> BrickList = new List<BrickExplode>();

    public GameObject WinPanel;

    public GameObject LosePanel;

    float timer = 0f;
    bool countingTimer;

    public GameObject StartPanel;

    public GameObject TimerText;


    // Start is called before the first frame update
    private void OnEnable()
    {


        if (GM.instance == null)
        {
            GM.instance = this;
        }
        else
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        Time.timeScale = 0f;
        StartPanel.SetActive(true);
        TimerText.SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {
        if (countingTimer)
        {
            timer += Time.deltaTime;
        }

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        TimerText.GetComponent<TextMeshProUGUI>().text = time;
    }

    public void bigMode() { 
        goBigMode.Invoke();
    }

    public void didWeWin() {
        if (BrickList.Count == 0) {
            Win();
        }
    }

    public void Win() {
        countingTimer = false;
        WinPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log(timer);

   
    }

    public void Lose(string cause) {
        countingTimer = false;
        LosePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        //cause is if i want to print it on the screen in the game over screen
        Debug.Log(timer);
    }

    public void restartScene() {
        //clear static variables

        BrickList.Clear();


        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void gameStart() {
        StartPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1.0f;
        timer = 0f;
        countingTimer = true;
        TimerText.SetActive(true);

    }



}
