﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{
    private Vector3 defaultPos;
    private Vector3 defaultRot;

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = gameObject.transform.position;
        defaultRot = gameObject.transform.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RestartTransform()
    {
        gameObject.transform.position = defaultPos;
        gameObject.transform.eulerAngles = defaultRot;
    }
}
