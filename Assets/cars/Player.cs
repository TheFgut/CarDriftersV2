using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CarMover
{
    [SerializeField] private MenusSwitcher menu;
    void FixedUpdate()
    {
        MoveTo(movementCircle.destination);
        checkOpportunityToTransfer();
    }

    private bool transferAbility = false;

    public void checkOpportunityToTransfer()
    {
        transferAbility = wayCircle.nextCircles.transferAwailable(transform.position);
    }
    public void goToNextCircle()
    {
        if(transferAbility == false)
        {
            return;
        }
        wayCircle.disconnectFromCircle(this);

        wayCircle.nextCircles.turnOff();
        wayCircle = wayCircle.nextCircles.getNextCircleToTransfer();

        if (wayCircle == null) return;
        movementCircle = wayCircle.connectToCircle(this);

        bool nextAwailable = wayCircle.nextCircles.turnOnNextCircles();
        if (nextAwailable == false)
        {
            menu.switchTo(1);

            return;
        }
        wayCircle.getMoveLineModule().ResetColor();
    }

    public bool turOnNextCircles()
    {
        return false;
    }


}


