using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BOMB : MonoBehaviour
{
    float PlantingTime = 5f;
    [SerializeField] GameObject PlantedBomb;
    [SerializeField] GameObject BombHUD;
    static bool IsPlanted;
    Coroutine bombplant;

   IEnumerator Planting()
   {
        PlantingTime -= Time.deltaTime;
        Debug.Log($"{PlantingTime}");
        yield return new WaitForSeconds(PlantingTime);
   }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && transform.parent.gameObject.GetComponent<WeaponManager>().CanIPlantBomb == true)
        {
            if (!IsPlanted && PlantingTime <= 0)
            {
                StopCoroutine(bombplant);
                Instantiate(PlantedBomb, transform.position, transform.rotation);
                gameObject.SetActive(false);

                transform.parent.GetComponent<WeaponManager>().BombPlanted();

                BombHUD.SetActive(true);
                IsPlanted = true;
            }
            else
            {
                bombplant = StartCoroutine(Planting());
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    PlantingTime = 5f;
                }
            }

        }
        else
        {
        }
    }
    }
