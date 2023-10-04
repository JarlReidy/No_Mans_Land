using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//----------------------------------------------------------------------------
//This script is responsible for all of the scenes built into the project. 
//----------------------------------------------------------------------------
public class Scene_Manager : MonoBehaviour
{
    public GameObject fadeSquare;//black square to be faded on the canvas.
    public Animator fadeAnimator;
    // Start is called before the first frame update
    public void EnterBattleground()
    {
        LevelChanger();
        SceneManager.LoadScene(1);
     
    }
    //Enter Block Out Level
    public void EnterBlockOut()
    {
        LevelChanger();   
        SceneManager.LoadScene(2);  
        
    }
    //Enter Controls Scene
    public void Controls()
    {
        SceneManager.LoadScene(3);
    }
    //Quit the Game
    public void LeaveBattleground()
    {
        Application.Quit();
    }
    //Enum Fade which lerps between alpha values of black
    //plays in reverse when opening the scene
    public void LevelChanger()
    {
        fadeAnimator.SetTrigger("fadeOut");
    }
    public IEnumerator Fade(bool fade = true, int fadeRate = 4)
    {
        Color squareColor = fadeSquare.GetComponent<SpriteRenderer>().color;//getting the color component of 2D square
        float fadeSum;

        if(fade)//if the square is fading
        {
            while(fadeSquare.GetComponent<SpriteRenderer>().color.a < 1)//while the alpha value is less than 1
            {
                fadeSum = squareColor.a + (fadeRate * Time.deltaTime);//slowly increase the alpha value over time

                squareColor = new Color(squareColor.r, squareColor.g, squareColor.b, fadeSum);//multiplying rgb values by a float value
                fadeSquare.GetComponent<SpriteRenderer>().color = squareColor;//setting the square's current color to the squareColor variable
                yield return null;
            }
        }
        else
        {
            //as above but in reverse, going from alpha value 0 -> 1
            while(fadeSquare.GetComponent<SpriteRenderer>().color.a > 0)
            {
                fadeSum = squareColor.a - (fadeRate * Time.deltaTime);

                squareColor = new Color(squareColor.r,squareColor.g, squareColor.b, fadeSum);
                fadeSquare.GetComponent<SpriteRenderer>().color = squareColor;
                yield return null;
            }
        }

        //yield return new WaitForEndOfFrame();
    }
}
