using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public UIManager uiManager;
    public Healthbar playerHealth;
    public Camera mainCamera;
    public float scanRange = 50f;
    public LayerMask scanLayers;

    void Update()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, scanRange, scanLayers))
        {
            GameObject obj = hit.collider.gameObject;

            if (obj.CompareTag("Fish"))
            {
                FishInfo creature = obj.GetComponent<FishInfo>();
                if (creature != null)
                {
                    uiManager.ShowCreatureInfo(creature);
                }
            }
            else if (obj.CompareTag("Battery"))
            {
                if (playerHealth != null)
                {
                    playerHealth.Heal(20f);
                }
                Destroy(obj);
            }
            else if (obj.CompareTag("Trash"))
            {
                if (uiManager != null)
                {
                    uiManager.CollectJunk(obj.GetInstanceID().ToString());
                }
                Destroy(obj);
            }
        }
    }
}