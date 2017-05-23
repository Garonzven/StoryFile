//By: Daniel Soto
//4dsoto@gmail.com
#if USE_NODE_CANVAS
using UnityEngine;
using System.Collections;
using ParadoxNotion.Design;
using NodeCanvas.Framework;
using ParadoxNotion;


namespace DDK.NodeCanvas.Tasks.Conditions
{
	[Name("Check Mecanim Float Approximately")]
	[Category("Mecanim")]
	public class MechanimCheckFloatEqualToApproximately : ConditionTask<Animator> {
		
		[RequiredField]
		public BBParameter<string> parameter;
		private CompareMethod _comparison = CompareMethod.EqualTo;
		public BBParameter<float> value;
		
		protected override string info{
			get
			{
				return "Mec.Float " + parameter.ToString() + OperationTools.GetCompareString(_comparison) + value;
			}
		}
		
		protected override bool OnCheck(){
			
			return Mathf.Approximately( (float)agent.GetFloat(parameter.value), (float)value.value );
		}
	}    
}
#endif