using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public int SpeedX;
    public int SpeedY;
    public int ScrollSpeed;

	void Start () {
        if (SpeedX <= 0)
            SpeedX = 3;
        if (SpeedY <= 0)
            SpeedY = 3;
        if (ScrollSpeed <= 0)
            ScrollSpeed = 3;
    }
	
	void Update () {
        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.position = new Vector3(transform.position.x+(Input.GetAxis("Horizontal") * SpeedX * Time.deltaTime), transform.position.y + (Input.GetAxis("Vertical") * SpeedY * Time.deltaTime), transform.position.z);
        }
        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float ExpectedSize = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed * 30 * Time.deltaTime;
            if (ExpectedSize > 0.5f)
                Camera.main.orthographicSize += -Input.GetAxis("Mouse ScrollWheel") * ScrollSpeed * 30 * Time.deltaTime;
        }
	}
}
