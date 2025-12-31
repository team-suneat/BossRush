using System;
using System.Collections;
using System.Collections.Generic;
using TeamSuneat;

namespace TeamSuneat
{
    public delegate void GlobalCallback();

    public delegate void GlobalCallback<T>(T arg1);

    public delegate void GlobalCallback<T, U>(T arg1, U arg2);

    public delegate void GlobalCallback<T, U, V>(T arg1, U arg2, V arg3);

    public delegate void GlobalCallback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);

    internal static class GlobalEventInternal
    {
        public static Hashtable Callbacks = new();

        public static UnregisterException ShowUnregisterException(GlobalEventType type)
        {
            return new UnregisterException(string.Format("GlobalEvent에 {0}가 등록되어 있지 않습니다.", type.ToString()));
        }

        public static SendException ShowSendException(GlobalEventType type)
        {
            return new SendException(string.Format("GlobalEvent에 {0}가 등록되어 있지 않습니다.", type.ToString()));
        }

        public class UnregisterException : Exception
        {
            public UnregisterException(string msg) : base(msg)
            {
            }
        }

        public class SendException : Exception
        {
            public SendException(string msg) : base(msg)
            {
            }
        }
    }

    /// <summary>
    /// no arguments
    /// </summary>
    public static class GlobalEvent
    {
        public static void Register(GlobalEventType type, GlobalCallback callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                List<GlobalCallback> callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback>;
                if (null == callbacks)
                {
                    callbacks = new List<GlobalCallback>();
                    GlobalEventInternal.Callbacks.Add(type, callbacks);
                }

                Log.Info(LogTags.Global, "콜백({3}) 등록: {0}.{1}, 콜백 수:{2}", callback.Method.DeclaringType, callback.Method.Name, callbacks.Count + 1, type);
                callbacks.Add(callback);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Unregister(GlobalEventType type, GlobalCallback callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                List<GlobalCallback> callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback>;
                if (null != callbacks)
                {
                    if (callbacks.Contains(callback))
                    {
                        Log.Info(LogTags.Global, "콜백({3}) 등록 해제: {0}.{1}, 콜백 수:{2}", callback.Method.DeclaringType, callback.Method.Name, callbacks.Count - 1, type);
                        callbacks.Remove(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Send(GlobalEventType type)
        {
            // 등록된 콜백이 없으면 바로 반환
            if (!GlobalEventInternal.Callbacks.ContainsKey(type))
            {
                return;
            }

            List<GlobalCallback> callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback>;
            if (callbacks == null || callbacks.Count == 0)
            {
                return;
            }

            // 현재 등록된 콜백 리스트의 복사본을 생성하여 순회
            var callbacksCopy = new List<GlobalCallback>(callbacks);
            for (int i = 0; i < callbacksCopy.Count; i++)
            {
                Log.Info(LogTags.Global, "콜백({3}) 호출: {0}.{1}, 콜백 인덱스:{2}",
                                              callbacksCopy[i].Method.DeclaringType,
                                              callbacksCopy[i].Method.Name,
                                              i,
                                              type);
                callbacksCopy[i]();
            }
        }
    }

    /// <summary>
    /// 1 argument
    /// </summary>
    public static class GlobalEvent<T>
    {
        public static void Register(GlobalEventType type, GlobalCallback<T> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T>>;
                if (null == callbacks)
                {
                    callbacks = new List<GlobalCallback<T>>();
                    GlobalEventInternal.Callbacks.Add(type, callbacks);
                }

                callbacks.Add(callback);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Unregister(GlobalEventType type, GlobalCallback<T> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T>>;
                if (null != callbacks)
                {
                    if (callbacks.Contains(callback))
                    {
                        callbacks.Remove(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Send(GlobalEventType type, T arg1)
        {
            // 등록된 콜백이 없으면 바로 반환
            if (!GlobalEventInternal.Callbacks.ContainsKey(type))
            {
                return false;
            }
            if (!(GlobalEventInternal.Callbacks[type] is List<GlobalCallback<T>>))
            {
                Log.Warning(LogTags.Global, "Global Event (arg1)의 매개변수가 잘못 등록되었습니다. {0}", type.ToString());
                return false;
            }

            var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T>>;
            if (callbacks == null || callbacks.Count == 0)
            {
                return false;
            }

            // 현재 등록된 콜백 리스트의 복사본을 생성하여 순회
            var callbacksCopy = new List<GlobalCallback<T>>(callbacks);
            for (int i = 0; i < callbacksCopy.Count; i++)
            {
                Log.Info(LogTags.Global, "콜백({3}) 호출: {0}.{1}, 콜백 인덱스:{2}",
                                              callbacksCopy[i].Method.DeclaringType,
                                              callbacksCopy[i].Method.Name,
                                              i,
                                              type);
                callbacksCopy[i](arg1);
            }

            return true;
        }
    }

    /// <summary>
    /// 2 arguments
    /// </summary>
    public static class GlobalEvent<T, U>
    {
        public static void Register(GlobalEventType type, GlobalCallback<T, U> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U>>)GlobalEventInternal.Callbacks[type];
                if (null == callbacks)
                {
                    callbacks = new List<GlobalCallback<T, U>>();
                    GlobalEventInternal.Callbacks.Add(type, callbacks);
                }

                if (false == callbacks.Contains(callback))
                {
                    callbacks.Add(callback);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Unregister(GlobalEventType type, GlobalCallback<T, U> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U>>)GlobalEventInternal.Callbacks[type];
                if (null != callbacks)
                {
                    if (callbacks.Contains(callback))
                    {
                        callbacks.Remove(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Send(GlobalEventType type, T arg1, U arg2)
        {
            // 등록된 콜백이 없으면 바로 반환
            if (!GlobalEventInternal.Callbacks.ContainsKey(type))
            {
                return false;
            }
            if (!(GlobalEventInternal.Callbacks[type] is List<GlobalCallback<T, U>>))
            {
                Log.Warning(LogTags.Global, "Global Event (arg1)의 매개변수가 잘못 등록되었습니다. {0}", type.ToString());
                return false;
            }

            var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T, U>>;
            if (callbacks == null || callbacks.Count == 0)
            {
                return false;
            }

            // 현재 등록된 콜백 리스트의 복사본을 생성하여 순회
            var callbacksCopy = new List<GlobalCallback<T, U>>(callbacks);
            for (int i = 0; i < callbacksCopy.Count; i++)
            {
                Log.Info(LogTags.Global, "콜백({3}) 호출: {0}.{1}, 콜백 인덱스:{2}",
                                              callbacksCopy[i].Method.DeclaringType,
                                              callbacksCopy[i].Method.Name,
                                              i,
                                              type);
                callbacksCopy[i](arg1, arg2);
            }

            return true;
        }
    }

    /// <summary>
    /// 3 arguments
    /// </summary>
    public static class GlobalEvent<T, U, V>
    {
        public static void Register(GlobalEventType type, GlobalCallback<T, U, V> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U, V>>)GlobalEventInternal.Callbacks[type];
                if (null == callbacks)
                {
                    callbacks = new List<GlobalCallback<T, U, V>>();

                    GlobalEventInternal.Callbacks.Add(type, callbacks);
                }

                callbacks.Add(callback);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Unregister(GlobalEventType type, GlobalCallback<T, U, V> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U, V>>)GlobalEventInternal.Callbacks[type];
                if (null != callbacks)
                {
                    if (callbacks.Contains(callback))
                    {
                        callbacks.Remove(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Send(GlobalEventType type, T arg1, U arg2, V arg3)
        {
            // 등록된 콜백이 없으면 바로 반환
            if (!GlobalEventInternal.Callbacks.ContainsKey(type))
            {
                return false;
            }
            if (!(GlobalEventInternal.Callbacks[type] is List<GlobalCallback<T, U, V>>))
            {
                Log.Warning(LogTags.Global, "Global Event (arg3)의 매개변수가 잘못 등록되었습니다. {0}", type.ToString());
                return false;
            }

            var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T, U, V>>;
            if (callbacks == null || callbacks.Count == 0)
            {
                return false;
            }

            // 현재 등록된 콜백 리스트의 복사본을 생성하여 순회
            var callbacksCopy = new List<GlobalCallback<T, U, V>>(callbacks);
            for (int i = 0; i < callbacksCopy.Count; i++)
            {
                Log.Info(LogTags.Global, "콜백({3}) 호출: {0}.{1}, 콜백 인덱스:{2}",
                                              callbacksCopy[i].Method.DeclaringType,
                                              callbacksCopy[i].Method.Name,
                                              i,
                                              type);
                callbacksCopy[i](arg1, arg2, arg3);
            }

            return true;
        }
    }

    /// <summary>
    /// 4 arguments
    /// </summary>
    public static class GlobalEvent<T, U, V, W>
    {
        public static void Register(GlobalEventType type, GlobalCallback<T, U, V, W> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U, V, W>>)GlobalEventInternal.Callbacks[type];
                if (null == callbacks)
                {
                    callbacks = new List<GlobalCallback<T, U, V, W>>();

                    GlobalEventInternal.Callbacks.Add(type, callbacks);
                }

                callbacks.Add(callback);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void Unregister(GlobalEventType type, GlobalCallback<T, U, V, W> callback)
        {
            if (null == callback)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                var callbacks = (List<GlobalCallback<T, U, V, W>>)GlobalEventInternal.Callbacks[type];
                if (null != callbacks)
                {
                    if (callbacks.Contains(callback))
                    {
                        callbacks.Remove(callback);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static bool Send(GlobalEventType type, T arg1, U arg2, V arg3, W arg4)
        {
            // 등록된 콜백이 없으면 바로 반환
            if (!GlobalEventInternal.Callbacks.ContainsKey(type))
            {
                return false;
            }
            if (!(GlobalEventInternal.Callbacks[type] is List<GlobalCallback<T, U, V, W>>))
            {
                Log.Warning(LogTags.Global, "Global Event (arg4)의 매개변수가 잘못 등록되었습니다. {0}", type.ToString());
                return false;
            }

            var callbacks = GlobalEventInternal.Callbacks[type] as List<GlobalCallback<T, U, V, W>>;
            if (callbacks == null || callbacks.Count == 0)
            {
                return false;
            }

            // 현재 등록된 콜백 리스트의 복사본을 생성하여 순회
            var callbacksCopy = new List<GlobalCallback<T, U, V, W>>(callbacks);
            for (int i = 0; i < callbacksCopy.Count; i++)
            {
                Log.Info(LogTags.Global, "콜백({3}) 호출: {0}.{1}, 콜백 인덱스:{2}",
                                              callbacksCopy[i].Method.DeclaringType,
                                              callbacksCopy[i].Method.Name,
                                              i,
                                              type);
                callbacksCopy[i](arg1, arg2, arg3, arg4);
            }

            return true;
        }
    }
}