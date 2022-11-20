using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating_Key : MonoBehaviour
{
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
    }
}
