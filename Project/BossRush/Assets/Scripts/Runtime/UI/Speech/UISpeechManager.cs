using System.Collections.Generic;
using TeamSuneat;
using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat.UserInterface
{
    public static class UISpeechManager
    {
        private static readonly Dictionary<CharacterNames, UISpeechBubbleText> _activeSpeechBubbles = new();
        private static readonly Dictionary<string, int> _lastDialogueIndex = new();

        public static bool IsShowingSpeech(CharacterNames speakerName)
        {
            return _activeSpeechBubbles.ContainsKey(speakerName) && _activeSpeechBubbles[speakerName] != null;
        }

        public static void StartDialogue(string timelineName)
        {
            ShowSpeech(timelineName, 0);
        }

        public static void ShowSpeech(string timelineName, int index)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return;
            }

            string key = $"{timelineName}_{index:D2}";
            StringDialogueData dialogueData = JsonDataManager.FindStringDialogueData(key);

            if (dialogueData == null || string.IsNullOrEmpty(dialogueData.ID))
            {
                Log.Warning(LogTags.UI, "StringDialogueData를 찾을 수 없습니다: {0}", key);
                return;
            }

            Character speaker = FindCharacter(dialogueData.SpeakerName);
            if (speaker == null)
            {
                Log.Warning(LogTags.UI, "화자를 찾을 수 없습니다: {0}", dialogueData.SpeakerName);
                return;
            }

            // 해당 캐릭터의 기존 대사가 있으면 끊기
            if (_activeSpeechBubbles.TryGetValue(dialogueData.SpeakerName, out UISpeechBubbleText existingBubble))
            {
                if (existingBubble != null)
                {
                    existingBubble.Interrupt();
                }
                _activeSpeechBubbles.Remove(dialogueData.SpeakerName);
            }

            // 새 대사 표시
            UISpeechBubbleText speechBubble = SpawnSpeechBubble(dialogueData, speaker);
            if (speechBubble != null)
            {
                _activeSpeechBubbles[dialogueData.SpeakerName] = speechBubble;
                _lastDialogueIndex[timelineName] = index;

                // Despawn 이벤트 등록
                speechBubble.RegisterDespawnEvent(OnSpeechBubbleDespawned);
            }
        }

        public static void ShowNextSpeech(string timelineName)
        {
            if (_lastDialogueIndex.TryGetValue(timelineName, out int lastIndex))
            {
                ShowSpeech(timelineName, lastIndex + 1);
            }
            else
            {
                StartDialogue(timelineName);
            }
        }

        public static void InterruptSpeech(CharacterNames speakerName)
        {
            if (_activeSpeechBubbles.TryGetValue(speakerName, out UISpeechBubbleText speechBubble))
            {
                if (speechBubble != null)
                {
                    speechBubble.Interrupt();
                }
                _activeSpeechBubbles.Remove(speakerName);
            }
        }

        private static Character FindCharacter(CharacterNames characterName)
        {
            if (characterName == CharacterNames.None)
            {
                return null;
            }

            CharacterManager characterManager = CharacterManager.Instance;
            if (characterManager == null)
            {
                return null;
            }

            // Player 확인
            if (characterName == CharacterNames.PlayerCharacter)
            {
                return characterManager.Player;
            }

            // Monsters 리스트에서 검색
            if (characterManager.Monsters != null && characterManager.Monsters.Count > 0)
            {
                for (int i = 0; i < characterManager.Monsters.Count; i++)
                {
                    if (characterManager.Monsters[i].Name == characterName)
                    {
                        return characterManager.Monsters[i];
                    }
                }
            }

            return null;
        }

        private static UISpeechBubbleText SpawnSpeechBubble(StringDialogueData dialogueData, Character speaker)
        {
            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                Log.Warning(LogTags.UI, "IngameWorldSpace 캔버스를 찾을 수 없습니다.");
                return null;
            }

            GameObject spawnedObject = ResourcesManager.SpawnPrefab("UISpeechBubbleText", canvasOrder.transform);
            if (spawnedObject == null)
            {
                Log.Warning(LogTags.UI, "UISpeechBubbleText 프리팹을 생성할 수 없습니다.");
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UISpeechBubbleText speechBubble = spawnedObject.GetComponent<UISpeechBubbleText>();
            if (speechBubble != null)
            {
                UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
                if (followObject != null)
                {
                    followObject.IsWorldSpaceCanvas = true;
                }

                speechBubble.Setup(dialogueData, speaker);
            }

            return speechBubble;
        }

        private static void OnSpeechBubbleDespawned(UISpeechBubbleText speechBubble)
        {
            if (speechBubble == null)
            {
                return;
            }

            // Dictionary에서 제거 (순회 방식이지만 일반적으로 1-2개만 있으므로 성능 문제 없음)
            CharacterNames? speakerNameToRemove = null;
            foreach (KeyValuePair<CharacterNames, UISpeechBubbleText> pair in _activeSpeechBubbles)
            {
                if (pair.Value == speechBubble)
                {
                    speakerNameToRemove = pair.Key;
                    break;
                }
            }

            if (speakerNameToRemove.HasValue)
            {
                _activeSpeechBubbles.Remove(speakerNameToRemove.Value);
            }
        }
    }
}
