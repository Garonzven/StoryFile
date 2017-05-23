//By: Daniel Soto
//4dsoto@gmail.com
using UnityEngine;
using System.Collections;
using DDK.Base.SoundFX;
using System.Collections.Generic;

#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
#endif


namespace DDK.NodeCanvas.DialogCanvas 
{
    #if USE_NODE_CANVAS
	/// <summary>
	/// Holds data of what's being said usualy by an actor
	/// </summary>
	[System.Serializable]
	public class MultiAudioStatement : Statement
	{		
		public float talkingRate = 0.1f;
		public List<DelayedAudioClip> secondaryAudios = new List<DelayedAudioClip>();
		
		//required
		public MultiAudioStatement(){}
		public MultiAudioStatement(string text){
			this.text = text;
		}		
		public MultiAudioStatement(string text, AudioClip audio) : this( text )
		{
			this.audio = audio;
		}		
		public MultiAudioStatement(string text, AudioClip audio, string meta) : this( text, audio )
		{
			this.meta = meta;
		}
		public MultiAudioStatement(string text, AudioClip audio, string meta, List<DelayedAudioClip> secondaryAudios) : 
			this( text, audio, meta )
		{
			this.secondaryAudios = secondaryAudios;
        }
		public MultiAudioStatement(string text, AudioClip audio, string meta, float talkingRate, 
		                           List<DelayedAudioClip> secondaryAudios) : this( text, audio, meta, secondaryAudios )
		{
			this.talkingRate = talkingRate;
		}


		/// <summary>
		/// Replace the text of the statement found in brackets, with blackboard variables ToString
		/// </summary>
		new public MultiAudioStatement BlackboardReplace(IBlackboard bb){
			var s = text;
			var i = 0;
			while ( (i = s.IndexOf('[', i)) != -1){
				
				var end = s.Substring(i + 1).IndexOf(']');
				var varName = s.Substring(i + 1, end);
				var output = s.Substring(i, end + 2);
				
				object o = null;
				if (bb != null){
					var v = bb.GetVariable(varName, typeof(object));
					if (v != null){
						o = v.value;
                    }
                }
                s = s.Replace(output, o != null? o.ToString() : output);
                
                i++;
            }
            
			return new MultiAudioStatement(s, audio, meta, talkingRate, secondaryAudios);
		}
    }
    #endif
}