using UnityEngine;

namespace RockPaperPistol.Utils
{   
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if(_instance != null)                
                    return _instance;

                _instance = FindFirstObjectByType<T>();
                Debug.Log(typeof(T).ToString());
                if(_instance == null)
                {
                    GameObject instanceObject = Instantiate(Resources.Load<GameObject>(typeof(T).ToString()));
                    _instance = instanceObject.GetComponent<T>();
                }  

                return _instance;
            }
        }
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}