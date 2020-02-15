using System.Collections;
using System;
using Geometry;
using System.Collections.Generic;
using UnityEngine;

/**
A script to be attached to the GameController. Works in tandem with DrawObject to actually create objects and their
cutouts.
*/
public class SpawnObject : MonoBehaviour
{
    LineRenderer lrenderer;
    GameObject polygon;
	Rigidbody2D rb;
	PolygonCollider2D pCollider;
	public GameObject drawnPrefab; //set in Inspector: prefab to create objects from
	public float density; //set in Inspector: density of created objects
	public Material cutoutColor; //set in Inspector: material to fill the cutout with (usually white); should be monochrome
	public float loopThreshold; //set in Inspector: the maximum allowable distance between start and end points.
	
    // Start is called before the first frame update
    void Start()
    {
		
    }

    // Update is called once per frame
    void Update()
    {
		
    }
	
	/**
	Materializes the currently drawn object; if successful, the corresponding cutout will also be created.
	If the object cannot be materialized, this does nothing.
	*/
    public void MaterializeObject(List<Vector3> points)
    {
		//if the object was not close enough to a loop, do not create it
		if (Vector3.Distance(points[points.Count-1], points[0]) > loopThreshold){
		Debug.Log("not a loop");
			return;
		}
		//otherwise, create the object
		polygon = Instantiate(drawnPrefab, this.gameObject.transform);
		polygon.tag = "Drawn";
		
		//make the outline
		lrenderer = polygon.GetComponent<LineRenderer>();
		lrenderer.useWorldSpace = false;
		lrenderer.positionCount = points.Count;
		lrenderer.SetPositions(points.ToArray());
		
		//create rigidbody, collider and fill shape
		rb = polygon.GetComponent<Rigidbody2D>();
		
		pCollider = polygon.GetComponent<PolygonCollider2D>();
		pCollider.points = ConvertTo2DLoop(points);
		pCollider.density = density;
		
		//prepare to add mesh to fill object
		MeshFilter meshFilter = polygon.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		MeshRenderer meshRenderer = polygon.GetComponent<MeshRenderer>();
		
		//initialization for the Triangulator, to create the mesh
		Vector2[] points2d = new Vector2[points.Count + 1];
		for (int i=0;i<points.Count; i++){
			points2d[i] = (Vector2) points[i];
		}
		points2d[points2d.Length-1] = points[0]; //make it loop back around
		points2d[points2d.Length-1].x += 0.01f;
		
		Vector3[] vertices = new Vector3[points2d.Length];
		Vector2[] uv = new Vector2[points2d.Length];
		
		for(int i=0; i<points2d.Length; i++){
            Vector2 point = points2d[i];
            vertices[i] = new Vector3(point.x, point.y, 0);
            uv[i] = point;
        }
		
		//set the fill color
		Color[] colors = new Color[points2d.Length];
		for (int i=0; i<points2d.Length; i++){
			colors[i] = Color.white;
		}
		
		Triangulator tr = new Triangulator(points2d);
		int [] triangles = tr.Triangulate(); //get triangles for the mesh using the Triangulator
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		
		//assign mesh to MeshFilter
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		
		//create white cutout on background
		CreateCutout(pCollider.points);
		GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>().ActivateRandomDrawArea();
		Debug.Log("materialized");
    }
	
	/**Creates the cutout corresponding to an array of points, ie a polygon.*/
	void CreateCutout(Vector2[] basePoints){
		
		//set up the new GameObject
		GameObject cutout = new GameObject();
		cutout.name = "Cutout";
		
		//give it a collider
		PolygonCollider2D cutoutCollider = cutout.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
		cutoutCollider.points = basePoints;
		cutout.layer = 8; //layer for cutouts
		
		//make the mesh and add necessary components
		MeshFilter meshFilter = cutout.AddComponent(typeof(MeshFilter)) as MeshFilter;
		Mesh mesh = new Mesh();
		MeshRenderer meshRenderer = cutout.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		
		//get points for the mesh
		Vector2[] points = new Vector2[basePoints.Length + 1];
		for (int i=0;i<basePoints.Length; i++){
			points[i] = basePoints[i];
		}
		points[points.Length-1] = basePoints[0]; //make it loop back around
		points[points.Length-1].x += 0.01f;
		
		Vector3[] vertices = new Vector3[points.Length];
		Vector2[] uv = new Vector2[points.Length];
		
		for(int i=0; i<points.Length; i++){
            Vector2 point = points[i];
            vertices[i] = new Vector3(point.x, point.y, 0);
            uv[i] = point;
        }
		
		Color[] colors = new Color[points.Length];
		for (int i=0; i<points.Length; i++){
			colors[i] = Color.white;
		}
		
		//finish setting up the mesh
		Triangulator tr = new Triangulator(points);
		int [] triangles = tr.Triangulate();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		
		//assign mesh to MeshFilter so it shows up
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		meshRenderer.material = cutoutColor;
	}
	
	/**
	Returns an array of Vector2s corresponding to the Vector3 points, in order.
	The first Vector2 is also repeated at the end to make the array a true loop.
	*/
	Vector2[] ConvertTo2DLoop(List<Vector3> points3d)
	{
		Vector2[] points2d = new Vector2[points3d.Count+1];
		for (int i = 0; i < points3d.Count; i++){
			points2d[i] = (Vector2) points3d[i];
		}
		points2d[points2d.Length-1] = (Vector2) points3d[0]; //repeat the first point at the end to loop around
		return points2d;
	}
}
