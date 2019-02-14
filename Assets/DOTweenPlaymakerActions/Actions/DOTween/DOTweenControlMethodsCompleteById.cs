// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Sends all tweens with the given ID to their end position (has no effect with tweens that have infinite loops).")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsCompleteById : FsmStateAction
    {
        [Tooltip("Select the tween ID type to use")]
        public Doozy.PlayMaker.Actions.TweenId tweenIdType;

        [UIHint(UIHint.FsmString)]
        [Tooltip("Use a String as the tween ID")]
        public FsmString stringAsId;

        [UIHint(UIHint.Tag)]
        [Tooltip("Use a Tag as the tween ID")]
        public FsmString tagAsId;

        [UIHint(UIHint.FsmGameObject)]
        [Tooltip("Use a GameObject as the tween ID")]
        public FsmGameObject gameObjectAsId;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("For Sequences only: if TRUE internal Sequence callbacks will be fired, otherwise they will be ignored.")]
        public FsmBool withCallbacks;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            stringAsId = new FsmString { UseVariable = false };
            tagAsId = new FsmString { UseVariable = false };
            gameObjectAsId = new FsmGameObject { UseVariable = false, Value = null };
            withCallbacks = new FsmBool { UseVariable = false, Value = false };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensCompleted = 0;
            switch (tweenIdType)
            {
                case Doozy.PlayMaker.Actions.TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) numberOfTweensCompleted = DOTween.Complete(stringAsId.Value, withCallbacks.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) numberOfTweensCompleted = DOTween.Complete(tagAsId.Value, withCallbacks.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseGameObject: if (gameObjectAsId.Value != null) numberOfTweensCompleted = DOTween.Complete(gameObjectAsId.Value, withCallbacks.Value); break;
            }
            if (debugThis.Value) State.Debug("DOTween Control Methods Complete By Id - Completed "+numberOfTweensCompleted+" tweens");
            Finish();
        }
    }
}
