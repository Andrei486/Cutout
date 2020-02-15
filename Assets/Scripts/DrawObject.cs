using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

/**
A script to be attached to the GameController. It handles drawing the object by adding points to it when constraints allow.
This script does not create the actual object or cutout; those are handled by SpawnObject.
*/
public class DrawObject : MonoBehaviour
{
	Vector3 lastMousePosition;
    Vector3 currentMousePosition;
    List<Vector3> points;
	float distanceDrawn;
	float maxDrawDistance;
	
	GameObject polygon;
    LineRenderer lrenderer;
	Rigidbody2D rb;
	PolygonCollider2D pCollider;
	
	IntersectionChecker ic;
	SpawnObject spawner;
	
	public float baseMaxDrawDistance; //set in Inspector
    Camera camera; //set in Inspector
    public float threshold; //set in Inspector: minimum distance between 2 points
	public Material temporaryColor; //set in Inspector: material for the temporary line
    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = new Vector3(0,0,0); //have a Vector3 ready
		ic = new IntersectionChecker();
		spawner = this.gameObject.GetComponent<SpawnObject>();
		camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
		SetupNewObject(); //set up a first object to be drawn
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        { //mouse is down
            currentMousePosition = Input.mousePosition;
			//get the mouse's world position
			currentMousePosition = camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, 10));
			
            if (((Vector3.Distance(currentMousePosition, lastMousePosition) > threshold) &&
			(maxDrawDistance - distanceDrawn > Vector3.Distance(currentMousePosition, lastMousePosition)) ||
			(points.Count == 0))) //do not keep point if it is too close, or if it exceeds remaining draw distance
			//unless it is the first point, in which case it should be added regardless
            {
				//check if the new point overlaps some collider that we can't draw over
				LayerMask mask = LayerMask.GetMask("Cutouts", "Terrain");
				Collider2D hitCollider = Physics2D.OverlapPoint((Vector2) currentMousePosition, mask);
				if (hitCollider != null){
					return; //if it does don't place it
				}
				//check if the new point overlaps a drawable zone
				mask = LayerMask.GetMask("Draw Area");
				hitCollider = Physics2D.OverlapPoint((Vector2) currentMousePosition, mask);
				if (hitCollider == null){
					return; //if it doesn't don't place it
				}
				
				//for points after the first, we need to check for self-intersections and going through cutouts.
				if (points.Count != 0){
					if (!ic.CheckIfAddable(currentMousePosition, points, threshold)){ //check self-intersection, separate check because it is long
						return;
					}
					distanceDrawn += Vector3.Distance(currentMousePosition, lastMousePosition); //reduce drawing distance
				}
				
				//add the point to the line
                points.Add(currentMousePosition);
                lrenderer.positionCount++;
				lrenderer.SetPosition(lrenderer.positionCount-1, points[points.Count-1]);
				
				lastMousePosition = currentMousePosition; //update position of last point
            }
        }
        else //if the mouse isn't down, input is done
		{
            if (points.Count > 1f) { //if there are more than 2 points already drawn, try to materialize it.
                spawner.MaterializeObject(points);
				SetupNewObject();
            }
        }
		if (Input.GetMouseButtonDown(1)){ //on right-click, cancel the drawing
			SetupNewObject();
		}
    }
	
	/**Creates a new object to which components will be added as they are drawn/materialized. Also resets the necessary variables. */
	void SetupNewObject()
	{
		if (polygon != null){
			Destroy(polygon); //destroy the previous temporary shape
		}
		points = new List<Vector3>(); //reset points
		maxDrawDistance = baseMaxDrawDistance; //reset draw distance
		distanceDrawn = 0f;
		//create the new object
		polygon = new GameObject();
		polygon.transform.Translate(new Vector3(0, 0, -1));
		polygon.name = "Temporary Object";
		//set up the LineRenderer
        lrenderer = polygon.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lrenderer.startWidth = 0.1f;
        lrenderer.endWidth = 0.1f;
        lrenderer.positionCount = 0;
        lrenderer.loop = false;
		lrenderer.useWorldSpace = false;
		lrenderer.material = temporaryColor;
	}
	
	/**Returns the remaining distance to draw, as a fraction of the maximum drawing distance. */
	public float GetRemainingDistance(){
		return 1f - (distanceDrawn / maxDrawDistance);
	}
}
