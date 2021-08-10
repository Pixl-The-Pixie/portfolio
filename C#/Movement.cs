using UnityEngine;

public class Movement : MonoBehaviour
{
    public MoveSettings moveSettings = new MoveSettings();
    public PhysSettings physSettings = new PhysSettings();
    public InputSettings inputSettings = new InputSettings();

    //An accessable class of floats that control movement based control configs.
    [System.Serializable]
    public class MoveSettings
    {
        //smoothing is(as the name implies) used to smooth out vector based movement to prevent choppy movements.
        //NOTE TO SELF: change sprintmod, disttogrounded, and dirmove and jumpinput to internal once finished debugging.
        internal float Smoothing = .05f;
        public bool idle = true;
        public float SprintMod = 1.75f;
        public float speed = 1.0f;
        public float jumpVel = 1.3f;
        public float distToGrounded = 1.01f;
        //These two variables are included in here for the sake of easy debug.
        public float DirMove, jumpInput;
    }

    //an accessable class that allows debug of the IsGrounded bool as well as the config of ground layers. It also allows debug of the velocity3 vector.
    [System.Serializable]
    public class PhysSettings
    {
        public ContactFilter2D Filter;
        public bool IsGrounded;
        public Vector3 velocity3 = Vector3.zero;
        public Rigidbody2D rb;
    }

    //an accessable class that allows debug and config of inputs.
    [System.Serializable]
    public class InputSettings
    {
        public float inputDelay = 0.1f;
        public string LRCont = "Horizontal";
        public string JmpCont = "Jump";

    }
    
    //a classless animator variable to store data and interact with the animation controller.
    public Animator animator;

    /*A function called upon at the start of the program. it initializes the jump and directional movement input variables,
    as well as initializing the sprites rigidbody asset.*/
    void Start()
    {
        physSettings.rb = GetComponent<Rigidbody2D>();
        moveSettings.DirMove = moveSettings.jumpInput = 0;
    }

    /*This function is called once per frame. When it is called, input is received through the GetInput() function, 
     as well as the function checking if the sprite is making contact with a ground layer, and updating the IsGrounded bool
    accordingly. This function also relays this information to the animator to trigger animations.*/
    void Update()
    {
        GetInput();
        physSettings.IsGrounded = physSettings.rb.IsTouching(physSettings.Filter);
        animator.SetBool("Grounded", physSettings.IsGrounded);
        animator.SetFloat("Speed", moveSettings.DirMove);
        animator.SetFloat("Jump Input", moveSettings.jumpInput);

    }

    /*This function is called once every 0.02 seconds(50 times per second), and is used for physics calculations. 
     When it is called*/
    void FixedUpdate()
    {
        Move();
        Jump();
    }

    /*Function called by the update void. This function checks to see for directional and jump input and stores them in their
     respective variables.*/
    void GetInput()
    {
        moveSettings.DirMove = Input.GetAxis(inputSettings.LRCont);
        moveSettings.jumpInput = Input.GetAxisRaw(inputSettings.JmpCont);
    }

    /*This function controls player movement and is called by the fixed update void. It checks to see if the sprint key is 
     being pressed, and if there is a movement key being pressed. If there is, it moves the player accordingly, and factors
    in the sprint mod if the sprint key is pressed. If no key is pressed, the idle bool is set to true. */
    void Move()
    {
        if (moveSettings.DirMove != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            //this is a nested if to prevent pointless reasignation of variables.
            if (moveSettings.idle == true)
            {
                moveSettings.idle = false;
            }
            Vector3 targetVelocity = new Vector2(moveSettings.DirMove * moveSettings.speed * moveSettings.SprintMod, physSettings.rb.velocity.y);
            physSettings.rb.velocity = Vector3.SmoothDamp(physSettings.rb.velocity, targetVelocity, ref physSettings.velocity3, moveSettings.Smoothing);
        }
        else if (moveSettings.DirMove != 0)
        {
            //this is a nested if to prevent pointless reasignation of variables.
            if (moveSettings.idle == true)
            {
                moveSettings.idle = false;
            }
            Vector3 targetVelocity = new Vector2(moveSettings.DirMove * moveSettings.speed, physSettings.rb.velocity.y);
            physSettings.rb.velocity = Vector3.SmoothDamp(physSettings.rb.velocity, targetVelocity, ref physSettings.velocity3, moveSettings.Smoothing);
        }
        //this is an elif rather than just an else to prevent pointlessly trying to reasign the variable to true when it already is.
        else if (moveSettings.idle == false)
        {
            moveSettings.idle = true;
        }
    }

    /*This function is called by the fixed update void. It checks if the sprite is grounded and if the player is providing jump input
     and causes sprite movement accordingly.*/
    void Jump()
    {
        Vector2 Jump = new Vector2(0, moveSettings.jumpVel);
        if (moveSettings.jumpInput != 0 && physSettings.IsGrounded == true)
        {
            physSettings.rb.AddForce(Jump, ForceMode2D.Impulse);
        }

    }
}
