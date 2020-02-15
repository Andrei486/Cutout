using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
A script to be attached to each player. Handles taking damage and showing the player's HP bar.
*/
public class Health : MonoBehaviour
{
	float health = 1.0f; //nonzero value to start
	public float maxHealth;
	public float minDamageInterval; //set in Inspector
	
	public Vector2 healthBarSize; //set in Inspector
	public Color healthBarColor; //set in Inspector
	public Color healthBarBackground; //set in Inspector
	public Font healthFont; //set in Inspector
	Texture2D healthBarTexture;
	Texture2D healthBarBG;
	Vector2 healthBarPosition;
	
	float lastDamageTime;
	float healthFraction;
	
	Camera camera;
	float playerHeight;
    // Start is called before the first frame update
    
	void Start()
    {
		health = maxHealth;
        lastDamageTime = -100.0f; //negative value to enable taking damage immediately
		
		//set position for the health bar
		camera = GameObject.FindGameObjectsWithTag("MainCamera")[0].GetComponent<Camera>();
		playerHeight = this.gameObject.GetComponent<PolygonCollider2D>().bounds.size.y;
		healthBarPosition = camera.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0f, playerHeight * 1.1f, 0));
		healthBarPosition += new Vector2(-0.5f * healthBarSize.x, healthBarSize.y); //center bar on player
		//Debug.Log(this.gameObject.transform.position);
		//Debug.Log(healthBarPosition);
		
		healthBarTexture = CreateFillTexture(healthBarColor, (int) healthBarSize.x, (int) healthBarSize.y);
		healthBarBG = CreateFillTexture(healthBarBackground, (int) healthBarSize.x, (int) healthBarSize.y);
		
    }

    // Update is called once per frame
    void Update()
    {
        healthFraction = health/maxHealth;
		healthBarPosition = camera.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0f, playerHeight * 1.1f, 0));
		healthBarPosition += new Vector2(-0.5f * healthBarSize.x, healthBarSize.y);
    }
	
	/** Makes the player take the specified amount of damage, if possible. */
	public void TakeDamage(float damage){
		if (Time.time - lastDamageTime < minDamageInterval){
			return;
		}
		lastDamageTime = Time.time;
		this.health -= damage;
		Debug.Log(this.gameObject.name +" took " + damage + " damage");
		return;
	}
	
	/** Returns the player's remaining health. */
	public float GetHealth(){
		return health;
	}
	
	//Activates every frame when GUI is drawn
	void OnGUI(){
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.label.font = healthFont;
		GUI.skin.label.normal.textColor = Color.black;
		GUI.BeginGroup(new Rect(healthBarPosition.x, Screen.height - healthBarPosition.y, healthBarSize.x, healthBarSize.y), GUIStyle.none);
			GUI.Box(new Rect(0, 0, healthBarSize.x, healthBarSize.y), healthBarBG, GUIStyle.none); //coordinates relative to group
			
			GUI.BeginGroup(new Rect(0, 0, healthBarSize.x * healthFraction, healthBarSize.y), GUIStyle.none);
				GUI.Box(new Rect(0, 0, healthBarSize.x, healthBarSize.y), healthBarTexture, GUIStyle.none);
			GUI.EndGroup();
			GUI.Label(new Rect(0, 0, healthBarSize.x, healthBarSize.y), this.gameObject.name);
		GUI.EndGroup();
		
		
	}
	
	/** Returns a monochrome fill texture with the specified color and dimensions. */
	Texture2D CreateFillTexture(Color fillColor, int x, int y){
		Color[] pixels = new Color[x * y];
		for(int i = 0; i < pixels.Length; i++){
			pixels[i] = fillColor;
		}
		Texture2D result = new Texture2D(x, y);
		result.SetPixels(pixels);
		result.Apply();
		return result;
	}
}
