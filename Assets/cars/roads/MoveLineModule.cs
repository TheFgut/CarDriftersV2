using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MoveLineModule
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private float pointsDensity;

    private WayCircle circle;
    private Material LineMaterial;

    [SerializeField] private bool enabled = false;
    private Coroutine routine;
    public void enable()
    {
        enabled = true;
        line.enabled = true;
        if (routine != null)
        {
            circle.StopCoroutine(routine);
        }
        routine = circle.StartCoroutine(setEnableRoutine(enabled));
    }
    private const float fadeTime = 1;
    private IEnumerator setEnableRoutine(bool enable)
    {
        float startA = enable ? defaultAlpha : 0;
        float endA = enable ? 0 : defaultAlpha;

        float timer = fadeTime;
        do
        {
            timer -= Time.fixedDeltaTime;
            float alpha = notNativeColor ? 1 : Mathf.Lerp(startA, endA, timer / fadeTime);
            changeLineAlpha(alpha);
            yield return new WaitForFixedUpdate();
        } while (timer > 0);

        if (enable == false)
        {
            line.enabled = false;
        }

    }

    public void disable()
    {
        enabled = false;
        if (routine != null)
        {
            circle.StopCoroutine(routine);
        }
        routine = circle.StartCoroutine(setEnableRoutine(enabled));
    }
    private void changeLineAlpha(float a)
    {

        Gradient gradient = line.colorGradient;
        GradientAlphaKey[] keys = gradient.alphaKeys;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].alpha = a;
        }
        gradient.alphaKeys = keys;
        line.colorGradient = gradient;
    }

    private float dirCoef;
    private float defaultAlpha;
    public void init(WayCircle circle)
    {
        this.circle = circle;
        line.transform.localScale = new Vector3(line.transform.localScale.x / line.transform.lossyScale.x,
           line.transform.localScale.y / line.transform.lossyScale.y,
           line.transform.localScale.z / line.transform.lossyScale.z);


        LineMaterial = line.material;

        initDefaultAlpha();


        dirCoef = circle.clockwise ? -1 : 1;
        Vector2 oldTextureScale = LineMaterial.mainTextureScale;
        LineMaterial.mainTextureScale = new Vector2(oldTextureScale.x * dirCoef, oldTextureScale.y);

        setUpLine(circle.getRadius());

        if (enabled) line.enabled = true;
        else line.enabled = false;

        colorGradient = line.colorGradient;
        initLineMoveAngleSpeed();
    }

    private void initLineMoveAngleSpeed()
    {
        lineMoveAngleSpeed = lineMoveSpeed/circle.getRadius();
    }
    private void initDefaultAlpha()
    {
        GradientAlphaKey[] alphas = line.colorGradient.alphaKeys;
        defaultAlpha = alphas[0].alpha;
    }
    private void setUpLine(float radius)
    {
        float C = 2 * Mathf.PI * radius;
        int pointsCount = (int)(C / pointsDensity);


        float angleStep = 360f / (pointsCount + 1);
        Vector3[] points = new Vector3[pointsCount];
        for (int num = 0; num < pointsCount; num++)
        {
            Quaternion rotation = Quaternion.Euler(0, angleStep * num, 0);
            Vector3 pointPos = (math.rotVector(new Vector3(0, 0, 1), rotation) * radius)
                + circle.transform.position;

            points[num] = pointPos + new Vector3(0, 0.1f, 0);
        }

        line.positionCount = pointsCount;
        line.SetPositions(points);
    }
    private const float lineMoveSpeed = 25f;
    private float lineMoveAngleSpeed;
    public void UpdateLine()
    {
        if (enabled == false)
        {
            return;
        }

        Vector3[] points = new Vector3[line.positionCount];
        int pointsCount = line.GetPositions(points);
        Quaternion rotation = Quaternion.Euler(0, lineMoveAngleSpeed * Time.fixedDeltaTime * -dirCoef, 0);
        for (int num = 0; num < pointsCount; num++)
        {
            Vector3 oldPos = points[num] - circle.transform.position;

            Vector3 newPos = math.rotVector(oldPos, rotation)
                + circle.transform.position;

            points[num] = newPos;
        }
        line.SetPositions(points);
    }

    private Gradient colorGradient;

    private bool notNativeColor = false;
    public void ChangeColorTo(Color color)
    {
        Gradient newGrad = new Gradient();
        GradientColorKey[] keys = colorGradient.colorKeys;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].color = color;
        }
        newGrad.colorKeys = keys;

        GradientAlphaKey[] alphaKeys = line.colorGradient.alphaKeys;
        newGrad.alphaKeys = alphaKeys;

        line.colorGradient = newGrad;

        changeLineAlpha(1);
        notNativeColor = true;
    }

    public void ResetColor()
    {
        notNativeColor = false;
        changeLineAlpha(defaultAlpha);
        line.colorGradient = colorGradient;
    }
}
