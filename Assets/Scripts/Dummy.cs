using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    private Animator Anim;
    private SpriteRenderer SR;

    private void Start(){Anim = GetComponent<Animator>();SR = GetComponent<SpriteRenderer>();}

    public void Damaged(int Damage){AnimatorStateInfo CurAnim = Anim.GetCurrentAnimatorStateInfo(0);
    if(CurAnim.IsName("Hit")){Anim.Play("Hit", -1, 0f);}else{Anim.Play("Hit");}StartCoroutine (DamageBlinkRed());}
    IEnumerator DamageBlinkRed()
    {
        Color originalColor = SR.color;
        Color RedColor = Color.red;
        Color WhiteColor = Color.white;
        for (int i = 0; i < 2; i++)
        {
            SR.color = RedColor;
            yield return new WaitForSeconds(0.05f);
            SR.color = WhiteColor;
            yield return new WaitForSeconds(0.05f);  
            SR.color = originalColor;    
            yield return new WaitForSeconds(0.05f); 
        }
    }
}
