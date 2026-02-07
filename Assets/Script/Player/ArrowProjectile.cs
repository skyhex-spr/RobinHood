using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArrowProjectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifetime;

    private Rigidbody2D _rb;
    private Vector2 _direction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    public void Init(Vector2 direction)
    {
        _direction = direction.normalized;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (_direction.x >= 0 ? 1f : -1f);
        transform.localScale = scale;

        _rb.linearVelocity = _direction * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }

}
