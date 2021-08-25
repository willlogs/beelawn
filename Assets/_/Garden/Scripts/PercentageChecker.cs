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
        private int width, height;
        private Color[] colors;

        private void Start(){
            _mainMaterial = _meshRenderer.materials[_matIndex];
            _txid = Shader.PropertyToID(_mainTexName);

            StartCoroutine(Check());

            Thread thread = new Thread(Calculate);
            thread.Start();
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
                        colors = t2d.GetPixels();
                        width = t2d.width;
                        height = t2d.height;
                    }
                }
                catch{
                    print("err reading percentage");
                }
            }
        }

        private void Calculate(){
            while(_isChecking){
                if(_checkSig){
                    float diff = 0;
                    for(int x = 0; x < width; x++){
                        int w = width * x;
                        for(int y = 0; y < height; y++){
                            Color c = colors[w + y];
                            diff += System.Math.Abs(c.r - _refree.r);
                        }
                    }

                    percentage = diff / width / height;
                    _checkSig = false;
                }
            }
        }
    }
}