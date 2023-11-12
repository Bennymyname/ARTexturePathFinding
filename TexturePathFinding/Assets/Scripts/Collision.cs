using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour
{
    private GameObject damage;

    void Start()
    {
        damage = GameObject.Find("Damage"); // Replace "Crosshair" with the actual name of your crosshair object
        damage.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("false"))
        {
            // Handle entering the specific block if needed
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("unfalse"))
        {
            damage.SetActive(true);
            other.gameObject.tag = "removeRedScreen";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("removeRedScreen"))
        {
            damage.SetActive(false);
        }
    }
}
