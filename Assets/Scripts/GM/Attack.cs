using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int Damage;

    private Carmina Player;
    private Dummy Dum;
    private Vampir Vamp;
    private Krauser krauser;



    private void OnTriggerEnter2D(Collider2D other)
    {        
        Player = other.GetComponent<Carmina>();
        Dum = other.GetComponent<Dummy>();     
        Vamp = other.GetComponent<Vampir>();       
        krauser  = other.GetComponent<Krauser>();

        if(Player != null){Player.Damaged(Damage);}
        if(Dum != null){Dum.Damaged(Damage);}
        if(Vamp != null){Vamp.Damaged(Damage);}
        if(krauser != null){krauser.Damaged(Damage);}

    }
}
