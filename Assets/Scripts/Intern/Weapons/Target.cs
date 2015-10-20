using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A Target is a class which is Destructible and Targetable
/// </summary>
public class Target : MonoBehaviour, ITargetable, IDestructible
{
    //the maximum amount of life of this entity
    [SerializeField]
    private float m_maxLife = 100;

    [SerializeField]
    private float m_life = 100;

    [SerializeField]
    private bool m_isAlive = true;


    public void TakeDammage( float amount )
    {
        //reduce life, clamp it between 0 and maxLife, if life <= 0 set m_isAlive at false
        m_isAlive = !Mathf.Approximately( Mathf.Clamp( ( m_life -= amount ), 0, m_maxLife ), 0 );
    }

    public bool isAlive()
    {
        return m_isAlive;
    }

    public void destroy()
    {
        //NOTHING
    }
}
