using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public GameObject player;
	public Vector3[] spawnPositions;
	GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
		//create new players
		players = new GameObject[2];
		
        players[0] = Instantiate(player, this.gameObject.transform);
		players[0].transform.position = spawnPositions[0];
		players[0].GetComponent<PolygonCollider2D>().enabled = true;
		
		players[1] = Instantiate(player, this.gameObject.transform);
		players[1].transform.position = spawnPositions[1];
		players[1].GetComponent<PolygonCollider2D>().enabled = true;
		//flip the second player
		Vector3 scale = players[1].transform.localScale;
		players[1].transform.localScale = new Vector3(scale.x, -scale.y, scale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
