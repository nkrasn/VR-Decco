using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Controller c;

    public State(Controller controller)
    {
        c = controller;
    }

    public virtual void enter() { }
    public virtual void update() { }
    public virtual void exit() { }
}