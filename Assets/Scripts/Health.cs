using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float health;
	public float maxHealth;
	public float minDamageInterval;
	float lastDamageTime;
	float healthFraction;
    // Start is called before the first frame update
    void Start()
    {
		health = maxHealth;
        lastDamageTime = -100.0f; //negative value to enable taking damage immediately
    }

    // Update is called once per frame
    void Update()
    {
        healthFraction = health/maxHealth;
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
}
