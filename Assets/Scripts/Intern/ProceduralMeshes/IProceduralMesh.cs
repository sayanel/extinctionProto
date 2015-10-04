using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IProceduralMesh 
{
    void Init();
    void Generate();
    void FlipNormals();
}
