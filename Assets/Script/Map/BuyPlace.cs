using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyPlace : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Human>())
        {
            collision.gameObject.transform.GetComponentInChildren<WeaponManager>().IsBuyCant = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Human>())
        {
            collision.gameObject.transform.GetComponentInChildren<WeaponManager>().IsBuyCant = false;
        }
    }
}
