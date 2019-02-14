// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Rewinds and pauses all tweens (meaning tweens that were not already rewinded)")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsRewindAll : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE includes the eventual tween delay, otherwise skips it.")]
        public FsmBool includeDelay;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            includeDelay = new FsmBool { UseVariable = false, Value = true };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensRewinded = DOTween.RewindAll(includeDelay.Value);
            if (debugThis.Value) State.Debug("DOTween Control Methods Rewind All - Rewinded and paused " + numberOfTweensRewinded + " tweens");
            Finish();
        }
    }
}
