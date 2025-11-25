using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableHotkey : MonoBehaviour
{
    public float disableTime = 5f; // default 5 seconds

    void Start()
    {
        Invoke("DisableObject", disableTime);
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }
}
