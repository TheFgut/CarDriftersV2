using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class InEditorCarMoveSimulator : MonoBehaviour
{

    [SerializeField] private float simulatedTime;
    public float time { get { return simulatedTime; }  }

    [SerializeField] public static InEditorCarMoveSimulator instance { get; private set; }

    void Update()
    {
        instance = this;
    }

}
#endif