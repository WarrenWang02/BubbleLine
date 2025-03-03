using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private float LevelTime;

    public float GetLevelTime()
    {
        return LevelTime;
    }
}
