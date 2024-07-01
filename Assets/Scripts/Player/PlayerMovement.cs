using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime; //How much time the player can hang in the air before jumping
    private float coyoteCounter; //How much time passed since the player ran off the edge

    [Header("Multiple Jumps")]
    [SerializeField] private int extraJumps;
    private int jumpCounter;

    [Header("Wall Jumping")]
    [SerializeField] private float wallJumpX; //Horizontal wall jump force
    [SerializeField] private float wallJumpY; //Vertical wall jump force
    //[SerializeField] private float wallSlidingSpeed;
    [SerializeField] private float gravityWhenSliding; 

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;

    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    public Room currentRoom{get; set;}

    private bool isLeft;
    private bool isRight;
    private bool isUp;
    private bool jumpRequest;

    private void Awake()
    {
        //Grab references for rigidbody and animator from object
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
    #if UNITY_EDITOR
        horizontalInput = Input.GetAxis("Horizontal");
        //Flip player when moving left-right
        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    #endif

        //Set animator parameters
        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if(isLeft){
            anim.SetBool("run", true);
            transform.Translate(-Vector2.right * speed * Time.deltaTime);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if(isRight){
            anim.SetBool("run", true);
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            transform.localScale = Vector3.one;
        }
    
        //Jump
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || (isUp && jumpRequest)){
            anim.SetBool("run", false);
            Jump();
            jumpRequest = false;
        }

        //Adjustable jump height
        #if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.Space) && body.velocity.y > 0)
        #elif UNITY_ANDROID
            if (isUp && jumpRequest && body.velocity.y > 0)
        #endif
            body.velocity = new Vector2(body.velocity.x, body.velocity.y / 2);

        if (onWall())
        {
            body.gravityScale = gravityWhenSliding;
            //body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
            body.velocity = Vector2.zero;
        }
        else
        {
            body.gravityScale = 7;
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (isGrounded())
            {
                coyoteCounter = coyoteTime; //Reset coyote counter when on the ground
                jumpCounter = extraJumps; //Reset jump counter to extra jump value
            }
            else
                coyoteCounter -= Time.deltaTime; //Start decreasing coyote counter when not on the ground
        }
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
    }

    public void JumpButton(){
        isUp = true;
        jumpRequest = true;
    }
    private void Jump()
    {
        if (coyoteCounter <= 0 && !onWall() && jumpCounter <= 0){
            //print("return");
            return;
        } 
        //If coyote counter is 0 or less and not on the wall and don't have any extra jumps don't do anything

        SoundManager.instance.PlaySound(jumpSound);

        if (onWall()){
            //print("wall jump");
            WallJump();
        }
        else
        {
            if (isGrounded()){
                //print("isGrounded");
                body.velocity = new Vector2(body.velocity.x, jumpPower);
            }
            else
            {
                //print("coyote");
                //If not on the ground and coyote counter bigger than 0 do a normal jump
                if (coyoteCounter > 0)
                    body.velocity = new Vector2(body.velocity.x, jumpPower);
                else
                {
                    if (jumpCounter > 0) //If we have extra jumps then jump and decrease the jump counter
                    {
                        body.velocity = new Vector2(body.velocity.x, jumpPower);
                        jumpCounter--;
                    }
                }
            }

            //Reset coyote counter to 0 to avoid double jumps
            coyoteCounter = 0;
        }
    }

    private void WallJump()
    {
        body.AddForce(new Vector2(-Mathf.Sign(transform.localScale.x) * wallJumpX, wallJumpY));
        //body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlidingSpeed, float.MaxValue));
        print("velocity.x =" + body.velocity.x + " velocity.y" + body.velocity.y);
        wallJumpCooldown = 0;
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
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