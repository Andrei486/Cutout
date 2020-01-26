using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject player;
	public Vector3[] spawnPositions;
	GameObject[] players;
	bool ended;
    // Start is called before the first frame update
    void Start()
    {
		//create new players
		players = new GameObject[2];
		
        players[0] = Instantiate(player, this.gameObject.transform);
		players[0].name = "Player 1";
		players[0].transform.position = spawnPositions[0];
		players[0].GetComponent<PolygonCollider2D>().enabled = true;
		
		players[1] = Instantiate(player, this.gameObject.transform);
		players[1].name = "Player 2";
		players[1].transform.position = spawnPositions[1];
		players[1].GetComponent<PolygonCollider2D>().enabled = true;
		//flip the second player
		Vector3 scale = players[1].transform.localScale;
		players[1].transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
    }

    // Update is called once per frame
    void Update()
    {
		if (!ended){
			if (players[0] == null){
				Debug.Log("Player 2 wins!");
				ended = true;
				StartCoroutine(EndGame());
			}
			if (players[1] == null){
				Debug.Log("Player 1 wins!");
				ended = true;
				StartCoroutine(EndGame());
			}
		}
		else {
			if (Input.GetKeyDown("space")){
				Application.Quit();
			}
		}
    }
	
	private IEnumerator EndGame(){
		
		//show the win text
		Time.timeScale = 0.1f;
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
	}
}
