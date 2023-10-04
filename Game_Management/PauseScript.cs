using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

//------------------------------------------------------------------------------------------------------------------------------
//This will be used to let the player pause the game and freeze the game state, accomplished by using Time.timescale
// where 1 = playing at normal speed, 0 = frozen. 
//------------------------------------------------------------------------------------------------------------------------------
public class PauseScript : MonoBehaviour
{
    public Game_Manager manager;//referencing the gameState script
    public bool isPaused;//is the game paused?
    public bool isRunning;//is the game running?

    [Header("Component Systems :")]
    [SerializeField] private GameObject canvasUI;//the game canvas
    [SerializeField] private GameObject pauseUI;//the pause canvas
    [SerializeField] private GameObject environment;//the environment
    [SerializeField] private GameObject managerObject;//the game manager
    [SerializeField] private VolumeProfile gVolume;//getting the volume


    // Start is called before the first frame update
    void Start()
    {
        isRunning = true;
        canvasUI.SetActive(true);
        pauseUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//if escape is pressed
        {
            if(isRunning)//is the game running?
            {
                PauseGame();
            }
            else
            {
               ResumeGame();
            }
        }
    }
    //pausing the game, setting the current time scale to 0 (freezing it)
     public void PauseGame()
    {
        pauseUI.SetActive(true);//setting the pause menu panel to active
        canvasUI.SetActive(false);//setting the game menu panel to inactive
        environment.SetActive(false);
        managerObject.SetActive(false);
        isRunning=false;//game is not running
        Time.timeScale = 0f;//setting the time scale to 0 so the game state is frozen
        
    }
    //disable the pause menu and return the time-state back to 1
     public void ResumeGame()
    {
        pauseUI.SetActive(false);
        canvasUI.SetActive(true);
        environment.SetActive(true);
        managerObject.SetActive(true);
        isRunning = true;
        Time.timeScale = 1;
    }
    //Opening the main menu
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    //Leave the game
    public void Leave()
    {
        Application.Quit();
    }
}
