using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTeleporting : State
{
    public StateTeleporting(Controller controller) : base(controller) { }

    public override void enter()
    {
        c.hideLaserPointer(true);
    }

    public override void update()
    {
        if(!c.input.touchpadHeld)
        {
            c.currentState = c.stateIdle;
        }
    }

    public override void exit()
    {

    }
}