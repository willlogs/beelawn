using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

namespace PT.Garden
{
    public class GrassPatchActivator : MonoBehaviour
    {
        public struct PixelChange{
            public float x;
            public float y;
            public float change;
        }

        public ComputeShader computeShader;
        public PixelChange[] results;

        [SerializeField] private GameObject grassParticle;
        private GameObject[] particlePool;
        private int particleIndex = 0;
        [SerializeField] private InkCanvas _removeCanvas, _windCanvas;
        [SerializeField] private MeshRenderer _mr;
        private bool _hasBeeIn = false, _hasCopy = false;
        public RenderTexture renderTexture;
        public RenderTexture lastRenderTexture;
        private ComputeBuffer resultsBFF;

        private void Start(){
            particlePool = new GameObject[40];
            for(int i = 0; i < 40; i++){
                particlePool[i] = Instantiate(grassParticle);
                particlePool[i].transform.parent = transform;
                particlePool[i].SetActive(false);
            }

            Material _mainMaterial = _mr.materials[0];
            int _txid = Shader.PropertyToID("_NoGrassTex");
            renderTexture = (RenderTexture)_mainMaterial.GetTexture(_txid);
            results = new PixelChange[renderTexture.width * renderTexture.height];
            resultsBFF = new ComputeBuffer(
                results.Length,
                sizeof(float) * 3
            );
        }

        private void DoParticle(Vector3 position){
            GameObject p = particlePool[particleIndex++];
            p.SetActive(true);
            p.transform.position = position;
        }

        private void OnTriggerEnter(Collider other){
            try{
                PositionPainter p = other.GetComponent<PositionPainter>();
                if(p != null){
                    _hasBeeIn = true;
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
                    _hasBeeIn = false;
                    _hasCopy = false;
                    p._isPainting = false;
                }
            }catch {}
        }

        private void Update(){
            if(_hasBeeIn){
                if(!_hasCopy){
                    lastRenderTexture = CopyTexture(renderTexture);
                    _hasCopy = true;
                }
                else{
                    RenderTexture curRT = CopyTexture(renderTexture);

                    computeShader.SetBuffer(0, "results", resultsBFF);
                    computeShader.SetFloat("width", curRT.width);
                    computeShader.SetTexture(0, "InputTXT", curRT);
                    computeShader.SetTexture(0, "LastInputTXT", lastRenderTexture);

                    computeShader.Dispatch(0, curRT.width / 8, curRT.height / 8, 1);
                    resultsBFF.GetData(results);

                    lastRenderTexture = curRT;
                }
            }
        }

        private RenderTexture CopyTexture(RenderTexture t){
            RenderTexture newrt = new RenderTexture(t);
            newrt.enableRandomWrite = true;
            newrt.Create();

            Graphics.Blit(t, newrt);
            return newrt;
        }
    }
}