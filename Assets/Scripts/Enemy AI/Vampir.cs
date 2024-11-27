using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vampir : MonoBehaviour
{
    public int MaxHealth;
    public float MaxSpeed,AttackRate,AttackRange;
    public AudioClip TookHit, DeathSound;
    public Slider HealthBar;

    private Rigidbody2D RB;private Animator Anim;private SpriteRenderer SR;private CapsuleCollider2D BC;private AudioSource AudioS;
    private bool FacingRight = true,FlipLock,MovementLock,AttackLock,WithinAttackRange,WithinDashRange;
    private float Speed,H,NextAttack,PlayerYPos,YPos,YDistance;
    private Transform PlayerTarget;
    private Vector2 TargetDistance;
    private Carmina PS;
    private int CurHealth;
    private Transform SlashSprite;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();Anim = GetComponent<Animator>();SR = GetComponent<SpriteRenderer>();
        BC = GetComponent<CapsuleCollider2D>();AudioS = GetComponent<AudioSource>();
        CurHealth = MaxHealth;Speed = MaxSpeed;
        PlayerTarget = FindObjectOfType<Carmina>().transform;
        PS = FindObjectOfType<Carmina>().GetComponent<Carmina>();
        HealthBar.maxValue = MaxHealth;HealthBar.value = CurHealth;
        SlashSprite = gameObject.transform.Find("Slash");
    }

    // Update is called once per frame
    void Update()
    {
         //Checks
        Anim.SetFloat("Speed", Mathf.Abs(Speed));
        TargetDistance = PlayerTarget.position - transform.position;
         /*Flip*/if (H > 0 && !FacingRight && !FlipLock){Flip();}else if (H < 0 && FacingRight && !FlipLock){Flip();}
        //
        //Chase Player
        H = TargetDistance.x / Mathf.Abs(TargetDistance.x);
        /*Check Long Range Reach*/ if (Mathf.Abs(TargetDistance.x) > AttackRange && Mathf.Abs(TargetDistance.x) <= 6f){WithinDashRange = true;}else{WithinDashRange = false;}
        /*Check Close Range Reach*/if (Mathf.Abs(TargetDistance.x) <= AttackRange){Speed = 0;WithinAttackRange = true;}
        else if(Mathf.Abs(TargetDistance.x) >= AttackRange && !MovementLock){Speed = MaxSpeed;RB.velocity = new Vector2(H * Speed, RB.velocity.y);WithinAttackRange = false;}
        //

       /* Attack*/if(WithinAttackRange && !AttackLock && Time.time > NextAttack){AttackPlayer();}
       /*Long Range Attack*/if(WithinDashRange && !AttackLock && Time.time > NextAttack){Anim.Play("Dash Attack");NextAttack = Time.time + AttackRate;}
    }

    //Functions
    void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
    public void Damaged(int Damage)
    {
        int RNGDodge = Random.Range(0,7);
        if(RNGDodge == 1){Roll();}
        else
        {
        CurHealth -= Damage;
        HealthBar.value = CurHealth;
        if(CurHealth <= 0){Anim.Play("Death");}
        else{Anim.Play("Hurt");StartCoroutine (DamageBlinkRed());}
        }
    }
    void AttackPlayer()
    {Anim.Play("Attack");NextAttack = Time.time + AttackRate;}
    void Roll()
    {
        Anim.Play("Roll");
        if(FacingRight){RB.AddForce(Vector2.right * 10000, ForceMode2D.Impulse);}
        else{RB.AddForce(Vector2.left * 10000, ForceMode2D.Impulse);}
    }
    public void SlashSet()
    {
        float RandomZ = Random.Range(-360f, 360f);SlashSprite.localRotation = Quaternion.Euler(0, 0, RandomZ);
    }
    IEnumerator DamageBlinkRed()
    {
        Color originalColor = SR.color;
        Color RedColor = Color.red;
        Color WhiteColor = Color.white;
        for (int i = 0; i < 3; i++)
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
    IEnumerator SprintDash()
    {float IntDir = 1;if(FacingRight){IntDir = 1;}else{IntDir = -1;}
    RB.velocity = new Vector2(20f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.2f);RB.velocity = new Vector2(10f * IntDir, RB.velocity.y);
    yield return new WaitForSeconds(0.1f);RB.velocity = new Vector2(5f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.05f);RB.velocity = new Vector2(0f, RB.velocity.y);}
    
//Animation Checks
    public void ReturnToIdle(){Anim.Play("Idle");}
    public void ResetActionsValues(){AttackLock = false; MovementLock = false;FlipLock = false;}
    public void AttackActionsLock(){MovementLock = true;AttackLock = true;FlipLock = true;}
    public void RollActionsLock(){FlipLock = true;MovementLock = true;AttackLock = true;}
    public void CounterStrikeLock(){FlipLock = false;MovementLock = true;AttackLock = true;}
    public void HurtActionsLock(){FlipLock = true;MovementLock = true;AttackLock = true;}
    public void DestroyENemy(){Destroy(gameObject);}
}
