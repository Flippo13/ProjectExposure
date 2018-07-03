using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour
{
    public bool _lookAt;

    private void Update()
    {
        if (_lookAt)
        {
            Vector3 deltaVec = Camera.main.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(-deltaVec);
        }
    }
}
