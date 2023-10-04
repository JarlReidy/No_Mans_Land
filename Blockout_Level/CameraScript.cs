using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------------------------------------------------------------------------------------------------------------------
//Rendered Obsolete as the idea has been scrapped due to the original camera angle being orthographic - later changed to a perspective camera
//This script has been inspired by the camera movement in the Total War series, controls are :
//- Scroll wheel to zoom
//- Q & R to rotate
//- W,A,S,D to move
//-------------------------------------------------------------------------------------------------------------------------------------------------
public class CameraScript : MonoBehaviour
{
    [Header(" Camera Qualities : ")]
    private Camera warCam;//referenced war camera
    private float zoomCam;//the zoom multiplier based on the orthographic camera view
    private float scrollCam;//the value of the scroll used on the camera
    //multiplier of the zoom value
    [SerializeField] private float multiplier = 3f;//serialized so can be changed in the inspector
    //The value which is incremented every second to ensure the camera movement is smooth and not jittery.
    [SerializeField] private float smoothCamRig = 10;//serialized so the value can be changed in the inspector
    // Start is called before the first frame update
    void Start()
    {
        warCam = Camera.main;
        zoomCam = warCam.orthographicSize;//the zoom multiplier 
    }

    // Update is called once per frame
    void Update()
    {
        scrollCam = Input.GetAxis("Mouse ScrollWheel");//scroll cam is equivalent to the data recorded by the scroller on the mouse

        zoomCam -= scrollCam * multiplier;//calculating the zoom of the camera
        zoomCam = Mathf.Clamp(zoomCam, 4f, 8f);//clamping the camera zoom so it doesn't clip inside of the scene
        warCam.orthographicSize = Mathf.Lerp(warCam.orthographicSize, zoomCam, Time.deltaTime);//interpolating the state of the camera so there is smooth transition between the zooming factors.

        //Controls Removed.

    }
}
