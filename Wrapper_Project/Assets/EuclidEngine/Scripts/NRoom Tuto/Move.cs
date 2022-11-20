using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EuclidEngine;

public class Move : MonoBehaviour
{
    public float speed = 2f;
    public GameObject roomObject;
    private NonVisualEuclidianRoom3 room; 
    private int current_config = 0;

    bool go_back = false;
    Vector3[] pos_base = new[] { new Vector3(-100, -100, -100), new Vector3(30.37f, 5.99f, 32.13f),
        new Vector3(30.37f, 5.99f, 32.13f), new Vector3(30.37f, 3.99f, 32.13f) };

    private Vector3 goal1 = new Vector3(17.726f, 7.01f, 30.15f);
    private Vector3 goal2 = new Vector3(30.37f, 5.99f, 32.13f);
    private Vector3 goal3 = new Vector3(30.37f, 10.9f, 32.13f);
    private Vector3 goal4 = new Vector3(30.37f, 3.99f, 32.13f);


    // Start is called before the first frame update
    void Start()
    {
        room = roomObject.GetComponent<NonVisualEuclidianRoom3>();
    }

    private void modifyPos(float x, float y, float z)
    {
        Vector3 new_pos = new Vector3(x, y, z);
        transform.position = new_pos;
    }

    // Update is called once per frame
    void Update()
    {
        int config = room.getConfig();
        if (current_config != config)
        {
            current_config = config;
            modifyPos(pos_base[config - 1].x, pos_base[config - 1].y, pos_base[config - 1].z);
            go_back = false;
        }

        if (config == 2)
        {
            if (!go_back)
                transform.position = Vector3.MoveTowards(transform.position, goal1, speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, goal2, speed * Time.deltaTime);
            if (transform.position.z >= 32.13f && transform.position.x >= 30.37f)
                go_back = false;
            else if (transform.position.z <= 30.15f)
                go_back = true;
        }
        if (config == 3)
        {
            if (!go_back)
                transform.position = Vector3.MoveTowards(transform.position, goal1, speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, goal2, speed * Time.deltaTime);
            if (transform.position.z >= 32.13f && transform.position.x >= 30.37f)
                go_back = false;
            else if (transform.position.z <= 30.15f)
                go_back = true;
        }
        if (config == 4)
        {
            if (!go_back)
                transform.position = Vector3.MoveTowards(transform.position, goal3, speed * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, goal4, speed * Time.deltaTime);
            if (transform.position.y <= 3.99f)
                go_back = false;
            else if (transform.position.y >= 10.9f)
                go_back = true;
        }
    }
}
