using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
A script to be attached to the GameController. Takes care of rendering the draw gauge,
using data from other scripts and the specified textures.
*/
public class DrawGauge : MonoBehaviour
{
	float remainingDistance;
	GameObject controller;
	Vector2 position = new Vector2(10, 10);
	Vector2 maxFilledSize = new Vector2(590, 15);
	Vector2 maxSize = new Vector2(600, 27);
	public Texture fillTexture; //set in Inspector: the texture of the actual bar
	public Texture backgroundTexture; //set in Inspector: the texture of the background (empty bar)
	
	void Start()
	{
		remainingDistance = 1f;
		controller = GameObject.FindGameObjectsWithTag("GameController")[0];
	}
	
	//Activates when GUI is drawn each frame
	void OnGUI()
	{
		GUI.BeginGroup(new Rect(position.x, position.y, maxSize.x, maxSize.y), GUIStyle.none);
			//background is rendered first, and does not scale
			GUI.Box(new Rect(0, 0, maxSize.x, maxSize.y), backgroundTexture, GUIStyle.none); //coordinates relative to group
			
			//filled bar is rendered after and scales based on remainingDistance
			GUI.BeginGroup(new Rect(5, 6, maxFilledSize.x * remainingDistance, maxFilledSize.y), GUIStyle.none);
				GUI.Box(new Rect(0,0, maxFilledSize.x, maxFilledSize.y), fillTexture, GUIStyle.none);
			GUI.EndGroup();
		GUI.EndGroup();
	}

    // Update is called once per frame
    void Update()
    {
		remainingDistance = controller.GetComponent<DrawObject>().GetRemainingDistance();
    }
}
