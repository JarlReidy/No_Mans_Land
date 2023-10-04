using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;


//-------------------------------------------------------------------------------------------------------------
//This is the Game_Manager Class
//It's responsible for containing and controlling the tile board itself and all of the interactions within it
//1) How the user can see and navigate the board by using different Navigation Layers 
//2) How the user can interact with units on their turn
//3) How the enemy units respond to the friendly turn
//4) How the winning conditions (defined in the beginning) can be used to influence the game narrative
//-------------------------------------------------------------------------------------------------------------
public class Game_Manager : MonoBehaviour
{
    //constants
    [Header(" Component Systems :")]
    private TroopScript[,] troopUnits;
    public static List<TroopScript> friendlyTroopList = new List<TroopScript>();
    public static List<TroopScript> enemyTroopList = new List<TroopScript>();
    public TroopScript[] enemytroopUnits;//the enemy forces
    private TroopScript movingUnit;//currently selected unit
    public TroopScript troopMisc;//capturing the supplies counter
    private SupplyScript[,] supplies;//supply boxes
    public SupplyScript munitions;//currently selected supply
    private const int tileCountX = 18;//number of tiles on x axis
    private const int tileCountY = 12;//number of tiles on y axis
    //private bool hovering = false;

    private GameObject[,] tiles;//2d array for tiles
    private Camera warCam;//warCamera used for raycast
    private Vector2Int currentHover;//current [x,y] val on the grid of mouse hovering 
    private Vector3 bounds;//bounds for the board
    private List<Vector2Int> ableMoves = new List<Vector2Int>();//a 2d list of tiles which each piece can move to
    private List<TroopScript> deadFriendlies = new List<TroopScript>();//a list containing the dead of the friendly team
    private List<TroopScript> deadEnemies = new List<TroopScript>();//a list containing the dead of the enemy team
    public TimerScript timescript;//timerscript reference

    //Game States and Movements
    [Header("Player and Enemy Statistics :")]
    public GameState state;//referencing the game state to control the workflow
    public bool playerTurn;//is it the player turn?
    public bool enemyTurn;//is it the enemy turn?
    public int movesMade = 0;//one move per turn
    public int enemyMovesMade = 0;
    public int turnsTaken = 0;//number of turns taken (will be used to stun enemies)
    public int enemyturnsTaken = 0;
    public int friendlyStrike = 0;//number of artillery hits
    public int enemyStriker = 0;
    public int friendlySupplies = 0;//friendly supplies
    public int enemySupplies = 0;//enemy supplies
    public TimerScript timekeeper;//the timerscript class
    public int currentindex = 0;//randomly chosen unit
    //public int[] enemyunits;
    public bool hasSelected;//currently selected unit?
    

    [Header("Units :")]
    //[SerializeField] private Mesh tileMesh;
    [SerializeField] private GameObject[] troops;//the prefabs of the friendly troops
    [SerializeField] private Material troopMats;//materials for the friendly troops
    
    [SerializeField] private GameObject[] enemytroops;//enemy_troops
    [SerializeField] private Material enemyMats;//materials for the enemy_troops

    [Header("Animator :")]
    [SerializeField] private Animator troopAnimator;//the animator which controls all of the troops

    [Header("World :")]
    [SerializeField] private GameObject supplybox;//the supply box
    [SerializeField] private GameObject explosion;//the explosion prefab used to show the stike area of the artillery
    [SerializeField] private float tileSize = 1.0f;//size of tile x*y
    [SerializeField] private float yOffset = 0.2f;// +y 
    [SerializeField] private Vector3 boardCenter = Vector3.zero;//where is the board pivoted around

    [Header("World Materials :")]
    [SerializeField] private Material tileMaterial;//base material for the tiles
    [SerializeField] private Material hoverMaterial;//slightly lighter material for when you hover over the tile
    [SerializeField] private Material availableMaterial;//material which demonstrates the available moves
    [SerializeField] private Material selectedMaterial;//for left clicking a unit, to select it


    [Header("Sound FX :")]
    [SerializeField] public AudioClip footsteps;//audioclip played while troop is currently moving
    [SerializeField] public AudioClip landingShell;//the audioclip played when the artillery shell lands
    [SerializeField] public AudioSource whistle;//whistle which signifies the beginning of the game
    //[SerializeField] public AudioSource timer;//the timer sfx played whenever it's the players' turn
    [SerializeField] public AudioClip yesSir;//for reinforcements
    [SerializeField] public AudioClip dismount;//for enemy reinforcements
    [SerializeField] public AudioClip fScream;//death noise for friendlies
    [SerializeField] public AudioClip eScream;//death noise for enemies

    [Header("UI :")]
    [SerializeField] private GameObject startButton;//button which starts off the engagement
    [SerializeField] private float counter = 23;
    [SerializeField] private bool hasStartedGame;//has the game started?
    public TextMeshProUGUI friendlySuppliesText;
    public TextMeshProUGUI enemySuppliesText;
    public TextMeshProUGUI supplyText;
    public TextMeshProUGUI movesLeft;

    [Header("Winning Game UI")]
    //friendlies
    public GameObject matchWin;
    public GameObject matchLoss;
    public bool isFinished;
    public TextMeshProUGUI friendlyTurns;
    public TextMeshProUGUI friendlyShells;
    public TextMeshProUGUI friendlyLosses;
    //
    public TextMeshProUGUI enemyTurns;
    public TextMeshProUGUI enemyShells;
    public TextMeshProUGUI enemyLosses;
    //
    [Header("Losing Game UI")]
    public TextMeshProUGUI friendlyTurnsLoss;
    public TextMeshProUGUI friendlyShellsLoss;
    public TextMeshProUGUI friendlyLossesLoss;
    //
    public TextMeshProUGUI enemyTurnsLoss;
    public TextMeshProUGUI enemyShellsLoss;
    public TextMeshProUGUI enemyLossesLoss;
    


    [SerializeField] private GameObject canvasUI;//the game canvas
    [SerializeField] private GameObject environment;//the environment
    [SerializeField] private GameObject managerObject;//the game manager

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //Base code which makes up the game loop.
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    #region Base Code
    //processed first
    private void Awake()
    {
        SpawnAllTiles(tileSize, tileCountX, tileCountY);//tile-board has constant values
        
        SpawnAllSupplies();

        state.currentState = GameState.Game_States.START;//it is the beginnning of the game
        hasSelected = false;//no units have been selected
        hasStartedGame = false;//game has not started yet
        matchWin.SetActive(false);
        matchLoss.SetActive(false);
       // munitions.friendlySupplies = friendlySupplies;
    }


    private void Update()
    {
        //Methods called to the main game loop
        MouseMachine();
        StateMachine();
        

        if (state.currentState == GameState.Game_States.PLAYER_TURN)
        {
            if (Input.GetKeyDown(KeyCode.R) && TroopScript.friendlySupplies >= 50)
            {
                ReInforce();
            }
        }

        if(state.currentState == GameState.Game_States.ENEMY_TURN)
        {
            if(TroopScript.enemySupplies >=50)
            {
                ReInforceEnemy();
            }
        }
        if (hasStartedGame)
        {
            if (Game_Manager.friendlyTroopList.Count == 0)
            {
                Invoke("Loss", 2f);
            }
            else if (Game_Manager.enemyTroopList.Count == 0)
            {
                Invoke("Victory", 2f);
            }
        }
        

        //if the game has started, the start button should not appear

        //Debug.Log(movingUnit.friendlySupplies);
        //Debug.Log(timescript.timer);
        // Debug.Log(friendlySupplies);

        //friendlySuppliesText.text = friendlySupplies.ToString();
        //enemySuppliesText.text = enemySupplies.ToString();

        friendlySuppliesText.text = TroopScript.friendlySupplies.ToString();
        enemySuppliesText.text = TroopScript.enemySupplies.ToString();
        movesLeft.text = "M O V E S   L E F T : " + (Game_Manager.friendlyTroopList.Count - movesMade);

        //winning game screen texts
        friendlyTurns.text = turnsTaken.ToString();
        friendlyShells.text = (friendlyStrike * 150).ToString();
        friendlyLosses.text = (deadFriendlies.Count * 100).ToString();
        enemyTurns.text = enemyturnsTaken.ToString();
        enemyShells.text = (enemyStriker * 150).ToString();
        enemyLosses.text = (deadEnemies.Count * 100).ToString();
        //Debug.Log(counter);
        friendlyTurnsLoss.text = turnsTaken.ToString();
        friendlyShellsLoss.text = (friendlyStrike * 150).ToString();
        friendlyLossesLoss.text = (deadFriendlies.Count * 100).ToString();
        enemyTurnsLoss.text = enemyturnsTaken.ToString();
        enemyShellsLoss.text = (enemyStriker * 150).ToString();
        enemyLossesLoss.text = (deadEnemies.Count * 100).ToString();


    }
    #endregion

    #region Game Manager


    //----------------------------------------------------------------------------------------------------
    //Mouse Manager Method
    //To navigate the board and control the pieces on it, the player must use the mouse
    //Left-Click for dragging units to available locations on the tile board
    //Right-Click for aiming the artillery 
    //---------------------------------------------------------------------------------------------------
    public void MouseMachine()
    {
        if (state.currentState == GameState.Game_States.PLAYER_TURN)//player turn?
        {
            playerTurn = true;
           // hasStartedGame = true;
           // timer.Play();
            if (!warCam)//if there is no camera
            {
                warCam = Camera.main;
                return;
            }
            RaycastHit ray;
            Ray rayCam = warCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(rayCam, out ray, 100, LayerMask.GetMask("Tile", "Hover", "Available")))
            {
                //---------------------------------------------------------------------------------------------------------------------------------------
                //Highlighting the Tiles we are currently hovering over
                // Get the indexes of the tile we hit with the ray we emit when we click
                //---------------------------------------------------------------------------------------------------------------------------------------
                Vector2Int hitPosition = GetIndex(ray.transform.gameObject);
                if (currentHover == new Vector2Int(-1, -1))
                {
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = hoverMaterial;
                }
                if (currentHover != new Vector2Int(-1, -1))
                {
                    //tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                    tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                    tiles[hitPosition.x, hitPosition.y].GetComponent<MeshRenderer>().material = hoverMaterial;
                }

                //---------------------------------------------------------------------------------------------------------------------------------------
                //---------------------------------------------------------------------------------------------------------------------------------------
                //Moving the individual Units 
                //-- Based on the transform.position of the currently selected unit, it will travel to the tile-center of commanded position using the MoveUnit function.
                //if press the move command 'left click mouse'
                //---------------------------------------------------------------------------------------------------------------------------------------
                //---------------------------------------------------------------------------------------------------------------------------------------

                if (Input.GetMouseButtonDown(0))
                {
                    //if there is a unit placed on the current tile clicked on via RayCast
                    if (troopUnits[hitPosition.x, hitPosition.y] != null)
                    {
                        tiles[hitPosition.x + 1, hitPosition.y].layer = LayerMask.NameToLayer("Available");
                        tiles[hitPosition.x + 1, hitPosition.y].GetComponent<MeshRenderer>().material = availableMaterial;
                        hasSelected = true;
                    }
                    //if mouse button is held down, a unit has been selected and moves are possible
                    if (Input.GetMouseButtonDown(0) && hasSelected && movesMade < friendlyTroopList.Count)
                    {
                        if (troopUnits[hitPosition.x, hitPosition.y] != null)//if the currently selected tile has a unit positioned on it
                        {
                            //Player turn?
                            if (true)
                            {
                                movingUnit = troopUnits[hitPosition.x, hitPosition.y];//currently selected unit 
                                                                                      //Getting a list of available moves which the troops can make
                                                                                      //highlighting available tiles
                                ableMoves = movingUnit.AvailableMoves(ref troopUnits, tileCountX, tileCountY);
                                tiles[hitPosition.x + 1, hitPosition.y].layer = LayerMask.NameToLayer("Available");
                                tiles[hitPosition.x + 1, hitPosition.y].GetComponent<MeshRenderer>().material = availableMaterial;
                                tiles[hitPosition.x, hitPosition.y + 1].layer = LayerMask.NameToLayer("Available");
                                tiles[hitPosition.x, hitPosition.y + 1].GetComponent<MeshRenderer>().material = availableMaterial;
                                TargetMove();
                            }
                        }
                    }
                }
                //if releasing the mouse button and there is a unit selected, and are able to move the unit
                if (movingUnit != null && movesMade < friendlyTroopList.Count && Input.GetMouseButtonUp(0))
                {
                    //if()
                    Vector2Int previousPosition = new Vector2Int(movingUnit.currentX, movingUnit.currentY);//previous position is the current value of the unit before it's updated in case move is invalid
                    bool canMove = MoveUnit(movingUnit, hitPosition.x, hitPosition.y);//uses movingUnit
                    troopAnimator.SetBool("isMoving", true);
                    AudioSource.PlayClipAtPoint(footsteps, movingUnit.transform.position, 1.0f);
                   
                    if(movingUnit.currentX == 17)
                    {
                        isFinished = true;
                        Invoke("Victory", 2f);
                    }
                    if (!canMove)//if cannot move
                    {
                        movingUnit.transform.position = Snap(previousPosition.x, previousPosition.y);//if the move is invalid, re-position the unit back to its original tile
                        TurnOffTargetMove();
                    }
                    
                    //resetting all tile materials back to normal
                    //this is so that after every move- visually the board returns to how it is meant to look
                    for (int i = 0; i < tileCountX; i++)
                    {
                        for (int u = 0; u < tileCountY; u++)
                        {
                            tiles[i, u].GetComponent<MeshRenderer>().material = tileMaterial;//resetting all of the tile materials
                        }
                    }
                    movingUnit = null;//unit has already moved so there is no current unit selected anymore
                    movesMade = movesMade + 1;//movesmade counter - once this reaches 5 it will call the StateMachine function
                    troopAnimator.SetBool("isMoving", false);

                }



                //------------------------------------------------------------------------------------------------------------------------------------------------
                //------------------------------------------------------------------------------------------------------------------------------------------------
                // Shooting the artillery
                // - Method of attacking the enemy
                // - Will use right-click to select a tile
                //------------------------------------------------------------------------------------------------------------------------------------------------
                //------------------------------------------------------------------------------------------------------------------------------------------------
               
                    if (Input.GetMouseButtonUp(1) && TroopScript.friendlySupplies >=50)
                    {
                        Instantiate(explosion, Snap(hitPosition.x, hitPosition.y), Quaternion.identity);//instantiating an instance of explosion at the strike position, with the a quaternion identity
                        AudioSource.PlayClipAtPoint(landingShell, transform.position, 10.0f);//play the landing shell sound effect
                        friendlyStrike = friendlyStrike + 1;//incrementing the strike counter
                        TroopScript.friendlySupplies -= 50;
                                                            //Checking if there is a unit in the strike position, works similar to checking if the unit is available to move() using the hitPosition value.
                        if (troopUnits[hitPosition.x, hitPosition.y] != null)
                        {
                            Destroy(troopUnits[hitPosition.x, hitPosition.y].gameObject);//destroy the troopUnits currently placed on that tile.
                            enemyTroopList.Remove(troopUnits[hitPosition.x, hitPosition.y]);//removing the troopUnits from the list
                            AudioSource.PlayClipAtPoint(eScream, transform.position, 1.0f);
                            deadEnemies.Add(troopUnits[hitPosition.x, hitPosition.y]);//adding enemies to dead list
                        }
                    }
                
            }
            else
            {
                if (currentHover != new Vector2Int(-1, -1))
                {
                    tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");// (InRange(ref canMove, currentHover)) ? LayerMask.NameToLayer("Tile") : LayerMask.NameToLayer("Available");
                    tiles[currentHover.x, currentHover.y].GetComponent<MeshRenderer>().material = tileMaterial;
                    currentHover = new Vector2Int(-1, -1);
                }
                //if selected an appropriate unit (friendly team) and released left mouse button
                if (movingUnit && Input.GetMouseButtonUp(0) && movingUnit.team == 0)
                {
                    movingUnit.transform.position = Snap(movingUnit.currentX, movingUnit.currentY);//if the move is invalid, re-position the unit back to its original tile
                    movingUnit = null;

                }
                
            }



            //Switched off as it doesn't look good with the chosen camera angle for the game 
            //Hovering Mechanic for when you select a unit..
            /*
            if (movingUnit && state.currentState==GameState.Game_States.PLAYER_TURN)
            {
                Plane movingPlane = new Plane(Vector3.up, Vector3.up * yOffset);//places directly on top of the board
                float distance = 0.0f;
                if (movingPlane.Raycast(rayCam, out distance))
                {
                    movingUnit.SetPosition(rayCam.GetPoint(distance) + Vector3.up * 0.5f);//getting the exact point of the raycast

                }
            }*/
        }


        if (state.currentState == GameState.Game_States.ENEMY_TURN)
        {
            enemyTurn = true;
            hasStartedGame = true;
            counter -= Time.deltaTime;
            foreach (TroopScript i in enemyTroopList)
            {
                movingUnit = i;
                bool canMove = MoveUnit(movingUnit, movingUnit.currentX - 1, movingUnit.currentY);
                enemyMovesMade = enemyMovesMade + 1;
                if(!canMove)
                {
                    movingUnit.transform.position = Snap(movingUnit.currentX, movingUnit.currentY);//re-position to the center of the current tile if the move isn't valid
                }

            }
            if (movingUnit.currentX == 0)
            {
                Invoke("Loss", 2f);
            }
            if (enemyMovesMade == enemyTroopList.Count)
            {
                bool hasMoved = true;
                movingUnit = null;
                Invoke("EnemyStrike", 2f);
            }
            

            //Animation Controller
            if (movingUnit)
            {
                troopAnimator.SetBool("isMoving", true);
            }
            else if (!movingUnit)
            {
                troopAnimator.SetBool("isMoving", false);
            }

        }
    }
    
    public void StartPressed()
    {
        state.currentState = GameState.Game_States.PLAYER_TURN;
        hasStartedGame = true;
        whistle.Play();
        SpawnAll();//spawning all of the units, they spawn at set values within the tile board
        CommandAllUnits();
        Destroy(startButton);
        supplyText.gameObject.SetActive(true);
        
    }

    public void Victory()
    {
        matchWin.SetActive(true);
        environment.SetActive(false);
        managerObject.SetActive(false);
        canvasUI.SetActive(false);
        Time.timeScale = 0f;

    }

    public void Loss()
    {
        matchLoss.SetActive(true);
        environment.SetActive(false);
        managerObject.SetActive(false);
        canvasUI.SetActive(false);
        Time.timeScale = 0f;
    }

    //The State Machine dictates whether it is the players turn or the enemy turn
    //The player can choose to command whichever unit they like, but only have a capable number of turns equalling to 5
    //This is to encourage the player to commit to using certain units to carry out certain tasks
    public void StateMachine()
    {
        //if maximum moves made OR artillery shot OR timer runs out
        //this way, players can carry out incomplete moves and are punished for indecisiveness
        if ((friendlyTroopList.Count == movesMade) || timescript.timer ==0)
        {
            turnsTaken = turnsTaken + 1;//used to track number of turns taken during the match
            state.currentState = GameState.Game_States.ENEMY_TURN;//now it is the enemy turn
            movesMade = 0;//resetting the player movement
            //friendlyStrike = 0;//resetting striking counter
        }
        /*
        else if(timekeeper.timer <=0)
        {
            turnsTaken = turnsTaken + 1;//used to track number of turns taken during the match
            state.currentState = GameState.Game_States.ENEMY_TURN;//now it is the enemy turn
            movesMade = 0;//resetting the player movement 
        }
       */

        if ((enemyTroopList.Count == enemyMovesMade))
        {
            enemyturnsTaken = enemyturnsTaken + 1;
            movingUnit = null;//halts the units after they've taken their turn
            state.currentState = GameState.Game_States.PLAYER_TURN;
            enemyMovesMade = 0;
            timescript.timer = 20;

        }
    }
    
    private void EnemyStrike()
    {
        
        Vector2Int strikePos = new Vector2Int(UnityEngine.Random.Range(1, 6), UnityEngine.Random.Range(0, tileCountY));
        Instantiate(explosion, Snap(strikePos.x, strikePos.y), Quaternion.identity);
        AudioSource.PlayClipAtPoint(landingShell, transform.position, 10.0f);//play the landing shell sound effect
        if (troopUnits[strikePos.x, strikePos.y] != null)
        {
            Destroy(troopUnits[strikePos.x, strikePos.y].gameObject);//destroy the troopUnits currently placed on that tile.
            friendlyTroopList.Remove(troopUnits[strikePos.x, strikePos.y]);//removing the troopUnits from the list
            AudioSource.PlayClipAtPoint(fScream, transform.position, 1.0f);
            deadFriendlies.Add(troopUnits[strikePos.x, strikePos.y]);//adding friendlies dead to death toll
        }
        enemyStriker = enemyStriker + 1;

    }
    #endregion
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //Inspired by Epitome's Create an Online Chess Game Series available on YouTube - Episode 1: The Placement Grid
    //https://www.youtube.com/watch?v=FtGy7J8XD90
    //Also Inspired by Tarodev's Create a Grid in Unity - perfect for turn based strategy games!
    //https://www.youtube.com/watch?v=kkAjpQAM-jE
    //Generating a tile board which will blend into the environment. The tile board will be constructed using vertices and a 2 dimensional array, the transparent
    //quads will act as moving/ attacking markers in 3d space for the players and enemies to interact with.
    //The tile board is 18(x) * 12(y) to ensure that there is longevity to the encounter, whilst making the battlefield feel stretched, especially useful when challenging the player to make decisions.
    //----------------------------------------------------------------------------------------------------------------------------------------------------


    #region Tile - Board Manager

    //taking in 3 parameters:
    //size of the tile{1}, the number of tiles in x{2},y{3} axis

    private void SpawnAllTiles(float tileSize, int tileX, int tileY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;//where the units movements and attacks can take place

        tiles = new GameObject[tileCountX, tileCountY];//creating the tileboard
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = SpawnOneTile(tileSize, x, y);

            }
        }
    }

    private GameObject SpawnOneTile(float tileSize, int x, int y)
    {
        GameObject tile = new GameObject(string.Format("X:{0}, Y:{1}", x, y));//how the tiles will be formatted within the game
        tile.transform.parent = transform;//constraining the tileboard to the gameobjects transform

        Mesh mesh = new Mesh();//will store our tile
        //mesh = tileMesh;
        //assigning the mesh and material of each tile.
        tile.AddComponent<MeshFilter>().mesh = mesh;
        tile.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];//creating the 4 vertices of the tile
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;


        int[] triTiles = new int[] { 0, 1, 2, 1, 3, 2 };//creating triangles from the defined vertices

        mesh.vertices = vertices;
        mesh.triangles = triTiles;

        mesh.RecalculateNormals();//recalculating the normals for light source to prevent visual noise

        tile.AddComponent<BoxCollider>();//adding a box collider for the raycast
        tile.layer = LayerMask.NameToLayer("Tile");//when the play button is pressed, the tiles will be assigned to the 'Tile' layer

        return tile;
    }
    #endregion


    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //Spawning the elements of the game, split up into 2 separate methods: spawning a single element, and spawning all of the units for debugging purposes
    //Snap() method used to snap the units into the centre of each tile by doing tilewidth, height /2, allowing for consistency of movement and organisation within the game
    //Snap() method will also be used during artillery strikes
    //The troop types as determined in their own classes, will be assigned an index and a team. This will control how they interact in the game
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //Spawning the Supply Boxes, split into two separate methods: spawning a single box, and then spawning all of the boxes
    

    #region Spawning Manager

    //Spawning Troops

    //Re-inforcement Method
    //Works by collecting supplies in the tile-board, the player can spend them whenever they choose, whereas the enemy will take every opportunity to spend their supplies
    //for re-inforcements.
    //No new instance of TroopScript is required as the unit spawned will be in the same sub-system as the other units spawned in the beginning.
    //for friendly units
    private void ReInforce()
    {
        if (state.currentState == GameState.Game_States.PLAYER_TURN)
        {
            AudioSource.PlayClipAtPoint(yesSir, transform.position, 0.5f);
            int friendlyTeam = 0;//friendly team
            troopUnits[0, 4] = SpawnUnit(TroopType.Light_Infantry, friendlyTeam);//spawn at 0 = tileX, 4 = tileY
            TroopScript.friendlySupplies -= 50;//friendly supplies de-crement by 50 each time
            CommandAllUnits();//Command All Units so new units snap to the grid and are usable (within the game-state bounds)
            friendlyTroopList.Add(troopUnits[0, 4]);//adding to friendlyTroopList to record the number of units per team
        }
        
    }
    //for enemy units
    private void ReInforceEnemy()
    {
        if (state.currentState == GameState.Game_States.ENEMY_TURN)
        {
            AudioSource.PlayClipAtPoint(dismount, transform.position, 0.5f);
            int enemyTeam = 0;
            troopUnits[17, 4] = SpawnEnemyUnit(EnemyTroopType.Light_Infantry, enemyTeam);
            TroopScript.enemySupplies -= 50;
            CommandAllUnits();
            enemyTroopList.Add(troopUnits[17, 4]);//adding the reinforcements to the enemytrooplist
        }
    }

    //spawns all units (called in Awake)
    private void SpawnAll()
    {
        //creating new instance of troopscript taking in the tile parameters (controls where they will be spawned)
        troopUnits = new TroopScript[tileCountX, tileCountY];
        

        int friendlyTeam = 0;
        int enemyTeam = 1;

        // spawning the friendly team
        troopUnits[0, 0] = SpawnUnit(TroopType.Light_Infantry, friendlyTeam);
        troopUnits[0, 2] = SpawnUnit(TroopType.Normal_Infantry, friendlyTeam);
        troopUnits[0, 5] = SpawnUnit(TroopType.Heavy_Infantry, friendlyTeam);
        troopUnits[0, 8] = SpawnUnit(TroopType.Normal_Infantry, friendlyTeam);
        troopUnits[0, 10] = SpawnUnit(TroopType.Light_Infantry, friendlyTeam);

        //adding the spawned friendlies to a list (so their size can be dynamic)
        friendlyTroopList.Add(troopUnits[0, 0]);
        friendlyTroopList.Add(troopUnits[0, 2]);
        friendlyTroopList.Add(troopUnits[0, 5]);
        friendlyTroopList.Add(troopUnits[0, 8]);
        friendlyTroopList.Add(troopUnits[0,10]);
        

        //spawning the enemy team
        troopUnits[17, 0] = SpawnEnemyUnit(EnemyTroopType.Light_Infantry, enemyTeam);
        troopUnits[17, 2] = SpawnEnemyUnit(EnemyTroopType.Normal_Infantry, enemyTeam);
        troopUnits[17, 5] = SpawnEnemyUnit(EnemyTroopType.Heavy_Infantry, enemyTeam);
        troopUnits[17, 7] = SpawnEnemyUnit(EnemyTroopType.Normal_Infantry, enemyTeam);
        troopUnits[17, 8] = SpawnEnemyUnit(EnemyTroopType.Light_Infantry, enemyTeam);
        troopUnits[17, 9] = SpawnEnemyUnit(EnemyTroopType.Light_Infantry, enemyTeam);
        troopUnits[17, 11] = SpawnEnemyUnit(EnemyTroopType.Heavy_Infantry, enemyTeam);

        //adding the spawned enemy units to a list (so their size can be dynamic)
        enemyTroopList.Add(troopUnits[17, 0]);
        enemyTroopList.Add(troopUnits[17, 2]);
        enemyTroopList.Add(troopUnits[17, 5]);
        enemyTroopList.Add(troopUnits[17, 7]);
        enemyTroopList.Add(troopUnits[17, 8]);
        enemyTroopList.Add(troopUnits[17, 9]);
        enemyTroopList.Add(troopUnits[17, 11]);

        //array method abandoned because it didn't accomodate for dynamic unit pooling size + deprecated the reinforcement system.
        //an alternative method would be to 'create a new instance of array each time' but this isn't performance-friendl to append/discard consistently
        /*
        enemytroopUnits = new TroopScript[5];

        enemytroopUnits[0] = troopUnits[17, 0];
        enemytroopUnits[1] = troopUnits[17, 2];
        enemytroopUnits[2] = troopUnits[17, 5];
        enemytroopUnits[3] = troopUnits[17, 8];
        enemytroopUnits[4] = troopUnits[17, 10];
        */
    }

    //spawning a single friendly unit
    private TroopScript SpawnUnit(TroopType type, int team)
    {
        TroopScript troop = Instantiate(troops[(int)type -1], transform).GetComponent<TroopScript>();//instantiating a new instance of the troop-script at run-time

        //assigning the troop type and teams
        troop.type = type;
        troop.team = team;
        troop.GetComponent<MeshRenderer>().material = troopMats;//differentiating the teams based on their enum

        return troop;//returning the script
    }

    //spawning a single enemy unit
    private TroopScript SpawnEnemyUnit(EnemyTroopType enemytype, int team)
    {
        TroopScript enemytroop = Instantiate(enemytroops[(int)enemytype-1], transform).GetComponent<TroopScript>();

        //assigning the troop type and teams
        enemytroop.enemytype = enemytype;
        enemytroop.team = team;
        enemytroop.GetComponent<MeshRenderer>().material = enemyMats;//differentiating the teams based on their enum

        return enemytroop;
    }

    //spawns all the supplies (called in Awake)
    private void SpawnAllSupplies()
    {
        supplies = new SupplyScript[tileCountX, tileCountY];//stores the 2d index location of where the supply boxes are

        //Spawning 12 munitions (supply) boxes randomly in the middle of the battle-field
        //to be collected by the units in order to spawn re-inforcements.
        //this prevents the player from employing pre-emptive strategies before the game and cannot *learn* where the supply boxes are/will be as they're different each time
        supplies[0,0] = SpawnBox();
        supplies[1,1] = SpawnBox();
        supplies[2,2] = SpawnBox();
        supplies[3,3] = SpawnBox();
        supplies[4,4] = SpawnBox();
        supplies[5,5] = SpawnBox();
        supplies[6, 6] = SpawnBox();
        supplies[7, 7] = SpawnBox();
        supplies[8, 8] = SpawnBox();
        supplies[9, 9] = SpawnBox();
        supplies[10,10] = SpawnBox();
        supplies[11,11] = SpawnBox();

        
      
    }
    //spawning a single supply box
    private SupplyScript SpawnBox()
    {

        //instantiating all of the supply boxes                                            1 and - 1 so the supplies don't spawn in the friendly and enemy lines
        SupplyScript supplies = Instantiate(supplybox, Snap(UnityEngine.Random.Range(1, tileCountX - 1), UnityEngine.Random.Range(0, tileCountY)), Quaternion.identity).GetComponent<SupplyScript>();//instantiating a  new instance of the supply-script at run-time

        return supplies;//returning the script
    }

    //helper functions
    //GetIndex returns the current x and y values for elements in the tile board
    private Vector2Int GetIndex(GameObject index)
    {
        //iterating through the list of tiles
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
              
                if (tiles[x, y] == index)
                {
                    return new Vector2Int(x, y);//returning selected tile
                }
            }
        }
      
        return -Vector2Int.one;//if there is no tile board to access // invalid
    }

    #endregion

    //----------------------------------------------------------------------------------------------------------------------------------------------------
    //Based on a tile system, the units will change their transform position based on the centre of the tile and the key that is released
    //will be carried as a boolean in the MouseMachine script, split into the player and enemy game state.
    //----------------------------------------------------------------------------------------------------------------------------------------------------

    #region Moving Units
    private bool MoveUnit(TroopScript ts, int x, int y)
    {
        //Player-Turn Moving Units
        if (state.currentState == GameState.Game_States.PLAYER_TURN)
        {
            if (ts.team == 0)//is the unit selected on the player team?
            {
                Vector2Int previousPosition = new Vector2Int(ts.currentX, ts.currentY);//previous position (before move) is current position

                if (troopUnits[x, y] != null)//if unit has been selected
                {
                    TroopScript tru = troopUnits[x, y];
                    if (ts.team == tru.team)//same team?
                    {
                        return false;//invalid move as we are moving onto an already occupied tile
                    }
                    //if it's the enemy team
                    if (tru.team == 0)
                    {
                        deadFriendlies.Add(ts);//add to dead friendlies list (used for death toll statistic)
                        Destroy(ts.gameObject);//destroy the gameObject
                        friendlyTroopList.Remove(ts);//remove from the friendly troop list
                        AudioSource.PlayClipAtPoint(fScream, transform.position, 1.0f);//play friendly scream
                    }
                    else
                    {
                        deadEnemies.Add(tru);//add to the dead enemies list (used for the death toll statistic
                        Destroy(tru.gameObject);//destroy the gameObject
                        enemyTroopList.Remove(tru);//remove from the enemy troop list
                        AudioSource.PlayClipAtPoint(eScream, transform.position, 1.0f);//play enemy scream
                    }
                }

                troopUnits[x, y] = ts;//setting position to previous pos
                troopUnits[previousPosition.x, previousPosition.y] = null;//emptying the troopUnit previous position co-ords as the move was valid.

                CommandSingleUnit(x, y);//command single unit using the SetPosition() function.
            }
        }
        //Enemy Turn Moving Units
        //same as above, but flipped around
        if (state.currentState == GameState.Game_States.ENEMY_TURN)
        {
            Vector2Int previousPosition = new Vector2Int(ts.currentX, ts.currentY);

            if (troopUnits[x, y] != null)
            {
                TroopScript tru = troopUnits[x, y];
                if (ts.team == tru.team)//same team?
                {
                    return false;//invalid move as we are moving onto an already occupied tile
                }
                //if it's the enemy team
                if (tru.team == 0)
                {
                    deadFriendlies.Add(tru);
                    Destroy(tru.gameObject);
                    friendlyTroopList.Remove(tru);
                    AudioSource.PlayClipAtPoint(eScream, transform.position, 1.0f);
                }
                else
                {
                    deadEnemies.Add(ts);
                    Destroy(ts.gameObject);
                    enemyTroopList.Remove(ts);
                    AudioSource.PlayClipAtPoint(eScream, transform.position, 1.0f);
                    
                }
            }

            troopUnits[x, y] = ts;//setting position to previous pos
            troopUnits[previousPosition.x, previousPosition.y] = null;//emptying the enemy previous move co-ordinates as the move was valid.

            CommandSingleUnit(x, y);//command single unit using the SetPosition function 
        }
        return true;
        
    }


    //Positioning the Troops at beginning of the game
    private void CommandAllUnits()
    {
        //looping through all of the tiles
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                if (troopUnits[x, y] != null)//if there are some units left
                {
                    CommandSingleUnit(x, y, true);
                }
            }
        }
    }

    //setting the troops positional co-ordinates to correspond with the game board
    private void CommandSingleUnit(int x, int y, bool force = false)
    {
        troopUnits[x, y].currentX = x;
        troopUnits[x, y].currentY = y;
        troopUnits[x, y].SetPosition(Snap(x, y), force);
    }

    //snapping the units and any game element into the centre of the tile. 
    //taking the width and height of each tile / 2
    //yOffset so the tiles are visible above the terrain (the terrain is uneven)
    private Vector3 Snap(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    #endregion
    
    #region Highlighting Moves
    //Highlighting the Available Tiles
    private void TargetMove()
    {
        for(int i=0; i<ableMoves.Count; i++)
        {
            tiles[ableMoves[i].x, ableMoves[i].y].layer = LayerMask.NameToLayer("Available");
        }
    }

    //Turning Available Highlights Off
    private void TurnOffTargetMove()
    {
        for (int i = 0; i < ableMoves.Count; i++)
        {
            tiles[ableMoves[i].x, ableMoves[i].y].layer = LayerMask.NameToLayer("Tile");//returning back to tile layer
        }
        ableMoves.Clear();//clearing the list
    }

    
    private bool InRange(ref List<Vector2Int> readymoves, Vector2 position)
    {
        for( int i=0; i < readymoves.Count; i++)
        {
            if (readymoves[i].x==position.x && readymoves[i].y==position.y)
            {
                tiles[readymoves[i].x, readymoves[i].y].GetComponent<MeshRenderer>().material = availableMaterial;
                return true;
                
            }
        }

        return false;
    }
    
    #endregion
}


