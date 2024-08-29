using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPlace : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Human>())
        {
            collision.gameObject.transform.GetComponentInChildren<Inventory_Manager>().CanIPlantBomb = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Human>())
        {
            collision.gameObject.transform.GetComponentInChildren<Inventory_Manager>().CanIPlantBomb = false;
        }
    }
}
