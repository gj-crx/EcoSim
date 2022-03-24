using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Loc : MonoBehaviour
{
    public string CurrentLanguageSelected = "nothing";
    public string LocalizationPathToGenerate = @"\Localizations\NewLocalization.txt";
    public string LocalizationPathToTranslate = @"\Localizations\NewLocalization.txt";
    public string LocalizationBasePath = @"\Localizations\";
    public GameObject prefabLocalizationVariantPanel;
    public GameObject contentLocalizationVariants;
    public bool GenerateOrder = false;
    public bool MakeTranslationFile = false;
    public bool UseTranslationFile = false;
    public bool LoadOrder = false;

    [Header("Values used in game")]
    public string[] ActionsLocalized = new string[5];


    public FileWork fw;
    void Start()
    {
            FindAllLocalizations();
        //PreloadLocalization(Resources.Load<TextAsset>("kazahstan").ToString());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GenerateOrder)
        {
            GenerateOrder = false;
            GenerateLocalization();
        }
        if (LoadOrder)
        {
            LoadOrder = false;
            ApplyLocalization(LocalizationPathToGenerate);
        }
        if (MakeTranslationFile)
        {
            MakeTranslationFile = false;
            CreateTranslationSubfile();
        }
        if (UseTranslationFile)
        {
            UseTranslationFile = false;
            UseTranslationOnLocalization(LocalizationPathToGenerate);
        }
    }
    public void GenerateLocalization(string EndingSymbol = "&")
    {
        using (StreamWriter w = new StreamWriter(Application.persistentDataPath + LocalizationPathToGenerate))
        {
            w.WriteLine("Localization file_True" + EndingSymbol);
            w.WriteLine("Localization name_English" + EndingSymbol);
            for (int i = 0; i < fw.gm.g1.CreaturesToSpawn.Length; i++)
            {
                w.WriteLine("Creature " + i + "]" + fw.gm.g1.CreaturesToSpawn[i].GetComponent<bio>().CreatureName + EndingSymbol);
            }
            for (int i = 0; i < ActionsLocalized.Length; i++)
            {
                w.WriteLine("Action " + i + "]" + ActionsLocalized[i] + EndingSymbol);
            }
            var trs = GameObject.Find("GUI").gameObject.GetComponentsInChildren(typeof(Transform), true);
            trs = UniteComponentArrays(trs, GameObject.Find("MainMenuGUI").gameObject.GetComponentsInChildren(typeof(Transform), true));
            foreach (var v in trs)
            {
                Text a = v.GetComponent<Text>();
                if (a != null && a.text.Length > 2 && a.transform.parent.gameObject.tag != "Localization")
                {
                    w.WriteLine(a.transform.parent.gameObject.name + "|" + a.transform.gameObject.name + "]"
                        + a.text + EndingSymbol);
                }
            }
            w.WriteLine("end" + EndingSymbol);
        }
    }
    public void FindAllLocalizations()
    {
        foreach (var v in Resources.LoadAll<TextAsset>("Localizations/"))
        {
            Debug.Log(v.name);
            PreloadLocalization(v.ToString(), v.name);
        }
        
    }
    public void ApplyLocalization(string FullLocalizationPath)
    {
        using (StreamReader r = new StreamReader(FullLocalizationPath))
        {  //line structure: ParentName|ObjectName]comment
            r.ReadLine(); //skip first line
            CurrentLanguageSelected = fw.LoadValueString(r.ReadLine());
            //loading creatures names
            for (int i = 0; i < fw.gm.g1.CreaturesToSpawn.Length; i++)
            {
                fw.gm.g1.CreaturesToSpawn[i].GetComponent<bio>().CreatureName = fw.LoadValueString(r.ReadLine(), ']');
            }
            //loading actions names
            for (int i = 0; i < 6; i++)
            {
                ActionsLocalized[i] = fw.LoadValueString(r.ReadLine(), ']');
            }
            var trs = GameObject.Find("GUI").gameObject.GetComponentsInChildren(typeof(Transform), true);
            trs = UniteComponentArrays(trs, GameObject.Find("MainMenuGUI").gameObject.GetComponentsInChildren(typeof(Transform), true));
            string s = r.ReadLine(); //getting the first line
            while (s != "end")
            {
                string parent = "null";
                string child = "null";
                string text = "no localization";
                try
                {
                    parent = s.Substring(0, s.IndexOf("|"));
                    child = s.Substring(s.IndexOf("|") + 1, s.IndexOf("]") - 1 - s.IndexOf("|"));
                    text = s.Substring(s.IndexOf("]") + 1);
                    foreach (var n in trs)
                    {
                        if (n.gameObject.name == child && n.transform.parent.gameObject.name == parent)
                        {
                            n.GetComponent<Text>().text = text;
                           //Debug.Log("Найдено " + parent + child + " " + text);
                        }
                        else
                        {
                            // Debug.Log("Не найдено " + parent + child);
                        }
                    }
                }
                catch
                {
                    Debug.Log("Ошибка " + parent + child);
                }
                s = r.ReadLine();
            }
        }
        fw.gm.gui.LocalizationSelectionPanel.SetActive(false);
        fw.gm.gui.MainPanel.SetActive(true);
        fw.SaveData();
    }
    public GameObject FindObjects(GameObject parent, string name)
    {
        var trs = parent.GetComponentsInChildren(typeof(Transform), true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
    public Component[] UniteComponentArrays(Component[] c1, Component[] c2)
    {
        Component[] c3 = new Component[c1.Length + c2.Length];
        int n = 0;
        for (int i = 0; i < c3.Length; i++)
        {
            if (i < c1.Length)
            {
                c3[i] = c1[i];
            }
            else
            {
                c3[i] = c2[n];
                n++;
            }
        }
        return c3;
    }
    public void CreateTranslationSubfile()
    {
        List<string> LocalizationValues = new List<string>();
        using (StreamReader r = new StreamReader(Application.persistentDataPath + LocalizationPathToTranslate))
        {
            r.ReadLine();
            r.ReadLine(); //пропускаем первые две строки
            string s = r.ReadLine(); //принимаем входную строку
            while (s != null)
            {
                string OutputString = s.Substring(s.IndexOf("]") + 1);
                LocalizationValues.Add(OutputString);
                s = r.ReadLine();
            }
        }
        using (StreamWriter w = new StreamWriter("To Translate.txt"))
        {
            foreach (string v in LocalizationValues)
            {
                w.WriteLine(v);
            }
        }
    }
    public void UseTranslationOnLocalization(string LocalizationPath)
    {
        List<string> LocalizationValues = new List<string>();
        using (StreamReader r = new StreamReader("To Translate.txt"))
        {
            string s = r.ReadLine(); //принимаем входную строку
            while (s != null)
            {
                LocalizationValues.Add(s);
                s = r.ReadLine();
            }
        }
        List<string> LocalizationValues2 = new List<string>();
        using (StreamReader r = new StreamReader(Application.persistentDataPath + LocalizationBasePath + "English.txt"))
        {
            r.ReadLine();
            r.ReadLine(); //пропускаем первые две строки
            string s = r.ReadLine(); //принимаем входную строку
            int i = 0;
            while (s != null && s != "end" && s != "end&")
            {
                Debug.Log(LocalizationValues[i] + s);
                string OutputString = s.Substring(0, s.IndexOf("]") + 1) + LocalizationValues[i];
                LocalizationValues2.Add(OutputString);
                s = r.ReadLine();
                i++;
            }
        }
        File.Delete(Application.persistentDataPath + LocalizationPath);
        using (StreamWriter w = new StreamWriter(Application.persistentDataPath + LocalizationPath))
        {
            w.WriteLine("Localization file_True");
            w.WriteLine("Localization name_Undefined");
            foreach (string v in LocalizationValues2)
            {
                w.WriteLine(v);
            }
            w.WriteLine("end&");
        }
        Debug.Log("Complited, put this file to Resources folder " + LocalizationPath);
    }
    public void PreloadLocalization(string source, string LocalizationName)
    {
        if (Directory.Exists(Application.persistentDataPath + LocalizationBasePath) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + LocalizationBasePath);
        }
        if (File.Exists(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt"))
        {
            File.Delete(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt");
        }
        using (StreamWriter w = new StreamWriter(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt"))
        {
            int i = 0;
            foreach (var v in source.Split('&'))
            {
                if (v.Contains("|") || v.Contains("_") || v.Contains("]"))
                {
                    w.WriteLine(v);
                }
                i++;
            }
        }
        List<string> l = new List<string>();
        using (StreamReader r = new StreamReader(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt"))
        {
            string n;
            while ((n = r.ReadLine()) != null)
            {
                if (n.Length > 2)
                {
                    l.Add(n);
                }
            }
        }
        if (File.Exists(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt"))
        {
            File.Delete(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt");
        }
        using (StreamWriter w = new StreamWriter(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt"))
        {
            foreach (var v in l)
            {
                w.WriteLine(v);
            }
            w.WriteLine("end");
        }
        // AddLineEndSymbol(Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt");
        GameObject ng = Instantiate(prefabLocalizationVariantPanel);
        ng.transform.SetParent(contentLocalizationVariants.transform);
        ng.name = LocalizationName;
        ng.transform.Find("Text").GetComponent<Text>().text = ng.name;
        ng.GetComponent<IndicatorElement>().LocalizationPath = Application.persistentDataPath + LocalizationBasePath + LocalizationName + ".txt";
    }

}
