using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusSwitcher : MonoBehaviour
{
    [SerializeField] private Menu[] menus;
    void Start()
    {
        foreach(Menu menu in menus)
        {
            menu.Init(this);
        }
    }

    //switch to another menu
    private int currentMenu;
    private int nextMenu;
    public void switchTo(int menuId)
    {
        if (controls == false) return;
        if(menuId >= menus.Length || menuId < 0)
        {
            Debug.LogError("try to switch to menu that not exist");
            return;
        }
        disableControls();
        menus[currentMenu].turnOff(turnOnNext);
        nextMenu = menuId;

    }

    private void turnOnNext()
    {

        menus[nextMenu].turnOn(enableControls);
        currentMenu = nextMenu;
    }
    //controls
    private bool controls = true;
    private void enableControls()
    {
        controls = true;
    }
    private void disableControls()
    {
        controls = false;
    }
    //other classes
    [Serializable]
    private class Menu
    {
        [SerializeField]private Canvas canvas;

        private Button[] buttons;
        private Image[] images;
        private Text[] texts;

        private MenusSwitcher switcher;
        public void Init(MenusSwitcher switcher)
        {
            this.switcher = switcher;
            buttons = canvas.GetComponentsInChildren<Button>();
            images = canvas.GetComponentsInChildren<Image>();
            texts = canvas.GetComponentsInChildren<Text>();
        }
        //switchin
        [Header("transition parameters")]
        [SerializeField] private float transitTime;
        [SerializeField] private Easings transitEasing;
        [SerializeField] private CallableByMenuSwitcher[] callOnEnable;
        private Coroutine routine = null;
        public void turnOff(finalCallback callback)
        {

            if(routine == null)
            {
                routine = switcher.StartCoroutine(turnOffRoutine(callback));
            }
        }

        private IEnumerator turnOffRoutine(finalCallback callback)
        {
            disableButtons();
            float coef = 0;
            do
            {
                coef += Time.fixedDeltaTime/ transitTime;
                float eased = transitEasing.ease(coef);
                setMenuAlpha(Mathf.Lerp(1, 0, eased));

                yield return new WaitForFixedUpdate();
            } while (coef < 1);
            canvas.gameObject.SetActive(false);
            routine = null;
            callback();
        }

        public void turnOn(finalCallback callback)
        {
            if (routine == null)
            {
                routine = switcher.StartCoroutine(turnOnRoutine(callback));
            }
        }

        private IEnumerator turnOnRoutine(finalCallback callback)
        {
            canvas.gameObject.SetActive(true);
            float coef = 0;
            do
            {
                coef += Time.fixedDeltaTime / transitTime;
                float eased = transitEasing.ease(coef);

                setMenuAlpha(Mathf.Lerp(0, 1, eased));
                yield return new WaitForFixedUpdate();
            } while (coef < 1);
            enableButtons();
            routine = null;
            callAll();
            callback();
        }

        private void setMenuAlpha(float a)
        {
            foreach(Button but in buttons)
            {
                setButtonColorAlpha(but,a);
            }
            foreach (Image img in images)
            {
                setImageColorAlpha(img, a);
            }
            foreach (Text text in texts)
            {
                setTextColorAlpha(text, a);
            }
        }

        private void callAll()
        {
            if (callOnEnable == null) return;
            foreach (CallableByMenuSwitcher element in callOnEnable)
            {
                element.EnableCall();
            }
        }
        //buttons
        private void disableButtons()
        {
            foreach(Button but in buttons)
            {
                but.interactable = false;
            }
        }

        private void enableButtons()
        {
            foreach (Button but in buttons)
            {
                but.interactable = true;
            }
        }

        private void setButtonColorAlpha(Button but, float a)
        {
            ColorBlock colorBlock = but.colors;
            colorBlock.colorMultiplier = a;
            but.colors = colorBlock;
        } 
        //images
        private void setImageColorAlpha(Image image, float a)
        {
            Color color = image.color;
            color.a = a;
            image.color = color;
        }
        //texts
        private void setTextColorAlpha(Text text, float a)
        {
            Color color = text.color;
            color.a = a;
            text.color = color;
        }
    }

    protected delegate void finalCallback();
}

[Serializable]

public class Easings
{
    [SerializeField] private EaseType easingType;

    public float ease(float x)
    {
        switch (easingType)
        {
            case EaseType.Linear:
                return x;
            case EaseType.OutSine:
                return outSine(x);
            case EaseType.OutCubic:
                return outCubic(x);
            case EaseType.OutQuat:
                return outQuat(x);
        }
        throw new Exception($"easing type not implemented {easingType}");
    }

    private enum EaseType
    {
        Linear,
        OutSine,
        OutCubic,
        OutQuat
    }

    private float outSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1) / 2;
    }

    private float outCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    private float outQuat(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }
}

public interface CallableByMenuSwitcher
{
    public void EnableCall();
}