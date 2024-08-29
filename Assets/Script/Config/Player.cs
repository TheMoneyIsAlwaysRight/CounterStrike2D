using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : Human
{
    [SerializeField] GameObject BuyLogo;
    [SerializeField] float movespeed;
    Vector2 moveDir;
    public static Vector2 mouse;
    float angle;
    Coroutine f;
    void cursor()
    {
        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        angle = Mathf.Atan2(mouse.y - transform.position.y, mouse.x - transform.position.x) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void Update()
    {
        this.Hpcheck();
        cursor();
        gameObject.transform.Translate(moveDir * movespeed * Time.deltaTime,Space.World);
        if(gameObject.transform.GetComponentInChildren<Inventory_Manager>().IsBuyCant)
        {
            BuyLogo.SetActive(true);
        }
        else
        {
            BuyLogo.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Fire();
        }
    }
    void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.y = value.Get<Vector2>().y;
    }
}
