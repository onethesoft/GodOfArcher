using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class Util : MonoBehaviour
{
    static string[] unitSymbol = new string[] { "", "만", "억", "조", "경", "해" ,"자" , "양", "구" , "간" , "정", "재" , "극"};
   
    static string unitySymbolPattern = "(극|재|정|간|구|양|자|해|경|조|억|만)";

    static StringBuilder gStrBuilder = new StringBuilder();

    public static string GetTimeString(System.DateTime? dateTime)
    {
        return dateTime.HasValue ? dateTime.Value.ToString(PlayFab.Internal.PlayFabUtil._defaultDateTimeFormats[PlayFab.Internal.PlayFabUtil.DEFAULT_UTC_OUTPUT_INDEX]) : dateTime.ToString();
    }
    public static DateTime? TryParseDateTime(string datetime)
    {
        DateTime _ret;
        if (DateTime.TryParse(datetime, null, System.Globalization.DateTimeStyles.RoundtripKind, out _ret))
            return _ret;
        else
            return null;
      
    }
    public static string GetBigIntegerUnit(BigInteger Number, bool isFirstOrder = false)
    {
        if (Number == 0) { return "0"; }

        //return GetMoneyString(Number);

        int unitID = 0;

        string number = string.Format("{0:# #### #### #### #### #### #### #### #### #### #### #### ####}", Number).TrimStart();
        string[] splits = number.Split(' ');

        gStrBuilder.Clear();
        //StringBuilder sb = new StringBuilder();

        int _highSymbolIndex = -1;
        for (int i = splits.Length; i > 0; i--)
        {

            int digits = 0;
            if (int.TryParse(splits[i - 1], out digits))
            {
                // 앞자리가 0이 아닐때
                if (digits != 0)
                {
                    gStrBuilder.Insert(0, $"{ digits}{ unitSymbol[unitID] } ");
                    //sb.Insert(0, $"{ digits}{ unitSymbol[unitID] } ");
                    _highSymbolIndex = unitID;
                }
               
            }
            else
            {
                // 마이너스나 숫자외 문자
                //sb.Insert(0, $"{ splits[i - 1] }");
                gStrBuilder.Insert(0, $"{ splits[i - 1] }");
              
            }
            unitID++;
           
        }

        if (_highSymbolIndex > 2)
        {
            string _ret = gStrBuilder.ToString();
            int _retCount = 0;
            int _findindex = 0;
            for (int j = unitSymbol.Length - 1; j >= 0; j--)
            {
                if(_ret.Contains(unitSymbol[j]))
                {

                    _retCount++;
                    if (_retCount >= 2)
                    {
                        _findindex = _ret.IndexOf(unitSymbol[j]);
                        break;
                      
                    }
                           
                }
            }
           
            return _ret.Remove(_findindex + 1);
           
            /*
            string _ret = "";
            string _pattern = @"([0-9]{1,4}" + unitySymbolPattern + ")";
            Match m = Regex.Match(gStrBuilder.ToString(), _pattern);
            int _retCount = 0;
            int _order = isFirstOrder ? 0 : 1;
            while(m.Success)
            {
                _ret += m.Value;
               
                m = m.NextMatch();
                _retCount++;
                if (_retCount > _order)
                    break;

            }
            return _ret;
            */
        }
        else
        {
           
            return gStrBuilder.ToString();
        }
    }

    public static string GetMoneyString(BigInteger Number)
    {
        string ret;
        BigInteger MaxExponent = 1000;
        int idx = 0;

        while (MaxExponent < Number)
        {
            MaxExponent *= 1000;
            idx++;
        }

        if (idx < 1)
            ret = Number.ToString();
        else if (idx == 1)
        {
            int Target = (int)Number;
            int FirstInteger = Target / 1000;
            int FirstDecimal = (Target % 1000) / 100;
            int SecondDecimal = ((Target % 1000) % 100) / 10;

            ret = FirstInteger.ToString() + "." + FirstDecimal.ToString() + SecondDecimal.ToString() + "A";
        }
        else
        {
            MaxExponent /= 1000;
            BigInteger SecondExponent = MaxExponent / 10;
            BigInteger ThirdExponent = MaxExponent / 100;

            int FirstInteger = (int)(Number / MaxExponent);
            int FirstDecimal = (int)((Number % MaxExponent) / SecondExponent);
            int SecondDecimal = (int)(((Number % MaxExponent) % SecondExponent) / ThirdExponent);

            string Unit;

            if (idx > 26)
            {
                int temp = idx / 26;
                int sub = idx % 26;

                if (temp == 0)
                {
                    char sec_ch = (char)(sub + 64);
                    Unit = sec_ch.ToString();
                }
                else
                {
                    char fir_ch = (char)(temp + 64);
                    char sec_ch = (char)(sub + 64);
                    Unit = fir_ch.ToString() + sec_ch.ToString();
                }

            }
            else
            {
                char s = (char)(idx + 64);
                Unit = s.ToString();
            }



            ret = FirstInteger.ToString() + "." + FirstDecimal.ToString() + SecondDecimal.ToString() + Unit;
        }
        return ret;

    }
    public static bool IsStringFormat(string text)
    {
        Regex reg = new Regex(@"(^\{\d\})?$");
        if(reg.Match(text).Success)
            return true;
        else
            return false;
    }
    public static int GetIntFromString(string text)
    {
        Debug.Log(text);
        string tempStr = Regex.Replace(text, @"[^0-9]", "");
        int rstInt = int.Parse(tempStr);
        return rstInt;
    }
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static UnityEngine.Component GetOrAddComponent(GameObject go, System.Type type) 
    {
        return go.GetComponent(type) ? go.GetComponent(type) : go.AddComponent(type);
    }

    // GameObject 를 찾아주는 함수
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;
        else
            return transform.gameObject;
    }


    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || name == transform.name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || name == component.name)
                    return component;

            }

        }

        return null;
    }


    



}
