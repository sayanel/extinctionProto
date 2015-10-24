using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class TopDownSelector : MonoBehaviour
{
    [SerializeField]
    private float m_triggerHeight = 200;

    private bool m_selecting = false;

    private Vector2 m_beginPoint;
    private Vector2 m_endPoint;

    private BoxCollider m_thisTrigger;

    private Vector3 m_triggerAnchor;

    [SerializeField]
    private List<string> m_selectableTags = new List<string>();

    [SerializeField]
    private Material lineMaterial;

    //property lineColor
    [SerializeField]
    private Color m_lineColor = Color.green;
    public Color lineColor{
        get{
            return m_lineColor;
        }
        set {
            lineMaterial.color = value;
            m_lineColor = value;
        }
    }
    //endProperty lineColor

    [SerializeField]
    private List<TopDownAgent> m_selected = new List<TopDownAgent>();

    //GUI which will display the current selection
    [SerializeField]
    private RectTransform m_GUISelection;


    void Awake()
    {
        m_thisTrigger = GetComponent<BoxCollider>();
        m_thisTrigger.size = new Vector3(1, 1, 1);
        m_thisTrigger.center = new Vector3(0.5F, 0, 0.5F);
        m_thisTrigger.isTrigger = true;

        Rigidbody thisRigidbody = GetComponent<Rigidbody>();
        thisRigidbody.useGravity = false;
        thisRigidbody.isKinematic = true;
    }

    void Start()
    {
        transform.rotation = Quaternion.identity;
        lineColor = m_lineColor;
    }

    //remove the controle we have on each agent og the previous selection. 
    void clearSelection()
    {
        foreach(TopDownAgent agent in m_selected)
        {
            agent.isControllable = false;
        }

        m_selected.Clear();
    }

    void BeginSelection()
    {
        m_selecting = true;

        m_beginPoint.x = Input.mousePosition.x;
        m_beginPoint.y = Input.mousePosition.y;

        m_endPoint.x = Input.mousePosition.x;
        m_endPoint.y = Input.mousePosition.y;

        clearSelection();

        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if(Physics.Raycast(selectionRay, out hitInfo, 10000, LayerMask.GetMask("Terrain")))
        {
            m_triggerAnchor = hitInfo.point;

            transform.position = m_triggerAnchor;

            transform.localScale = new Vector3(1, m_triggerHeight, 1);

            //m_thisTrigger.size = new Vector3(1, m_triggerHeight, 1);
            //m_thisTrigger.center = new Vector3(m_thisTrigger.size.x * 0.5F, 0, m_thisTrigger.size.z * 0.5F);
        }

        m_thisTrigger.enabled = true;
    }

    void UpdateSelection()
    {
        m_endPoint.x = Input.mousePosition.x;
        m_endPoint.y = Input.mousePosition.y;

        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(selectionRay, out hitInfo, 10000, LayerMask.GetMask("Terrain")))
        {
            Vector3 diagVector = hitInfo.point - m_triggerAnchor;

            transform.position = m_triggerAnchor;

            transform.localScale = new Vector3(diagVector.x, m_triggerHeight, diagVector.z);

            //m_thisTrigger.size = new Vector3(diagVector.x, m_triggerHeight, diagVector.z);
            //m_thisTrigger.center = new Vector3(m_thisTrigger.size.x * 0.5F, 0, m_thisTrigger.size.z * 0.5F);
        }
    }

    void EndSelection()
    {
        m_selecting = false;

        m_endPoint.x = Input.mousePosition.x;
        m_endPoint.y = Input.mousePosition.y;

        m_thisTrigger.enabled = false;

        updateGUI();
    }

    //update the visual of the gui with the new selection.
    void updateGUI()
    {
        // TO COMPLETE

        //clear the gui 
        int childCount = m_GUISelection.childCount;
        for(int i = 0; i < childCount; i++)
        {
            Destroy(m_GUISelection.GetChild(0).gameObject);
        }

        //repopulate gui with the first selected item
        if (m_selected.Count > 0)
        {
            GameObject newIconeGameObject = new GameObject();
            Image newIcone = newIconeGameObject.AddComponent<Image>();

            newIcone.sprite = m_selected[0].getIcone();

            newIconeGameObject.transform.SetParent(m_GUISelection);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach(string tag in m_selectableTags)
        {
            if(other.CompareTag(tag))
            {
                TopDownAgent selectedAgent = other.GetComponent<TopDownAgent>();
                if(selectedAgent != null && !m_selected.Contains(selectedAgent))
                {
                    m_selected.Add(selectedAgent);
                    selectedAgent.isControllable = true;
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (string tag in m_selectableTags)
        {
            if (other.CompareTag(tag))
            {
                TopDownAgent selectedAgent = other.GetComponent<TopDownAgent>();
                if (selectedAgent != null)
                {
                    m_selected.Remove(selectedAgent);
                    selectedAgent.isControllable = false;
                }
            }
        }
    }

    void Update()
    {
        //selection of gameEntities with correct tag
        if(Input.GetMouseButtonDown(0))
        {
            BeginSelection();
        }
        else if(m_selecting)
        {
            if (Input.GetMouseButtonUp(0))
            {
                EndSelection();
            }
            else
            {
                UpdateSelection();
            }
        }

        ////dispatch actions to current selection of gameEntities
        //if(Input.GetMouseButtonDown(1))
        //{
        //    //Fire entities actions : 
        //    foreach(TopDownController tdController in m_selected)
        //    {
        //        tdController.OnInput();
        //    }
        //}
    }

    

    void OnGUI()
    {
        if(m_selecting)
            GUIUtils.DrawScreenRectBorder( new Rect( m_beginPoint.x, Camera.main.pixelHeight - m_beginPoint.y, m_endPoint.x - m_beginPoint.x, m_beginPoint.y - m_endPoint.y ), 1, Color.red );
    }

    //deprecated methode to render a select box on screen. 
    //issued render bug with this methode.
    // replaced by OnGUI render
    /*
    public void OnRenderObject()
    {
        if (!m_selecting)
            return;

        float width = m_thisTrigger.size.x;
        float height = m_thisTrigger.size.y;

        //CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.Begin(GL.LINES);
            GL.Color(new Color(1, 0, 0, 1));

            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0 * width, 0, 1 * height);

            GL.Vertex3(0 , 0, 1 * height);
            GL.Vertex3(1 * width, 0, 1 * height);

            GL.Vertex3(1 * width, 0, 1 * height);
            GL.Vertex3(1 * width, 0, 0);

            GL.Vertex3(1 * width, 0, 0);
            GL.Vertex3(0, 0, 0 * height);
        GL.End();
        GL.PopMatrix();
    }*/

}
