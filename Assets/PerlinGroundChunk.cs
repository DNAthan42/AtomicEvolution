using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PerlinGroundChunk : MonoBehaviour
{
	public Vector2 ChunkID = new Vector2(0, 0);
	public int xSize = 256;
	public int zSize = 256;

	private Mesh mesh;
	private Vector3[] verts;
	private int[] tris;

    // Start is called before the first frame update
    void Start()
    {
		mesh = GetComponent<MeshFilter>().mesh;

		verts = new Vector3[(xSize + 1) * (zSize + 1)];
		tris = new int[xSize * zSize * 6];

		CreateMesh();
		UpdateMesh();
    }

	private void CreateMesh()
	{
		int count = 0;
		for (int x = 0; x < xSize + 1; x++)
		{
			for (int z = 0; z < zSize + 1; z++)
			{
				float xScale = (float)(x + ChunkID.x) + .1f;
				float zScale = (float)(z + ChunkID.y) + .1f;
				float y = Mathf.PerlinNoise(xScale, zScale) * 3f;
				verts[count] = new Vector3(x, y, z);

				count++;
			}
		}

		int c = 0;
		int v = 0;
		for (int x = 0; x < xSize; x++)
		{
			for (int z = 0; z < zSize; z++)
			{
				tris[c] = v;
				tris[c + 1] = v + 1;
				tris[c + 2] = v + xSize + 1;
				tris[c + 3] = v + 1;
				tris[c + 4] = v + xSize + 2;
				tris[c + 5] = v + xSize + 1;

				v++;
				c += 6;
			}
			v++;
		}
		
	}

	private void UpdateMesh()
	{
		mesh.Clear();
		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.RecalculateNormals();
	}

	//private void OnDrawGizmos()
	//{
	//	if (verts == null)
	//	{
	//		return;
	//	}
	//	Gizmos.color = Color.black;
	//	for (int i = 0; i < verts.Length; i++)
	//	{
	//		Gizmos.DrawSphere(verts[i], 0.1f);
	//	}
	//}

	// Update is called once per frame
	void Update()
    {
    }
}
