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

    public bool OnJump;
    public bool OnFall;
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
        movement();
        jump();
    }

    private void movement()
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
    }

    private void jump()
    {
        if (InputController.Isjumping && IsGround)
        {
            rig.linearVelocity = new Vector2(0, JumpForce);
            animator.SetBool("jump",true);
            OnJump = true;
        }

        if (rig.linearVelocityY < 0 && !IsGround)
        {
            OnFall = false;
            animator.SetBool("fall", true);
            animator.SetBool("jump", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            IsGround = true;
            animator.SetBool("fall", false);
            animator.SetBool("jump", false);
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
