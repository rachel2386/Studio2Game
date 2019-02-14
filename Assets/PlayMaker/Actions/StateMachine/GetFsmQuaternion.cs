// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine)]
    [ActionTarget(typeof(PlayMakerFSM), "gameObject,fsmName")]
	[Tooltip("Get the value of a Quaternion Variable from another FSM.")]
	public class GetFsmQuaternion : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject that owns the FSM.")]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.FsmName)]
		[Tooltip("Optional name of FSM on Game Object")]
		public FsmString fsmName;

		[RequiredField]
		[UIHint(UIHint.FsmQuaternion)]
		public FsmString variableName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmQuaternion storeValue;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

        private GameObject goLastFrame;
        private string fsmNameLastFrame;
		protected PlayMakerFSM fsm;

		public override void Reset()
		{
			gameObject = null;
			fsmName = "";
			variableName = "";
			storeValue = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetFsmVariable();

			if (!everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			DoGetFsmVariable();
		}

        private void DoGetFsmVariable()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null)
			{
				return;
			}

            if (go != goLastFrame || fsmName.Value != fsmNameLastFrame)
            {
                goLastFrame = go;
                fsmNameLastFrame = fsmName.Value;
                // only get the fsm component if go or fsm name has changed
				fsm = ActionHelpers.GetGameObjectFsm(go, fsmName.Value);
			}

			if (fsm == null || storeValue == null)
			{
				return;
			}

			var fsmVar = fsm.FsmVariables.GetFsmQuaternion(variableName.Value);

			if (fsmVar != null)
			{
				storeValue.Value = fsmVar.Value;
			}
		}

	}
}