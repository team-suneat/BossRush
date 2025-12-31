//#define HIDE_TRAIL_IN_HIERRARCHY

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace JF_SpriteTrail
{
    public enum TrailActivationCondition
    {
        AlwaysEnabled,
        Manual,
        VelocityMagnitude

        //    Acceleration
    }

    public enum TrailDeactivationCondition
    {
        Manual,
        VelocityMagnitude,
        Time
    }

    public sealed class SpriteTrail : MonoBehaviour
    {
        [Tooltip("트레일 이름")]
        public string m_TrailName = "";

        [Tooltip("현재 사용되는 트레일 설정입니다.")]
        public TrailPreset m_CurrentTrailPreset;

        [Tooltip("트레일 요소들의 부모입니다. 필드가 비어있으면 월드 공간에 트레일을 생성합니다. 트레일 생성자가 이동 요소에 있어서 트레일을 로컬 공간에 생성하려는 경우 유용할 수 있습니다. \n SetTrailParent(Transform trailParent)로 설정할 수 있습니다")]
        [SerializeField]
        private Transform m_TrailParent;

        [Tooltip("트레일의 기준으로 사용될 스프라이트 렌더러를 지정합니다. 스크립트가 스프라이트 렌더러와 같은 게임오브젝트에 있다면 비워둘 수 있습니다")]
        public SpriteRenderer m_SpriteToDuplicate;

        [Layer]
        [Tooltip("트레일 요소들의 레이어 이름을 지정하는 데 사용합니다")]
        public int m_Layer = 0;

        [Range(0, 1)]
        [Tooltip("스프라이트의 z-위치를 수정하여 겹침을 방지합니다")]
        public float m_ZMoveStep = 0.0001f;

        [Range(0, 10)]
        [Tooltip("스프라이트 위치에 허용되는 최대 z-위치입니다")]
        public float m_ZMoveMax = 0.9999f;

        [Tooltip("트레일이 비활성화될 때 숨기려면 체크하세요")]
        public bool m_HideTrailOnDisabled = true;

        [Tooltip("트레일을 시작하는 요구사항입니다")]
        public TrailActivationCondition m_TrailActivationCondition = TrailActivationCondition.AlwaysEnabled;

        [Tooltip("트레일을 중지하는 요구사항입니다")]
        [FormerlySerializedAs("m_TrailDisactivationCondition")]
        public TrailDeactivationCondition m_TrailDeactivationCondition = TrailDeactivationCondition.Manual;

        [Tooltip("속도가 한계 아래일 때만 트레일을 시작하려면 체크하세요")]
        public bool m_StartIfUnderVelocity = false;

        [Tooltip("속도가 로컬 공간에 있는지 체크하세요")]
        public bool m_VelocityStartIsLocalSpace = false;

        [Tooltip("트레일을 시작하는 데 필요한 최소 속도입니다")]
        public float m_VelocityNeededToStart;

        [Tooltip("속도가 한계를 넘을 때만 트레일을 중지하려면 체크하세요")]
        public bool m_StopIfOverVelocity = false;

        [Tooltip("속도가 로컬 공간에 있는지 체크하세요")]
        public bool m_VelocityStopIsLocalSpace = false;

        [Tooltip("트레일을 중지하는 데 필요한 최소 속도입니다")]
        public float m_VelocityNeededToStop;

        [Tooltip("트레일 활성화 지속 시간(초)입니다. 이 지연 후 트레일이 자동으로 비활성화됩니다")]
        public float m_TrailActivationDuration;

        [SortingLayer]
        public int m_SortingLayerID;
        public int m_OrderInSortingLayer;

        public Queue<TrailElement> m_ElementsInTrail = new();
        public List<TrailElement> m_DisabledElementsInTrail = new();

        private TrailPreset m_PreviousTrailPreset;
        private static GameObject m_TrailElementTemplate;

        public static GameObject TrailElementTemplate
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    return null;
                }
#endif

                if (m_TrailElementTemplate == null)
                {
                    m_TrailElementTemplate = new GameObject(
                            "TrailElementTemplate",
                            typeof(SpriteRenderer),
                            typeof(TrailElement));
                    DontDestroyOnLoad(m_TrailElementTemplate);
                }
                return m_TrailElementTemplate;
            }

            set => m_TrailElementTemplate = value;
        }

        private Transform m_LocalTrailContainer;
        private static Transform m_GlobalTrailContainer;

        public static Transform GlobalTrailContainer
        {
            get
            {
#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    return null;
                }
#endif
                if (m_GlobalTrailContainer == null)
                {
                    GameObject container = GameObject.Find("GlobalTrailContainer");

                    if (container == null)
                    {
                        m_GlobalTrailContainer = new GameObject("GlobalTrailContainer").transform;
#if HIDE_TRAIL_IN_HIERRARCHY
                m_GlobalTrailContainer.hideFlags = HideFlags.HideInHierarchy;
#endif
                    }
                    else
                    {
                        m_GlobalTrailContainer = container.transform;
                    }

                    DontDestroyOnLoad(GlobalTrailContainer);
                }

                return m_GlobalTrailContainer;
            }
            set => m_GlobalTrailContainer = value;
        }

        private float m_CurrentDisplacement;
        private float m_TimeTrailStarted;
        private float m_PreviousTimeSpawned;
        private int m_PreviousFrameSpawned;
        private Vector2 m_PreviousPosSpawned;
        private Vector2 m_PreviousFrameLocalPos;
        private Vector2 m_PreviousFrameWorldPos;
        private Vector2 m_VelocityLocal;
        private Vector2 m_VelocityWorld;
        private bool m_FirstVelocityCheck = false;
        private bool m_CanBeAutomaticallyActivated = true;
        private bool m_EffectEnabled;
        private bool m_WillBeActivatedOnEnable = false;

        #region PUBLIC

        /// <summary>
        /// Set the trail parent if your item is in another moving item, and you want the trail to be in local space
        /// -- Example : your character enters a moving train : you probably want the trail to be in the train local space, so SetTrailParent(TheTrailTransform).
        /// </summary>
        /// /// <param name="trailParent">The parent transform. Set it to null if you want it in world space</param>
        public void SetTrailParent(Transform trailParent)
        {
            if (trailParent == null)
            {
                return;
            }

            m_TrailParent = trailParent;
            Transform localTrailContainer = trailParent.Find("LocalTrailContainer");
            if (localTrailContainer == null)
            {
                m_LocalTrailContainer = new GameObject("LocalTrailContainer").transform;
#if HIDE_TRAIL_IN_HIERRARCHY
            m_LocalTrailContainer.hideFlags = HideFlags.HideInHierarchy;
#endif
                m_LocalTrailContainer.transform.SetParent(trailParent, false);
            }
            else
            {
                m_LocalTrailContainer = localTrailContainer;
            }
        }

        /// <summary>
        /// Modify the trail effect
        /// </summary>
        /// /// <param name="preset">The trail preset you want to set</param>
        public void SetTrailPreset(TrailPreset preset)
        {
            m_PreviousTrailPreset = m_CurrentTrailPreset;
            m_CurrentTrailPreset = preset;
        }

        /// <summary>
        /// Trail start
        /// </summary>
        public void EnableTrail()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            m_CanBeAutomaticallyActivated = true;
            m_PreviousPosSpawned = m_SpriteToDuplicate.transform.position;
            switch (m_TrailActivationCondition)
            {
                case TrailActivationCondition.Manual:
                    EnableTrailEffect();
                    break;

                case TrailActivationCondition.AlwaysEnabled:
                    break;

                case TrailActivationCondition.VelocityMagnitude:
                    break;
            }
        }

        /// <summary>
        /// Trail stop
        /// </summary>
        public void DisableTrail()
        {
            m_WillBeActivatedOnEnable = false;
            m_CanBeAutomaticallyActivated = false;
            DisableTrailEffect();
        }

        /// <summary>
        /// NOT RECOMMANDED : use EnableTrail() instead. Enable the trail effect
        /// </summary>
        /// <param name="forceTrailCreation">Set it to true if you want to force the creation of the trail immediately and spawn the first element</param>
        public void EnableTrailEffect(bool forceTrailCreation = true)
        {
            if (m_SpriteToDuplicate == null)
            {
                Debug.LogAssertion("Warning : trying to EnableTrailEffect while not having a sprite to duplicate set");
                return;
            }

            if (m_CurrentTrailPreset == null)
            {
                Debug.LogAssertion("Warning : trying to EnableTrailEffect while not having a trail preset set");
                return;
            }

            m_TimeTrailStarted = Time.time;
            m_EffectEnabled = true;
            m_CurrentDisplacement = m_ZMoveMax;
            m_PreviousPosSpawned = m_SpriteToDuplicate.transform.position;
            m_PreviousTimeSpawned = Time.time;
            m_PreviousFrameSpawned = Time.frameCount;
            if (forceTrailCreation)
            {
                GenerateNewTrailElement();
            }
        }

        /// <summary>
        /// NOT RECOMMANDED : use DisableTrail() instead. Disable the trail effect
        /// </summary>
        public void DisableTrailEffect()
        {
            m_EffectEnabled = false;
            if (m_HideTrailOnDisabled)
            {
                HideTrail(true);
            }
        }

        /// <summary>
        /// Hide the current trail, delete all trails elements in this trail.
        /// </summary>
        public void HideTrail(bool addToFree)
        {
            m_DisabledElementsInTrail.Clear();
            while (m_ElementsInTrail.Count > 0)
            {
                TrailElement e = m_ElementsInTrail.Dequeue();
                if (e != null)
                {
                    e.Hide(addToFree);
                    if (!addToFree)
                    {
                        m_DisabledElementsInTrail.Add(e);
                    }
                }
            }
        }

        /// <summary>
        /// Return true if the trail effect is active
        /// </summary>
        public bool IsEffectEnabled()
        {
            return m_EffectEnabled;
        }

        #endregion PUBLIC

        #region PRIVATE

        static SpriteTrail()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            TrailElement.RefreshFreeElements();
        }

        private void Awake()
        {
            if (m_SpriteToDuplicate == null)
            {
                Debug.LogError($"this sprite trail not contains sprite to duplicate. this: {name}");
            }

            SetTrailParent(m_TrailParent);
            m_CurrentDisplacement = m_ZMoveMax;

            if (m_SpriteToDuplicate == null)
            {
                m_SpriteToDuplicate = GetComponent<SpriteRenderer>();
            }
            if (m_SpriteToDuplicate == null)
            {
                Debug.LogError(
                        "You need a SpriteRenderer on the same GameObject as the SpriteTrail script. Else, you can set the SpriteToDuplicate variable from the inspector");
                return;
            }

            m_PreviousPosSpawned = m_SpriteToDuplicate.transform.position;

            /*if (m_EnabledByDefault)
            EnableTrailEffect();*/
        }

        private void CalculateCurrentVelocity()
        {
            if (m_FirstVelocityCheck)
            {
                m_VelocityLocal = ((Vector2)m_SpriteToDuplicate.transform.localPosition - m_PreviousFrameLocalPos) / Time.deltaTime;
                m_VelocityWorld = ((Vector2)m_SpriteToDuplicate.transform.position - m_PreviousFrameWorldPos) / Time.deltaTime;
            }
            m_FirstVelocityCheck = true;
            m_PreviousFrameLocalPos = m_SpriteToDuplicate.transform.localPosition;
            m_PreviousFrameWorldPos = m_SpriteToDuplicate.transform.position;
        }

        private void Update()
        {
            if (m_PreviousTrailPreset != m_CurrentTrailPreset)
            {
                if (m_HideTrailOnDisabled)
                {
                    HideTrail(false);
                }
                m_PreviousTrailPreset = m_CurrentTrailPreset;
            }
            if (m_TrailActivationCondition == TrailActivationCondition.VelocityMagnitude ||
                m_TrailDeactivationCondition == TrailDeactivationCondition.VelocityMagnitude)
            {
                CalculateCurrentVelocity();
            }

            if (m_EffectEnabled)
            {
                if (m_CurrentTrailPreset == null)
                {
                    return;
                }

                if (m_SpriteToDuplicate == null)
                {
                    return;
                }

                switch (m_TrailDeactivationCondition)
                {
                    case TrailDeactivationCondition.Manual:
                        break;

                    case TrailDeactivationCondition.Time:
                        if (m_TimeTrailStarted + m_TrailActivationDuration <= Time.time)
                        {
                            DisableTrailEffect();
                            return;
                        }

                        break;

                    case TrailDeactivationCondition.VelocityMagnitude:
                        float _Velocity;
                        if (m_VelocityStopIsLocalSpace)
                        {
                            _Velocity = m_VelocityLocal.magnitude;
                        }
                        else
                        {
                            _Velocity = m_VelocityWorld.magnitude;
                        }

                        if (!m_StopIfOverVelocity && _Velocity >= m_VelocityNeededToStop)
                        {
                            DisableTrailEffect();
                        }
                        else if (m_StopIfOverVelocity && _Velocity <= m_VelocityNeededToStop)
                        {
                            DisableTrailEffect();
                        }
                        break;
                }

                switch (m_CurrentTrailPreset.m_TrailElementSpawnCondition)
                {
                    case TrailElementSpawnCondition.Time:
                        if (m_PreviousTimeSpawned + m_CurrentTrailPreset.m_TimeBetweenSpawns <= Time.time)
                        {
                            float _TimeError = Time.time - (m_PreviousTimeSpawned + m_CurrentTrailPreset.m_TimeBetweenSpawns);
                            if (m_CurrentTrailPreset.m_TimeBetweenSpawns > 0)
                            {
                                while (_TimeError >= m_CurrentTrailPreset.m_TimeBetweenSpawns)
                                {
                                    _TimeError -= m_CurrentTrailPreset.m_TimeBetweenSpawns;
                                }
                            }

                            if (_TimeError > m_CurrentTrailPreset.m_TimeBetweenSpawns)
                            {
                                _TimeError = 0;
                            }

                            m_PreviousTimeSpawned = Time.time - _TimeError;

                            GenerateNewTrailElement();
                        }

                        break;

                    case TrailElementSpawnCondition.FrameCount:
                        if (m_PreviousFrameSpawned + m_CurrentTrailPreset.m_FramesBetweenSpawns <= Time.frameCount)
                        {
                            m_PreviousFrameSpawned = Time.frameCount;
                            GenerateNewTrailElement();
                        }
                        break;

                    case TrailElementSpawnCondition.Distance:
                        if (m_CurrentTrailPreset.m_DistanceCorrection && m_CurrentTrailPreset.m_DistanceBetweenSpawns > 0)
                        {
                            //TODO : if error corrected : generate a timer error ( 3 elements generated at the same frame == 3 elements that update the same)
                            while (Vector2.Distance(m_PreviousPosSpawned, m_SpriteToDuplicate.transform.position) >=
                                   m_CurrentTrailPreset.m_DistanceBetweenSpawns)
                            {
                                Vector2 _dir = (Vector2)m_SpriteToDuplicate.transform.position - m_PreviousPosSpawned;
                                Vector2 _tmpPos = m_PreviousPosSpawned + (_dir.normalized * m_CurrentTrailPreset.m_DistanceBetweenSpawns);
                                GenerateNewTrailElement(new Vector3(_tmpPos.x, _tmpPos.y, m_SpriteToDuplicate.transform.position.z));
                                m_PreviousPosSpawned = _tmpPos;
                            }
                        }
                        else if (Vector2.Distance(m_PreviousPosSpawned, m_SpriteToDuplicate.transform.position) >=
                                   m_CurrentTrailPreset.m_DistanceBetweenSpawns)
                        {
                            GenerateNewTrailElement();
                            m_PreviousPosSpawned = m_SpriteToDuplicate.transform.position;
                        }

                        break;
                }
            }
            else if (m_CanBeAutomaticallyActivated)//check activation condition
            {
                switch (m_TrailActivationCondition)
                {
                    case TrailActivationCondition.AlwaysEnabled:
                        EnableTrailEffect(false);
                        break;

                    case TrailActivationCondition.Manual:
                        break;

                    case TrailActivationCondition.VelocityMagnitude:
                        float _Velocity;
                        if (m_VelocityStopIsLocalSpace)
                        {
                            _Velocity = m_VelocityLocal.magnitude;
                        }
                        else
                        {
                            _Velocity = m_VelocityWorld.magnitude;
                        }

                        if (!m_StartIfUnderVelocity && _Velocity >= m_VelocityNeededToStart)
                        {
                            EnableTrailEffect();
                        }
                        else if (m_StartIfUnderVelocity && _Velocity <= m_VelocityNeededToStart)
                        {
                            EnableTrailEffect();
                        }
                        break;
                }
            }
        }

        private void GenerateNewTrailElement(Vector3 pos)
        {
            m_CurrentDisplacement -= m_ZMoveStep;
            if (m_CurrentDisplacement <= m_ZMoveStep)
            {
                m_CurrentDisplacement = m_ZMoveMax;
            }

            TrailElement _TmpElement = TrailElement.GetFreeElement();
            //GameObject _tmpGo = GameObject.Instantiate(m_TrailElementPrefab);
            //_tmpGo.name = "TRAIL_" + m_SpriteToDuplicate.name;
            //_tmpGo.hideFlags = HideFlags.None;
            //_tmpGo.transform.Equals(m_SpriteToDuplicate.transform);
            _TmpElement.m_Transform.SetParent(m_SpriteToDuplicate.transform, true);
            _TmpElement.m_Transform.localScale = new Vector3(1, 1, 1);
            _TmpElement.m_Transform.localRotation = Quaternion.identity;
            _TmpElement.m_Transform.localPosition = Vector3.zero;
            if (m_TrailParent == null)
            {
                _TmpElement.m_Transform.SetParent(m_GlobalTrailContainer, true);
            }
            else
            {
                _TmpElement.m_Transform.SetParent(m_LocalTrailContainer, true);
            }
            Vector3 _NewPos = pos;
            _NewPos.z += m_CurrentDisplacement;
            _TmpElement.m_Transform.position = _NewPos;
            _TmpElement.Initialise(this);
            _TmpElement.m_myGameObject.layer = m_Layer;
            _TmpElement.SpriteRenderer.sortingLayerID = m_SortingLayerID;
            _TmpElement.SpriteRenderer.sortingOrder = m_OrderInSortingLayer;
            /*m_CurrentDisplacement -= m_ZMoveStep;
            if (m_CurrentDisplacement <= m_ZMoveStep)
                m_CurrentDisplacement = m_ZMoveMax;
            TrailElement trail = TrailElement.GetFreeElement();
            trail.m_Transform.localScale = new Vector3(1, 1, 1);
            trail.m_Transform.localRotation = Quaternion.identity;
            trail.m_Transform.localPosition = Vector3.zero;
            if (m_TrailParent != null) {
                trail.m_Transform.SetParent(m_LocalTrailContainer, true);
            }

            Vector3 newPos = pos;
            newPos.z += m_CurrentDisplacement;
            trail.m_Transform.position = newPos;
            trail.Initialise(this);
            trail.gameObject.layer = m_Layer;
            trail.SpriteRenderer.sortingLayerID = m_SortingLayerID;
            trail.SpriteRenderer.sortingOrder = m_OrderInSortingLayer;*/
        }

        private void GenerateNewTrailElement()
        {
            m_CurrentDisplacement -= m_ZMoveStep;
            if (m_CurrentDisplacement <= m_ZMoveStep)
            {
                m_CurrentDisplacement = m_ZMoveMax;
            }

            TrailElement _TmpElement = TrailElement.GetFreeElement();

            //GameObject _tmpGo = GameObject.Instantiate(m_TrailElementPrefab);
            //_tmpGo.name = "TRAIL_" + m_SpriteToDuplicate.name;
            //_tmpGo.hideFlags = HideFlags.None;
            //_tmpGo.transform.Equals(m_SpriteToDuplicate.transform);
            _TmpElement.m_Transform.SetParent(m_SpriteToDuplicate.transform, true);
            _TmpElement.m_Transform.localScale = new Vector3(1, 1, 1);
            _TmpElement.m_Transform.localRotation = Quaternion.identity;
            _TmpElement.m_Transform.localPosition = Vector3.zero;
            if (m_TrailParent == null)
            {
                _TmpElement.m_Transform.SetParent(GlobalTrailContainer, true);
            }
            else
            {
                _TmpElement.m_Transform.SetParent(m_LocalTrailContainer, true);
            }
            Vector3 _NewPos = _TmpElement.transform.position;
            _NewPos.z = m_CurrentDisplacement;
            _TmpElement.m_Transform.position = _NewPos;
            _TmpElement.Initialise(this);
            _TmpElement.m_myGameObject.layer = m_Layer;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
#endif
            bool _effectWasEnabled = m_EffectEnabled;

            DisableTrailEffect();
            switch (m_TrailActivationCondition)
            {
                case TrailActivationCondition.Manual:
                    m_WillBeActivatedOnEnable = _effectWasEnabled;
                    break;
            }
        }

        private void OnEnable()
        {
            switch (m_TrailActivationCondition)
            {
                case TrailActivationCondition.Manual:
                    if (m_WillBeActivatedOnEnable)
                    {
                        EnableTrail();
                    }

                    break;

                default:
                    if (m_CanBeAutomaticallyActivated)
                    {
                        EnableTrail();
                    }

                    break;
            }
        }

        #endregion PRIVATE
    }
}