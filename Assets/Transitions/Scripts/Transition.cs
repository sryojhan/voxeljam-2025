using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SceneTransition
{
    [CreateAssetMenu (fileName = "new Scene Transition", menuName = "Scene Transition")]
    public class Transition : ScriptableObject
    {
        [Header("In transition")]
        public          bool inEnabled = true;
        public     Texture2D inTransition;
        public         float inDuration = 1;
        public          bool inInverted = false;
        public Interpolation inInterpolation;


        [Header("Middle screen")]
        public         Color backgroundColor = Color.white;
        public     Texture2D background;
        public         float middleScreenDuration = 1;

        [Header ("Out transition")]
        public          bool outEnabled = true;
        public     Texture2D outTransition;
        public         float outDuration = 1;
        public          bool outInverted = false;
        public Interpolation outInterpolation;
    }

}