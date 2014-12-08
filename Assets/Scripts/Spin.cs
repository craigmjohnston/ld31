using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
    public Vector3 axis;
    public float speed;
    public bool random;

	void Start () {
	    if (random) {
            axis = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
	        speed = Random.Range(0.1f, 45f);
	    }
	}
	
	void Update () {
	    transform.Rotate(axis, speed*Time.deltaTime);
	}
}
