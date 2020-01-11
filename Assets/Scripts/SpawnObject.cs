using System.Collections;
using System;
using Geometry;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    Vector3 lastMousePosition;
    Vector3 currentMousePosition;
	bool isLoop;
    List<Vector3> points;
	List<Vector3> screenPoints;
    LineRenderer lrenderer;
    GameObject polygon;
	Rigidbody2D rb;
	PolygonCollider2D pCollider;
	public float distanceDrawn;
	public float maxDrawDistance;
	public float baseMaxDrawDistance;
    public Camera camera;
    public float threshold;
	public Material materializedColor;
	public Material temporaryColor;
	public Material cutoutColor;
	public Material fillColor;
	IntersectionChecker ic;
    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = new Vector3(0,0,0); //have a Vector3 ready
        Debug.Log(lastMousePosition);
		ic = new IntersectionChecker();
		SetupNewObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        { //mouse is down
            currentMousePosition = Input.mousePosition;
			currentMousePosition = camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, 10));
			
            if (((Vector3.Distance(currentMousePosition, lastMousePosition) > threshold) &&
			(Vector3.Distance(currentMousePosition, lastMousePosition) < maxDrawDistance - distanceDrawn)) ||
			(points.Count == 0)) //do not keep point if it is too close, or if it exceeds draw distance
			//unless it is the first point
            {
				if (points.Count != 0){ //increase distance drawn if it is not the first point
					if (!ic.CheckIfAddable(currentMousePosition, points)){ //separate check because it is long
						Debug.Log("cannot add, would create self-intersection");
						return;
					}
					distanceDrawn += Vector3.Distance(currentMousePosition, lastMousePosition);
				} else {
					
				}
					
                points.Add(currentMousePosition);
                lrenderer.positionCount++;
				lrenderer.SetPosition(lrenderer.positionCount-1, points[points.Count-1]); //add the point to temporary line
				
				lastMousePosition = currentMousePosition; //update position of last point
            }
        }
        else
		{
            if (points.Count > 1f) {
                MaterializeObject();
            }
			points = new List<Vector3>();
			lrenderer.positionCount = 0;
        }
		if (Input.GetMouseButtonDown(1)){
			Destroy(polygon);
			SetupNewObject();
		}
    }

    void MaterializeObject()
    {
		if (Vector3.Distance(points[points.Count-1], points[0]) > threshold * 10.0f){ //TODO change threshold
			Destroy(polygon);
			SetupNewObject();
			return;
		}
		
		//finalize creation of previous object
		lrenderer.material = materializedColor;
		lrenderer.startWidth = 0.05f;
		lrenderer.endWidth = 0.05f;
		lrenderer.loop = true;
		
		//create rigidbody, collider and fill shape
		rb = polygon.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		rb.gravityScale = 1.0f;
		rb.isKinematic = false;
		rb.useAutoMass = true;
		
		pCollider = polygon.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
		pCollider.points = ConvertTo2DLoop(points);
		
		//add mesh to fill object
		MeshFilter meshFilter = polygon.AddComponent(typeof(MeshFilter)) as MeshFilter;
		Mesh mesh = new Mesh();
		MeshRenderer meshRenderer = polygon.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		
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
		
		Color[] colors = new Color[points2d.Length];
		for (int i=0; i<points2d.Length; i++){
			colors[i] = Color.white;
		}
		
		Triangulator tr = new Triangulator(points2d);
		int [] triangles = tr.Triangulate();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		meshRenderer.material = fillColor;
		
		//create white cutout on background
		CreateCutout(pCollider.points);
		
		Debug.Log("Total distance: " + distanceDrawn);
		Debug.Log("materialized");
        //create new object for later
		SetupNewObject();
    }
	
	void SetupNewObject()
	{
		points = new List<Vector3>();
		screenPoints = new List<Vector3>();
		maxDrawDistance = baseMaxDrawDistance;
		distanceDrawn = 0f;
		polygon = new GameObject();
		polygon.name = "Drawn Object";
        lrenderer = polygon.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lrenderer.startWidth = 0.1f;
        lrenderer.endWidth = 0.1f;
        lrenderer.positionCount = 0;
        lrenderer.loop = false;
		lrenderer.useWorldSpace = false;
		lrenderer.material = temporaryColor;
	}
	
	void CreateCutout(Vector2[] basePoints){
		Vector2[] points = new Vector2[basePoints.Length + 1];
		for (int i=0;i<basePoints.Length; i++){
			points[i] = basePoints[i];
		}
		points[points.Length-1] = basePoints[0]; //make it loop back around
		points[points.Length-1].x += 0.01f;
		GameObject cutout = new GameObject();
		cutout.name = "Cutout";
		PolygonCollider2D cutoutCollider = cutout.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
		cutout.layer = 8; //layer on which raycast will be used for cutouts
		MeshFilter meshFilter = cutout.AddComponent(typeof(MeshFilter)) as MeshFilter;
		Mesh mesh = new Mesh();
		MeshRenderer meshRenderer = cutout.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		
		
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
		
		Triangulator tr = new Triangulator(points);
		int [] triangles = tr.Triangulate();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uv;
		mesh.colors = colors;
		meshFilter.mesh = mesh;
		mesh.RecalculateBounds();
		meshRenderer.material = cutoutColor;
	}
	
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
