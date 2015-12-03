using UnityEngine;
using System.Collections;
using System;

public class Meshable : MonoBehaviour {

	private Mesh mesh;
	private Vector3[] vertices;
	private Vector2[] uvs;
	private int[] triangles;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void CreateMesh(int width, int height)
	{
		mesh = new Mesh ();
		this.GetComponent<MeshFilter> ().mesh = mesh;

		vertices = new Vector3[width * height];
		uvs = new Vector2[width * height];
		triangles = new int[6 * ((width - 1) * (height - 1))];

		int triIndex = 0;

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				int index = (y * width) + x;

				vertices[index] = new Vector3(x, -y, 0);
				uvs[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));

				if ((x != width - 1) && y != (height - 1)) {
					int topLeft = index;
					int topRight = topLeft + 1;
					int bottomLeft = topLeft + width;
					int bottomRight = bottomLeft + 1;

					triangles[triIndex++] = topLeft;
					triangles[triIndex++] = topRight;
					triangles[triIndex++] = bottomLeft;
					triangles[triIndex++] = bottomLeft;
					triangles[triIndex++] = topRight;
					triangles[triIndex++] = bottomRight;
				}
			}
		}

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		transform.position = new Vector3 (-width / 2, height / 2, 0);
	}

	public void setZs(ref ushort[] zS)
	{
		for (int i = 0; i < vertices.Length; i++) {
			vertices[i].z = Convert.ToSingle (zS[i]) * 0.1f;

		}
		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();

	}
}
