using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class SetFpsControllerState : FsmStateAction
	{
        ShyFPSController sfc;


        public bool lockMouseLook;
        public bool lockMove;

        public bool tmpShowCursor;

        public bool centerDot = true;

        // Code that runs on entering the state.
        public override void OnEnter()
		{
            var sfc = GameObject.FindObjectOfType<ShyFPSController>();
            sfc.lockMouseLook = lockMouseLook;
            sfc.lockMove = lockMove;

            sfc.SetTempShowCursor(tmpShowCursor);

            var shyUI = GameObject.FindObjectOfType<ShyUI>();
            shyUI.ShowCenerDot(centerDot);
            Finish();
		}


	}

}
