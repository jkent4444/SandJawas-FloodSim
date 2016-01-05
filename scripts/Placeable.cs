using UnityEngine;
using System.Collections;

public class Placeable : MonoBehaviour {

    private Vector3 size;
    private Vector3 position;
    private Vector3 rotation;
    private GameObject objectToBePlaced;

    public void setPlaceable(Vector3 size, Vector3 position, Vector3 rotation, GameObject objectToBePlaced)
    {
        this.size = size;
        this.position = position;
        this.rotation = rotation;
        //find the local space on the meshmap then place the gameobject there with rotation and position
    }
}
