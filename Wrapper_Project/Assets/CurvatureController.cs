using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvatureController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float curv = 0.0f;

    // Update is called once per frame
    void Update()
    {
        var add = (Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
        curv += 0.05f * add * Time.deltaTime;

        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            if (renderer.sharedMaterial.HasProperty("_Curvature"))
            {
                renderer.sharedMaterial.SetFloat("_Curvature", curv);
            }
        }
    }

    
}
