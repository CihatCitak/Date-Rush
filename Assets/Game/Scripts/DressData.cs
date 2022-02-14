using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DressParts
{
    Chest,
    Leg,
    Foot
}
    
public enum DressStyles
{
    Underwear,
    Casual,
    Night,
    Party,
    Wedding
}
public class DressData : MonoBehaviour
{
    public DressParts dressPart;
    public DressStyles dressStyle;
    
    //public SkinnedMeshRenderer skinnedMeshRenderer;

    private void Start()
    {
        //skinnedMeshRenderer=GetComponent<SkinnedMeshRenderer>();
    }

    public void DressActive(bool active)
    {
        if (active)
            gameObject.SetActive(true);
        else
            gameObject.SetActive(false);
    }
}
