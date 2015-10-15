using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Projectile : MonoBehaviour, IDestructible
{
    // after this delay, the projectile is destroy
    [SerializeField]
    protected float m_lifeTime = 10; 

    [SerializeField]
    protected float m_dammage;
    public float Dammage
    {
        get { return m_dammage; }
        set { m_dammage = value; }
    }

    //if an entity has one of these tags, the projectile can hit the entity.
    [SerializeField]
    protected string[] m_targetTag;
    public string[] TargetTag
    {
        get { return m_targetTag; }
        set { m_targetTag = value; }
    }

    private bool m_isAlive = true;
    private Renderer m_thisRenderer;
    private Collider m_thisCollider;

    void Awake()
    {
        m_thisRenderer = GetComponent<Renderer>();
        m_thisCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        foreach(string tag in m_targetTag)
        {
            if(other.CompareTag(tag))
            {
                ITargetable target = other.GetComponent<ITargetable>();
                target.TakeDammage( m_dammage );
                m_lifeTime = 0; //delete this
            }
        }
    }

    void Update()
    {
        //destroy the projectile after the lifeTime has been elapsed
        m_lifeTime -= Time.deltaTime;
        if( m_lifeTime < 0 )
        {
            destroy();
        }     
    }

    public bool isAlive()
    {
        return m_isAlive;
    }

    public void destroy()
    {
        m_isAlive = false;

        m_thisCollider.enabled = false;
        m_thisRenderer.enabled = false;
        Destroy( this.gameObject, 1 );
    }
}
