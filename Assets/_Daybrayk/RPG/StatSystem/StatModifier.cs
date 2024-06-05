using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk.rpg
{
    [System.Serializable]
    public class StatModifier
    {
        public enum ModType
        {
            Flat,
            DecimalAdd,
            DecimalMul,
        }

        [SerializeField]
        float _value;
        public float value { get { return _value; } private set { _value = value; } }
        
        [SerializeField]
        [Tooltip("The order in which this modifier is applied in relation to other modifiers. Larger orders are applied later")]
        int _order;
        public int order { get { return _order; } private set { _order = value; } }
        
        [SerializeField]
        [Tooltip("For percent modifieres values are expected to be in decimal format. Values less than 1 are considered a stat decrease")]
        ModType _modType;
        public ModType modType { get { return _modType; } private set { _modType = value; } }

        public object source { get; set; }

        public StatModifier(float value, ModType modType, int order, object source)
        {
            this.value = value;
            this.order = order;
            this.modType = modType;
            this.source = source;
        }
    }
}
