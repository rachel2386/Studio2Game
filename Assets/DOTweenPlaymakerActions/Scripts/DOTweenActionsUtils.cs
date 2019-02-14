// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using HutongGames.PlayMaker;

namespace Doozy.PlayMaker.Actions
{
    public static class DOTweenActionsUtils
    {
        public static void Debug(this FsmState state, string message)
        {
            UnityEngine.Debug.Log("GameObject [" + state.Fsm.GameObjectName + "] -> FSM [" + state.Fsm.Name + "] -> State [" + state.Name + "]: " + message);
        }
    }
}