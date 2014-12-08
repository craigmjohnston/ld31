using UnityEngine;

public class Orbit : MonoBehaviour {
    public Vector3 pivot;
    public Vector3 axis;
    public float speed;

	void Start () {
	
	}
	
	void Update () {
        transform.RotateAround(pivot, axis, speed * Time.deltaTime);
	}
}