using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private Transform playerTransform;

	// Use this for initialization
	void Start () {
        //playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerTransform == null)
		{
			RefreshPlayer();
		}
		transform.position = Vector3.forward * playerTransform.position.z;
	}
	public void RefreshPlayer()
    {
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
}
