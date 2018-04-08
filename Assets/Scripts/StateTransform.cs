using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransform {

    // Use this for initialization\
    public StateTransform(Transform a)
    {
        transform = a;
        State = 0;
    }

    public Transform transform { get; private set; }
    public short State;
}
