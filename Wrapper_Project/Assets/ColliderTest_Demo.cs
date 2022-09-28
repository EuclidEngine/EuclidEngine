using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest_Demo : MonoBehaviour
{
    public CurvatureController curvController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        FPSController pit;
        if (other.gameObject.TryGetComponent(out pit))
        {
            curvController.UpdateCurv(0.0f);
        }
    }
}
