using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // -={Private Variables}=-
    Rigidbody theRB;

    // -={Public Variables}=-
    public float bulletSpeed = 15f;
    public AudioClip BulletHitAudio;
    public GameObject bulletImpactEffect;
    public int damage = 10;

    void Awake()
    {
        theRB = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        print(originalDirection);
        transform.forward = originalDirection;
        theRB.velocity = transform.forward * bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        Destroy(gameObject);
    }
}
