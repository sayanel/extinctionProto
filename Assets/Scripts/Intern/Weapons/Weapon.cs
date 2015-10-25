using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base class for all weapons.
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    public enum AmmoType { DEFAULT };

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


    [SerializeField]
    protected GameObject m_weaponEventModel;

    protected List<WeaponEvent> m_weaponEvents = new List<WeaponEvent>();

    /// <summary>
    /// the position from which the projectile will spawn 
    /// </summary>
    [SerializeField]
    protected Transform m_anchor;

    int m_nextEventActive = 0;

    /// <summary>
    /// parameter to check the elapsed time since the last shoot. 
    /// </summary>
    protected float m_previousTime;


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
    /// Implement this function to define how the weapon fire.
    /// </summary>
    public abstract void Fire();

    /// <summary>
    /// Automaticaly create a pool of WeaponEvent, based on the m_weaponEventModel parameter.
    /// It has to be called during initialisation, otherwise weaponEvents called by OnFire won't work.
    /// </summary>
    public virtual void InitWeaponEvents()
    {
        //create a pool of shoot event for the weapon
        if( m_weaponEventModel != null )
        {
            for( int i = 0; i < 10; ++i )
            {
                GameObject newWeaponEvent = Instantiate( m_weaponEventModel, m_anchor.position, m_anchor.rotation ) as GameObject;
                newWeaponEvent.transform.SetParent( this.transform );
                m_weaponEvents.Add( newWeaponEvent.GetComponent<WeaponEvent>() );
            }
        }
    }

    /// <summary>
    /// OnFire should be called on Fire function. It deals with events triggered by this weapon.
    /// </summary>
    public virtual void OnFire()
    {
        if( m_weaponEvents.Count == 0 )
            return;

        m_weaponEvents[m_nextEventActive].transform.position = m_anchor.position;
        m_weaponEvents[m_nextEventActive].transform.rotation = m_anchor.rotation;
        m_weaponEvents[m_nextEventActive].enabled = true;
        m_weaponEvents[m_nextEventActive].OnFire();

        m_nextEventActive++;
        if( m_nextEventActive >= m_weaponEvents.Count )
            m_nextEventActive = 0;
    }

}
