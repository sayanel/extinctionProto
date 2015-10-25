using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implementation of a weapon which cast a ray to detect the enemy.
/// </summary>
public class RayWeapon : Weapon 
{
    /// <summary>
    /// an offset for the begin point of the ray.
    /// </summary>
    [SerializeField]
    protected float m_minDistance = 1;

    /// <summary>
    /// the length of the ray from its begin point to its end point
    /// </summary>
    [SerializeField]
    protected float m_rayLength = 100;

    /// <summary>
    /// the ray will only hit targets on these layers
    /// </summary>
    [SerializeField]
    protected string[] m_targetLayers;

    /// <summary>
    /// The transform from which the ray will start.
    /// </summary>
    [SerializeField]
    protected Transform m_anchor;

    /// <summary>
    /// m_maxDistance = ( m_rayLength + m_minDistance );
    /// max distance the ray can reach, from m_anchor.position.
    /// </summary>
    protected float m_maxDistance = 100;

    void Awake()
    {
        m_previousTime = Time.time;

        m_maxDistance = ( m_rayLength + m_minDistance );

        //by default, if m_anchor has't been assign, it will be the first child of this
        if(m_anchor == null && transform.childCount > 0)
        {
            m_anchor = transform.GetChild( 0 );
        }
    }

    public override void Fire()
    {
        if( (Time.time - m_previousTime) >= m_fireRate )
        {
            RaycastHit hitInfo;
            
            //deals with all event this weapon triggers when it shoots
            foreach( IWeaponEvent weaponEvent in m_weaponEvents )
            {
                weaponEvent.OnFire();
            }

            //perform raycast
            if( Physics.Raycast( m_anchor.position + m_anchor.forward * m_minDistance, m_anchor.forward, out hitInfo, m_rayLength, LayerMask.GetMask( m_targetLayers ) ) )
            {
                Debug.Log( "ray !!! " );
                ITargetable target = hitInfo.transform.GetComponent<ITargetable>();

                if( target != null )
                    target.TakeDammage( m_dammage );
            }

            Debug.Log( "Fire" );
            Debug.DrawRay( m_anchor.position + m_anchor.forward * m_minDistance, m_anchor.forward * m_rayLength, Color.red, 10 );
            Debug.DrawLine( m_anchor.position + m_anchor.forward * m_minDistance, m_anchor.position + m_anchor.forward * ( m_minDistance + m_rayLength ) );

            m_previousTime = Time.time;
        }
    }
}
