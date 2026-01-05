using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public partial class Excel4Unity : Editor
{
    private sealed class ListWrapper<T>
    {
        public List<T> items;
    }

    [MenuItem("Tools/Excel/모든 엑셀 파일 불러오기")]
    public static void ConvertAllExcelToJSON()
    {
        string[] excelNames = System.Enum.GetNames(typeof(ExcelNames));

        for (int i = 0; i < excelNames.Length; i++)
        {
            ParseExcel(string.Format("/{0}.xlsx", excelNames[i].ToString()));
        }

        EditorUtility.DisplayDialog("Successed!", "Parse all files.", "OK");
    }

    public static void ConvertOneExcelToJSON(ExcelNames[] excelNames)
    {
        string excelFileName;
        for (int i = 0; i < excelNames.Length; i++)
        {
            excelFileName = string.Format("/{0}.xlsx", excelNames[i].ToString());
            ParseExcel(excelFileName);
        }

        string dialogContent = string.Format("Parse [{0}] file.", JoinToString(excelNames));
        EditorUtility.DisplayDialog("Successed!", dialogContent, "OK");
    }

    public static void ConvertOneExcelToJSON(ExcelNames excelName)
    {
        if (ParseExcel(string.Format("/{0}.xlsx", excelName.ToString())))
        {
            string dialogContent = string.Format("Parse [{0}] file.", excelName.ToString());

            EditorUtility.DisplayDialog("Successed!", dialogContent, "OK");
        }
    }

    public static void ConvertStringExcelsToJSON()
    {
        ExcelNames[] excelNames = new ExcelNames[]
        {
            ExcelNames.String,
        };

        for (int i = 0; i < excelNames.Length; i++)
        {
            ParseExcel(string.Format("/{0}.xlsx", excelNames[i].ToString()));
        }

        EditorUtility.DisplayDialog("Successed!", "Parse String file.", "OK");
    }

    private static bool ParseExcel(string excelName)
    {
        try
        {
            DirectoryInfo dataPath = Directory.GetParent(Application.dataPath);
            string filePath = string.Format("{0}/Sheet/{1}", dataPath.ToString(), excelName);
            return ParseFile(excelName, filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());

            return false;
        }
    }

    public static bool ParseFile(string excelName, string filePath)
    {
        if (false == filePath.EndsWith("xlsx"))
        {
            Debug.LogErrorFormat("지원되지 않는 파일 형식입니다. {0}", filePath);
            return false;
        }

        Excel excel = ExcelHelper.LoadExcel(filePath);
        if (excel.Tables.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < excel.Tables.Count; i++)
        {
            ExcelTable table = excel.Tables[i];

            if (table.TableName.Contains("#"))
            {
                continue;
            }

            string tableName = table.TableName;
            string contents = WriteJson(table);
            if (string.IsNullOrEmpty(contents))
            {
                Debug.LogErrorFormat("엑셀 시트를 불러오는 데 실패했습니다. 시트 이름: {1}, 엑셀 파일 이름:{0}", filePath, tableName);
                continue;
            }

            float progressRate = (i + 1) / excel.Tables.Count;
            EditorUtility.DisplayProgressBar("엑셀 시트 불러오기", tableName, progressRate);
            Debug.LogFormat("엑셀 시트를 불러옵니다. 시트 이름: {1}, 엑셀 파일 이름:{0}", excelName, tableName);
            CreateJson(filePath, tableName, contents);
        }

        EditorUtility.ClearProgressBar();

        return true;
    }

    private static string WriteJson(ExcelTable table)
    {
        string tableName = table.TableName;
        string currentPropName = string.Empty;
        string currentPropType = string.Empty;
        int tableRow = 0;
        int tableColumn = 0;
        string v = string.Empty;

        try
        {
            bool language = tableName.ToLower().Contains("language");
            if (table.TableName.StartsWith("#"))
            {
                return null;
            }

            List<Dictionary<string, object>> items = new List<Dictionary<string, object>>();

            // row 0 : 키값
            // row 1 : 타입
            // row 2 : 설명
            for (int row = 0; row <= table.NumberOfRows; row++)
            {
                if (row < 4)
                {
                    continue;
                }

                tableRow = row;

                string idStr = table.GetValue(row, 1).ToString();

                if (idStr.Length <= 0)
                {
                    break;
                }

                Dictionary<string, object> item = new Dictionary<string, object>();

                for (int column = 1; column <= table.NumberOfColumns; column++)
                {
                    tableColumn = column;
                    string propName = table.GetValue(1, column).ToString();
                    string propType = table.GetValue(2, column).ToString();

                    propName = propName.Replace("*", string.Empty);
                    currentPropName = propName;
                    currentPropType = propType;

                    if (propName.StartsWith("#"))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(propName) || string.IsNullOrEmpty(propType))
                    {
                        continue;
                    }

                    v = table.GetValue(row, column).ToString();

                    if (language && v.Contains(" "))
                    {
                        v = v.Replace(" ", "\u00A0");
                    }

                    if (string.IsNullOrEmpty(v))
                    {
                        v = "-";

                        Debug.LogWarningFormat("v is null or empty. TableName: {0}, Prop: {1} ({2}), 행,열: ({3},{4})",
                            tableName, currentPropName, currentPropType, tableRow, tableColumn);
                    }

                    object parsedValue = null;

                    switch (propType)
                    {
                        case "bool":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = false;
                                }
                                else
                                {
                                    parsedValue = bool.Parse(v);
                                }
                            }
                            break;

                        case "int":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = 0;
                                }
                                else
                                {
                                    parsedValue = v.Length > 0 ? int.Parse(v) : 0;
                                }
                            }
                            break;

                        case "int[]":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = "0";
                                }
                                else
                                {
                                    parsedValue = v;
                                }
                            }
                            break;

                        case "float":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = 0f;
                                }
                                else
                                {
                                    float value = float.Parse(v);
                                    value = MathF.Round(value, 4);
                                    parsedValue = value;
                                }
                            }
                            break;

                        case "float[]":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = "0";
                                }
                                else
                                {
                                    parsedValue = v;
                                }
                            }
                            break;

                        case "double":
                            {
                                if (v.Equals("-"))
                                {
                                    parsedValue = 0;
                                }
                                else
                                {
                                    parsedValue = v.Length > 0 ? double.Parse(v) : 0;
                                }
                            }
                            break;

                        case "enum":
                            {
                                if (v.Equals("-") || string.IsNullOrEmpty(v))
                                {
                                    parsedValue = "None";
                                }
                                else
                                {
                                    parsedValue = v;
                                }
                            }
                            break;

                        case "enum[]":
                            {
                                if (v.Equals("-") || string.IsNullOrEmpty(v))
                                {
                                    parsedValue = "None";
                                }
                                else
                                {
                                    // 정규식 패턴을 사용하여 _x000D_ 시퀀스를 제거합니다.
                                    parsedValue = Regex.Replace(v, @"_x000D_", Environment.NewLine);
                                }
                            }
                            break;

                        case "string":
                        case "string[]":
                            {
                                if (v.Equals("-") || string.IsNullOrEmpty(v))
                                {
                                    parsedValue = string.Empty;
                                }
                                else
                                {
                                    // 정규식 패턴을 사용하여 _x000D_ 시퀀스를 제거합니다.
                                    parsedValue = Regex.Replace(v, @"_x000D_", Environment.NewLine);
                                }
                            }
                            break;

                        case "struct":
                            {
                                parsedValue = v;
                            }
                            break;
                    }

                    if (parsedValue != null)
                    {
                        item[propName] = parsedValue;
                    }
                }

                items.Add(item);
            }

            ListWrapper<Dictionary<string, object>> wrapper = new ListWrapper<Dictionary<string, object>>
            {
                items = items
            };

            return JsonConvert.SerializeObject(wrapper, Formatting.None);
        }
        catch (System.Exception e)
        {
            string msg = "실패!\n";
            msg += "TableName: ";
            msg += tableName;
            msg += ", Prop: ";
            msg += currentPropName;
            msg += ", PropType: ";
            msg += currentPropType;
            msg += ", tableRow: ";
            msg += tableRow;
            msg += ", tableColumn: ";
            msg += tableColumn;

            msg += ", Value: ";
            msg += v;

            EditorUtility.DisplayDialog("error!", msg, "ok");
            Debug.LogError(e.ToString());
            Debug.LogError(msg);
            return null;
        }
    }

    private static void CreateJson(string filePath, string tableName, string content)
    {
        string outputDir = Application.dataPath + OUTPUT_DIRECTORY_PATH;
        string outputPath = outputDir + tableName + ".json";

        CreateDirectory(outputDir);

        string excelString = ReadExcel(filePath); ;

        if (false == string.IsNullOrEmpty(excelString))
        {
            if (excelString != content)
            {
                File.WriteAllText(outputPath, content);
            }
        }
    }

    private static string ReadExcel(string excelPath)
    {
        if (File.Exists(excelPath))
        {
            byte[] bytes = File.ReadAllBytes(excelPath);

            UTF8Encoding encoding = new UTF8Encoding();

            return encoding.GetString(bytes);
        }

        return string.Empty;
    }

    private static void CreateDirectory(string outputDir)
    {
        if (false == Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
    }

    private static string JoinToString(ExcelNames[] values)
    {
        if (values != null && values.Length > 0)
        {
            string[] stringArray = new string[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                stringArray[i] = values[i].ToString();
            }

            return JoinToString(stringArray);
        }

        return "None";
    }

    private static string JoinToString(string[] values)
    {
        if (values == null || values.Length == 0)
        {
            return "None";
        }

        StringBuilder stringBuilder = new();

        for (int i = 0; i < values.Length; i++)
        {
            _ = stringBuilder.Append(values[i]);

            if (i < values.Length - 1)
            {
                _ = stringBuilder.Append(", ");
            }
        }

        return stringBuilder.ToString();
    }
}