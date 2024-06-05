using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk
{
	public class FloatToString : MonoBehaviour
	{
		public UnityEventString OnConvert;

		public void Convert(float value)
        {
			OnConvert.TryInvoke(value.ToString());
        }
	}
}