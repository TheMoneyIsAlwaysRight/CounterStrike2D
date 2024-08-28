using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletFire : MonoBehaviour
{
    Coroutine flashcoroutine;

    private void OnEnable()
    {
        flashcoroutine = StartCoroutine(FiretrackerCoroutine());
    }
    private void OnDisable()
    {
        StopCoroutine(flashcoroutine);
    }
    public IEnumerator FiretrackerCoroutine()
    {
        yield return new WaitForSeconds(0.05f);
        gameObject.SetActive(false);
    }
}
