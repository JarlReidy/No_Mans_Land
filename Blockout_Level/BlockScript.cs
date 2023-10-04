using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used for controls in the Block Out scene, controlling the camera and the player
//Conceptual, so the player can see/understand why the map has been built this way
//Toggling the cameras and the player movement is my own work, however the mouse look and camera rotate IS NOT MY WORK
//inspired heavily by Brackeys FIRST PERSON MOVEMENT in Unity - FPS Controller Video
//the methods which aren't mine include : 
// - LookAround()
// - MoveAround()
public class BlockScript : MonoBehaviour
{
    [Header("Camera Objects")]
    [SerializeField] private Camera birdsCam;//the birds eye POV camera
    public bool birdsCamEnabled = true;
    [SerializeField] private Camera fpCam;//the first person player camera

    [Header("Camera Qualities")]
    public float lookSens;// the multiplier which the player head looks around every frame
    private float xRot = 0f;
    // Start is called before the first frame update

    [Header("Player")]
    public CharacterController controller;
    public float movespeed = 12f;

    [Header("World")]
    [SerializeField] private GameObject BlockOut;//the geometric block out layout
    public bool blockOutEnabled = true;
    [SerializeField] private GameObject PolyBlockOut;//the poly-builder concept layout
    void Start()
    {
        //birds eye view camera enabled
        birdsCam.enabled = true;
        fpCam.enabled = false;

        //block out enabled
        BlockOut.SetActive(true);
        PolyBlockOut.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Toggle();
        MoveAround();
        LookAround();
        if (birdsCamEnabled)
        {
            birdsCam.enabled = true;
            fpCam.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;//confining the cursor to the game window
        }
        else if(!birdsCamEnabled)
        {
            birdsCam.enabled = false;
            fpCam.enabled = true;
            
            //LookAround();//can look around as the active camera is first person
            //MoveAround();
        }
        if(blockOutEnabled)
        {
            BlockOut.SetActive(true);
            PolyBlockOut.SetActive(false);
        }
        else if(!blockOutEnabled)
        {
            BlockOut.SetActive(false);
            PolyBlockOut.SetActive(true);
        }
    }

    void Toggle()
    {
        if (Input.GetKeyDown(KeyCode.Q) && birdsCam.enabled)
        {
            birdsCamEnabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !birdsCam.enabled)
        {
            birdsCamEnabled = true;
        }

        if(Input.GetKeyDown(KeyCode.E) && blockOutEnabled)
        {
            blockOutEnabled = false;
        }
        else if(Input.GetKeyDown(KeyCode.E) && !blockOutEnabled)
        {
            blockOutEnabled = true;
        }
    }

    //The following code is inspired by Brackeys FIRST PERSON in UNITY - FPS Controller video:
    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSens * Time.deltaTime;//will update every frame
        float mouseY = Input.GetAxis("Mouse Y") * lookSens * Time.deltaTime;

        xRot -= mouseY;
        //clamping rotation
        xRot = Mathf.Clamp(xRot, -90f, 90f);

       // transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
       
    }

    void MoveAround()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;//takes the player forward based on the way it is facing

        controller.Move(move * movespeed * Time.deltaTime);
    }
}
