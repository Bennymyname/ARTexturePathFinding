using System.Collections;
using UnityEngine;

public class VRDamageHandler : MonoBehaviour
{
    private GameObject damageScreen; // Reference to the red screen
    private bool isDamaged = false;  // Tracks whether damage screen is active

    void Start()
    {
        // Find the damage screen by name (replace "Damage" with the correct name of the UI object)
        damageScreen = GameObject.Find("Damage");
        if (damageScreen != null)
        {
            damageScreen.SetActive(false); // Ensure the screen is initially off
        }
        else
        {
            Debug.LogError("Damage screen not found! Ensure it's named correctly in the hierarchy.");
        }
    }

    // When entering the trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("unfalse") && !isDamaged)
        {
            ShowDamageScreen();
            other.tag = "removeRedScreen"; // Change the tag to prevent repeated triggering
        }
    }

    // When exiting the trigger zone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("removeRedScreen") && isDamaged)
        {
            HideDamageScreen();
        }
    }

    // Activates the damage screen
    private void ShowDamageScreen()
    {
        if (damageScreen != null)
        {
            damageScreen.SetActive(true);
            isDamaged = true;
        }
    }

    // Deactivates the damage screen
    private void HideDamageScreen()
    {
        if (damageScreen != null)
        {
            damageScreen.SetActive(false);
            isDamaged = false;
        }
    }
}
