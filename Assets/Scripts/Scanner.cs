using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public UIManager uiManager; // Reference to your UI Manager

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has tag "Fish"
        if (other.CompareTag("Fish"))
        {
            HewanLaut creature = other.GetComponent<HewanLaut>();

            if (creature != null)
            {
                Debug.Log("Scan kena: " + creature.creatureName);
                uiManager.ShowCreatureInfo(creature);
            }
            else
            {
                Debug.Log("Object punya tag Fish tapi bukan HewanLaut");
            }
        }
    }
}
