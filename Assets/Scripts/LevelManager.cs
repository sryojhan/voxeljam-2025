using System.Collections;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public enum Events
    {
        SpawnBasicEnemy, SpawnAcrobatEnemy, MovePlatforms, DestroyPlatform,


        End
    }

    [System.Serializable]
    public class EventInfo
    {
        public Events type;

        public float value;

        public float time;
    }

    public EventInfo[] levelEvents;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(1);

        foreach (EventInfo ev in levelEvents)
        {
            yield return new WaitForSeconds(ev.time);

            switch (ev.type)
            {
                case Events.SpawnBasicEnemy:

                    print("spawn enemy");

                    break;
                case Events.SpawnAcrobatEnemy:

                    print("spawn acrobat");

                    break;
                case Events.MovePlatforms:

                    print("move platforms");

                    break;
                case Events.DestroyPlatform:

                    print("destroy platforms");
                    break;
                case Events.End:

                    CurtainManager.instance.CloseCurtains("Sandbox");

                    break;
                default:
                    break;
            }
        }
    }
}
