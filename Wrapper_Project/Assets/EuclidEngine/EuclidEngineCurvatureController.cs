using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EuclidEngineCurvatureController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public void UpdateCurv(float curv)
    {
        worldCurvature = curv;

        var renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            foreach (var sMaterial in renderer.sharedMaterials)
            {
                if (sMaterial && sMaterial.HasProperty("_Curvature"))
                {
                    sMaterial.SetFloat("_Curvature", worldCurvature);
                }
            }
        }
    }

    public float worldCurvature = 0.0f;

    // Update is called once per frame
    void Update()
    {
        var add = (Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0);
        worldCurvature += 0.05f * add * Time.deltaTime;

        UpdateCurv(worldCurvature);
    }

    private void OnDestroy()
    {
    }
}
