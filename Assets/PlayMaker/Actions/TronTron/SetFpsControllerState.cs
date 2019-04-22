using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class SetFpsControllerState : FsmStateAction
	{
        ShyFPSController sfc;


        public FsmBool lockMouseLook = new FsmBool { UseVariable = true };
        public FsmBool lockMove = new FsmBool { UseVariable = true };

        public FsmBool tmpShowCursor = new FsmBool { UseVariable = true };

        public FsmBool centerDot = new FsmBool { UseVariable = true };

        public FsmBool menuMode = new FsmBool { UseVariable = true };

        public FsmBool allowMicroMv = new FsmBool { UseVariable = true };

        // Code that runs on entering the state.
        public override void OnEnter()
		{
            var sfc = GameObject.FindObjectOfType<ShyFPSController>();
            if(!lockMouseLook.IsNone)
                sfc.lockMouseLook = lockMouseLook.Value;

            if (!lockMove.IsNone)
                sfc.lockMove = lockMove.Value;

            if(!tmpShowCursor.IsNone)
                sfc.SetTempShowCursor(tmpShowCursor.Value);


            if(!centerDot.IsNone)
            {
                var shyUI = GameObject.FindObjectOfType<ShyUI>();
                shyUI.ShowCenerDot(centerDot.Value);
            }

            if(!menuMode.IsNone)
            {
                sfc.GetMouseLook().SetMenuMode(menuMode.Value);
            }

            if(!allowMicroMv.IsNone)
            {
                sfc.GetMouseLook().SetAllowMicroMv(allowMicroMv.Value);
            }

            Finish();
		}


	}

}
