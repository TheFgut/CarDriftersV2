using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectedCircles
{
    [SerializeField] private WayCircle[] nextCircles;
    [SerializeField] private float transferAngle;
    private int awailableToTransfer = -1;
    public WayCircle getNextCircleToTransfer()
    {
        if (awailableToTransfer == -1) return null;

        return nextCircles[awailableToTransfer];
    }

    public void turnOff()
    {
        lineModule.disable();
        for (int i = 0; i < nextCircles.Length; i++)
        {
            if (i == awailableToTransfer) continue;
            nextCircles[i].getMoveLineModule().disable();
        }
    }

    private bool prevTransferAv = false;
    public bool transferAwailable(Vector3 position)
    {
        if (nextCircles == null || nextCircles.Length == 0) return false;
        float bestAngleBetweenDirections = 180;
        int circWithBestAngleNum = 0;

        for (int i = 0; i < nextCircles.Length; i++)
        {
            Vector3 dirToNextCirc = nextCircles[i].transform.position - transform.position;
            Vector3 dirToPos = position - transform.position;
            float angleBetweenDirections = Vector3.Angle(dirToNextCirc, dirToPos);
            if (angleBetweenDirections < bestAngleBetweenDirections)
            {
                bestAngleBetweenDirections = angleBetweenDirections;
                circWithBestAngleNum = i;
            }
        }



        bool transfer = bestAngleBetweenDirections < transferAngle;
        if (awailableToTransfer != circWithBestAngleNum)
        {
            if (awailableToTransfer != -1)
            {
                nextCircles[awailableToTransfer].getMoveLineModule().ResetColor();
            }
            awailableToTransfer = circWithBestAngleNum;
            if (transfer == true)
            {
                nextCircles[circWithBestAngleNum].getMoveLineModule().ChangeColorTo(Color.green);
            }
        }
        if (prevTransferAv != transfer)
        {
            if (transfer == true)
            {
                nextCircles[circWithBestAngleNum].getMoveLineModule().ChangeColorTo(Color.green);
            }
            else
            {
                nextCircles[circWithBestAngleNum].getMoveLineModule().ResetColor();
            }
            prevTransferAv = transfer;
        }
        return bestAngleBetweenDirections < transferAngle;



    }
    private void turnOn()
    {
        lineModule.enable();
    }

    public bool turnOnNextCircles()
    {
        if (nextCircles == null || nextCircles.Length == 0) return false;
        foreach (WayCircle next in nextCircles)
        {
            next.nextCircles.turnOn();
        }
        return true;
    }

    private Transform transform;
    private MoveLineModule lineModule;
    public void Init(Transform transform, MoveLineModule lineModule)
    {
        this.transform = transform;
        this.lineModule = lineModule;
    }
}
