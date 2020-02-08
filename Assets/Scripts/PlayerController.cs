using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
	public Health healthScript;
	Animator animator;
	System.Random rng;
    // Start is called before the first frame update
    void Start()
    {
        healthScript = this.gameObject.GetComponent<Health>();
		animator = this.gameObject.transform.Find("Kliff_Model").GetComponent<Animator>();
		rng = new System.Random();
		StartCoroutine(RandomIdle());
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
		float momentum = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
		healthScript.TakeDamage(momentum);
	}
	
	public void Die(){
		//activate ragdoll
		Destroy(this.gameObject);
	}
	
	IEnumerator RandomIdle(){
		while (true) {
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Basic Idle")){
				double randomness = rng.NextDouble();
				Debug.Log(randomness);
				if (randomness < 0.1f){
					animator.ResetTrigger("Idle2");
					animator.SetTrigger("Idle1");
				} else if (randomness < 0.3f){
					animator.ResetTrigger("Idle1");
					animator.SetTrigger("Idle2");
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
		
	}
}
