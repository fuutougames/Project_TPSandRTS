using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public class Ticker : MonoBase
    {
        public delegate void UPDATE_FUNC(float dt);

        private HashSet<UPDATE_FUNC> updateFuncs = new HashSet<UPDATE_FUNC>(); 
        private HashSet<UPDATE_FUNC> lateUpdateFuncs = new HashSet<UPDATE_FUNC>(); 
        private HashSet<UPDATE_FUNC> fixedUpdateFuncs = new HashSet<UPDATE_FUNC>();

        private HashSet<UPDATE_FUNC> m_updateAddBuffer = new HashSet<UPDATE_FUNC>();  
        private HashSet<UPDATE_FUNC> m_lateupdateAddBuffer = new HashSet<UPDATE_FUNC>(); 
        private HashSet<UPDATE_FUNC> m_fixedupdateAddBuffer = new HashSet<UPDATE_FUNC>(); 

        public void RegisterUpdateFunc(UPDATE_FUNC func)
        {
            if (m_updating)
            {
                m_updateAddBuffer.Add(func);
                return;
            }

            if (updateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into updateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            updateFuncs.Add(func);
        }

        public void RegisterLateUpdateFunc(UPDATE_FUNC func)
        {
            if (m_lateupdating)
            {
                m_lateupdateAddBuffer.Add(func);
                return;
            }

            if (lateUpdateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into lateUpdateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            lateUpdateFuncs.Add(func);
        }

        public void RegisterFixedUpdateFunc(UPDATE_FUNC func)
        {
            if (m_fixedupdating)
            {
                m_fixedupdateAddBuffer.Add(func);
                return;
            }

            if (fixedUpdateFuncs.Contains(func))
            {
#if UNITY_EDITOR
                Debug.LogWarning(string.Format("Trying to add duplicate function instance into fixedUpdateFuncs list in ticker-{0}", gameObject.name));
#endif
                return;
            }

            fixedUpdateFuncs.Add(func);
        }

        public void UnregisterUpdateFunc(UPDATE_FUNC func)
        {
            updateFuncs.Remove(func);
            m_updateAddBuffer.Remove(func);
        }

        public void UnregisterLateUpdateFunc(UPDATE_FUNC func)
        {
            lateUpdateFuncs.Remove(func);
            m_lateupdateAddBuffer.Remove(func);
        }

        public void UnregisterFixedUpdateFunc(UPDATE_FUNC func)
        {
            fixedUpdateFuncs.Remove(func);
            m_fixedupdateAddBuffer.Remove(func);
        }

        private bool m_updating = false;
        protected override void OnUpdate()
        {
            base.OnUpdate();
            m_updating = true;
            HashSet<UPDATE_FUNC>.Enumerator iter = updateFuncs.GetEnumerator();
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
            m_updating = false;

            if (m_updateAddBuffer.Count == 0)
                return;

            iter = m_updateAddBuffer.GetEnumerator();
            while (iter.MoveNext())
            {
                RegisterUpdateFunc(iter.Current);
            }
            m_updateAddBuffer.Clear();
        }


        private bool m_lateupdating = false;
        protected override void OnLateUpdate()
        {
            base.OnLateUpdate();
            if (lateUpdateFuncs.Count > 0)
            {
                m_lateupdating = true;
                HashSet<UPDATE_FUNC>.Enumerator iter = lateUpdateFuncs.GetEnumerator();
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
                m_lateupdating = false;
            }

            if (m_lateupdateAddBuffer.Count > 0)
            {
                HashSet<UPDATE_FUNC>.Enumerator iter = m_lateupdateAddBuffer.GetEnumerator();
                while (iter.MoveNext())
                {
                    RegisterLateUpdateFunc(iter.Current);
                }
                m_lateupdateAddBuffer.Clear();
            }
        }

        private bool m_fixedupdating = false;
        protected override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            if (fixedUpdateFuncs.Count > 0)
            {
                m_fixedupdating = true;
                HashSet<UPDATE_FUNC>.Enumerator iter = fixedUpdateFuncs.GetEnumerator();
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
                m_fixedupdating = false;
            }

            if (m_fixedupdateAddBuffer.Count > 0)
            {
                HashSet<UPDATE_FUNC>.Enumerator iter = m_fixedupdateAddBuffer.GetEnumerator();
                while (iter.MoveNext())
                {
                    RegisterFixedUpdateFunc(iter.Current);
                }
                m_fixedupdateAddBuffer.Clear();
            }
        }
    }
}
