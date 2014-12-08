using System.Linq;
using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public LayerMask harvesterLayer;

    public Vector3 direction;
    public float speed;
    public float timeout;

    public Transform mothership;
    public HomeBase homeBase;

    protected float timeoutTimer;
    protected bool targeted = false;

    void OnTriggerEnter(Collider other) {
        var ship = other.GetComponent<PlayerShip>();
        if (ship != null && ship.transform != mothership) {
            ship.Damage(1);
            Destroy(gameObject);
        }
        var aiShip = other.GetComponent<AIShip>();
        if (aiShip != null && aiShip.transform != mothership) {
            aiShip.Damage(1);
            Destroy(gameObject);
        }
        var harvester = other.GetComponent<Harvester>();
        if (harvester != null) {
            harvester.Damage(1);
            //Debug.Log("harvester");
            Destroy(gameObject);
        }
        var asteroid = other.GetComponent<Asteroid>();
        if (asteroid != null) {
            //Debug.Log("asteroid");
            Destroy(gameObject);
        }
        var homeBase = other.GetComponent<HomeBase>();
        if (homeBase != null) {
            Destroy(gameObject);
        }
    }

	void Start () {
	    timeoutTimer = timeout;
	}
	
	void Update () {
	    timeoutTimer -= Time.deltaTime;
	    if (timeoutTimer <= 0) {
            Destroy(gameObject);
	    }
	    transform.position += direction*speed*Time.deltaTime;
	    if (GameValues.ScreenRect.Contains(transform.position)) {
	        //Destroy(gameObject);
	    }
	    if (!targeted) {
	        Collider[] harvesters = Physics.OverlapSphere(transform.position, 0.4f, harvesterLayer).Where(c => c.GetComponent<Harvester>().homeBase != homeBase).ToArray();
	        foreach (Collider harvester in harvesters) {
	            targeted = true;
	            direction = harvesters[0].transform.position - transform.position;
	            break; // todo obviously this is a pointless foreach loop, but I might need it later
	        }
	    }
	}
}
