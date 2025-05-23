using EasyButtons;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneTransition
{

    public class SceneTransitionManager : Singleton<SceneTransitionManager>
    {
        protected override bool DestroyOnLoad => false;

        public Material material;

        public Transition transition;


        private bool inTransition;



        //Illustration sequence patch
        public bool useCustomFunction = false;
        public delegate void CustomFunction();
        public CustomFunction customFunction = () => { };

        private void Start()
        {
            if (DestroyIfInitialised(this)) return;

            EnsureInitialised();
            ResetMaterial();
        }
        private void OnDestroy()
        {
            if (ImTheOne(this))
                ResetMaterial();
        }

        [Button]
        void ResetMaterial()
        {
            material.SetFloat("_time", 0);
            material.SetFloat("_inProgress", 0);
        }


        public bool IsAlreadyTransitioning()
        {
            return inTransition;
        }

        //TODO: mejorar la forma de indexar las escenas
        //public void ChangeScene(SceneIndex sceneIndex)
        //{
        //    ChangeScene((int)sceneIndex);
        //}

        public void ChangeScene(string sceneName)
        {
            if (inTransition)
            {
                throw new UnityException("There is already a transition in progress");
            }

            StartCoroutine(SceneSwap(sceneName));
        }

        public void CancelCurrentTransition()
        {
            if (!inTransition)
            {
                throw new UnityException("There is no active transition");
            }

            StopAllCoroutines();
            inTransition = false;
        }

        private IEnumerator SceneSwap(string sceneName)
        {
            inTransition = true;

            material.SetTexture("_Background", transition.background);
            material.SetColor("_BackgroundColor", transition.backgroundColor);
            material.SetTexture("_TransitionGradient", transition.inTransition);

            //Fade in
            material.SetFloat("_time", 0);
            material.SetFloat("_inverted", transition.inInverted ? 1 : 0);

            material.SetFloat("_inProgress", 1);

            if (transition.inEnabled)
                for (float i = 0; i < transition.inDuration; i += Time.deltaTime)
                {

                    material.SetFloat("_time", transition.inInterpolation.Interpolate(i / transition.inDuration));
                    yield return null;
                }

            material.SetFloat("_time", 1);
            material.SetFloat("_inverted", transition.outInverted ? 1 : 0);

            material.SetTexture("_TransitionGradient", transition.outTransition);
            //Load Scene
            if(!useCustomFunction)
            {
                SceneManager.LoadScene(sceneName);
            }
            else {

                customFunction();
            }


            if (transition.middleScreenDuration > 0)
                yield return new WaitForSeconds(transition.middleScreenDuration);

            //Wait screen

            //Fade out
            if (transition.outEnabled)
                for (float i = transition.outDuration; i > 0; i -= Time.deltaTime)
                {
                    material.SetFloat("_time", transition.outInterpolation.Interpolate(i / transition.outDuration));
                    yield return null;
                }

            material.SetFloat("_time", 0);

            material.SetFloat("_inProgress", 0);


            inTransition = false;

            print("transition end");
        }


        public bool IsTransitionComplete()
        {
            return !inTransition;
        }

        [Button]
        public void ChangeSceneToMenu()
        {
            ChangeScene("MenuScene");
        }

        [Button]
        public void ChangeSceneToDialogue()
        {
            ChangeScene("DialogueScene");
        }


        [Button]
        public void ChangeSceneToSample()
        {
            ChangeScene("SampleScene");
        }

    }

}