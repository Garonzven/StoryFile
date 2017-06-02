using UnityEngine;
using System.Collections;
using DDK.Base.Extensions;
using System.Collections.Generic;
using DDK.Base.Managers;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DDK.Base.Statics;
using DDK.Base.Classes;
using DDK.Base.SoundFX;
using System;

#if USE_NODE_CANVAS
using NodeCanvas.DialogueTrees;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
#endif

namespace DDK.NodeCanvas
{
	/// <summary>
	/// This class can be used by any script to inherit from and add more functionality to Unity's MonoBehaviour
	/// </summary>
	public class NCUtility : MonoBehaviour {

		#if USE_NODE_CANVAS
		public Blackboard[] blackboards;


		/// <summary>
		/// Returns the index of one element that is in a list, if the element is not contained in the list the return is -1
		/// </summary>
		/// <param name="listTarget">
		/// Is the list where is supposed to be the element
		///</param>
		/// <param name="element">
		/// Is the element that will be searched in the list
		///</param>
		public int IndexOf( List<GameObject> targetList, GameObject element )
		{
		return targetList.IndexOf(element);
		}
		/// <summary>
		/// Returns a sublist according to the initRange and endRange parameters
		/// </summary>
		/// <param name="listTarget">
		/// Is the list where the sublist will be taken from
		///</param>
		/// <param name="initRange">
		/// Is the initial index of the sublist inside the target list
		///</param>
		/// /// <param name="endRange">
		/// Is the end index of the sublist inside the target list
		///</param>
		public List<GameObject> GetRange( List<GameObject> targetList, int initRange, int endRange )
		{
		return targetList.GetRange(initRange,endRange);
		}
		/// <summary>
		/// Modify an int var of a blackboard, the paremeters are passed inside a string splited by ';' char
		/// </summary>
		/// <param name="vars">
		/// examples:
		/// to assign 3 to the variable named numParticles of the blackboards[2] the string "2;numParticles;3" must be passed
		/// to assign 100 to the variable named maxTime of the blackboards[0] the string "0;maxTime;100" must be passed
		///</param>
		public void SetBlackboardIntVariable(string vars)
		{
			string[] splitedVars = 	vars.Split(';');
			SetBlackboardIntVar( int.Parse(splitedVars[0]), splitedVars[1], int.Parse(splitedVars[2]) );
		}
		/// <summary>
		/// Modify an int var of a blackboard, the paremeters specifies wich variable and wich blackboard will be modified
		/// </summary>
		/// <param name="indexBlackboard">
		/// is the index inside the array of blackboards
		/// </param>
		/// <param name="variableName">
		/// is the name of the variable that will be changed
		/// </param>
		/// <param name="newValue">
		/// is the new value that will be assigned to the variable
		/// </param>
		public void SetBlackboardIntVar(int indexBlackboard, string variableName, int newValue )
		{
			blackboards[indexBlackboard].SetValue (variableName,newValue);
		}
		/// <summary>
		/// Modify a bool var of a blackboard, the paremeters are passed inside a string splited by ';' char
		/// </summary>
		/// <param name="vars">
		/// examples:
		/// to assign true to the variable named finished of the blackboards[5] the string "5;finished;true" must be passed
		/// to assign false to the variable named userAnswered of the blackboards[0] the string "0;userAnswered;false" must be passed
		///</param>
		public void SetBlackboardBoolVariable(string vars)
		{
		string[] splitedVars = 	vars.Split(';'); 
		SetBlackboardBoolVar( int.Parse(splitedVars[0]), splitedVars[1], bool.Parse(splitedVars[2]) );
		}
		/// <summary>
		/// Modify a bool var of a blackboard, the paremeters specifies wich variable and wich blackboard will be modified
		/// </summary>
		/// <param name="indexBlackboard">
		/// is the index inside the array of blackboards
		/// </param>
		/// <param name="variableName">
		/// is the name of the variable that will be changed
		/// </param>
		/// <param name="newValue">
		/// is the new value that will be assigned to the variable
		/// </param>
		public void SetBlackboardBoolVar(int indexBlackboard, string variableName, bool newValue )
		{
		blackboards[indexBlackboard].SetValue (variableName,newValue);
		}
		#endif

	}
}
