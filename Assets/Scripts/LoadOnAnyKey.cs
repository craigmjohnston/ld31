using UnityEngine;
using System.Collections;

public class LoadOnAnyKey : MonoBehaviour {
    public int sceneId;
	
	void Update () {
	    if (Input.anyKeyDown) {
	        Application.LoadLevel(sceneId);
	    }
	}
}
