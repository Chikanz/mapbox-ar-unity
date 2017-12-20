﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pelletFloat : MonoBehaviour {

    public bool alternate;
    public float magnitude;
    public float speed = 0.5f;

    private float yOffset;

	// Use this for initialization
	void Start ()
    {
        yOffset = transform.localPosition.y;

    }
	
	// Update is called once per frame
	void Update ()
    {
        var p = transform.localPosition;
        var offset = alternate ? magnitude : 0;
        transform.localPosition = new Vector3(p.x, yOffset + Mathf.PingPong(Time.time * speed + offset, magnitude), p.z);
	}
}
