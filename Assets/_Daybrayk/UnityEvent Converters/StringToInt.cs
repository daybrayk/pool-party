using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
	public class StringToInt : MonoBehaviour
	{
		public UnityEventInt OnConvert;

		public void Convert(string value)
        {
			int intValue;
			if (int.TryParse(value, out intValue)) OnConvert.TryInvoke(intValue);
        }
	}
}