using UnityEngine;
using System.Collections;

public class RotateCameraWithMouse : MonoBehaviour {
    public Vector3 pivot;
    public float acceleration;
    public float maxSpeed;
    public float drag;

    protected Vector2 mouseDownPosition;
    protected float angle = 0;

	void Start () {
	
	}
	
	void Update () {
	    if (Input.GetMouseButtonDown(1)) {
	        mouseDownPosition = Input.mousePosition;
	    } else if (Input.GetMouseButton(1)) {
	        transform.RotateAround(pivot, transform.up, Input.GetAxis("Mouse X")*maxSpeed*Time.deltaTime);
            transform.RotateAround(pivot, -transform.right, Input.GetAxis("Mouse Y") * maxSpeed * Time.deltaTime);
	    }
	}
}