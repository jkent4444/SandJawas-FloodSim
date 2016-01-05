using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopologyHeight : MonoBehaviour {

	public Mesh myMesh = Meshable.myMesh();
	public float distance = 5;
	private Vector3 heightPoint;
	private List<mountain> mountains = new List<mountain>();

	public struct layer
	{
		public float height;
		public Vector2[] points;

	}

	public struct mountain
	{
		public float maxHeight;
		public layer[] topograph;
		// lowest ring needs to be added 
	}


	// Use this for initialization
	//creates arraylist of all 
	List<Vector3> getLayers(float current, Vector3[] verticies)
	{
		List<Vector3> currentPoints = new List<Vector3>();
		foreach (Vector3 point in  verticies) {
			if (point == current) {
				currentPoints.Add(point);
			}
		}
		return currentPoints;
	}

	//adds and sorts layers to mountains 
	void addToMountains(List<Vector3> currentPoints) {
		List<Vector3> copyPoints = new List<Vector3> (currentPoints);
		// a ring is a height ring that exists for a mountain, 1 ring per mountain
		List<List<Vector3>> rings = new List<List<Vector3>> ();
		//keeps track of what ring we are looking at.
		int ringIndex = 0;
		//adds an intital 
		rings [ringIndex].Add (copyPoints [0]);
		copyPoints.Remove (copyPoints [0]);
		while (copyPoints.Count != 0) {
			int i = 0;
			bool check = false;
			while (i < copyPoints.Count) {
				int j = 0;
				if (rings[ringIndex].Count == 0) {
					rings [ringIndex].Add (copyPoints [0]);
					copyPoints.Remove (copyPoints [0]);
					check = true;
					break;
				}

				while (j < rings[ringIndex].Count) {
					if (Vector3.Distance (copyPoints [i], rings [ringIndex] [j]) > 1) {
						rings [ringIndex].Add (copyPoints [i]);
						copyPoints.Remove(copyPoints[i]); 
						check = true;
						break;
					}
					j++;
				}
				if (check) {
					break;
				}
				i++;
			}
			if (check) {
				check = false;
				continue;
			}
			else {
				ringIndex++;
			}
			
		}


	}



	void Start () 
	{
		//mountains;
		Vector3[] vertices = myMesh.vertices;
		heightPoint = highestPoint (vertices);
		float currentHeight = heightPoint.y;
		while (currentHeight > 0) {
			getLayers (currentHeight, verticies);
		}


	}

	private Vector3 highestPoint(Vector3[] vertices) 
	{
		Vector3 topPoint = new Vector3(0, -100, 0);
		int i = 0;
		while (i < vertices.Length) {
			if (vertices[i].y > topPoint.y) {
				topPoint = vertices[i];
			}
			i++;
		}
		return topPoint;
	}
}
	

