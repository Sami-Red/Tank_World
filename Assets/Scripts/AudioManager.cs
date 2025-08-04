using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject AudioPrefab;
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public void Play3D(AudioClip clip, Vector3 position)
    {
        GameObject audioGameObject = Instantiate(AudioPrefab, position, Quaternion.identity);
        AudioSource source = audioGameObject.GetComponent<AudioSource>();

        source.clip = clip;
        source.Play();

        Destroy(audioGameObject, clip.length);
    }
}
