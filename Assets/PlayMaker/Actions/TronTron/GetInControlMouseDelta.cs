using BindingsExample;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class GetInControlMouseDelta : FsmStateAction
	{

        [Tooltip("Repeat every frame")]
        public bool everyFrame;

        public FsmFloat storeLength;
        DoubleRachelPlayerActions pa;

        // Code that runs on entering the state.
        public override void OnEnter()
		{
            pa = LevelManager.Instance.PlayerActions;
            


            DoGetMouseDelta();
            if (!everyFrame)
			    Finish();
		}

        public override void OnUpdate()
        {
            if(everyFrame)
                DoGetMouseDelta();
        }

        void DoGetMouseDelta()
        {
            var v = new Vector2(pa.Look.X, pa.Look.Y);
            storeLength.Value = v.magnitude;
        }
	}

}
