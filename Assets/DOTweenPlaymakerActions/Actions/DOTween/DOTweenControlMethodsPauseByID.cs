// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Pauses all tweens with the given ID")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsPauseById : FsmStateAction
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

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            stringAsId = new FsmString { UseVariable = false };
            tagAsId = new FsmString { UseVariable = false };
            gameObjectAsId = new FsmGameObject { UseVariable = false, Value = null };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensPaused = 0;
            switch (tweenIdType)
            {
                case Doozy.PlayMaker.Actions.TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) numberOfTweensPaused = DOTween.Pause(stringAsId.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) numberOfTweensPaused = DOTween.Pause(tagAsId.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseGameObject: if(gameObjectAsId.Value != null) numberOfTweensPaused = DOTween.Pause(gameObjectAsId.Value); break;
            }
            if (debugThis.Value) State.Debug("DOTween Control Methods Pause By Id - Paused " + numberOfTweensPaused + " tweens");
            Finish();
        }
    }
}
