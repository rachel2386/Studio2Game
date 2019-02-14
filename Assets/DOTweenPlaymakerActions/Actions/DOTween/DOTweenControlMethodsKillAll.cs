// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Kills all tweens. A tween is killed automatically when it reaches completion (unless you prevent it using SetAutoKill(false)), but you can use this method to kill it sooner if you don't need it anymore.")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenControlMethodsKillAll : FsmStateAction
    {
        [UIHint(UIHint.FsmBool)]
        [Tooltip("If TRUE instantly completes the tween before killing it.")]
        public FsmBool complete;

        [Tooltip("KillAll only > Eventual ids to exclude from the operation.")]
        public FsmString[] idsToExclude;

        [ActionSection("Debug Options")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            complete = new FsmBool { UseVariable = false, Value = false };
            debugThis = new FsmBool { Value = false };
        }

        public override void OnEnter()
        {
            int numberOfTweensKilled = 0;
            if (idsToExclude.Length > 0)
            {
                var idList = new string[idsToExclude.Length];
                for (int i = 0; i < idList.Length; i++)
                    idList[i] = idsToExclude[i].Value;
                numberOfTweensKilled = DOTween.KillAll(complete.Value, idList);
            }
            else
            {
                numberOfTweensKilled = DOTween.KillAll(complete.Value);
            }
            if (debugThis.Value) State.Debug("DOTween Control Methods Kill All - Killed " + numberOfTweensKilled + " tweens");
            Finish();
        }
    }
}
