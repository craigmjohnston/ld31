using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
    public float speed;
    public HomeBase target;
    public Vector3 pivot;
    public Vector3 axis;

    protected bool destroyed = false;
    protected float destroyTimer;

    public delegate void MissileEventHandler(Missile sender);
    public event MissileEventHandler Destroyed = delegate {};

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (destroyed) {
	        destroyTimer -= Time.deltaTime;
	        if (destroyTimer <= 0) {
	            Destroy(gameObject);
	        }
	        return;
	    }
        transform.RotateAround(pivot, axis, speed * Time.deltaTime);
	}

    void OnTriggerEnter(Collider other) {
        var homeBase = other.gameObject.GetComponent<HomeBase>();
        if (homeBase != null && homeBase == target) {
            Destruct();
            homeBase.MissileHit();
            return;
        }
        var shield = other.gameObject.GetComponent<Shield>();
        if (shield != null && shield.transform.parent == target.transform) {
            Destruct();
            shield.MissileHit();
            return;
        }
    }

    protected void Destruct() {
        Destroyed(this);
        destroyed = true;
        destroyTimer = GetComponent<TrailRenderer>().time;
        renderer.enabled = false;
    }
}