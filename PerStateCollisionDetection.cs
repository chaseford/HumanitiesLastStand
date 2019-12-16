using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsefulCodeSnippet : MonoBehaviour
{
    //Prior to revamping group game, our concept was dodgeball
    //The following code was as script written by Julian Do in which the dodgell causes damage to the player

    private void OnCollisionEnter(Collision collision)
    {
        //If collider (ball) hits the player and the recieving player is not trying to catch, the player takes damage (death)
        if (collision.collider.tag == "Player")
        {
            //Logs player and ball collision
            Debug.Log("Player Collision");
            PlayerManager pm = collision.gameObject.GetComponent<PlayerManager>();
            //If than statement determining players catching mode
            if (pm.IsCatching == false)
            {
                //On collision player is destoyed
                Debug.Log("DEATH!");
                pm.Death();
                DestroyBall();
            }
            //After throwing ball
            if (pm.shoot == false)
            {
                DestroyBall();
            }
            //Adjust accordingly, whether removing certain amount of health, or by destroyin gameobject on collision
            pm.TakeDamage(damageAmount);
        }
    }

    //Destroys prefar after throw
    void DestroyBall()
    {
        Destroy(gameObject);
    }
}

//This script can take place in any throwing/recieving games or even adjusted to be used in a shooter
//Concept behind adjusting the damage based on which state (idle/catching) player is in can be very useful
