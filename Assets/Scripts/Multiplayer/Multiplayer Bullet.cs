using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

public class MultiplayerBullet : MonoBehaviour
{
    // -={Private Variables}=-
    Rigidbody theRB;

    // -={Public Variables}=-
    public float bulletSpeed = 15f;
    public AudioClip BulletHitAudio;
    public GameObject bulletImpactEffect;
    public int damage = 10;

    [HideInInspector]
    public Photon.Realtime.Player owner;

    void Awake()
    {
        theRB = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection, Photon.Realtime.Player givenPlayer)
    {
        transform.forward = originalDirection;
        theRB.velocity = transform.forward * bulletSpeed;

        owner = givenPlayer;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position);
        VFXManager.Instance.PlayVFX(bulletImpactEffect, transform.position);
        Destroy(gameObject);
    }
}
