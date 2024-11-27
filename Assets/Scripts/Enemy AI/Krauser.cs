using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Krauser : MonoBehaviour
{
    public int MaxHealth;
    public float MaxSpeed,AttackRate,AttackRange;
    public AudioClip TookHit, DeathSound;
    public Slider HealthBar;

    private Rigidbody2D RB;private Animator Anim;private SpriteRenderer SR;private CapsuleCollider2D BC;private AudioSource AudioS;
    private bool FacingRight = true,FlipLock,MovementLock,SRAL,LRAL,AttackLock,WithinYRange,StaggerLock;
    private float Speed,H,NextCloseAttack,NextLongAttack,PlayerYPos,YPos,TimerUntilWander,CamLeftEdge,CamRightEdge,HalfHeight,HalfWidth,YDistance;
    private Transform PlayerTarget;
    private Vector2 TargetDistance;
    private Carmina PS;
    private int CurHealth;
    // SRAL = Short Range Attack Lock | LRAL = Long Range Attack Lock
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();Anim = GetComponent<Animator>();SR = GetComponent<SpriteRenderer>();
        BC = GetComponent<CapsuleCollider2D>();AudioS = GetComponent<AudioSource>();
        CurHealth = MaxHealth;Speed = MaxSpeed;
        PlayerTarget = FindObjectOfType<Carmina>().transform;
        PS = FindObjectOfType<Carmina>().GetComponent<Carmina>();
        Camera MainCam = Camera.main;
        HalfHeight = MainCam.orthographicSize;HalfWidth = MainCam.aspect * HalfHeight;
        CamLeftEdge = MainCam.transform.position.x - HalfWidth;CamRightEdge = MainCam.transform.position.x + HalfWidth;
        HealthBar.maxValue = MaxHealth;HealthBar.value = CurHealth;
        LRAL = true;
        NextLongAttack = Time.time + AttackRate;
    }

    // Update is called once per frame
    void Update()
    {
        //Checks
        Anim.SetFloat("Speed", Mathf.Abs(Speed));
        TargetDistance = PlayerTarget.position - transform.position;
        H = TargetDistance.x / Mathf.Abs(TargetDistance.x);
        //
        //Chase Player
        if (Mathf.Abs(TargetDistance.x) <= AttackRange){Speed = 0;SRAL = false;}
        else if(Mathf.Abs(TargetDistance.x) >= AttackRange && !MovementLock){Speed = MaxSpeed;RB.velocity = new Vector2(H * Speed, RB.velocity.y);SRAL = true;}
        //

        /*Close Range Attack*/if (!SRAL && !AttackLock && Time.time > NextCloseAttack){AttackPlayerCloseRange();}
        /*Long Range Attack*/ if (!LRAL && !AttackLock && Time.time > NextLongAttack){AttackPlayerLongRange();}
        /*Flip*/if (H > 0 && !FacingRight && !FlipLock){Flip();}else if (H < 0 && FacingRight && !FlipLock){Flip();}               
    }

    void Flip()
	{
		FacingRight = !FacingRight;
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
	}
    public void Damaged(int Damage)
    {
        CurHealth -= Damage;
        HealthBar.value = CurHealth;
        StartCoroutine (DamageBlinkRed());
        if(CurHealth <= 0){Anim.Play("Death");}
        else{if(!StaggerLock){Anim.Play("Hurt");}}
    }
    public void AttackPlayerCloseRange()
    {Anim.Play("2ComboAttack");NextCloseAttack = Time.time + AttackRate;}
    public void AttackPlayerLongRange()
    {Anim.Play("Spin Attack");NextLongAttack = Time.time + AttackRate;}
    IEnumerator SprintDash()
    {float IntDir = 1;if(FacingRight){IntDir = 1;}else{IntDir = -1;}
    RB.velocity = new Vector2(20f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.2f);RB.velocity = new Vector2(10f * IntDir, RB.velocity.y);
    yield return new WaitForSeconds(0.1f);RB.velocity = new Vector2(5f * IntDir, RB.velocity.y);yield return new WaitForSeconds(0.05f);RB.velocity = new Vector2(0f, RB.velocity.y);}
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

    //Animation Checks
    public void ReturnToIdle(){Anim.Play("Idle");}
    public void ResetActionsValues(){AttackLock = false; MovementLock = false;FlipLock = false;SRAL = false;}
    public void AttackActionsLock(){MovementLock = true;AttackLock = false;FlipLock = true;}
    public void DashLock(){FlipLock = false;MovementLock = true;AttackLock = true;}
    public void HurtActionsLock(){FlipLock = true;MovementLock = true;AttackLock = true;}
    public void DestroyENemy(){Destroy(gameObject);}
}
