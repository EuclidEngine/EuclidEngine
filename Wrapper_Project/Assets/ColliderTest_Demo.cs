using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTest_Demo : MonoBehaviour
{
    public EuclidEngineCurvatureController curvController;

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
            StartCoroutine(UpdateCurvProgressively(0.5f));
        }
    }

    IEnumerator UpdateCurvProgressively(float delay)
    {
        float start = curvController.worldCurvature;
        float curs = 0.0f;
        while (curs < delay)
        {
            curs += Time.deltaTime;
            curvController.UpdateCurv(Mathf.Lerp(start, 0.0f, curs / delay));
            yield return null;
        }
        yield return null;
    }
}
