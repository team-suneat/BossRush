using UnityEngine;

namespace TeamSuneat
{
    public abstract class ICharacterDetector
    {
        public float MaxDistance { get; set; }

        public SID OwnerSID { get; set; }

        public LayerMask TargetMask { get; set; }

        public abstract Character DoDetectCharacter(Vector3 detectPosition);

        public abstract Character[] DoDetectCharacters(Vector3 detectPosition);
    }
}