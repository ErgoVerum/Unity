using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carmina : MonoBehaviour
{
    public float MaxHealth,MaxSpeed,JumpForce,DashForce; 

    private bool FacingRight = true,FlipLocked,MovementLocked,DashLocked,JumpLocked,AttackLocked,BlockLocked,IsBlocking,ChainAttack,OnGroundBack,OnGroundFront;
    private float CurrentSpeed,CurrentHealth,H,MinWIdth,MaxWIdth,MinHeight,MaxHeight,DashH;
    private AudioSource AudioS;
    private AnimatorStateInfo CurAnim;  
    private Animator Anim;
    private Rigidbody2D RB;
    private Transform GCBack,GCFront;
    private int GLayer,PLayer,LMask,PlayerLayer;
    private RaycastHit2D PlatRayCast;
    private Collider2D GCColl,PlatColl;
    private SpriteRenderer SR;
    // Start is called before the first frame update
    void Start()
    {
        //Components
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator> ();
        AudioS = GetComponent<AudioSource>();
        SR = GetComponent<SpriteRenderer>();        
        //
        //Set Variables
        CurrentSpeed = MaxSpeed;CurrentHealth = MaxHealth;
        //
        //Ground Collision Mamagement
        GCBack = gameObject.transform.Find("GroundCheckBack");GCFront = gameObject.transform.Find("GroundCheckFront");
        GCColl = gameObject.transform.Find("Ground Collider").GetComponent<Collider2D>();
        PLayer  = LayerMask.NameToLayer("Platform");
        GLayer  = LayerMask.NameToLayer("Ground");
        PlayerLayer = LayerMask.NameToLayer("Player Ground Collider");
        LMask = (1 << GLayer) | (1 << PLayer);
        //
    }

    // Update is called once per frame
    void Update()
    {
        //Status Checks
        Anim.SetFloat("Speed", Mathf.Abs(H)); 
        CurAnim = Anim.GetCurrentAnimatorStateInfo(0);    
        //
        //On Ground Check
        OnGroundBack = Physics2D.Linecast(transform.position, GCBack.position, LMask);
        OnGroundFront = Physics2D.Linecast(transform.position, GCFront.position, LMask);
        //
        //Anim Checks
        if(CurAnim.IsName("Jump Loop")/* || CurAnim.IsName("Air Dash Back")*/){if(OnGroundFront || OnGroundBack){Anim.Play("Jump Land");}}
        if(!OnGroundBack && !OnGroundFront && !CurAnim.IsName("Jump Landing") && !CurAnim.IsName("Roll") && !CurAnim.IsName("BackRoll") && !CurAnim.IsName("Sprint Attack")&& !CurAnim.IsName("Jump Start")){Anim.Play("Jump Loop");}
        if(CurAnim.IsName("Jump") && RB.velocity.y < 0 && (OnGroundFront || OnGroundBack)){Anim.Play("Jump Landing");}
        //    
        /*Cam Border Limit*/RB.position = new Vector3(Mathf.Clamp(RB.position.x, MinWIdth + 0.5f, MaxWIdth - 0.5f),Mathf.Clamp(RB.position.y, MinHeight + 0.5f, MaxHeight - 0.5f));
        //Platform check
        PlatRayCast = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, 1 << LayerMask.NameToLayer("Platform"));
        if (PlatRayCast.collider != null){PlatColl = PlatRayCast.collider;}
        //
        if (H > 0 && !FacingRight && !FlipLocked){Flip ();}else if (H < 0 && FacingRight && !FlipLocked){Flip ();} /*No clue if can flip when move locked. Check Later*/
        //Movement
        if(!MovementLocked)
        {
        H = Input.GetAxisRaw ("Horizontal");
        RB.velocity = new Vector2 (H * CurrentSpeed, RB.velocity.y);
        //Platform Collider
        if (Input.GetAxis("Vertical") < 0 && PlatColl != null){Physics2D.IgnoreCollision(GCColl, PlatColl, true);}
        else if (PlatColl != null){Physics2D.IgnoreCollision(GCColl, PlatColl, false);}
        //
        if (Input.GetKeyDown(KeyCode.Space)){CheckJump();} 
        }
        /*Attack*/if(!AttackLocked && Input.GetButtonDown ("Fire1")){CheckAttack();}  
        //End of Movement
        //Dash
        if(!DashLocked){if (Input.GetKey(KeyCode.F)){DashH = Input.GetAxisRaw ("Horizontal");MovementLocked = true;FlipLocked = true;CheckDash();}}
        //
        //Block
        if(Input.GetKey(KeyCode.R) && !BlockLocked)
        if(!CurAnim.IsName("Block") && !CurAnim.IsName("Block Start"))
        {  }
        //
        //Light Cut
        if(Input.GetKey(KeyCode.Q)){Anim.Play("Spell Light Dash");}        
        //
    }

    private void FixedUpdate() 
    {
        //Camera
        MinWIdth = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x;MaxWIdth = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x;
        MinHeight = Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y;MaxHeight = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y;
        //
    }
    //Non Animation Functions
    void Flip()
	{
		FacingRight = !FacingRight;Vector3 scale = transform.localScale;
		scale.x *= -1;transform.localScale = scale;
        string CurrentAnim = Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        Anim.Play("Flip");        
	}
    void CheckJump()
    {if(OnGroundFront || OnGroundBack && !JumpLocked){RB.velocity += Vector2.up * JumpForce; Anim.Play("Jump Start");}}
    void CheckDash()
    {
        if(!DashLocked)
        {
            if(OnGroundFront || OnGroundBack && DashH != 0)RB.velocity = Vector2.zero;
            {switch (FacingRight)
            {
                case true:
                {if(DashH > 0){Anim.Play("Roll");RB.AddForce(Vector2.right * DashForce, ForceMode2D.Impulse);}
                else if (DashH < 0){Anim.Play("BackRoll");RB.AddForce(Vector2.left * DashForce, ForceMode2D.Impulse);}}
                break;
                case false:
                {if(DashH < 0){Anim.Play("Roll");RB.AddForce(Vector2.left * DashForce, ForceMode2D.Impulse);}
                else if (DashH > 0){Anim.Play("BackRoll");RB.AddForce(Vector2.right * DashForce, ForceMode2D.Impulse);}}
                break;                
            }}
        }
    }
    void SprintAttack()
    {
        if(FacingRight){RB.AddForce(Vector2.right * DashForce, ForceMode2D.Impulse);}else{RB.AddForce(Vector2.left * DashForce, ForceMode2D.Impulse);}
    }
    public void CheckAttack()
    {       
        string CurrentAnim = Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        switch (CurrentAnim)
        {
            case "Attack 1":ChainAttack = true;break;
            case "Attack 2":ChainAttack = true;break;
            case "Attack 3":ChainAttack = true;break;
            case "Attack 4":ChainAttack = true;break;
            case "Attack 5":ChainAttack = true;break;
            case "Attack 6":ChainAttack = true;break;
            case "Idle":Anim.Play("Attack 1");break;
            case "Flip":Anim.Play("Attack 1");break;
            case "Run Loop":if(H > 0 || H < 0){Anim.Play("Sprint Attack");SprintAttack();}else{Anim.Play("Attack 1");}break;
            case "Run Start":Anim.Play("Sprint Attack");SprintAttack();break;
            case "Run End":if(H > 0 || H < 0){Anim.Play("Sprint Attack");SprintAttack();}else{Anim.Play("Attack 1");}break;
        }
    }
    public void CheckChainAttack()
    {
        if(ChainAttack)
        {
            string CurrentAnim = Anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            switch (CurrentAnim)
            {
            case "Attack 1":Anim.Play("Attack 2");break;
            case "Attack 2":Anim.Play("Attack 3");break;
            case "Attack 3":Anim.Play("Attack 4");break;  
            case "Attack 4":Anim.Play("Attack 5");break;
            case "Attack 5":Anim.Play("Attack 6");break;    
            case "Attack 6":Anim.Play("Attack 1");break;            
            }
        }
            else{Anim.Play("Idle");}ChainAttack = false;
    }
    public void Damaged(int Damage)
    {
        if(CurAnim.IsName("Block") || CurAnim.IsName("Block Start")){Anim.Play("Block Hit");}
        else{
        CurrentHealth -= Damage;
        StartCoroutine (DamageBlinkRed());
        if(CurrentHealth <= 0){Anim.Play("Death");LockAllActions();/*GameManager.CallGameOverScene();*/}
        else{Anim.Play("Hurt");}}
    }
    IEnumerator DamageBlinkRed()
    {
        Color originalColor = SR.color;
        Color RedColor = Color.red;
        Color WhiteColor = Color.white;
        for (int i = 0; i < 1f; i++)
        {
            SR.color = RedColor;
            yield return new WaitForSeconds(0.1f);
            SR.color = WhiteColor;
            yield return new WaitForSeconds(0.1f);  
            SR.color = RedColor;    
            yield return new WaitForSeconds(0.1f); 
            SR.color = WhiteColor;
        }
    }
    IEnumerator SprintAttackDash()
    {float IntDir = 1;if(FacingRight){IntDir = 1;}else{IntDir = -1;}
    RB.velocity = new Vector2(20f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.2f);RB.velocity = new Vector2(10f * IntDir, RB.velocity.y);
    yield return new WaitForSeconds(0.1f);RB.velocity = new Vector2(5f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.05f);RB.velocity = new Vector2(0f, RB.velocity.y);}
    IEnumerator LightCutDash()
    {float IntDir = 1;if(FacingRight){IntDir = 1;}else{IntDir = -1;}
    RB.velocity = new Vector2(50f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.2f);RB.velocity = new Vector2(5f * IntDir, RB.velocity.y);
    yield return new WaitForSeconds(0.1f);RB.velocity = new Vector2(0f, RB.velocity.y);}
    
    
    //Animation Functions
    public void ReturnToIdle(){Anim.Play("Idle");}
    public void UnlockAllMovement(){MovementLocked = false;FlipLocked = false;JumpLocked = false;DashLocked = false;AttackLocked = false;ChainAttack = false;BlockLocked = false;}
    public void DashActionsLock(){FlipLocked = true;DashLocked = true;MovementLocked = true;ChainAttack = false;}
    public void SprintAttackLock(){RB.velocity = Vector2.zero;FlipLocked = true;MovementLocked = true;JumpLocked = true;SprintAttack();}
    public void HurtActionsLock(){RB.velocity = Vector2.zero;FlipLocked = true;MovementLocked = true;JumpLocked = true;AttackLocked = true;ChainAttack = false;}
    public void LockAllActions(){RB.velocity = Vector2.zero;FlipLocked = true;MovementLocked = true;JumpLocked = true;AttackLocked = true;DashLocked = true;ChainAttack = false;}
    public void AttackActionsLock(){MovementLocked = true;JumpLocked = true;}
    public void BlockActionsLock(){MovementLocked = true;}
    public void LightCutActionsLock(){MovementLocked = true;FlipLocked = true;JumpLocked = true;AttackLocked = true;ChainAttack = false;DashLocked = true;RB.velocity = Vector2.zero;}
}
