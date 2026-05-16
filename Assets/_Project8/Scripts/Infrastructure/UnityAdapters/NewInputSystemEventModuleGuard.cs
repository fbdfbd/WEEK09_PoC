using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Project8.Infrastructure.UnityAdapters
{
    [RequireComponent(typeof(EventSystem))]
    public sealed class NewInputSystemEventModuleGuard : MonoBehaviour
    {
        private void Awake()
        {
            var oldModule = GetComponent<StandaloneInputModule>();

            if (oldModule != null)
            {
                Destroy(oldModule);
            }

            if (GetComponent<InputSystemUIInputModule>() == null)
            {
                gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }
    }
}
