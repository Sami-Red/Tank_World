using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class Player_Multiplayer : MonoBehaviour, IPunObservable
{
    // -={Public Variables}=-
    public float movementSpeed = 10f;
    public float fireRate = 0.75f;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    public AudioClip playerShootingAudio;
    public GameObject bulletFiringEffect;


    [HideInInspector]
    public int health = 100;
    public Slider healthBar;

    // -={Private Variables}=-
    Rigidbody rb;
    float nextFire;


    PhotonView photonView;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if(photonView.IsMine)
        {
            CameraTracking cameraTracker = FindObjectOfType<CameraTracking>();
            if (cameraTracker != null)
            {
                cameraTracker.SetTarget(transform);
            }
        }
        Move();
        if (Input.GetKey(KeyCode.Space))
        {
            photonView.RPC("Fire", RpcTarget.AllViaServer);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            MultiplayerBullet bullet = collision.gameObject.GetComponent<MultiplayerBullet>();
            TakeDamage(bullet);
        }
    }
    void TakeDamage(MultiplayerBullet bullet)
    {
        health -= bullet.damage;
        healthBar.value = health;
        if (health <= 0)
        {
            bullet.owner.AddScore(1);
            PlayerDied();
        }
    }

    void PlayerDied()
    {
        if (!GetComponent<PhotonView>().IsMine) return;

        health = 100;
        healthBar.value = health;

       Respawn();
    }

    void Move()
    {
        if (ChatManager.instance.playerChatting)
        {
            return; 
        }
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        var rotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0, verticalInput));
        transform.rotation = rotation;

        Vector3 movementDir = transform.forward * Time.deltaTime * movementSpeed;
        rb.MovePosition(rb.position + movementDir);
    }
    [PunRPC]
    void Fire()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            GameObject bullet = Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
            bullet.GetComponent<MultiplayerBullet>()?.InitializeBullet(transform.rotation * Vector3.forward, photonView.Owner);
            AudioManager.Instance.Play3D(playerShootingAudio, transform.position);

            VFXManager.Instance.PlayVFX(bulletFiringEffect, bulletPosition.position);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
            healthBar.value = health;
        }
    }
    void Respawn()
    {
        Transform randomSpawnPoint = SpawnManager.Instance.GetRandomSpawnPoint();
        if (randomSpawnPoint != null)
        {
            transform.position = randomSpawnPoint.position;
            transform.rotation = randomSpawnPoint.rotation;
        }
    }
}