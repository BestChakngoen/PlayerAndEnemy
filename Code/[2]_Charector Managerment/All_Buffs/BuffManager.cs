using UnityEngine;
using System.Collections.Generic;

namespace BasicEnemy
{
    public class BuffManager : MonoBehaviour
    {
        private List<Buff> activeBuffs = new List<Buff>();

        void Update()
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                Buff buff = activeBuffs[i];
                buff.OnTick(Time.deltaTime);

                if (buff.IsFinished)
                {
                    buff.OnExpire();
                    activeBuffs.RemoveAt(i);
                }
            }
        }
        
        public void AddBuff(Buff buff)
        {
            buff.OnApply(this.gameObject); 
            
            if (!buff.IsFinished)
            {
                activeBuffs.Add(buff);
            }
        }
        public float ApplyDamageModifiers(float initialDamage)
        {
            float modifiedDamage = initialDamage;
            
            foreach (Buff buff in activeBuffs)
            {
                modifiedDamage = buff.ModifyIncomingDamage(modifiedDamage);
            }

            return modifiedDamage;
        }
        public void ClearAllBuffs()
        {
            for (int i = activeBuffs.Count - 1; i >= 0; i--)
            {
                activeBuffs[i].OnExpire();
            }
            activeBuffs.Clear(); 
        }
    }
}