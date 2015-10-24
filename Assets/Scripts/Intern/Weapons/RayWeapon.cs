using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Implementation of a weapon which cast a ray to detect the enemy.
/// </summary>
public class RayWeapon : Weapon 
{

    [SerializeField]
    protected float m_minDistance = 1;

    [SerializeField]
    protected float m_rayLength = 100;

    [SerializeField]
    protected string[] m_targetTag;

    [SerializeField]
    protected Transform m_anchor;

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
            if( Physics.Raycast( m_anchor.position + m_anchor.forward * m_minDistance, m_anchor.forward, out hitInfo, m_rayLength, LayerMask.GetMask( m_targetTag ) ) )
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
