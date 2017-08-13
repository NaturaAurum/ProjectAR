using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Linq;

public static class GoSheet
{

    public static string[][] GetGoogleSheet(string url, string gid = "0")
    {
#if UNITY_EDITOR
        return GetGoogleSheetWebClient(url, gid);
#else
		return GetGoogleSheetNative(url, gid);
#endif
    }

    public static string[][] GetGoogleSheetWebClient(string url, string gid = "0")
    {
        ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        using (var client = new WebClient())
        {
            try
            {
                url = url.Replace("/edit?usp=sharing", "");
                url += "/export?format=tsv&gid=" + gid;
                client.Encoding = System.Text.Encoding.UTF8;
                string result = client.DownloadString(url);
                string[] lines = result.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
                string[][] flines = lines.Select(s => s.Split("\t".ToCharArray())).ToArray().ToArray();
                return flines;
            }
            catch (WebException e)
            {
                Debug.Log(e.Message.ToString());
                return null;
            }
        }
    }

    public static string[][] GetGoogleSheetNative(string url, string gid = "0")
    {
        url = url.Replace("/edit?usp=sharing", "");
        url += "/export?format=tsv&gid=" + gid;
        WWW data = new WWW(url);
        while (!data.isDone) { }
        string result = data.text;
        string[] lines = result.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        string[][] flines = lines.Select(s => s.Split("\t".ToCharArray())).ToArray().ToArray();
        return flines;
    }

    public static bool CellToBool(string cell)
    {
        if (cell.ToLower().IndexOf("{bool:true}") > -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int CellToInt(string cell)
    {
        float result = 0;
        if (float.TryParse(cell, out result))
        {
            return Mathf.RoundToInt(result);
        }
        else
        {
            return 0;
        }
    }

    public static float CellToFloat(string cell)
    {
        float result = 0;
        if (float.TryParse(cell, out result))
        {
            return result;
        }
        else
        {
            return 0;
        }
    }

    public static Vector2 CellToVector2(string cell)
    {
        if (cell.ToLower().IndexOf("x:") > -1 && cell.ToLower().IndexOf("y:") > -1)
        {
            string x = cell.Substring(cell.ToLower().IndexOf("{x:") + 3, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("{x:") + 3))) - (cell.ToLower().IndexOf("{x:") + 3));
            string y = cell.Substring(cell.ToLower().IndexOf("y:") + 2, (cell.ToLower().IndexOf("}", (cell.ToLower().IndexOf("y:") + 2))) - (cell.ToLower().IndexOf("y:") + 2));
            return new Vector2(CellToFloat(x), CellToFloat(y));
        }
        else
        {
            return new Vector2(0, 0);
        }
    }

    public static Vector3 CellToVector3(string cell)
    {
        if (cell.ToLower().IndexOf("x:") > -1 && cell.ToLower().IndexOf("y:") > -1 && cell.ToLower().IndexOf("z:") > -1)
        {
            string x = cell.Substring(cell.ToLower().IndexOf("{x:") + 3, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("{x:") + 3))) - (cell.ToLower().IndexOf("{x:") + 3));
            string y = cell.Substring(cell.ToLower().IndexOf("y:") + 2, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("y:") + 2))) - (cell.ToLower().IndexOf("y:") + 2));
            string z = cell.Substring(cell.ToLower().IndexOf("z:") + 2, (cell.ToLower().IndexOf("}", (cell.ToLower().IndexOf("z:") + 2))) - (cell.ToLower().IndexOf("z:") + 2));
            return new Vector3(CellToFloat(x), CellToFloat(y), CellToFloat(z));
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    public static Vector4 CellToVector4(string cell)
    {
        if (cell.ToLower().IndexOf("x:") > -1 && cell.ToLower().IndexOf("y:") > -1 && cell.ToLower().IndexOf("z:") > -1 && cell.ToLower().IndexOf("w:") > -1)
        {
            string x = cell.Substring(cell.ToLower().IndexOf("{x:") + 3, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("{x:") + 3))) - (cell.ToLower().IndexOf("{x:") + 3));
            string y = cell.Substring(cell.ToLower().IndexOf("y:") + 2, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("y:") + 2))) - (cell.ToLower().IndexOf("y:") + 2));
            string z = cell.Substring(cell.ToLower().IndexOf("z:") + 2, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("z:") + 2))) - (cell.ToLower().IndexOf("z:") + 2));
            string w = cell.Substring(cell.ToLower().IndexOf("w:") + 2, (cell.ToLower().IndexOf("}", (cell.ToLower().IndexOf("w:") + 2))) - (cell.ToLower().IndexOf("w:") + 2));
            return new Vector4(CellToFloat(x), CellToFloat(y), CellToFloat(z), CellToFloat(w));
        }
        else
        {
            return new Vector4(0, 0, 0, 0);
        }
    }

    public static Color CellToColor(string cell)
    {
        if (cell.ToLower().IndexOf("r:") > -1 && cell.ToLower().IndexOf("g:") > -1 && cell.ToLower().IndexOf("b:") > -1 && cell.ToLower().IndexOf("a:") > -1)
        {
            string r = cell.Substring(cell.ToLower().IndexOf("{r:") + 3, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("{r:") + 3))) - (cell.ToLower().IndexOf("{r:") + 3));
            string g = cell.Substring(cell.ToLower().IndexOf("g:") + 2, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("g:") + 2))) - (cell.ToLower().IndexOf("g:") + 2));
            string b = cell.Substring(cell.ToLower().IndexOf("b:") + 2, (cell.ToLower().IndexOf(",", (cell.ToLower().IndexOf("b:") + 2))) - (cell.ToLower().IndexOf("b:") + 2));
            string a = cell.Substring(cell.ToLower().IndexOf("a:") + 2, (cell.ToLower().IndexOf("}", (cell.ToLower().IndexOf("a:") + 2))) - (cell.ToLower().IndexOf("a:") + 2));
            return new Color(CellToInt(r) / 255f, CellToInt(g) / 255f, CellToInt(b) / 255f, CellToInt(a) / 255f);
        }
        else
        {
            return Color.white;
        }
    }

}