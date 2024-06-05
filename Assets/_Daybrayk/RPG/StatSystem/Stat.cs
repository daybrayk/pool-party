using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daybrayk.rpg
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField]
        float _baseValue = 0;
        public float baseValue => _baseValue;

        float _value;
        public float value 
        { 
            get 
            { 
                if (isDirty) UpdateValue(); 
                return _value; 
            } 
        }

        bool isDirty = true;

        List<StatModifier> modifiers = new List<StatModifier>();

        public Stat()
        {
            _value = _baseValue;
        }

        public bool AddModifier(StatModifier mod)
        {
            if (ContainsModifier(mod.source)) return false;
            modifiers.Add(mod);
            isDirty = true;

            return true;
        }

        public bool RemoveModifier(object obj)
        {
            for(int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].source == obj)
                {
                    modifiers.RemoveAt(i);
                    isDirty = true;
                    return true;
                }
            }
            
            return false;
        }

        public bool RemoveModifier(StatModifier mod)
        {
            for (int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].source == mod.source)
                {
                    modifiers.RemoveAt(i);
                    isDirty = true;
                    return true;
                }
            }

            return false;
        }

        public void RemoveAllModifiers()
        {
            modifiers.Clear();
            isDirty = true;
        }

        public bool ContainsModifier(object obj)
        {
            for(int i = 0; i < modifiers.Count; i++)
            {
                if (modifiers[i].source == obj) return true;
            }

            return false;
        }

        private void UpdateValue()
        {
            float total = baseValue;
            float flatTotal = 0;
            float addTotal = 0;
            float mulTotal = 0;
            int currentOrderValue = 0;

            IEnumerable<StatModifier> mods = modifiers.OrderBy(x => x.order);

            //Debug.Log("Updating Stat.value. Mod Count: " + mods.Count());

            for(int i = 0; i <= mods.Count(); i++)
            {
                StatModifier current =  i < mods.Count() ? mods.ElementAt(i) : null;

                if (i >= mods.Count())
                {
                    //Debug.Log("Add Total: " + addTotal);
                    total *= Mathf.Max(0, (1 + addTotal));
                    total += mulTotal;
                    total += flatTotal;
                    addTotal = 0;
                    mulTotal = 0;
                    flatTotal = 0;
                    break;
                    //currentOrderValue = current.order;
                }
                else if(currentOrderValue != current.order)
                {
                    //Debug.Log("Add Total: " + addTotal);
                    total *= Mathf.Max(0, (1 + addTotal));
                    total += mulTotal;
                    total += flatTotal;
                    addTotal = 0;
                    mulTotal = 0;
                    flatTotal = 0;
                    currentOrderValue = current.order;
                    //Debug.Log("Updating Stat.currentOrderValue to " + current.order);
                }

                switch (current.modType)
                {
                    case StatModifier.ModType.Flat:
                        //Debug.Log("Flat ModType: " + current.value);
                        flatTotal += current.value;
                        break;
                    case StatModifier.ModType.DecimalAdd:
                        //Debug.Log("Percent Add ModType: " + current.value);
                        if(current.value >= 1) addTotal += (1 - current.value);
                        else
                        {
                            addTotal -= current.value;
                        }
                        break;
                    case StatModifier.ModType.DecimalMul:
                        //Debug.Log("Percent Multiply ModType: " + current.value);
                        mulTotal = ((mulTotal + total) * (current.value)) - total;
                        break;
                }
            }
            _value = total;
            isDirty = false;
        }
    }
}
