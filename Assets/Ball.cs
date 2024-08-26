using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hitSound;
    [SerializeField] Rigidbody rb;
    float minVelocityForSound = 4f;
    public int place;
    bool soundIsPlaying;
    Vector3 startingPosition, teleportPosition = new Vector3(0, 100, 0);

    void Start()
    {
        startingPosition = transform.position;
    }

    void OnCollisionEnter(Collision x)
    {
        float relativeVelocity = x.relativeVelocity.magnitude;
        if (relativeVelocity >= minVelocityForSound && !soundIsPlaying)
        {
            soundIsPlaying = true;
            audioSource.PlayOneShot(hitSound);
            Invoke("ResetIsPlayingFlag", hitSound.length);
        }

        if (x.gameObject.CompareTag("Ground"))
        {
            rb.isKinematic = true;
            transform.position = startingPosition;
            gameManager.Finished(name);
            place = gameManager.FinishersCount();
        }
    }
    void ResetIsPlayingFlag() { soundIsPlaying = false; }

    public void Teleport()
    {
        place = 0;
        rb.isKinematic = false;
        transform.position = teleportPosition + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f));
    }
}