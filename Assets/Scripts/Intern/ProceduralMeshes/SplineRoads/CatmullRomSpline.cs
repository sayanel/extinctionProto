using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour 
{

	[SerializeField] List<Transform> m_controlPointsList = new List<Transform>();
	[SerializeField] bool m_isLooping = false;
	public bool IsLooping {
		get {return m_isLooping;}
		set {m_isLooping = value;}
	}	

	public void addControlPoint()
	{
		Transform newPoint = new GameObject("splineControlPoint").transform;
		newPoint.gameObject.AddComponent<CatmullRomPoint> ();
		newPoint.SetParent(this.transform);
		if (getSize() > 0)
			newPoint.localPosition = m_controlPointsList [getSize() - 1].transform.localPosition;
		else
			newPoint.localPosition = Vector3.zero;

		m_controlPointsList.Add (newPoint);
	}

    public void addControlPoint(Transform tr)
    {
        Transform newPoint = new GameObject("splineControlPoint").transform;
        newPoint.gameObject.AddComponent<CatmullRomPoint>();
        newPoint.SetParent( this.transform );
        newPoint.position = tr.position;
        newPoint.localScale = tr.localScale;
        newPoint.rotation = tr.rotation;

        m_controlPointsList.Add( newPoint );
    }

    public void removeLastControlPoint()
	{
		m_controlPointsList.RemoveAt (m_controlPointsList.Count-1);
	}

	public int getSize()
	{
		return m_controlPointsList.Count;
	}

	public Transform getSplineControlPoint(int index)
	{
		return m_controlPointsList [index];
	}

    //set the width of all control points 
    public void setAllWidths(float width)
    {
        foreach( Transform tr in m_controlPointsList )
        {
            CatmullRomPoint splineControlPoint = tr.GetComponent<CatmullRomPoint>();
            splineControlPoint.Weight = width;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos() {

		Gizmos.color = Color.white;


		for (int i = 0; i < m_controlPointsList.Count; i++) {

			Gizmos.DrawWireSphere(m_controlPointsList[i].position, 0.3f);
		
		
			if ((i == 0 || i == m_controlPointsList.Count - 2 || i == m_controlPointsList.Count - 1) && !m_isLooping) 
				continue;
			
			DisplayCatmullRomSpline(i);
		
		}

	}
#endif

	public Vector3 getSplinePoint(int controlPointIndex, float distanceFromThisPoint)
	{
		Vector3 p0 = m_controlPointsList[ClampListPos(controlPointIndex - 1)].position;
		Vector3 p1 = m_controlPointsList[controlPointIndex].position;
		Vector3 p2 = m_controlPointsList[ClampListPos(controlPointIndex + 1)].position;
		Vector3 p3 = m_controlPointsList[ClampListPos(controlPointIndex + 2)].position;

		return ReturnCatmullRom(distanceFromThisPoint, p0, p1, p2, p3);
	}

	public Vector3 getSplinePointDirection(int controlPointIndex, float distanceFromThisPoint)
	{
		Vector3 p0 = m_controlPointsList[ClampListPos(controlPointIndex - 1)].position;
		Vector3 p1 = m_controlPointsList[controlPointIndex].position;
		Vector3 p2 = m_controlPointsList[ClampListPos(controlPointIndex + 1)].position;
		Vector3 p3 = m_controlPointsList[ClampListPos(controlPointIndex + 2)].position;

		if(distanceFromThisPoint < 1)
			return Vector3.Normalize(ReturnCatmullRom(distanceFromThisPoint+0.1F, p0, p1, p2, p3) - ReturnCatmullRom(distanceFromThisPoint, p0, p1, p2, p3));
		else
			return Vector3.Normalize(ReturnCatmullRom(distanceFromThisPoint, p0, p1, p2, p3) - ReturnCatmullRom(distanceFromThisPoint-0.1F, p0, p1, p2, p3));

	}

	void DisplayCatmullRomSpline(int pos) {
		//Clamp to allow looping
		Vector3 p0 = m_controlPointsList[ClampListPos(pos - 1)].position;
		Vector3 p1 = m_controlPointsList[pos].position;
		Vector3 p2 = m_controlPointsList[ClampListPos(pos + 1)].position;
		Vector3 p3 = m_controlPointsList[ClampListPos(pos + 2)].position;
		
		
		//Just assign a tmp value to this
		Vector3 lastPos = Vector3.zero;
		
		//t is always between 0 and 1 and determines the resolution of the spline
		//0 is always at p1
		for (float t = 0; t < 1; t += 0.1f) {
			//Find the coordinates between the control points with a Catmull-Rom spline
			Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);
			
			//Cant display anything the first iteration
			if (t == 0) {
				lastPos = newPos;
				continue;
			}
			
			Gizmos.DrawLine(lastPos, newPos);
			lastPos = newPos;
		}
		
		//Also draw the last line since it is always less than 1, so we will always miss it
		Gizmos.DrawLine(lastPos, p2);
	}

	int ClampListPos(int pos)  {
		if (pos < 0) {
			pos = m_controlPointsList.Count - 1;
		}
		
		if (pos > m_controlPointsList.Count) {
			pos = 1;
		}
		else if (pos > m_controlPointsList.Count - 1) {
			pos = 0;
		}
		
		return pos;
	}

	Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		Vector3 a = 0.5f * (2f * p1);
		Vector3 b = 0.5f * (p2 - p0);
		Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
		Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);
		
		Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);
		
		return pos;
	}


}
