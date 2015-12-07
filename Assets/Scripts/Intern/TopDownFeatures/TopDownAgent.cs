using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//Robot implementation
//Robot is both an ISelectable and ICharacter entity.
[RequireComponent(typeof(NavMeshAgent))]
public class TopDownAgent : MonoBehaviour, ISelectable, ICharacter
{
    //tasks the TopDownAgent can do
    public enum TopDownBehaviour { IDLE, ATTACK, MOVE };

    //property isControllable
    //player Inputs can't be received by this until m_isControllable is false 
    private bool m_isControllable = false;

    public bool isControllable
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

    // the current task this agent is doing
    private TopDownBehaviour m_behaviour = TopDownBehaviour.IDLE;
    public TopDownBehaviour Behaviour
    {
        get { return m_behaviour; }
        set { m_behaviour = value; }
    }

    void Awake()
    {
        m_thisNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// return true if this agent is able to attack this a weapon
    /// </summary>
    public bool CanAttack()
    {
        if( m_weapons.Count == 0 )
            return false;
        if( !m_canAttack )
            return false;

        return true;
    }

    /// <summary>
    /// return true if this agent can directly attack the target
    /// </summary>
    public bool CanAttack(Target target)
    {
        if( m_weapons.Count == 0 )
            return false;
        if( !m_canAttack )
            return false;

        float distanceToTarget = Vector3.Distance( transform.position, target.transform.position );
        foreach(Weapon weapon in m_weapons)
        {
            if(weapon.getRange() < distanceToTarget)
                return false;
        }

        return true;
    }

    /// <summary>
    /// directlry fire with all weapons
    /// </summary>
    public void attack()
    {
        foreach(Weapon weapon in m_weapons)
        {
            weapon.Fire();
        }
    }

    /// <summary>
    /// Rotate toward the target, then fire
    /// </summary>
    public void attack(Target target)
    {
        transform.LookAt( target.transform );
        attack();
    }

    /// <summary>
    /// Move the agent to a position
    /// </summary>
    public void Move( Vector3 position )
    {
        m_thisNavMeshAgent.Resume();
        m_thisNavMeshAgent.SetDestination( position );
    }

    /// <summary>
    /// stop the movement of this agent, if it is walking
    /// </summary>
    public void StopWalking()
    {
        m_thisNavMeshAgent.Stop();
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
