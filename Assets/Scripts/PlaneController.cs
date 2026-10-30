using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlaneController : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float moveSpeed = 5f;

    public AudioClip flySound;
    public AudioClip collisionSound;
    public AudioClip passSound;

    public Rigidbody rb;
    public AudioSource flyAudio;
    public AudioSource sfxAudio;

    private Vector2 moveInput;
    private bool isCrashed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        flyAudio = GetComponent<AudioSource>();
        flyAudio.clip = flySound;
        flyAudio.loop = true;
        flyAudio.playOnAwake = false;
        flyAudio.spatialBlend = 0f;

        sfxAudio = GetComponent<AudioSource>();
        sfxAudio.playOnAwake = false;
        sfxAudio.spatialBlend = 0f;

    }


    void Start()
    {
        if (flySound != null)
        {
            flyAudio.Play();
        }
    }

    void Update()
    {
        if (isCrashed) return;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        if (isCrashed) return;
        Vector3 movement = new Vector3(
            moveInput.x * moveSpeed,
            moveInput.y * moveSpeed,
            forwardSpeed
            );

        rb.MovePosition(
            rb.position + movement * Time.fixedDeltaTime
         );
    }

    private void OnTriggerEnter(Collider other) 
    { 
        if (isCrashed) return;

        if (other.gameObject.CompareTag("Obstacle"))
        {
            isCrashed = true;
            flyAudio.Stop();

            if (collisionSound != null)
                sfxAudio.PlayOneShot(collisionSound);

            Debug.Log("Crashed!");
        }
    }

    public void PassObstacle()
    {
        if (isCrashed) return;

        if (passSound != null)
            sfxAudio.PlayOneShot(passSound);

        Debug.Log("Passed!");
    }
}
