using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalObjAssign : MonoBehaviour
{
    public Vector4 hyperOrigin;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphericalObject>().SetHyperOrigin(hyperOrigin);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
