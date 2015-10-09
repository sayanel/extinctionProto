using UnityEngine;
using System.Collections;
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

    void Awake()
    {
        m_thisNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void attack()
    {
        //TODO
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

    public void move(Vector3 position)
    {
        m_thisNavMeshAgent.SetDestination(position);
    }

    public void pickUp()
    {
       //TODO
    }

}
