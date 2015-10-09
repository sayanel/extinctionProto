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

    //property isControllable
    //player Inputs can't be received by this until m_isControllable is false 
    private bool m_isControllable = false;

    public bool IsControllable{
        get { return m_isControllable; }
        set { m_isControllable = value; }
    }
    //end property isControllable

    //general switches for TopDownEntity
    private bool m_isCastingSpell = false;
    private bool m_canAttack = true;
    private bool m_canMove = true;

    //used to store information on the entity the mouse has clicked on (if any)
    MouseTargetInfo m_mouseTargetInfo;

    //the target this component control. If null during instanciation, try to find a ICharacter on this entity
    [SerializeField]
    private ICharacter m_target;

    void Start()
    {
        //if m_target isn't set up, try to find a ICharacter component on this entity
        if (m_target == null)
            m_target = GetComponent<ICharacter>();
    }
	
	void Update()
    {
        //update inputs only if this controller is active
        if(m_isControllable)
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
