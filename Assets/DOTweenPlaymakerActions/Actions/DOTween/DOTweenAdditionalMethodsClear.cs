// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using DG.Tweening;
using Doozy.PlayMaker.Actions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("DOTween")]
    [Tooltip("Kills all tweens, clears all pools, resets the max Tweeners/Sequences capacities to the default values.")]
    [HelpUrl("http://dotween.demigiant.com/documentation.php")]
    public class DOTweenAdditionalMethodsClear : FsmStateAction
    {
        [Tooltip("If TRUE also destroys DOTween's gameObject and resets its initialization, default settings and everything else (so that next time you use it it will need to be re-initialized).")]
        public FsmBool destroy;

        [ActionSection("Debug Options")] public FsmBool debugThis;

        public override void Reset()
        {
            base.Reset();
            destroy = new FsmBool {UseVariable = false, Value = false};
            debugThis = new FsmBool {Value = false};
        }

        public override void OnEnter()
        {
            DOTween.Clear(destroy.Value);
            if (debugThis.Value) State.Debug("DOTween Additional Methods Clear");
            Finish();
        }
    }
}