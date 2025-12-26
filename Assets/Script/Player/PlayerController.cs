using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public PlayerInputController InputController;

    public Rigidbody2D rig;

    public float MoveSpeed;

    public SpriteRenderer Sprite;

    public float JumpForce = 10;

    public bool IsGround;

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputController = GetComponent<PlayerInputController>();

        Sprite = GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rig.linearVelocityX = (InputController.MoveData * MoveSpeed) * Time.deltaTime;


        //right
        if (InputController.MoveData == 1)
        {
            Sprite.flipX = false;
            animator.SetInteger("Movement", 1);
        }
        //left
        else if (InputController.MoveData == -1)
        {
            Sprite.flipX = true;
            animator.SetInteger("Movement", 1);
        }
        else
        {
            animator.SetInteger("Movement", 0);
        }

        if (InputController.Isjumping && IsGround)
        {
            rig.linearVelocity = new Vector2(0, JumpForce);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGround = false;
        }
    }
}
