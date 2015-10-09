using UnityEngine;
using System.Collections;
using System;

//Robot implementation
//Robot is both an ISelectable and ICharacter entity.
[RequireComponent(typeof(NavMeshAgent))]
public class Robot : MonoBehaviour, ISelectable, ICharacter
{
    [SerializeField]
    private Sprite m_icone;

    [SerializeField]
    private string m_name = "Robot";

    [SerializeField]
    private string m_description = "a robot...";

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
