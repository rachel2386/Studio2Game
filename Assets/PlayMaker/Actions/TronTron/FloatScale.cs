using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class FloatScale : FsmStateAction
	{
        public FsmFloat value;
        public FsmFloat store;

        public FsmFloat originScale;
        public FsmFloat targetScale;       

        public bool everyFrame = false;

		// Code that runs on entering the state.
		public override void OnEnter()
		{
            DoScale();
            if(!everyFrame)
			    Finish();
		}

        public override void OnUpdate()
        {
            if(everyFrame)
                DoScale();
        }

        public void DoScale()
        {
            var coef = 1.0f / originScale.Value * targetScale.Value;
            store.Value = value.Value * coef;
        }


	}

}
