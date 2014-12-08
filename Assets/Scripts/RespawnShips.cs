using UnityEngine;
using System.Collections;

public class RespawnShips : MonoBehaviour {
    public PlayerShip playerShipPrefab;
    public AIShip aiShipPrefab;

    public HomeBase playerBase;
    public HomeBase enemyBase;

    public float spawnTimeout;

    protected float playerSpawnTimer = 0;
    protected float aiSpawnTimer = 0;

	void Start () {
	
	}
	
	void Update () {
	    if (FindObjectOfType<PlayerShip>() == null && playerSpawnTimer == 0) {
	        playerSpawnTimer = spawnTimeout;
	    }
        if (FindObjectOfType<AIShip>() == null && aiSpawnTimer == 0) {
            aiSpawnTimer = spawnTimeout;
        }
	    playerSpawnTimer -= Time.deltaTime;
	    aiSpawnTimer -= Time.deltaTime;
	    if (FindObjectOfType<PlayerShip>() == null && playerSpawnTimer <= 0) {
	        var player = (PlayerShip)Instantiate(playerShipPrefab);
	        playerSpawnTimer = 0;
	    }
        if (FindObjectOfType<AIShip>() == null && aiSpawnTimer <= 0) {
            var enemy = (AIShip)Instantiate(aiShipPrefab);
            enemy.homeBase = enemyBase;
            aiSpawnTimer = 0;
        }
	}
}
