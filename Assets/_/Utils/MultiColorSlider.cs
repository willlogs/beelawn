using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Utils
{
    public class MultiColorSlider : MonoBehaviour
    {
        [SerializeField] private Image[] _filleds;
        [SerializeField] private Color[] _Colors;
        [SerializeField] private float[] _values;
        [SerializeField] private float _sum = 0;

        public void SetValue(int idx, float val)
        {
            _sum = 0;
            for (int i = _filleds.Length - 1; i >= 0; i--)
            {
                _sum += _values[i];
            }

            float oneMinusSum = 1 - _sum;
            val = oneMinusSum < val && oneMinusSum != 0 ? oneMinusSum : val;
            if (_sum <= 1.15 && idx < _values.Length)
            {
                _sum += val;
                _values[idx] = val;
                UpdateImages();
            }
        }

        public void UpdateImages()
        {
            float sum = 0;
            for (int i = _filleds.Length - 1; i >= 0; i--)
            {
                _filleds[i].fillAmount = sum + _values[i];
                sum += _values[i];
            }
        }

        private void Start()
        {
            UpdateColors();

            // test
            StartCoroutine(TestFill());
        }

        private IEnumerator TestFill(){
            int i = 0;
            float val = 0;
            while(i < _filleds.Length){
                yield return new WaitForEndOfFrame();

                val += Time.deltaTime;
                SetValue(i, val);

                if(val >= 0.2f){
                    val = 0f;
                    i++;
                }
            }
        }

        private void UpdateColors()
        {
            for (int i = 0; i < _filleds.Length; i++)
            {
                _filleds[i].color = _Colors[i];
            }
        }
    }
}