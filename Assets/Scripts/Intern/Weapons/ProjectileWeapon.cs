using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Implementation of Weapon for a weapon which shoot projectils.
/// </summary>
public class ProjectileWeapon : Weapon
{
    [SerializeField]
    protected Projectile m_projectileModel;

    [SerializeField]
    protected float m_velocity = 10;

    /// <summary>
    /// the position from which the projectile will spawn 
    /// </summary>
    [SerializeField]
    protected Transform m_anchor;

    /// <summary>
    /// if an entity has one of these tags, the projectile can hit the entity.
    /// </summary>
    [SerializeField]
    protected string[] m_targetTag;

    void Awake()
    {
        //reset the timer
        m_previousTime = Time.time;

        //by default, if m_anchor has't been assign, it will be the first child of this
        if( m_anchor == null && transform.childCount > 0 )
        {
            m_anchor = transform.GetChild( 0 );
        }
    }

    public override void Fire()
    {
        if( ( Time.time - m_previousTime ) >= m_fireRate )
        {
            Projectile projectile = Instantiate( m_projectileModel, m_anchor.position, m_anchor.rotation ) as Projectile;

            //projectile initialisation :
            projectile.Dammage = m_dammage;
            projectile.TargetTag = m_targetTag;

            Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();

            //deals with all event this weapon triggers when it shoots
            foreach( IWeaponEvent weaponEvent in m_weaponEvents )
            {
                weaponEvent.OnFire();
            }

            //launch the projectile
            if( projectileBody != null )
                projectileBody.AddForce( m_anchor.forward * m_velocity );

            m_previousTime = Time.time;
        }
    }
}
