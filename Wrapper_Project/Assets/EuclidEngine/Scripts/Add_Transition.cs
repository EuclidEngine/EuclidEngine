using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EuclidEngineArea))]
public class Add_Transition : MonoBehaviour
{
    // Start is called before the first frame update

    public Vector3 internalSize = new Vector3(1.5f, 0.5f, 1.5f);

    EuclidEngineArea Area;
    void Start()
    {
        Area = gameObject.GetComponent<EuclidEngineArea>();
        Area.SetSize(new Vector3(3, 1, 3), internalSize, new Vector3(1, 1, 1));
    }

    // Update is called once per frame
    void Update()
    {
    }
}
