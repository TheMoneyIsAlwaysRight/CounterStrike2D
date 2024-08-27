using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDectector : MonoBehaviour
{
    [SerializeField] AIStatePatten aIStatePatten;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            aIStatePatten.isCaptureEnemy = true; 
            aIStatePatten.player = collision.gameObject;
            aIStatePatten.ChangeState(AIStatePatten.State.Battle);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            aIStatePatten.isCaptureEnemy = false;
            aIStatePatten.ChangeState(AIStatePatten.State.Mission);
        }
    }
}
