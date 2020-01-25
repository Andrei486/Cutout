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
	
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.collider.tag != "Drawn"){
			return;
		}
		float momentum = collision.rigidbody.velocity.magnitude * collision.rigidbody.mass;
		healthScript.TakeDamage(momentum);
	}
	
	public void Die(){
		//activate ragdoll
		Destroy(this.gameObject);
	}
}
