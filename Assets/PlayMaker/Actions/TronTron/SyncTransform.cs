using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

	[ActionCategory("TronTron")]
	public class SyncTransform : FsmStateAction
	{
        public FsmOwnerDefault source;
        public FsmGameObject target;

        public bool syncPosition = false;
        public bool syncRotation = false;
        public bool syncLocalScale = false;
        public bool everyFrame;


        CharacterController cc;
        ShyFPSController sfc;
		// Code that runs on entering the state.
		public override void OnEnter()
        {
            cc = source.GameObject.Value.GetComponent<CharacterController>();
            sfc = source.GameObject.Value.GetComponent<ShyFPSController>();

            DoSync();
            if(!everyFrame)
			    Finish();
		}

        public override void OnUpdate()
        {
            if (everyFrame)
                DoSync();
        }

        void DoSync()
        {
            if (cc)
                cc.enabled = false;


            if (syncPosition)
                source.GameObject.Value.transform.position = target.Value.transform.position;

            if (syncRotation)
            {
                
                
                source.GameObject.Value.transform.rotation = target.Value.transform.rotation;
                if (sfc)
                    sfc.GetMouseLook().ForceSetRotationFromCurrentGameObject();
            }

            if(syncLocalScale)
                source.GameObject.Value.transform.localScale = target.Value.transform.localScale;

            if (cc)
                cc.enabled = true;
        }


	}

}
