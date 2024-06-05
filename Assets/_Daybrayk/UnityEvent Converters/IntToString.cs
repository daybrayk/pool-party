using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Daybrayk
{
	public class IntToString : MonoBehaviour
	{
		public UnityEventString OnConvert;
		
		public void Convert(int value)
        {
			OnConvert.TryInvoke(value.ToString());
        }
	}
}