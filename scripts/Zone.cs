using UnityEngine;
using System.Collections;

public class Zone : MonoBehaviour {

    public int radius;
    public Vector3 position;
    public zoneType zone;
    public int solubility;
    public GameObject[] objectsToBePlaced;
    public int objectDensity;

    public void createZone(int radius, Vector3 position, zoneType type, int solubility, int objectDensity)
    {

        this.radius = radius;
        this.position = position;
        this.zone = type;
        this.solubility = solubility;
        this.objectDensity = objectDensity;
    }
    
    //function for spawning objects within the zone
}
