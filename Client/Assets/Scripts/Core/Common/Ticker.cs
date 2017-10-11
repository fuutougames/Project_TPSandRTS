using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public class Ticker : MonoBase
    {
        public delegate void ON_UPDATE(float dt);
        public delegate void ON_LATE_UPDATE(float dt);
        public delegate void ON_FIXED_UPDATE(float dt);

        //public ON_UPDATE onUpdate = null;
        //public ON_LATE_UPDATE onLateUpdate = null;
        //public ON_FIXED_UPDATE onFixedUpdate = null;

        private HashSet<ON_UPDATE> updateFuncs = new HashSet<ON_UPDATE>(); 
        private HashSet<ON_LATE_UPDATE> lateUpdateFuncs = new HashSet<ON_LATE_UPDATE>(); 
        private HashSet<ON_FIXED_UPDATE> fixedUpdateFuncs = new HashSet<ON_FIXED_UPDATE>(); 

        public void RegisterUpdateFunc(ON_UPDATE func)
        {
            if (updateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into updateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            updateFuncs.Add(func);
        }

        public void RegisterLateUpdateFunc(ON_LATE_UPDATE func)
        {
            if (lateUpdateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into lateUpdateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            lateUpdateFuncs.Add(func);
        }

        public void RegisterFixedUpdateFunc(ON_FIXED_UPDATE func)
        {
            if (fixedUpdateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into fixedUpdateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            fixedUpdateFuncs.Add(func);
        }

        public void UnregisterUpdateFunc(ON_UPDATE func)
        {
            updateFuncs.Remove(func);
        }

        public void UnregisterLateUpdateFunc(ON_LATE_UPDATE func)
        {
            lateUpdateFuncs.Remove(func);
        }

        public void UnregisterFixedUpdateFunc(ON_FIXED_UPDATE func)
        {
            fixedUpdateFuncs.Remove(func);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            HashSet<ON_UPDATE>.Enumerator iter = updateFuncs.GetEnumerator();
            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.Invoke(Time.deltaTime);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("{0}\n{1}", e.Message, e.StackTrace));
#else
                    // runtime error handling
#endif
                }
            }
        }


        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            HashSet<ON_LATE_UPDATE>.Enumerator iter = lateUpdateFuncs.GetEnumerator();
            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.Invoke(Time.deltaTime);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("{0}\n{1}", e.Message, e.StackTrace));
#else
                    // runtime error handling
#endif
                }
            }
        }

        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            HashSet<ON_FIXED_UPDATE>.Enumerator iter = fixedUpdateFuncs.GetEnumerator();
            while (iter.MoveNext())
            {
                try
                {
                    iter.Current.Invoke(Time.deltaTime);
                }
                catch (Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError(string.Format("{0}\n{1}", e.Message, e.StackTrace));
#else
                    // runtime error handling
#endif
                }
            }
        }
    }
}
