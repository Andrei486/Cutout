using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Geometry;

public class DrawObject : MonoBehaviour
{
	Vector3 lastMousePosition;
    Vector3 currentMousePosition;
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
	public float density;
	public Material temporaryColor;
	IntersectionChecker ic;
	SpawnObject spawner;
    // Start is called before the first frame update
    void Start()
    {
        lastMousePosition = new Vector3(0,0,0); //have a Vector3 ready
		ic = new IntersectionChecker();
		spawner = this.gameObject.GetComponent<SpawnObject>();
		SetupNewObject(); //set up a first object to be drawn
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        { //mouse is down
            currentMousePosition = Input.mousePosition;
			currentMousePosition = camera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, 10));
			
            if (((Vector3.Distance(currentMousePosition, lastMousePosition) > threshold) &&
			(maxDrawDistance - distanceDrawn > Vector3.Distance(currentMousePosition, lastMousePosition)) ||
			(points.Count == 0))) //do not keep point if it is too close, or if it exceeds remaining draw distance
			//unless it is the first point, in which case it should be added regardless
            {
				
				LayerMask mask = LayerMask.GetMask("Cutouts", "Terrain");
				Collider2D hitCollider = Physics2D.OverlapPoint((Vector2) currentMousePosition, mask);
				if (hitCollider != null){
					return;
				}
				mask = LayerMask.GetMask("Draw Area");
				hitCollider = Physics2D.OverlapPoint((Vector2) currentMousePosition, mask);
				if (hitCollider == null){
					return;
				}
				
				if (points.Count != 0){
					if (!ic.CheckIfAddable(currentMousePosition, points, threshold)){ //check self-intersection, separate check because it is long
						return;
					}
					distanceDrawn += Vector3.Distance(currentMousePosition, lastMousePosition); //reduce drawing distance
				} else {
					
				}
				
				//add the point to the line
                points.Add(currentMousePosition);
                lrenderer.positionCount++;
				lrenderer.SetPosition(lrenderer.positionCount-1, points[points.Count-1]);
				
				lastMousePosition = currentMousePosition; //update position of last point
            }
        }
        else
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
			Destroy(polygon);
		}
		points = new List<Vector3>();
		maxDrawDistance = baseMaxDrawDistance;
		distanceDrawn = 0f;
		polygon = new GameObject();
		polygon.transform.Translate(new Vector3(0, 0, -1));
		polygon.name = "Temporary Object";
        lrenderer = polygon.AddComponent(typeof(LineRenderer)) as LineRenderer;
        lrenderer.startWidth = 0.1f;
        lrenderer.endWidth = 0.1f;
        lrenderer.positionCount = 0;
        lrenderer.loop = false;
		lrenderer.useWorldSpace = false;
		lrenderer.material = temporaryColor;
	}
}
