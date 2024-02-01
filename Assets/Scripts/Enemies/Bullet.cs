using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public float liveTime = 10f;
    public float damage = 0f;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] clips;

    private void Start()
    {
        Destroy(this.gameObject, liveTime);
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<ObjectHealth>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }*/

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.gameObject.GetComponent<ObjectHealth>().TakeDamage(damage);
            audioSource.Stop();
            audioSource.clip = clips[0];
            audioSource.volume = 0.2f;
            audioSource.pitch = 1;
            float add = (Random.Range(0, 20) - 10) / 100.0f;
            audioSource.pitch += add;
            audioSource.Play();

            GetComponent<MeshRenderer>().enabled = false;
            Destroy(this.gameObject, 0.5f);
        }
    }
}
