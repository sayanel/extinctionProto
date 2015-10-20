using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour 
{
    public enum AmmoType {DEFAULT};

    [SerializeField]
    protected float m_fireRate = 1;

    [SerializeField]
    protected AmmoType m_ammoType = AmmoType.DEFAULT;

    [SerializeField]
    protected float m_dammage = 1;

    //parameter to check the elapsed time since the last shoot. 
    protected float m_previousTime;

    public abstract void Fire();
        
}
