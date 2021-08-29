using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Bees;
using DG.Tweening;

namespace PT.Garden
{
    public class HoneyStorage : MonoBehaviour
    {
        public Dictionary<string, Honey> honeys = new Dictionary<string, Honey>();
        [SerializeField] private Bee _mainBee;
        [SerializeField] private float _honeyAmount;
        [SerializeField] private LiquidVolumeAnimator[] _jars;
        [SerializeField] private int nextFreeJarIdx = 0;

        private void OnTriggerEnter(Collider other)
        {
            Bee b = other.GetComponent<Bee>();
            if (b != null)
            {
                if (b.isMain)
                {
                    _mainBee = b;
                    Honey[] honeys = b._chain.GetHoneys();
                    for (int i = 0; i < honeys.Length; i++)
                    {
                        CatchHoney(honeys[i]);
                    }
                    UpdateJars();
                }
            }
        }

        private void CatchHoney(Honey h)
        {
            Honey honeyRef;
            if (honeys.ContainsKey(h.ID))
            {
                honeys.TryGetValue(h.ID, out honeyRef);
                honeyRef.amount += h.amount;
            }
            else
            {
                honeyRef = new Honey(h);
                honeyRef.jarIndex = nextFreeJarIdx++;
                honeyRef.amount = h.amount;
                honeys.Add(h.ID, honeyRef);
            }

            _honeyAmount += h.amount;
        }

        private void UpdateJars(){
            foreach(Honey h in honeys.Values){
                LiquidVolumeAnimator jar = _jars[h.jarIndex];

                DOTween.To(
                    () => jar.level,
                    (x) => {jar.level = x;},
                    h.amount / 2000,
                    1f
                );

                _jars[h.jarIndex].mats[0].SetColor("_EmissionColor", h.color);
                _jars[h.jarIndex].mats[0].SetColor("_SEmissionColor", h.color);
            }
        }
    }
}