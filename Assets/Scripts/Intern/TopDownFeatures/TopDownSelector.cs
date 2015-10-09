using UnityEngine;
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
    public Color LineColor{
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
    private List<TopDownController> m_selected = new List<TopDownController>();

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
        LineColor = m_lineColor;
    }

    void BeginSelection()
    {
        m_selecting = true;

        m_beginPoint.x = Input.mousePosition.x;
        m_beginPoint.y = Input.mousePosition.y;

        m_endPoint.x = Input.mousePosition.x;
        m_endPoint.y = Input.mousePosition.y;

        m_selected.Clear();

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
    }

    void OnTriggerEnter(Collider other)
    {
        foreach(string tag in m_selectableTags)
        {
            if(other.CompareTag(tag))
            {
                TopDownController selectedController = other.GetComponent<TopDownController>();
                if(selectedController != null && !m_selected.Contains(selectedController))
                {
                    m_selected.Add(selectedController);
                    selectedController.IsControllable = true;
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
                TopDownController selectedController = other.GetComponent<TopDownController>();
                if (selectedController != null)
                {
                    m_selected.Remove(selectedController);
                    selectedController.IsControllable = false;
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
    }

}
