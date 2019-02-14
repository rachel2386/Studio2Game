// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Kills all tweens with the given ID. A tween is killed automatically when it reaches completion (unless you prevent it using SetAutoKill(false)), but you can use this method to kill it sooner if you don't need it anymore.")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsKillById : FsmStateAction
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
        [Tooltip("If TRUE instantly completes the tween before killing it.")]
        public FsmBool complete;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            stringAsId = new FsmString { UseVariable = false };
            tagAsId = new FsmString { UseVariable = false };
            gameObjectAsId = new FsmGameObject { UseVariable = false, Value = null };
            complete = new FsmBool { UseVariable = false, Value = false };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensKilled = 0;
            switch (tweenIdType)
            {
                case Doozy.PlayMaker.Actions.TweenId.UseString: if (string.IsNullOrEmpty(stringAsId.Value) == false) numberOfTweensKilled = DOTween.Kill(stringAsId.Value, complete.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseTag: if (string.IsNullOrEmpty(tagAsId.Value) == false) numberOfTweensKilled = DOTween.Kill(tagAsId.Value, complete.Value); break;
                case Doozy.PlayMaker.Actions.TweenId.UseGameObject: if (gameObjectAsId.Value != null) numberOfTweensKilled = DOTween.Kill(gameObjectAsId.Value, complete.Value); break;
            }
            if (debugThis.Value) State.Debug("DOTween Control Methods Kill By Id - Killed " + numberOfTweensKilled + " tweens");
            Finish();
        }
    }
}
