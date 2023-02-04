using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationZero : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = new Vector3(0, -1, 0);
    }
}
