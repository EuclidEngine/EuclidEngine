using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTK_Pool : MonoBehaviour
{

    public Animator anim_door;
    public Animator anim_door1;

    bool completed = false;
    public List<GameObject> colliderList = new List<GameObject>();

    private AudioSource audioSource;
    public AudioClip clip;
    public float volume = 0.3f;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (completed)
            return;

        foreach (GameObject enigma in colliderList)
        {
            if (enigma.transform.localScale.x >= 0.5 && enigma.transform.localScale.x <= 0.51 && enigma.transform.localScale.z >= 0.5 && enigma.transform.localScale.z <= 0.51 && enigma.transform.localScale.y >= 4.90 && enigma.transform.localScale.y <= 5)
            {
                if (enigma.transform.localPosition.x >= 3.1 && enigma.transform.localPosition.x <= 3.45)
                {
                    if (enigma.transform.localPosition.y <= -3.12 && enigma.transform.localPosition.y >= -3.133)
                    {
                        completed = true;
                        audioSource.PlayOneShot(clip, volume);
                        anim_door.SetBool("Open", true);
                        anim_door1.SetBool("Open2", true);
                    }
                }
            }
        }
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (completed)
            return;

        if (!colliderList.Contains(collider.gameObject))
        {
            colliderList.Add(collider.gameObject);
            Debug.Log(collider.gameObject.name);
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (completed)
            return;

        if (colliderList.Contains(collider.gameObject))
        {
            colliderList.Remove(collider.gameObject);
        }
    }
}
