using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]


public class PlaneController : MonoBehaviour
{
    public enum AircraftInputMode
    {
        Auto,
        Mouse,
        Touch
    }

    public float forwardSpeed = 10f;
    public float moveSpeed = 5f;

    public AudioClip flySound;
    public AudioClip collisionSound;
    public AudioClip passSound;

    public Rigidbody rb;
    public AudioSource flyAudio;
    public AudioSource sfxAudio;

    private Vector2 moveInput;
    public bool isCrashed = false;

    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField]
    private AircraftInputMode inputMode = AircraftInputMode.Auto;

    private bool waitForInputRelease = false;

    int score = 0;

    private void Awake()
    {
#if UNITY_EDITOR
        Input.simulateMouseWithTouches = false;
#endif
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

        gameOver.SetActive(false);

    }


    void Start()
    {
        if (flySound != null)
        {
            flyAudio.Play();
        }
    }


    void OnEnable()
    {
        moveInput = Vector2.zero;
    }

    void OnDisable()
    {
        moveInput = Vector2.zero;
    }



    private void Update()
    {
        if (isCrashed)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = Vector2.zero;

        Vector2 inputPosition = Vector2.zero;
        bool isPressed = false;

        // Optional: prevent a held restart touch from
        // controlling the aircraft after scene reload.
        if (waitForInputRelease)
        {
            if (Input.touchCount == 0 &&
                !Input.GetMouseButton(0))
            {
                waitForInputRelease = false;
            }

            return;
        }

        bool allowMouse = inputMode != AircraftInputMode.Touch;
        bool allowTouch = inputMode != AircraftInputMode.Mouse;

        // Mouse takes priority in Auto mode.
        if (allowMouse && Input.GetMouseButton(0))
        {
            bool mouseOverUI =
                EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject();

            if (!mouseOverUI)
            {
                inputPosition = Input.mousePosition;
                isPressed = true;
            }
        }

        // Touch input is used if no mouse input was accepted.
        if (!isPressed && allowTouch && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Ended ||
                    touch.phase == TouchPhase.Canceled)
                    continue;

                bool touchOverUI =
                    EventSystem.current != null &&
                    EventSystem.current.IsPointerOverGameObject(
                        touch.fingerId
                    );

                if (touchOverUI)
                    continue;

                inputPosition = touch.position;
                isPressed = true;
                break;
            }
        }

        // Calculate movement using the 3x3 screen grid.
        if (isPressed)
        {
            float x = Mathf.Clamp01(
                inputPosition.x / Screen.width
            );

            float y = Mathf.Clamp01(
                inputPosition.y / Screen.height
            );

            moveInput.x =
                x < 1f / 3f ? -1f :
                x > 2f / 3f ? 1f : 0f;

            moveInput.y =
                y < 1f / 3f ? -1f :
                y > 2f / 3f ? 1f : 0f;

            moveInput = Vector2.ClampMagnitude(
                moveInput, 1f
            );
        }
        else
        {
            // Keyboard fallback
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            moveInput = Vector2.ClampMagnitude(
                moveInput, 1f
            );
        }
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

            gameOver.SetActive(true);

            Debug.Log("Crashed!");
        }
    }

    public void PassObstacle()
    {
        if (isCrashed) return;

        if (passSound != null)
            sfxAudio.PlayOneShot(passSound);

        score++;
        scoreText.text = score.ToString();

        Debug.Log("Passed!");
    }
}
