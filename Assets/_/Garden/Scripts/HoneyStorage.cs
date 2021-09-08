using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Bees;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Linq;

namespace PT.Garden
{
    public class HoneyStorage : MonoBehaviour
    {
        public Dictionary<string, Honey> honeys = new Dictionary<string, Honey>();
        [SerializeField] private Bee _mainBee;
        [SerializeField] private float _honeyAmount;
        [SerializeField] private LiquidVolumeAnimator[] _jars;
        [SerializeField] private int nextFreeJarIdx = 0;
        [SerializeField] private TMPro.TextMeshProUGUI _text;
        [SerializeField] private GameObject _sellButton;

        public void SellHoney(){
            if(_honeyAmount > 0){
                int idx = SceneManager.GetActiveScene().buildIndex;
                if(idx == SceneManager.sceneCount - 1){
                    idx = 0;
                }
                SceneManager.LoadScene(idx + 1);
            }
        }

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
                    _text.text = _honeyAmount + "";
                    UpdateJars();

                    if(_honeyAmount > 0){
                        _sellButton.SetActive(true);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other){
            Bee b = other.GetComponent<Bee>();
            if (b != null)
            {
                if (b.isMain)
                {
                    _sellButton.SetActive(false);
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

        private void Start(){
            _sellButton.SetActive(false);
        }
    }
}