using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
	public int playerNumber;
	public Health healthScript;
	public float minimumDamageReaction;
	Animator animator;
    // Start is called before the first frame update
    void Start()
    {
		animator = this.gameObject.transform.Find("Kliff_Model").GetComponent<Animator>();
		StartCoroutine(RandomIdle());
    }

    // Update is called once per frame
    void Update()
    {
		//Debug.Log(this.healthScript.health);
        if (this.healthScript.GetHealth() <= 0.0f){
			Die();
		}
    }
	
	void OnCollisionEnter2D(Collision2D collision){
		if (collision.collider.tag != "Drawn"){
			return;
		}
		float momentum = collision.relativeVelocity.magnitude * collision.rigidbody.mass;
		healthScript.TakeDamage(momentum);
		if (momentum > minimumDamageReaction){
			animator.SetTrigger("Hit");
		}
	}
	
	public void Die(){
		//activate ragdoll
		StartCoroutine(GameController.EndGame(playerNumber));
		Destroy(this.gameObject);
	}
	
	IEnumerator RandomIdle(){
		while (true) {
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Basic Idle")){
				double randomness = new System.Random((int) DateTime.Now.Ticks).NextDouble(); //so both players have different pseudo-random generators
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
