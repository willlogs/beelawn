using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Bees;
using Es.InkPainter;

namespace PT.Garden
{
    public class PositionPainter : MonoBehaviour
    {
        [SerializeField] private Brush _removeBrush, _bendBrush;
        [SerializeField] private InkCanvas _bendCanvas, _removeCanvas;
        [SerializeField] private Bee _bee;

        private void Update(){
            _bendCanvas.Paint(
                _bendBrush,
                new Vector3(
                    transform.position.x,
                    _bendCanvas.transform.position.y,
                    transform.position.z
                )
            );

            _removeCanvas.Paint(
                _removeBrush,
                new Vector3(
                    transform.position.x,
                    _removeCanvas.transform.position.y,
                    transform.position.z
                )
            );
        }
    }
}