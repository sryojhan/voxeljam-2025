using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SceneTransition
{


    [ExecuteInEditMode]
    public class ApplyImageEffect : MonoBehaviour
    {
        public Material imageEffect;


        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            print("se llama al metodo este");
            Graphics.Blit(source, destination, imageEffect);
        }

    }

}