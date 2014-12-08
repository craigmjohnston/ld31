using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum ResourceType {
    Mass, Energy
}

public class Asteroid : MonoBehaviour {
    public ResourceType resourceType;
    public int quantity;
    public static List<Asteroid> Instances = new List<Asteroid>();

	void Start () {
	    Instances.Add(this);
	}
	
	void Update () {
	
	}
}
