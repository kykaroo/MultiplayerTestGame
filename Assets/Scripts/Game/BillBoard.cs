using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera _cam;
    
    private void Update()
    {
        if (_cam == null)
            _cam = FindObjectOfType<Camera>();
        if (_cam == null) return;
        
        transform.LookAt(_cam.transform);
    }
}
