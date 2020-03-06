using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

/**
A script to be attached to the GameController. Handles game setup and win conditions among others.
*/
public class GameController : MonoBehaviour
{
	public GameObject player; //set in Inspector: player prefab for the rightmost player (player 1)
	public Vector3[] spawnPositions; //set in Inspector: places to spawn the players, in order
	public float maxPlayerHealth; //set in Inspector
	public GameObject drawAreas; //set in Inspector: the parent to all drawable areas
	public int currentArea;
	GameObject[] players;
	float[] maxAreas;
	public float[] filledAreas;
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
		
		maxAreas = new float[drawAreas.transform.childCount];
		filledAreas = new float[drawAreas.transform.childCount];
		for (int i = 0; i < maxAreas.Length; i++){
			foreach (Transform subArea in drawAreas.transform.GetChild(i)){
				BoxCollider2D box = subArea.GetComponent<BoxCollider2D>();
				maxAreas[i] += box.size.x * box.size.y * subArea.localScale.x * subArea.localScale.y;
			}
			Debug.Log(maxAreas[i]);
		}

		ActivateDrawArea(0);
    }

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)){
			Application.Quit();
		}
    }
	
	/**
	Attempts to activate the draw area specified by the index, and returns true if the activation was successful.
	If the draw area was already active (or the index is out of range), does nothing but returns false.
	*/
	public bool ActivateDrawArea(int index){
		if (index < 0 || index >= drawAreas.transform.childCount){
			return false; //for some reason an impossible index was passed
		}
		if (drawAreas.transform.GetChild(index).gameObject.activeSelf){
			return false; //if the specified area is already the active one
		}

		if (filledAreas[index] / maxAreas[index] > 0.9){
			return false; //if the draw area is too full
		}
		
		foreach(Transform child in drawAreas.transform){
			child.gameObject.SetActive(false); //disable all random areas
		}
		currentArea = index;
		drawAreas.transform.GetChild(index).gameObject.SetActive(true); //then enable only the correct one
		return true;
	}
	
	/**
	Activates a random draw area from the list, different from the currently activated one.
	*/
	public void ActivateRandomDrawArea(){
		System.Random rng = new System.Random();
		int selectableAreas = 0;
		for (int i = 0; i < maxAreas.Length; i++){
			if (filledAreas[i] / maxAreas[i] <= 0.9){
				selectableAreas++;
			}
		}
		if (selectableAreas == 0){ //if there is not enough space in any draw area
			StartCoroutine(EndGame(0));
			return;
		}
		while(!ActivateDrawArea(rng.Next(0, drawAreas.transform.childCount))){
			;
		}
	}
	
	/**
	Ends the game and causes effects to play.
	*/
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
