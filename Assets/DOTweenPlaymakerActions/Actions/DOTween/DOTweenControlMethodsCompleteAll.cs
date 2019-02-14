// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Sends all tweens to their end position (has no effect with tweens that have infinite loops).")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsCompleteAll : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        [Tooltip("For Sequences only: if TRUE internal Sequence callbacks will be fired, otherwise they will be ignored.")]
        public FsmBool withCallbacks;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            withCallbacks = new FsmBool { UseVariable = false, Value = false };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensCompleted = DOTween.CompleteAll(withCallbacks.Value);
            if (debugThis.Value) State.Debug("DOTween Control Methods Complete All - Completed "+numberOfTweensCompleted+" tweens");
            Finish();
        }
    }
}
