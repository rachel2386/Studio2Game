using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class GetMouseWheelDelta : FsmStateAction
	{
        public FsmFloat store;
        public bool everyFrame;
		// Code that runs on entering the state.
		public override void OnEnter()
		{
            DoGetDelta();

            if(!everyFrame)
                Finish();
		}

        public override void OnUpdate()
        {
            if (everyFrame)
                DoGetDelta();
        }


        void DoGetDelta()
        {
            store.Value = Input.mouseScrollDelta.y;
        }


	}

}
