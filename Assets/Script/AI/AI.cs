using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class AI : Human
{
    [SerializeField] public float movespeed;
    Vector2 moveDir;
}
