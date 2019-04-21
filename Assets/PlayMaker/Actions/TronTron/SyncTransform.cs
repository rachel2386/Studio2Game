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
        public bool useLocal = false;


        CharacterController cc;
        ShyFPSController sfc;
		// Code that runs on entering the state.
		public override void OnEnter()
        {
            var go = Fsm.GetOwnerDefaultTarget(source);
            cc = go.GetComponent<CharacterController>();
            sfc = go.GetComponent<ShyFPSController>();

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
            var go = Fsm.GetOwnerDefaultTarget(source);

            if (cc)
                cc.enabled = false;


            if (syncPosition)
            {
                if(useLocal)
                    go.transform.localPosition = target.Value.transform.localPosition;
                else
                    go.transform.position = target.Value.transform.position;
            }
                

            if (syncRotation)
            {

                if (useLocal)
                    go.transform.localRotation = target.Value.transform.localRotation;
                else
                    go.transform.rotation = target.Value.transform.rotation;

                if (sfc)
                    sfc.GetMouseLook().ForceSetRotationFromCurrentGameObject();
            }

            if(syncLocalScale)
                go.transform.localScale = target.Value.transform.localScale;

            if (cc)
                cc.enabled = true;
        }


	}

}
