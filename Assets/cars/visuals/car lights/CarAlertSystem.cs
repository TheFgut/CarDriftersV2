using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAlertSystem : MonoBehaviour
{
    [SerializeField] private CarLight[] lights;
    [SerializeField] private dangerShadow shadow;

    void Start()
    {
        Stop();
        turnLightsOff();
        shadow.init();
    }

    void FixedUpdate()
    {
        shadow.flickerTick(Time.fixedDeltaTime);
    }

    private Coroutine routine;

    public void Alert()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(lightersFlick());
        shadow.turnOn();
    }

    private void turnLightsOn()
    {
        foreach (CarLight light in lights)
        {
            light.turnON();
        }
    }

    private void turnLightsOff()
    {
        foreach (CarLight light in lights)
        {
            light.turnOFF();
        }

    }

    private const float flickTime = 0.3f;
    private const float flickingDuration = 1.2f;
    private IEnumerator lightersFlick()
    {
        float timer = flickingDuration;
        do
        {
            turnLightsOn();
            yield return new WaitForSeconds(flickTime);
            turnLightsOff();
            yield return new WaitForSeconds(flickTime);
            timer -= flickTime * 2;
        } while (timer > 0);
        routine = null;
    }

    public void Stop()
    {
        shadow.turnOff();
    }

    public void setAlertPower(float power)
    {
        shadow.setPower(power);
    }

    [System.Serializable]
    private class dangerShadow
    {
        [SerializeField] private SpriteRenderer shadow;
        private Color defaultColor;
        public void init()
        {
            defaultColor = shadow.color;
            setAlpha(0);
        }
        private bool ON = false;
        public void turnOn()
        {
            ON = true;
            shadow.enabled = true;
        }

        public void turnOff()
        {
            shadow.enabled = false;
            timer = 0;
            reverse = false;
            ON = false;
        }
        private float power;
        public void setPower(float power)
        {
            this.power = power;

        }

        private const float flickTime = 0.5f;
        private float timer;
        private bool reverse;
        public void flickerTick(float deltaTime)
        {
            if(ON == false)
            {
                return;
            }
            if (reverse == false)
            {
                timer += deltaTime;
                if(timer > flickingDuration)
                {
                    reverse = true;
                }
            }
            else
            {
                timer -= deltaTime;
                if (timer < 0)
                {
                    reverse = false;
                }
            }
            setAlpha(power * (timer / flickingDuration));
        }

        private void setAlpha(float a)
        {
            Color newCol = defaultColor;
            newCol.a *= a;
            shadow.color = newCol;
        }
    }

}
