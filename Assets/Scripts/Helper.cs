using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Helpers
{
    public static class Helper
    {
		

	   
        public static void RaiseAction(this Action action)
        {
            var handler = action;
            if (handler != null)
                handler();
        }

        public static void RaiseAction<T>(this Action<T> action, T param)
        {
            var handler = action;
            if (handler != null)
                handler(param);
        }

        public static void RaiseAction<T1, T2>(this Action<T1, T2> action, T1 param1, T2 param2)
        {
            var handler = action;
            if (handler != null)
                handler(param1, param2);
        }

        public static void RaiseAction<T1, T2, T3>(this Action<T1, T2, T3> action, T1 param1, T2 param2, T3 param3)
        {
            var handler = action;
            if (handler != null)
                handler(param1, param2, param3);
        }

        public static void RaiseAction<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action, T1 param1, T2 param2, T3 param3, T4 param4)
        {
            var handler = action;
            if (handler != null)
                handler(param1, param2, param3, param4);
        }

        public static void StopTaskIfRunning(this Task task)
        {
            if (task != null && task.Running) task.Stop();
        }

        public static GameObject CreateCopyOfUiPrefab(GameObject prefab, Transform parent)
        {
            var go = MonoBehaviour.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.name = prefab.name;
            go.transform.SetParent(parent, false);
            return go;
        }

        public static void SetActive(this CanvasGroup group, bool isActive)
        {
            group.alpha = isActive ? 1 : 0;
            group.interactable = group.blocksRaycasts = isActive;
        }

        public static AnimationCurve GetFadeInCurve()
        {
            var keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 0.0f);
            keys[1] = new Keyframe(1.0f, 1.0f);
            return new AnimationCurve(keys);
        }

        public static AnimationCurve GetFadeOutCurve()
        {
            var keys = new Keyframe[2];
            keys[0] = new Keyframe(0.0f, 1.0f);
            keys[1] = new Keyframe(1.0f, 0.0f);
            return new AnimationCurve(keys);
        }



        public static IPAddress GetIpAddress()
        {

            var ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var address in ipHostInfo.AddressList)
            {
	            if (IsAddressValid(address))
	            {
		            return address;
	            }
            }

            Debug.LogError("No ip!");
            return null;
        }

        private static bool IsAddressValid(IPAddress ipAddress)
        {
	        if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
	        {
		        return false;
	        }
	        return true;
#if !CAR_DEBUG
            return ipAddress.ToString().StartsWith("192") || ipAddress.ToString().StartsWith("172");
#else
            return true;
#endif
        }

        public static int GetDay()
        {
            //return 21;
            if (DateTime.Today.Month != 3) return -1;
            return DateTime.Today.Day;
        }
    }
}