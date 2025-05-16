using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 10f;
    public float lifetime = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}