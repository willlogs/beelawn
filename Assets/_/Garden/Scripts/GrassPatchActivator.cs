using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

namespace PT.Garden
{
    public class GrassPatchActivator : MonoBehaviour
    {
        [SerializeField] private InkCanvas _removeCanvas, _windCanvas;
        private void OnTriggerEnter(Collider other){
            try{
                PositionPainter p = other.GetComponent<PositionPainter>();
                if(p != null){
                    p._removeCanvas = _removeCanvas;
                    p._bendCanvas = _windCanvas;
                    p._isPainting = true;
                }
            }catch {}
        }

        private void OnTriggerExit(Collider other){
            try{
                PositionPainter p = other.GetComponent<PositionPainter>();
                if(p != null){
                    p._isPainting = false;
                }
            }catch {}
        }
    }
}