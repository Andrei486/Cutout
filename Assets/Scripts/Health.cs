using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float health = 1.0f; //nonzero value to start
	public float maxHealth;
	public float minDamageInterval;
	public Vector2 healthBarSize;
	public Color healthBarColor;
	public Color healthBarBackground;
	public Font healthFont;
	Texture2D healthBarTexture;
	Texture2D healthBarBG;
	float lastDamageTime;
	float healthFraction;
	Vector2 healthBarPosition;
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
	
	public void TakeDamage(float damage){
		if (Time.time - lastDamageTime < minDamageInterval){
			return;
		}
		lastDamageTime = Time.time;
		this.health -= damage;
		Debug.Log(this.gameObject.name +" took " + damage + " damage");
		return;
	}
	
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
