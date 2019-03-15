using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public float movSpeed;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

		float x = Input.GetAxis ("Horizontal");
		float y = Input.GetAxis ("Vertical");

		transform.Translate (x * Time.deltaTime * movSpeed, y * Time.deltaTime * movSpeed, 0);
    }
}
