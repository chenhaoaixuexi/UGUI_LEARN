using System.Collections.Generic;

namespace UnityEngine.EventSystems
{
    internal static class RaycasterManager
    {
        private static readonly List<BaseRaycaster> s_Raycasters = new List<BaseRaycaster>();

        //! UnityEngine.EventSystems.BaseRaycaster.OnEnable 调用
        public static void AddRaycaster(BaseRaycaster baseRaycaster)
        {
            if (s_Raycasters.Contains(baseRaycaster))
                return;

            s_Raycasters.Add(baseRaycaster);
        }

        //! 被 UnityEngine.EventSystems.EventSystem.RaycastAll 调用
        public static List<BaseRaycaster> GetRaycasters()
        {
            return s_Raycasters;
        }

        //! UnityEngine.EventSystems.BaseRaycaster.OnDisable 调用
        public static void RemoveRaycasters(BaseRaycaster baseRaycaster)
        {
            if (!s_Raycasters.Contains(baseRaycaster))
                return;
            s_Raycasters.Remove(baseRaycaster);
        }
    }
}
