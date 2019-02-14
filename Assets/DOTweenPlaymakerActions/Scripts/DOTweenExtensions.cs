// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Doozy.PlayMaker
{
    public static class DOTweenExtensions
    {
        public static void SetTweenId(this Tween tween, TweenId tweenId, FsmString stringAsId, FsmString tagAsId, GameObject gameObject)
        {
            switch (tweenId)
            {
                case TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) tween.SetId(stringAsId.Value); break;
                case TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) tween.SetId(tagAsId.Value); break;
                case TweenId.UseGameObject: tween.SetId(gameObject); break;
            }
        }
        
        public static void SetTweenId(this Sequence sequence, TweenId tweenId, FsmString stringAsId, FsmString tagAsId, GameObject gameObject)
        {
            switch (tweenId)
            {
                case TweenId.UseString:     if (string.IsNullOrEmpty(stringAsId.Value) == false) sequence.SetId(stringAsId.Value); break;
                case TweenId.UseTag:        if (string.IsNullOrEmpty(tagAsId.Value) == false) sequence.SetId(tagAsId.Value); break;
                case TweenId.UseGameObject: sequence.SetId(gameObject); break;
            }
        }

        public static void SetSelectedEase(this Tween tween, SelectedEase selectedEase, Ease easeType, FsmAnimationCurve animationCurve)
        {
            switch (selectedEase)
            {
                case SelectedEase.EaseType: tween.SetEase(easeType); break;
                case SelectedEase.AnimationCurve: tween.SetEase(animationCurve.curve); break;
            }
        }
        
        public static void SetSelectedEase(this Sequence sequence, SelectedEase selectedEase, Ease easeType, FsmAnimationCurve animationCurve)
        {
            switch (selectedEase)
            {
                case SelectedEase.EaseType: sequence.SetEase(easeType); break;
                case SelectedEase.AnimationCurve: sequence.SetEase(animationCurve.curve); break;
            }
        }
    }
}