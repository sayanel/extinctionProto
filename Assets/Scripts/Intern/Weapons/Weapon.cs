using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour 
{
    public enum AmmoType {DEFAULT};

    /// <summary>
    /// The fire rate of this weapon
    /// </summary>
    [SerializeField]
    protected float m_fireRate = 1;

    /// <summary>
    /// The type of ammo this weapon can use
    /// </summary>
    [SerializeField]
    protected AmmoType m_ammoType = AmmoType.DEFAULT;

    /// <summary>
    /// the dammage of the weapon 
    /// </summary>  
    [SerializeField]
    protected float m_dammage = 1;

    /// <summary>
    /// the range of the weapon
    /// is used by IAs to know when they can hit their targets
    /// </summary>
    [SerializeField]
    protected float m_range = 10;


    //overrideable assessors : 

    public virtual float getDammage()
    {
        return m_dammage;
    }

    public virtual AmmoType getAmmoType()
    {
        return m_ammoType;
    }

    public virtual float getFireRate()
    {
        return m_fireRate;
    }

    public virtual float getRange()
    {
        return m_range;
    }

    /// <summary>
    /// parameter to check the elapsed time since the last shoot. 
    /// </summary>
    protected float m_previousTime;

    /// <summary>
    /// Implement this function to define how the weapon fire.
    /// </summary>
    public abstract void Fire();
        
}
