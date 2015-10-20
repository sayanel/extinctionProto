﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Robot implementation
//Robot is both an ISelectable and ICharacter entity.
[RequireComponent(typeof(NavMeshAgent))]
public class TopDownAgent : MonoBehaviour, ISelectable, ICharacter
{
    //property isControllable
    //player Inputs can't be received by this until m_isControllable is false 
    private bool m_isControllable = false;

    public bool IsControllable
    {
        get { return m_isControllable; }
        set { m_isControllable = value; }
    }
    //end property isControllable

    //general switches for the TopDownAgent
    private bool m_isCastingSpell = false;
    private bool m_canAttack = true;
    private bool m_canMove = true;

    [SerializeField]
    private Sprite m_icone;

    [SerializeField]
    private string m_name = "TopDownAgent";

    [SerializeField]
    private string m_description = "a TopDownAgent...";

    //the entity must have a navMeshAgent component
    private NavMeshAgent m_thisNavMeshAgent;

    private ITargetable m_currentTarget;

    [SerializeField]
    private List<Weapon> m_weapons = new List<Weapon>();

    void Awake()
    {
        m_thisNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    //return true if this agent is able to attack this a weapon
    public bool CanAttack()
    {
        if( m_weapons.Count == 0 )
            return false;
        if( !m_canAttack )
            return false;

        return true;
    }

    //return true if this agent can directly attack the target 
    public bool CanAttack(Target target)
    {
        if( m_weapons.Count == 0 )
            return false;
        if( !m_canAttack )
            return false;

        float distanceToTarget = Vector3.Distance( transform.position, target.transform.position );
        foreach(Weapon weapon in m_weapons)
        {
            if(weapon.Range < distanceToTarget)
                return false;
        }

        return true;
    }

    //directlry fire with all weapons
    public void attack()
    {
        foreach(Weapon weapon in m_weapons)
        {
            weapon.Fire();
        }
    }

    //rotate toward the target, then fire
    public void attack(Target target)
    {
        transform.LookAt( target.transform );
        attack();
    }

    //move to a position
    public void move( Vector3 position )
    {
        m_thisNavMeshAgent.SetDestination( position );
    }

    public string getDescription()
    {
        return m_description;
    }

    public Sprite getIcone()
    {
        return m_icone;
    }

    public string getName()
    {
        return m_name;
    }

    public void pickUp()
    {
       //TODO
    }

}
