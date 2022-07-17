using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Grid; 
using static Demon;
using static Player;
using static HeadQuarter;
using UnityEngine.SceneManagement; 

using TMPro;

public class Main : MonoBehaviour
{   
    public GameObject gridHolder;
    public Grid actualGrid;
    public float difficultyFactor = 0.5f;
    public int gridHorizontal = 30;
    public int gridVertical = 5;
    public float gridMinX = -3.0f;
    public float gridMinY = -1.0f;
    public float gridHorizontalStep = 1.5f;
    public float gridVerticalStep = 1.5f;
    public GameObject tilePrefab;

    public GameObject textKilled;
    public List<Demon> playerDemons;
    public List<Demon> enemyDemons;
    public List<Demon> toScavenge;

    public Player playerController;

    public HeadQuarter playerHQ;
    public HeadQuarter enemyHQ;

    public Jukebox jukeBox;
    public RarenessConfig rarenessConfig;
    public TextMeshProUGUI pausedText;

    public bool gameIsOnPause = false;
    public bool gameIsOver = false;
    void Awake()
    {
        actualGrid = new Grid(gridHorizontal, gridVertical,gridMinX, gridMinY, gridHorizontalStep, gridVerticalStep, tilePrefab, gridHolder);
        playerHQ = GetComponent<HeadQuarter>();
        gameIsOnPause = false;
        gameIsOver = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P)){
            gameIsOnPause = !gameIsOnPause;
            if (gameIsOver){
                //change scene
                Scene scene = SceneManager.GetActiveScene(); 
                SceneManager.LoadScene(scene.name);
                return;
            }
            if(gameIsOnPause){
                jukeBox.togglePitch();
                pausedText.text = "Game Paused";
            }else{
                jukeBox.togglePitch();
                pausedText.text = "";
            }
        }
    }

    public void setGameOver(bool playerWon){
        gameIsOnPause = true;
        gameIsOver = true;
        if (playerWon){
            pausedText.text = "Congratulations!\nYou succesfully cast the demon killer ray.";
        }else{
            pausedText.text = "Game Over!\nYou run out of souls.";
        }
        pausedText.text += "\n\nPress Space to restart.";
    }
}
