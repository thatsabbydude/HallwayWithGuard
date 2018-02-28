using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    Vector3 offset;

	// Use this for initialization
	void Start () {
        offset = target.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        float targetAngle = target.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, targetAngle, 0);

        transform.position = target.transform.position - offset;
        //transform.LookAt(target.transform);
    }
}
