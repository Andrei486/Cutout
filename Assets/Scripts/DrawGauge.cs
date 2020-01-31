using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGauge : MonoBehaviour
{
	float remainingDistance;
	GameObject controller;
	Vector2 position = new Vector2(10, 10);
	Vector2 maxFilledSize = new Vector2(590, 15);
	Vector2 maxSize = new Vector2(600, 27);
	public Texture fillTexture;
	public Texture backgroundTexture;
	
	void Start()
	{
		controller = GameObject.FindGameObjectsWithTag("GameController")[0];
	}
	
	void OnGUI()
	{
		GUI.BeginGroup(new Rect(position.x, position.y, maxSize.x, maxSize.y), GUIStyle.none);
			GUI.Box(new Rect(0, 0, maxSize.x, maxSize.y), backgroundTexture, GUIStyle.none); //coordinates relative to group
			
			GUI.BeginGroup(new Rect(5, 6, maxFilledSize.x * remainingDistance, maxFilledSize.y), GUIStyle.none);
				GUI.Box(new Rect(0,0, maxFilledSize.x, maxFilledSize.y), fillTexture, GUIStyle.none);
			GUI.EndGroup();
		GUI.EndGroup();
	}

    // Update is called once per frame
    void Update()
    {
		remainingDistance = 1f - (controller.GetComponent<DrawObject>().distanceDrawn / controller.GetComponent<DrawObject>().maxDrawDistance);
    }
}
