using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateRuler : State
{
    private Controller otherController;

    public StateRuler(Controller controller) : base(controller)
    {
        if(c == c.controllerManager.leftController)
            otherController = c.controllerManager.rightController;
        else
            otherController = c.controllerManager.leftController;
    }
    
    public override void enter()
    {

    }

    public override void update()
    {
        if(c.input.menuPressed)
        {
            c.teleporter.disabledController = null;
            c.hideLaserPointer(false);
            c.controllerManager.ruler.SetActive(false);
            c.currentState = c.stateIdle;
            return;
        }
    }

    public override void exit()
    {
        
    }
}
