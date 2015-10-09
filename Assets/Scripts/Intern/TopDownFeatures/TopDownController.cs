using UnityEngine;
using System.Collections;

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
    private TopDownAgent m_target;

    void Start()
    {
        //if m_target isn't set up, try to find a TopDownAgent component on this entity
        if (m_target == null)
            m_target = GetComponent<TopDownAgent>();
    }
	
	void Update()
    {
        //update inputs only if this controller is active
        if(m_target.IsControllable)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (checkMouseTarget(out m_mouseTargetInfo)) // first step : rayCast and store information on the target
                {
                    m_target.move(m_mouseTargetInfo.position);
                    // m_thisNavMeshAgent.SetDestination(m_mouseTargetInfo.position);
                }
            }
        }
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
