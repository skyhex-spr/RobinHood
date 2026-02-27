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

    [Header("Shoot")]
    public ArrowProjectile ArrowPrefab;
    public Transform ShootPoint;
    public float Shootcooldown;

    private float shootTimer;


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

    private void Update()
    {
        if(shootTimer > 0)
            shootTimer -= Time.deltaTime;

        if (InputController.ShootPressed && IsGround)
        {
            animator.SetTrigger("shoot");
        }
        if (InputController.MaleePressed && IsGround)
        {
            animator.SetTrigger("malee");
        }
    }

    private void movement()
    {
        if (IsShootingAnimation() || IsMaleeAnimation())
        {
            rig.linearVelocityX = 0;
            return;
        }

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
        if (IsShootingAnimation() || IsMaleeAnimation())
            return;

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

    private bool IsShootingAnimation()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsTag("shoot"))
            return true;
        else
            return false;
    }

    private bool IsMaleeAnimation()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsTag("malee"))
            return true;
        else
            return false;
    }

    public void Hit()
    {
        animator.SetTrigger("hit");
    }

    public void TryShootArrow()
    {
        if (ArrowPrefab == null)
            return;

        if (shootTimer > 0)
            return;

        shootTimer = Shootcooldown;

        Vector2 dir;

        if (Sprite != null && Sprite.flipX)
            dir = Vector2.left;
        else 
            dir = Vector2.right;

        Vector3 spawnPos = ShootPoint.position;

        ArrowProjectile arrow = Instantiate(ArrowPrefab, spawnPos, Quaternion.identity);
        arrow.Init(dir);
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
