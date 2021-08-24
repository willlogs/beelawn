using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PT.Garden{
    public class PercentageChecker : MonoBehaviour
    {
        public float percentage = 0;

        [SerializeField] private Color _refree;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private int _matIndex = 0;
        [SerializeField] private string _mainTexName = "_NoGrassTex";
        [SerializeField] private float _betweenReads = 0.5f;
        private int _txid;
        private Material _mainMaterial;
        private Texture _texture;
        private bool _isChecking = true, _checkSig = false;

        private void Start(){
            _mainMaterial = _meshRenderer.materials[_matIndex];
            _txid = Shader.PropertyToID(_mainTexName);

            StartCoroutine(Check());
        }
        
        private IEnumerator Check(){
            while(_isChecking){
                yield return new WaitForSeconds(_betweenReads);

                try{
                    if(!_checkSig){
                        RenderTexture t = (RenderTexture)_texture;
                        _texture = _mainMaterial.GetTexture(_txid);
                        Texture2D t2d = new Texture2D(_texture.width, _texture.height, TextureFormat.RGBA32, false);

                        RenderTexture currentRT = RenderTexture.active;
                        RenderTexture.active = t;
                        t2d.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);
                        t2d.Apply();
                        RenderTexture.active = currentRT;

                        _checkSig = true;
                        Thread thread = new Thread(() => {
                            Calculate(t2d);
                        });
                        thread.Start();
                    }
                }
                catch{
                    print("err reading percentage");
                }
            }
        }

        private void Calculate(Texture2D t2d){
            if(_checkSig){                        
                float diff = 0;
                for(int x = 0; x < t2d.width; x++){
                    for(int y = 0; y < t2d.height; y++){
                        Color c = t2d.GetPixel(x, y);
                        diff += Mathf.Abs(c.r - _refree.r) + Mathf.Abs(c.g - _refree.g) + Mathf.Abs(c.b - _refree.b);
                    }
                }

                percentage = diff / t2d.width / t2d.height;
                _checkSig = false;
            }
        }
    }
}