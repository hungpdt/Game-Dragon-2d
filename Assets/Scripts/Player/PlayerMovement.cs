using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float gravity;
    [Space(5)]

    [Header("Dash")]
    private bool canDash = true;
    private bool dashed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCoolDown;
    [SerializeField] private float dashSpeed;
    [Space(5)]

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge
    [Space(5)]

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;
    [Space(5)]

    [Header("Dash effect")]
    [SerializeField] private GameObject dashAnimation;
    [Space(5)]

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force
    //[SerializeField] private float wallSlidingSpeed;
    [SerializeField] private float gravityWhenSliding;
    [Space(5)]

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [Space(5)]

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D rigiBody2dPlayer;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    public Room currentRoom{get; set;}

    private bool isLeft;
    private bool isRight;
    private bool isUp;
    private bool jumpRequest;
    private PlayerStateList playerStateList;
    public static PlayerMovement Instance;
    private HealthPlayer playerHealth;

    private void Awake()
    {
        if(Instance != null && Instance != this){
            Destroy(Instance);
        }else{
            Instance = this;
        }
        //Grab references for rigidbody and animator from object
        playerStateList = GetComponent<PlayerStateList>();
        rigiBody2dPlayer = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        GetInput();
        Flip();
#endif

        //Set animator parameters
        anim?.SetBool("run", horizontalInput != 0);
        anim?.SetBool("grounded", isGrounded());

        if(isLeft){
            anim?.SetBool("run", true);
            transform.Translate(-Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if(isRight){
            anim?.SetBool("run", true);
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.localScale = Vector3.one;
        }

        //Dash
        if (playerStateList.dash)
        {
            //Debug.Log("playerStateList.dash = true ");
            return;
        }

        Jump();
        StartDash();
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || (isUp && jumpRequest))
        {
            anim?.SetBool("run", false);
            
            if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0){
                //Debug.Log(" ========= return ===========");
                return;
            } 
            //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

            SoundManager.instance.PlaySound(jumpSound);
            anim.SetTrigger("jump");
            if (onWall()){
                print("wall jump");
                WallJump();
            }
            else
            {
                playerStateList.jump = true;
                if (isGrounded()){
                    //print("isGrounded");
                    rigiBody2dPlayer.velocity = new Vector2(rigiBody2dPlayer.velocity.x, jumpPower);
                }
                else
                {
                    //print("coyote");
                    //If not on the ground and coyote counter bigger than 0 do a normal jump
                    if (coyoteCounter > 0)
                        rigiBody2dPlayer.velocity = new Vector2(rigiBody2dPlayer.velocity.x, jumpPower);
                    else
                    {
                        if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                        {
                            rigiBody2dPlayer.velocity = new Vector2(rigiBody2dPlayer.velocity.x, jumpPower);
                            jumpCounter--;
                        }
                    }
                }
                //Reset coyote counter to 0 to avoid double jumps
                coyoteCounter = 0;
            }

            jumpRequest = false;
        }

        //Adjustable jump height
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Space) && rigiBody2dPlayer.velocity.y > 0)
#elif UNITY_ANDROID
            if (isUp && jumpRequest && body.velocity.y > 0)
#endif
            rigiBody2dPlayer.velocity = new Vector2(rigiBody2dPlayer.velocity.x, rigiBody2dPlayer.velocity.y / 2);

        if (onWall())
        {
            rigiBody2dPlayer.gravityScale = gravityWhenSliding;
            //body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
            rigiBody2dPlayer.velocity = Vector2.zero;
        }
        else
        {
            rigiBody2dPlayer.gravityScale = gravity;
            rigiBody2dPlayer.velocity = new Vector2(horizontalInput * speed, rigiBody2dPlayer.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        }
    }
    private void Flip()
    {
        //Flip player when moving left-right
        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    private void GetInput()  {
        horizontalInput = Input.GetAxis("Horizontal");
        //Debug.Log("horizontal input = "+ horizontalInput);
    }
    private void StartDash(){
        if(Input.GetKeyDown(KeyCode.F) && canDash && !dashed){
            //Debug.Log("StartDash ");
            StartCoroutine(Dash());
            dashed = true;
        }

        if (isGrounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash(){
        canDash = false;
        playerStateList.dash = true;
        anim.SetTrigger("dash");
        rigiBody2dPlayer.gravityScale = 0;
        rigiBody2dPlayer.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        if(isGrounded()){
            Instantiate(dashAnimation, transform);
        }
        yield return new WaitForSeconds(dashTime);
        playerStateList.dash = false;
        rigiBody2dPlayer.gravityScale = gravity;

        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }
    public void LeftButtonDown(){
        isLeft = true;
        isRight = false;
    }

    public void RightButtonDown(){
        isLeft = false;
        isRight = true;
    }

    public void UpButton(){
        isRight = false;
        isLeft = false;
        isUp = false;
        jumpRequest = false;
    }

    public void JumpButton(){
        isUp = true;
        jumpRequest = true;
    }

    private void WallJump()
    {
        rigiBody2dPlayer.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        //body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
        print("velocity.x =" + rigiBody2dPlayer.velocity.x + " velocity.y" + rigiBody2dPlayer.velocity.y);
        wallJumpCooldown = 0;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        if(raycastHit.collider != null){
            //Debug.Log(" player is in grounded");
            return true;
        } else{
            //Debug.Log("player is NOT in grounded");
            return false;
        }
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !onWall();
    }
}