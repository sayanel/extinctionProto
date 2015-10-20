using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopDownController : MonoBehaviour
{
    //small struct to retrieve the information of the entity mouse the mouse has clicked on
    struct MouseTargetInfo
    {
        public Vector3 position;
        public GameObject gameObject;
        public string tag;
    }

    //used to store information on the entity the mouse has clicked on (if any)
    MouseTargetInfo m_mouseTargetInfo;

    //the target this component control. If null during instanciation, try to find a TopDownAgent on this entity
    [SerializeField]
    private TopDownAgent m_agent;

    //the right click target
    [SerializeField]
    private Target m_currentTarget;

    //all potential targets (targets which are )
    private List<Target> m_potentialTargets;

    //delay between two decisions taken by the ia
    [SerializeField]
    private float m_iaDelay;

    public void addPotentialTarget(Target target)
    {
        m_potentialTargets.Add(target);
    }

    public void removePotentialTarget(Target target)
    {
        m_potentialTargets.Remove( target );
    }

    void Start()
    {
        //if m_target isn't set up, try to find a TopDownAgent component on this entity
        if (m_agent == null)
            m_agent = GetComponent<TopDownAgent>();
    }
	
	void Update()
    {
        //update inputs only if this controller is active
        if(m_agent.isControllable)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (checkMouseTarget(out m_mouseTargetInfo)) // first step : rayCast and store information on the target
                {
                    if(m_mouseTargetInfo.tag == "Target")
                    {
                        //store a pointer to the current target
                        m_currentTarget = m_mouseTargetInfo.gameObject.GetComponent<Target>();

                        //launch a coroutine for attack behaviour
                        if(m_currentTarget != null)
                            MoveAndAttack();
                    }
                    else
                        m_agent.move(m_mouseTargetInfo.position);
                    // m_thisNavMeshAgent.SetDestination(m_mouseTargetInfo.position);
                }
            }
        }
	}

    void MoveAndAttack()
    {
        StartCoroutine( MoveAndAttackRoutine() );
    }

    IEnumerator MoveAndAttackRoutine()
    {
        //Check if agent can attack
        if( m_agent.CanAttack( m_currentTarget ) )
        {
            m_agent.attack(m_currentTarget);
        }
        else
        {
            m_agent.move( m_currentTarget.transform.position );
        }

        yield return new WaitForSeconds( m_iaDelay );
    }

    bool checkMouseTarget(out MouseTargetInfo info)
    {
        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(selectionRay, out hitInfo))
        {
            info.gameObject = hitInfo.collider.gameObject;
            info.position = hitInfo.point;
            info.tag = hitInfo.collider.tag;
            return true;
        }
        else
        {
            info.gameObject = null;
            info.position = Vector3.zero;
            info.tag = "none";
            return false;
        }
    }

}
