using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class SendEventToList : FsmStateAction
	{
        [RequiredField]
        
        [ArrayEditor(VariableType.GameObject)]
        public FsmArray list;
        public FsmString eventName;
		// Code that runs on entering the state.
		public override void OnEnter()
		{
			Finish();

            foreach(var v in list.Values)
            {
                GameObject go = v as GameObject;
                go.MySendEventToAll(eventName.Value);
            }
		}


	}

}
