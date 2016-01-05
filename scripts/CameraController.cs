using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public bool inGame;
    public float ScrollSpeed = 15f;
    public float ScrollEdge = 0.01f;
 
    private int HorizontalScroll = 1;
    private int VerticalScroll = 1;
    private int DiagonalScroll = 1;
 
    public int PanSpeed = 10;
 
    public Vector2 ZoomRange = new Vector2(-5,5);
    public float CurrentZoom = 0f;
    public float ZoomZpeed = 1f;
    public float ZoomRotation = 1f;
 
    private Vector3 InitPos;
    private Vector3 InitRotation;
    void Start()
    {
        inGame = false;
        InitPos = this.transform.position;
        InitRotation = this.transform.eulerAngles;
    }
	// Update is called once per frame
	void Update () 
    {
        if (inGame)
        {
           //PAN
    if ( Input.GetKey("mouse 2") )
    {
        //(Input.mousePosition.x - Screen.width * 0.5)/(Screen.width * 0.5)
        transform.Translate(Vector3.right  * (int)Time.deltaTime * PanSpeed  * (int)((Input.mousePosition.x - Screen.width * 0.5)/(Screen.width * 0.5)), Space.World);
        transform.Translate(Vector3.forward * Time.deltaTime * PanSpeed * (int)((Input.mousePosition.y - Screen.height * 0.5)/(Screen.height * 0.5)), Space.World);
 
    }
    else
    {
        if ( Input.GetKey("d") || Input.mousePosition.x >= Screen.width * (1 - ScrollEdge) )
        {
            transform.Translate(Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
        }
        else if ( Input.GetKey("a") || Input.mousePosition.x <= Screen.width * ScrollEdge )
        {
            transform.Translate(Vector3.right * Time.deltaTime * -ScrollSpeed, Space.World);
        }
       
        if ( Input.GetKey("w") || Input.mousePosition.y >= Screen.height * (1 - ScrollEdge) )
        {
            transform.Translate(Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
        }
        else if ( Input.GetKey("s") || Input.mousePosition.y <= Screen.height * ScrollEdge )
        {
            transform.Translate(Vector3.forward * Time.deltaTime * -ScrollSpeed, Space.World);
        }
    }
   
//ZOOM IN/OUT
   
    CurrentZoom -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 1000 * ZoomZpeed;
   
    CurrentZoom = Mathf.Clamp(CurrentZoom,ZoomRange.x,ZoomRange.y);
    
    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y - (transform.position.y - (InitPos.y + CurrentZoom)) * 0.1f, this.transform.position.z);
    this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - (transform.eulerAngles.x - (InitRotation.x + CurrentZoom * ZoomRotation)) * 0.1f,this.transform.eulerAngles.y, this.transform.eulerAngles.z);

        }
        
	}

    public void setInGame(bool boolean)
    {
        inGame = boolean;
    }
}
