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
            float value = Input.mouseScrollDelta.y;

            if (Input.GetMouseButton(0))
            {
                value = 0.5f;
            }

            store.Value = value;
            // Debug.Log(store.Value);          

        }


	}

}
