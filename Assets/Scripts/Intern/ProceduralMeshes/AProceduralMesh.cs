using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class AProceduralMesh : MonoBehaviour, IProceduralMesh
{
    public virtual void FlipNormals()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        if( filter != null )
        {
            Mesh mesh = filter.sharedMesh;

            MeshHandler.flipNormals( ref mesh );
        }
    }

    public abstract void Generate();

    public abstract void Init();
}
