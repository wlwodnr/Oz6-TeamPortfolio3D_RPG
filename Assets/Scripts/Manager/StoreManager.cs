using System;
using UnityEngine;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

}
