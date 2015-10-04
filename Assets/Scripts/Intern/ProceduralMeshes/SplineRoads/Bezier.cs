using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bezier : MonoBehaviour 
{

    Vector3[] points = new Vector3[4];
   
    float ti = 0.0f;
   
    private Vector3 b0 = Vector3.zero;
    private Vector3 b1 = Vector3.zero;
    private Vector3 b2 = Vector3.zero;
    private Vector3 b3 = Vector3.zero;

    private Vector3 A = Vector3.zero;
    private Vector3 B = Vector3.zero;
    private Vector3 C = Vector3.zero;
 
    // Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
    // handle1 = v0 + v1
    // handle2 = v3 + v2
    public void setControlPoints( Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        points[0] = v0;
        points[1] = v1;
        points[2] = v2;
        points[3] = v3;

        SetConstant();
    }

    public Vector3 GetControlPoint(int index)
    {
        if( index < 0 || index >= 4 )
            return Vector3.zero;

        return points[index];
    }

    // 0.0 >= t <= 1.0
    public Vector3 GetPointAtTime( float t )
    {
        //CheckConstant();

        //float t2 = t * t;
        //float t3 = t * t * t;
        //float x = A.x * t3 + B.x * t2 + C.x * t + points[0].x;
        //float y = A.y * t3 + B.y * t2 + C.y * t + points[0].y;
        //float z = A.z * t3 + B.z * t2 + C.z * t + points[0].z;

        //return ( new Vector3( x, y, z ) );

        float u = 1.0f - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * points[0]; //first term
        p += 3 * uu * t * points[1]; //second term
        p += 3 * u * tt * points[2]; //third term
        p += ttt * points[3]; //fourth term

        return p;
    }

    public Vector3 GetDirectionAtTime( float t, float step )
    {
        if(t + step < 1.0f)
        {
            Vector3 B = GetPointAtTime( t + step );
            Vector3 A = GetPointAtTime( t );
            return (B - A).normalized;
        }
        else
        {
            Vector3 B = GetPointAtTime( t );
            Vector3 A = GetPointAtTime( t - step );
            return (B - A).normalized;
        }
    }

    private void SetConstant()
    {
        C.x = 3 * ( ( points[0].x + points[1].x ) - points[0].x );
        B.x = 3 * ( ( points[3].x + points[2].x ) - ( points[0].x + points[1].x ) ) - C.x;
        A.x = points[3].x - points[0].x - C.x - B.x;

        C.y = 3 * ( ( points[0].y + points[1].y ) - points[0].y );
        B.y = 3 * ( ( points[3].y + points[2].y ) - ( points[0].y + points[1].y ) ) - C.y;
        A.y = points[3].y - points[0].y - C.y - B.y;

        C.z = 3 * ( ( points[0].z + points[1].z ) - points[0].z );
        B.z = 3 * ( ( points[3].z + points[2].z ) - ( points[0].z + points[1].z ) ) - C.z;
        A.z = points[3].z - points[0].z - C.z - B.z;

    }

// Check if p0, p1, p2 or p3 have changed
    private void CheckConstant()
    {
        if( points[0] != b0 || points[1] != b1 || points[2] != b2 || points[3] != b3 )
        {
            SetConstant();

            b0 = points[0];
            b1 = points[1];
            b2 = points[2];
            b3 = points[3];
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for( int i = 0; i < 4; i++)
        {
            Gizmos.DrawWireSphere( points[i] , 1.0f );
        }

        for( float i = 0; i < 1; i+=0.01f )
        {
            Gizmos.DrawLine( GetPointAtTime( i ), GetPointAtTime( i+0.01f ) );
        }
    }
#endif

}
