using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameController : MonoBehaviour
{
	public GameObject player;
	public Vector3[] spawnPositions;
	GameObject[] players;
	public float maxPlayerHealth;
	public GameObject drawAreas;
	bool ended;
    // Start is called before the first frame update
    void Start()
    {
		//create new players
		players = new GameObject[2];
		
        players[0] = Instantiate(player, this.gameObject.transform);
		players[0].name = "Player 1";
		players[0].GetComponent<PlayerController>().playerNumber = 0;
		players[0].transform.position = spawnPositions[0];
		
		players[1] = Instantiate(player, this.gameObject.transform);
		players[1].name = "Player 2";
		players[1].GetComponent<PlayerController>().playerNumber = 1;
		players[1].transform.position = spawnPositions[1];
		
		foreach(GameObject player in players){
			player.GetComponent<PolygonCollider2D>().enabled = true;
			player.GetComponent<Health>().maxHealth = maxPlayerHealth;
			player.GetComponent<Health>().enabled = true;
		}
		//flip the second player
		Vector3 scale = players[1].transform.localScale;
		players[1].transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
		ActivateDrawArea(0);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)){
			Application.Quit();
		}
    }
	
	public bool ActivateDrawArea(int index){
		if (index < 0 || index >= drawAreas.transform.childCount){
			return false; //for some reason an impossible index was passed
		}
		if (drawAreas.transform.GetChild(index).gameObject.activeSelf){
			return false; //if the specified area is already the active one
		}
		
		foreach(Transform child in drawAreas.transform){
			child.gameObject.SetActive(false); //disable all random areas
		}
		drawAreas.transform.GetChild(index).gameObject.SetActive(true); //then enable only the correct one
		return true;
	}
	
	public void ActivateRandomDrawArea(){
		System.Random rng = new System.Random();
		while(!ActivateDrawArea(rng.Next(0, drawAreas.transform.childCount))){
			;
		}
	}
	
	public static IEnumerator EndGame(int loser){
		int winner = 1 - loser;
		GameController controller = GameObject.FindGameObjectsWithTag("GameController")[0].GetComponent<GameController>();
		Debug.Log(controller.players[winner].name + " wins!");
		GameObject.FindGameObjectsWithTag("Win Text")[0].transform.GetChild(winner).gameObject.GetComponent<Text>().enabled = true;
		controller.ended = true;
		//show the win text
		Time.timeScale = 0.1f;
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
	}
}
