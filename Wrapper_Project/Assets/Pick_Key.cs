using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pick_Key : MonoBehaviour
{
    // Start is called before the first frame update
    private bool has_key = false;
    public Animator chest_open;

    private AudioSource audioSource;
    public AudioClip clip;
    public AudioClip clip2;

    public float volume = 0.1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key")) {
            Destroy(other.gameObject);
            has_key = true;
            audioSource.PlayOneShot(clip, volume);
        }

        if (other.CompareTag("Chest") && has_key == true)
        {
            has_key = false;
            chest_open.SetBool("Key_Near", true);
            audioSource.PlayOneShot(clip2, volume);
        }
    }
}
