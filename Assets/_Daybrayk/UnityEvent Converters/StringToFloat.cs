using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
	public class StringToFloat : MonoBehaviour
	{
		public UnityEventFloat OnConvert;

		public void Convert(string Value)
        {
			float fValue;

			if (float.TryParse(Value, out fValue)) OnConvert.TryInvoke(fValue);
        }
	}
}