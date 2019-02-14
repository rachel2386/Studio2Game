// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Sends all tweens with the given ID to the given position (calculating also eventual loop cycles)")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsGoToById : FsmStateAction
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

        [RequiredField]
        [UIHint(UIHint.FsmFloat)]
        [Tooltip("Time position to reach (if higher than the whole tween duration the tween will simply reach its end).")]
        public FsmFloat to;

        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE the tween will play after reaching the given position, otherwise it will be paused.")]
        public FsmBool andPlay;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            stringAsId = new FsmString { UseVariable = false };
            tagAsId = new FsmString { UseVariable = false };
            gameObjectAsId = new FsmGameObject { UseVariable = false, Value = null };
            to = new FsmFloat { UseVariable = false };
            andPlay = new FsmBool { UseVariable = false, Value = false };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensInvolved = 0;
            switch (tweenIdType)
            {
                case Doozy.PlayMaker.Actions.TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) numberOfTweensInvolved = DOTween.Goto(stringAsId.Value, to.Value, andPlay.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) numberOfTweensInvolved = DOTween.Goto(tagAsId.Value, to.Value, andPlay.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseGameObject: if (gameObjectAsId.Value != null) numberOfTweensInvolved = DOTween.Goto(gameObjectAsId.Value, to.Value, andPlay.Value); break;
            }
            if (debugThis.Value) State.Debug("DOTween Control Methods Go To By Id - " + numberOfTweensInvolved + " tweens involved");
            Finish();
        }
    }
}
