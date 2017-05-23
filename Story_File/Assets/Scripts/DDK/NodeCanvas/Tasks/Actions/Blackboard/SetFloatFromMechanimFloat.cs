//By: Daniel Soto
//4dsoto@gmail.com
#if USE_NODE_CANVAS
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using ParadoxNotion;
using UnityEngine;


namespace DDK.NodeCanvas.Tasks.Actions{

	[Category("✫ Blackboard")]
	[Description("Set a blackboard float variable")]
	public class SetFloatFromMechanimFloat : ActionTask<Animator> {
		
		[BlackboardOnly]
		public BBParameter<float> valueA;
		public OperationMethod Operation = OperationMethod.Set;
		[RequiredField]
		public BBParameter<string> parameter;
		
		protected override string info{
			get	{return valueA + OperationTools.GetOperationString(Operation) + parameter;}
		}
		
		protected override void OnExecute(){
			valueA.value = OperationTools.Operate(valueA.value, agent.GetFloat(parameter.value), Operation);
			EndAction(true);
		}
	}
}
#endif
