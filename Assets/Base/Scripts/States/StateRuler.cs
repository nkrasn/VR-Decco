using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRuler : State
{
    public StateRuler(Controller controller) : base(controller) { }

    public override void enter()
    {
        
    }

    public override void update()
    {
        if(c.input.menuPressed)
        {
            c.teleporter.disabledController = null;
            c.hideLaserPointer(false);
            c.ruler.SetActive(false);
            c.currentState = c.stateIdle;
            return;
        }
    }

    public override void exit()
    {
        
    }
}
