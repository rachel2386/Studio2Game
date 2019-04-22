using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShyUI : MonoBehaviour
{
    public GameObject centerDot;
    public GameObject centerText;

    public GameObject curtainRoot;
    public GameObject topCurtain;
    public GameObject bottomCurtain;

    public GameObject centerProgress;
    public GameObject progressBack;
    public GameObject progressTop;
    public GameObject progressChange;

    float curProgress;
    Color progressChangeColor;

    public void SetCurtainHeight(float height)
    {
        var sizeH = topCurtain.GetComponent<RectTransform>().sizeDelta;
        sizeH.y = height;
        topCurtain.GetComponent<RectTransform>().sizeDelta = sizeH;

        var sizeB = bottomCurtain.GetComponent<RectTransform>().sizeDelta;
        sizeB.y = height;
        bottomCurtain.GetComponent<RectTransform>().sizeDelta = sizeB;
    }

    public float GetCurtainHeight()
    {
        return bottomCurtain.GetComponent<RectTransform>().sizeDelta.y;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitCurtain();
        InitProgress();
    }

    void InitCurtain()
    {
        curtainRoot.SetActive(true);
        SetCurtainHeight(0);
    }

    public void ShowCutain()
    {
        curtainRoot.SetActive(true);
        SetCurtainHeight(180);
    }

    public void HideCurtain()
    {
        curtainRoot.SetActive(false);
    }

    void InitProgress()
    {
        centerProgress.SetActive(false);
        progressChange.SetActive(false);
        SetProgress(0);
        progressChangeColor = progressChange.GetComponent<Image>().color;
    }

    void SetProgress(float progress)
    {
        progress = Mathf.Clamp(progress, 0, 1);
        curProgress = progress;
        UpdateProgressUI();
    }

    public float GetProgress()
    {
        return curProgress;
    }

    public void ShowProgress(bool show)
    {
        centerProgress.SetActive(show);
    }

    public void AddProgress(float add)
    {
        if (inDecreaseAnimation)
            return;

        SetProgress(curProgress + add);
    }

    bool inDecreaseAnimation = false;
    Sequence changeProgressTween;
    public void SuddenDecreaseProgress(float value)
    {
        if (changeProgressTween != null)
            changeProgressTween.Kill();

        inDecreaseAnimation = true;

        var rt = progressChange.transform.eulerAngles;
        rt.z = -1 * curProgress * 360f;
        progressChange.transform.eulerAngles = rt;

        curProgress -= value;
        curProgress = Mathf.Max(0, curProgress);
        UpdateProgressUI();

        var fadeDt = 0.6f;

        var image = progressChange.GetComponent<Image>();
        progressChange.SetActive(true);
        image.fillAmount = value;


        var c = image.color;
        c.a = 0;
        image.color = c;
        
        changeProgressTween = DOTween.Sequence();
        changeProgressTween.Append(image.DOFade(progressChangeColor.a, fadeDt / 2));
        changeProgressTween.Append(image.DOFade(0f, fadeDt / 2));
        changeProgressTween.AppendCallback(()=>{
            progressChange.SetActive(false);
            inDecreaseAnimation = false;
        });       

    }

    void UpdateProgressUI()
    {
        progressTop.GetComponent<Image>().fillAmount = curProgress;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowCenerDot(bool show)
    {
        centerDot.SetActive(show);
    }
}
