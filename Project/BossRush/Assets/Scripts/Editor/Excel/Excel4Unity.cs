using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public partial class Excel4Unity : Editor
{
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

        StringBuilder stringBuilder = new StringBuilder();
        LitJson.JsonWriter writer = new LitJson.JsonWriter(stringBuilder);

        try
        {
            bool language = tableName.ToLower().Contains("language");
            if (table.TableName.StartsWith("#"))
            {
                return null;
            }

            writer.WriteArrayStart();

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

                writer.WriteObjectStart();

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

                    writer.WritePropertyName(propName);

                    switch (propType)
                    {
                        case "bool":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write(false);
                                    continue;
                                }

                                writer.Write(bool.Parse(v));
                            }
                            break;

                        case "int":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write(0);
                                    continue;
                                }
                                int value = v.Length > 0 ? int.Parse(v) : 0;
                                writer.Write(value);
                            }
                            break;

                        case "int[]":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write("0");
                                    continue;
                                }
                                writer.Write(v);
                            }
                            break;

                        case "float":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write(0f);
                                }
                                else
                                {
                                    float value = float.Parse(v);
                                    value = MathF.Round(value, 4);
                                    writer.Write(value);
                                }
                            }
                            break;

                        case "float[]":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write("0");
                                    continue;
                                }

                                writer.Write(v);
                            }
                            break;

                        case "double":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write(0);
                                    continue;
                                }

                                double value = v.Length > 0 ? double.Parse(v) : 0;
                                writer.Write(value);
                            }
                            break;

                        case "enum":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write("None");
                                    continue;
                                }
                                else if (string.IsNullOrEmpty(v))
                                {
                                    writer.Write("None");
                                    continue;
                                }

                                writer.Write(v);
                            }
                            break;

                        case "enum[]":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write("None");
                                    continue;
                                }
                                else if (string.IsNullOrEmpty(v))
                                {
                                    writer.Write("None");
                                    continue;
                                }

                                // 정규식 패턴을 사용하여 _x000D_ 시퀀스를 제거합니다.
                                v = Regex.Replace(v, @"_x000D_", Environment.NewLine);

                                writer.Write(v);
                            }
                            break;

                        case "string":
                        case "string[]":
                            {
                                if (v.Equals("-"))
                                {
                                    writer.Write(string.Empty);
                                    continue;
                                }
                                else if (string.IsNullOrEmpty(v))
                                {
                                    writer.Write(string.Empty);
                                    continue;
                                }

                                // 정규식 패턴을 사용하여 _x000D_ 시퀀스를 제거합니다.
                                v = Regex.Replace(v, @"_x000D_", Environment.NewLine);

                                writer.Write(v);
                            }
                            break;

                        case "struct":
                            {
                                writer.Write(v);
                            }
                            break;
                    }
                }

                writer.WriteObjectEnd();
            }

            writer.WriteArrayEnd();

            return stringBuilder.ToString();
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