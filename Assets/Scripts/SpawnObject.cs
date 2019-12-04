using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    Vector3 lastMousePosition;
    Vector3 currentMousePosition;
	bool isLoop;
    List<Vector3> points;
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
    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = new Vector3(0,0,0); //have a Vector3 ready
        Debug.Log(lastMousePosition);
		SetupNewObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        { //mouse is down
            currentMousePosition = Input.mousePosition;
			Debug.Log(currentMousePosition);
			
            if (((Vector3.Distance(currentMousePosition, lastMousePosition) > threshold) &&
			(Vector3.Distance(currentMousePosition, lastMousePosition) < maxDrawDistance - distanceDrawn)) ||
			(points.Count == 0)) //do not keep point if it is too close, or if it exceeds draw distance
			//unless it is the first point
            {
				if (points.Count != 0){ //increase distance drawn if it is not the first point
					distanceDrawn += Vector3.Distance(currentMousePosition, lastMousePosition); 
				}
                points.Add(camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, 10))); //add new point to shape
                lrenderer.positionCount++;
				lrenderer.SetPosition(lrenderer.positionCount-1, points[points.Count-1]); //add the point to temporary line
				
				lastMousePosition = currentMousePosition; //update position of last point
                Debug.Log("Added point");
            } else {
				Debug.Log("failed to add point");
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
		if (Vector3.Distance(points[points.Count-1], points[0]) < 0.25f){
			isLoop = true;
		} else{
			isLoop = false;
		}
		
		//finalize creation of previous object
		lrenderer.material = materializedColor;
		lrenderer.startWidth = 0.05f;
		lrenderer.endWidth = 0.05f;
		lrenderer.loop = isLoop;
		
		//create rigidbody, collider and fill shape
		rb = polygon.AddComponent(typeof(Rigidbody2D)) as Rigidbody2D;
		rb.gravityScale = 1.0f;
		rb.isKinematic = false;
		rb.useAutoMass = true;
		
		pCollider = polygon.AddComponent(typeof(PolygonCollider2D)) as PolygonCollider2D;
		if (isLoop){
			pCollider.points = ConvertTo2DLoop(points);
		} else {
			pCollider.points = ConvertTo2DLine(points);
		}
		
		
		
		//create white cutout on background
		
		Debug.Log("Total distance: " + distanceDrawn);
		Debug.Log("materialized");
        //create new object for later
		SetupNewObject();
    }
	
	void SetupNewObject()
	{
		points = new List<Vector3>();
		maxDrawDistance = baseMaxDrawDistance;
		distanceDrawn = 0f;
		polygon = new GameObject();
        lrenderer = polygon.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lrenderer.startWidth = 0.1f;
        lrenderer.endWidth = 0.1f;
        lrenderer.positionCount = 0;
        lrenderer.loop = false;
		lrenderer.useWorldSpace = false;
		lrenderer.material = temporaryColor;
	}
	
	Vector2[] ConvertTo2DLoop(List<Vector3> points3d)
	{
		Vector2[] points2d = new Vector2[points3d.Count+1];
		for (int i = 0; i < points3d.Count; i++){
			points2d[i] = (Vector2) points3d[i];
		}
		points2d[points2d.Length-1] = (Vector3) points3d[0]; //repeat the first point at the end to loop around
		return points2d;
	}
	
	Vector2[] ConvertTo2DLine(List<Vector3> points3d)
	{
		Vector2[] points2d = new Vector2[points3d.Count*2-1];
		for (int i = 0; i < points3d.Count; i++){
			points2d[i] = (Vector2) points3d[i];
		}
		for (int i = 0; i < points3d.Count; i++){
			points2d[points2d.Length-i-1] = (Vector2) points3d[i];
			points2d[points2d.Length-i-1].x += 0.07f; //give the line some width
		}
		return points2d;
	}
}
