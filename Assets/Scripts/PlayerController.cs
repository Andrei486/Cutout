using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public Health healthScript;
    // Start is called before the first frame update
    void Start()
    {
        healthScript = this.gameObject.GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.healthScript.health <= 0.0f){
			Die();
		}
    }
	
	public void Die(){
		//activate ragdoll
	}
}
