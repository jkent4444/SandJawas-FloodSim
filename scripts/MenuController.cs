using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {
	public Canvas MainMenuUI;
	public Canvas KinectMenuUI;
	public Canvas LoadedMapUI;


	// Use this for initialization
	void Start () {
		disableMenuUI ();
		enableMainMenuUI ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void enableMainMenuUI(){
		disableMenuUI ();
		MainMenuUI.GetComponent<Canvas> ().enabled = true;
	}

	public void enableKinectMenuUI(){
		disableMenuUI ();
		KinectMenuUI.GetComponent<Canvas> ().enabled = true;
	}

	public void enableLoadedMapUI() {
		disableMenuUI ();
		LoadedMapUI.GetComponent<Canvas> ().enabled = true;

	}

	public void disableMenuUI(){
		MainMenuUI.GetComponent<Canvas> ().enabled = false;
		KinectMenuUI.GetComponent<Canvas> ().enabled = false;
		LoadedMapUI.GetComponent<Canvas> ().enabled = false;
	}

}
