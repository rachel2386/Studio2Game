// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Clears all cached tween pools.")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenAdditionalMethodsClearCachedTweens : FsmStateAction
    {
        [ActionSection("Debug Options")] public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            debugThis = new FsmBool {Value = false};
        }

        public override void OnEnter()
        {
            DOTween.ClearCachedTweens();
            if (debugThis.Value) State.Debug("DOTween Additional Methods Clear Cached Tweens");
            Finish();
        }
    }
}