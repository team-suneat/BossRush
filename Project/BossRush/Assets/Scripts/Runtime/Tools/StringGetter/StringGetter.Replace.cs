using System;
using System.Text.RegularExpressions;
using TeamSuneat.Setting;
using UnityEngine;

namespace TeamSuneat
{
    public static partial class StringGetter
    {
        private static MatchCollection GetMatches(string input, string pattern)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    return Regex.Matches(input, pattern);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"매치 결과 객체를 얻지 못했습니다. input: {input}, pattern: {pattern}\n" + ex.ToString());
            }

            return null;
        }

        //

        public static string ReplaceCharacterName(string input)
        {
            return ReplaceCharacterName(input, GameSetting.Instance.Language.Name);
        }

        private static string ReplaceBuffName(string input)
        {
            return ReplaceBuffName(input, GameSetting.Instance.Language.Name);
        }

        private static string ReplaceStateEffect(string input)
        {
            return ReplaceStateEffect(input, GameSetting.Instance.Language.Name);
        }

        private static string ReplaceStatName(string input)
        {
            return ReplaceStatName(input, GameSetting.Instance.Language.Name);
        }

        public static string ReplaceMonsterName(string input)
        {
            return ReplaceMonsterName(input, GameSetting.Instance.Language.Name);
        }

        public static string ReplaceItemName(string input)
        {
            return ReplaceItemName(input, GameSetting.Instance.Language.Name);
        }

        public static string ReplaceStageName(string input)
        {
            return ReplaceStageName(input, GameSetting.Instance.Language.Name);
        }

        public static string ReplaceAreaName(string input)
        {
            return ReplaceAreaName(input, GameSetting.Instance.Language.Name);
        }

        public static string ReplaceCurrencyName(string input)
        {
            return ReplaceCurrencyName(input, GameSetting.Instance.Language.Name);
        }

        //

        public static string ReplaceCharacterName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Character\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;
                    if (Enum.TryParse(nameString, out CharacterNames enumPlayerName))
                    {
                        input = input.Replace(match.Value, $"<style=Character>{enumPlayerName.GetLocalizedString(languageName)}</style>");
                        continue;
                    }

                    Log.Error("캐릭터 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Character."))
            {
                Log.Error("치환되지 않은 캐릭터 이름이 남아있습니다. {0}", input);
            }

            return input;
        }

        private static string ReplaceBuffName(string input, LanguageNames languageName)
        {
            string pattern = @"\[BuffName\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string buffNameString = match.Groups[1].Value;
                    if (!Enum.TryParse(buffNameString, out BuffNames enumBuffName))
                    {
                        Log.Error("버프 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    input = input.Replace(match.Value, $"<style=BuffName>{enumBuffName.GetLocalizedString(languageName)}</style>");
                }
            }
            if (input.Contains("[BuffName."))
            {
                Log.Error("치환되지 않은 버프 이름이 남아있습니다. {0}", input);
            }
            return input;
        }

        private static string ReplaceStateEffect(string input, LanguageNames languageName)
        {
            string pattern = @"\[StateEffect\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string stateEffectNameString = match.Groups[1].Value;

                    if (!Enum.TryParse(stateEffectNameString, out StateEffects enumStateEffectName))
                    {
                        Log.Error("상태이상 이름을 변환할 수 없습니다. {0}", match.Value);
                        continue;
                    }

                    input = input.Replace(match.Value, $"<style=StateEffect>{enumStateEffectName.GetLocalizedString(languageName)}</style>");
                }
            }
            if (input.Contains("[StateEffect."))
            {
                Log.Error("치환되지 않은 상태이상 이름이 남아있습니다. {0}", input);
            }
            return input;
        }

        private static string ReplaceStatName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Stat\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string statNameString = match.Groups[1].Value;

                    if (Enum.TryParse(statNameString, out StatNames enumStatName))
                    {
                        string statNameContent = enumStatName.GetLocalizedString();
                        input = input.Replace(match.Value, $"<style=StatName>{statNameContent}</style>");
                        continue;
                    }

                    Log.Error("능력치 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }

            if (input.Contains("[Stat."))
            {
                Log.Error("치환되지 않은 능력치 이름이 남아있습니다. {0}", input);
            }

            return input;
        }

        public static string ReplaceMonsterName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Monster\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string nameString = match.Groups[1].Value;

                    if (Enum.TryParse(nameString, out CharacterNames enumMonsterName))
                    {
                        input = input.Replace(match.Value, $"<style=Monster>{enumMonsterName.GetLocalizedString(languageName)}</style>");
                        continue;
                    }

                    Log.Error("몬스터 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Monster."))
            {
                Log.Error("치환되지 않은 몬스터 이름이 남아있습니다. {0}", input);
            }

            return input;
        }

        public static string ReplaceItemName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Item\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];
                    string nameString = match.Groups[1].Value;
                    if (Enum.TryParse(nameString, out ItemNames enumItemName))
                    {
                        Color gradeColor = GradeNames.Common.GetGradeColor();
                        string replaced;

                        replaced = enumItemName.GetNameString(languageName);
                        replaced = replaced.ToColorString(gradeColor);

                        input = input.Replace(match.Value, replaced);
                        continue;
                    }

                    Log.Error("아이템 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Item."))
            {
                Log.Error("치환되지 않은 아이템 이름이 남아있습니다. {0}", input);
            }

            return input;
        }

        public static string ReplaceStageName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Stage\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string StageNameString = match.Groups[1].Value;

                    if (Enum.TryParse(StageNameString, out StageNames enumStageName))
                    {
                        input = input.Replace(match.Value, $"<style=Stage>{enumStageName.GetLocalizedString(languageName)}</style>");
                        continue;
                    }

                    Log.Error("스테이지 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Stage."))
            {
                Log.Error("치환되지 않은 스테이지 이름이 남아있습니다. {0}", input);
            }
            return input;
        }

        public static string ReplaceAreaName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Area\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string areaNameString = match.Groups[1].Value;

                    if (Enum.TryParse(areaNameString, out AreaNames enumAreaName))
                    {
                        input = input.Replace(match.Value, $"<style=Area>{enumAreaName.GetLocalizedString(languageName)}</style>");
                        continue;
                    }

                    Log.Error("지역 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Area."))
            {
                Log.Error("치환되지 않은 지역 이름이 남아있습니다. {0}", input);
            }

            return input;
        }

        public static string ReplaceCurrencyName(string input, LanguageNames languageName)
        {
            string pattern = @"\[Currency\.([a-zA-Z0-9_]+)\]";
            MatchCollection matches = GetMatches(input, pattern);
            if (matches != null)
            {
                foreach (Match match in matches)
                {
                    string CurrencyNameString = match.Groups[1].Value;

                    if (Enum.TryParse(CurrencyNameString, out CurrencyNames enumCurrencyName))
                    {
                        switch (enumCurrencyName)
                        {
                            case CurrencyNames.Gold:
                                input = input.Replace(match.Value, $"<style=Gold>{enumCurrencyName.GetLocalizedString(languageName)}</style>");
                                break;

                            case CurrencyNames.Diamond:
                                input = input.Replace(match.Value, $"<style=Diamond>{enumCurrencyName.GetLocalizedString(languageName)}</style>");
                                break;
                            case CurrencyNames.Emerald:
                                input = input.Replace(match.Value, $"<style=Emerald>{enumCurrencyName.GetLocalizedString(languageName)}</style>");
                                break;
                        }

                        continue;
                    }

                    Log.Error("화폐 이름을 변환할 수 없습니다. {0}", match.Value);
                }
            }
            if (input.Contains("[Currency."))
            {
                Log.Error("치환되지 않은 화폐 이름이 남아있습니다. {0}", input);
            }

            return input;
        }
    }
}