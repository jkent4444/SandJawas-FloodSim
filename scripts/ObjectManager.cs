using UnityEngine;
using System.Collections.Generic;
using System.Collections;

//controls the placement and removal of objects and stores information for all placed objects
public class ObjectManager : MonoBehaviour {

    private List<Placeable> placeAbles;
    public GameObject dam;

    public void placeDam()
    {
        Instantiate(dam, new Vector3(0,0,0), Quaternion.identity);

    }
}
