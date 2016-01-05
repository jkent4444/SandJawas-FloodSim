using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WaterHolder: MonoBehaviour  {




	public Vector3 start;
	public Vector3 end;
	public float width;
	public GameObject connector;
	public Vector3 offset;
	public float curWidth;


	public float GetWdith() {
		return width;
	}

	public void IncreaseWidth() {
		width += 0.5f;
	}

	public void DecreaseWidth() {
		width -= 0.5f;
	}

	public void changeSize() {
		if (curWidth < width) {
			curWidth += 0.1f;
			connector.transform.localScale = new Vector3 (curWidth, offset.magnitude / 2.0f, curWidth);
		}
	}

	public WaterHolder(Vector3 start, Vector3 end) {  
		this.start = start;
		this.end = end;
		width = 0.5f;
		curWidth = 0.0f;

		offset = end - start;

		var scale = new Vector3(curWidth, offset.magnitude / 2.0f, curWidth);
		var position = start + (offset / 2.0f);

		GameObject myPrefab = (GameObject)Resources.Load("Prefab/waterConnector");
		connector = Instantiate(myPrefab, position, Quaternion.identity) as GameObject;
		connector.transform.up = offset;
		connector.transform.localScale = scale;


	}

	public Vector3 GetStart() {
		return start;
	}

	public void GetStart(Vector3 value) {
		start = value;
	}

	public Vector3 GetEnd() {
		return end;
	}

	public void GetEnd(Vector3 value) {
		end = value;
	}

	public override bool Equals(System.Object obj) {
		if (obj == null) {
			return false;
		}
		// If parameter cannot be cast waterFlow return false.
		WaterHolder other = obj as WaterHolder;
		if ((System.Object)obj == null)
		{
			return false;
		}

		if (start.Equals(start) && end.Equals(end)) {
			return true;
		} else {
			return false;
		}
	}

	public override int GetHashCode ()
	{
		return (start.GetHashCode()+end.GetHashCode())*13;

	}
}


public class WaterFlow : MonoBehaviour {

	public List<WaterHolder> points = new List<WaterHolder>();

	// Use this for initialization

	//for testing
	void Start () {
		Vector3 test1 = new Vector3 (0, 0, 0);
		Vector3 test2 = new Vector3 (0, 0, 0);
		new Vector3 (0, 0, 0);
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(10, 0, 10), new Vector3(13, 4, 15));
		Debug.Log (points.Count);
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Flow(new Vector3(0, 0, 0), new Vector3(10, 0, 10));
		Debug.Log (points.Count);
		Debug.Log (test1.Equals (test2));
	}
	
	// Update is called once per frame
	//for testing
	void Update () {
		foreach (WaterHolder point in points) {
			point.changeSize ();
		}
	
	}

	void Clear() {
		points.Clear ();
	}

	// check if prefab with start and end exist
	// if not create the prefab
	// have switch for to render or not,
	// each time we look at those points, increase size.
	//every so often decrease size. 

	// Call when increasing width.
	void Flow(Vector3 curPoint, Vector3 prevPoint) {
		bool found = false;
		foreach (WaterHolder point in points) {
			Debug.Log (point.start);
			Debug.Log (point.end);
			if (point.start.Equals (prevPoint) && (point.end.Equals (curPoint))) {
				point.IncreaseWidth ();
				found = true;
				break;
			}
		}
		if (!found) {
			points.Add(new WaterHolder(prevPoint, curPoint));
		}

	}

	void UpdateFlow() {
	//	foreach (WaterHolder point in points) {
			
	//	}
	}
}