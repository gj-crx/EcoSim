using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileWork : MonoBehaviour
{
    public GM gm;
    public string GameDataPath = @"Gamedata/MainData.txt";
    public string SpeciesDataPath = @"Gamedata/Species/";
    public void WriteBio(bio b)
    {
        string path = SpeciesDataPath;
        if (Directory.Exists(Application.persistentDataPath + "/" + SpeciesDataPath) == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + SpeciesDataPath);
        }
        path = Application.persistentDataPath + "/" + SpeciesDataPath;
        using (StreamWriter w = new StreamWriter(path + b.CreatureName + ".txt"))
        {
            w.WriteLine(b.CreatureName);
            w.WriteLine(b.BioID);
            w.WriteLine(b.ModelID);
            w.WriteLine(b.color.r);
            w.WriteLine(b.color.g);
            w.WriteLine(b.color.b);
            w.WriteLine(b.Damage);
            w.WriteLine(b.HPCurrent);
            w.WriteLine(b.HPMax);
            w.WriteLine(b.SenceRange);
            w.WriteLine(b.energy);
            w.WriteLine(b.EnergyMaximum);
            w.WriteLine(b.AttackSpeed);
            w.WriteLine(b.AttackRange);
            w.WriteLine(b.MoveSpeed);
            w.WriteLine(b.Regen);
            w.WriteLine(b.EnergyRegen);
            w.WriteLine(b.FearRange);
            w.WriteLine(b.Age);
            w.WriteLine(b.MaxAge);
            w.WriteLine(b.AdultAge);
            w.WriteLine(b.EnergyAfterBreed);
            w.WriteLine(b.EnergyGivenToKiller);
            w.WriteLine(b.HabitationType);
            w.WriteLine(b.FoodType);
            w.WriteLine(b.SimpleBreeder);
            w.WriteLine(b.AgressionToAgression);
            w.WriteLine(b.ReincarnateChance);
            w.WriteLine(b.ReincarnateTime);
        }
    }
    public bio LoadBio(string path)
    {
        bio b = new bio();
        using (StreamReader r = new StreamReader(path))
        {
            b.CreatureName = r.ReadLine();
            b.BioID = LoadValueInt(r.ReadLine());
            b.ModelID = LoadValueInt(r.ReadLine());
            float red = 1.0f, green = 1.0f, blue = 1.0f;
            red = LoadValueFloat(r.ReadLine());
            green = LoadValueFloat(r.ReadLine());
            blue = LoadValueFloat(r.ReadLine());
            b.color = new Color(red, green, blue);
            b.Damage = LoadValueFloat(r.ReadLine());
            b.HPCurrent = LoadValueFloat(r.ReadLine());
            b.HPMax = LoadValueFloat(r.ReadLine());
            b.SenceRange = LoadValueFloat(r.ReadLine());
            b.energy = LoadValueFloat(r.ReadLine());
            b.EnergyMaximum = LoadValueFloat(r.ReadLine());
            b.AttackSpeed = LoadValueFloat(r.ReadLine());
            b.AttackRange = LoadValueFloat(r.ReadLine());
            b.MoveSpeed = LoadValueFloat(r.ReadLine());
            b.Regen = LoadValueFloat(r.ReadLine());
            b.EnergyRegen = LoadValueFloat(r.ReadLine());
            b.FearRange = LoadValueFloat(r.ReadLine());
            b.Age = LoadValueFloat(r.ReadLine());
            b.MaxAge = LoadValueFloat(r.ReadLine());
            b.AdultAge = LoadValueFloat(r.ReadLine());
            b.EnergyAfterBreed = LoadValueFloat(r.ReadLine());
            b.EnergyGivenToKiller = LoadValueFloat(r.ReadLine());
            b.HabitationType = LoadValueInt(r.ReadLine());
            b.FoodType = LoadValueInt(r.ReadLine());
            b.SimpleBreeder = LoadValueBool(r.ReadLine());
            b.AgressionToAgression = LoadValueBool(r.ReadLine());
            b.ReincarnateChance = LoadValueFloat(r.ReadLine());
            b.ReincarnateTime = LoadValueFloat(r.ReadLine());
        }
        return b;
    }

    public float LoadValueFloat(string s)
    {
        return float.Parse(s.Substring(s.IndexOf("_") + 1, s.Length - s.IndexOf("_") - 1));
    }
    public void SaveData()
    {
        string path = GameDataPath;
        Debug.Log("Data saving");
        path = Application.persistentDataPath + "/Gamedata/MainData.txt";
        if (Directory.Exists(path))
        {
            File.Delete(path);
        }
        using (StreamWriter w = new StreamWriter(path))
        {
            w.WriteLine("Selected language_" + gm.loc.CurrentLanguageSelected + ".txt");
            for (int i = 0; i < gm.gamedata.MissionsCompleted.Length; i++)
            {
                w.WriteLine("Mission " + i + "_" + gm.gamedata.MissionsCompleted[i].ToString());
            }
        }
    }
    public void LoadData()
    {
        gm.gamedata = new GM.Gamedata();
        gm.gamedata.MissionsCompleted = new bool[6];
        string path = Application.persistentDataPath + "/Gamedata/MainData.txt";
        if (File.Exists(path) == false)
        {
            SaveData();
        }
        else
        { //loading GameData
            using (StreamReader r = new StreamReader(path))
            {
                gm.gamedata.SelectedLanguage = LoadValueString(r.ReadLine());
                for (int i = 0; i < gm.gamedata.MissionsCompleted.Length; i++)
                {
                    gm.gamedata.MissionsCompleted[i] = LoadValueBool(r.ReadLine());
                    if (i < 5)
                    {
                        gm.gui.MissionCompleteCheckmarks[i].SetActive(gm.gamedata.MissionsCompleted[i]);
                    }
                }
            }
            if (gm.gamedata.SelectedLanguage != null && gm.gamedata.SelectedLanguage != "nothing" && gm.gamedata.SelectedLanguage != "nothing.txt")
            {
                gm.loc.ApplyLocalization(Application.persistentDataPath + gm.loc.LocalizationBasePath + gm.gamedata.SelectedLanguage);
            }
            else
            {
                gm.gui.MainPanel.SetActive(false);
                gm.gui.LocalizationSelectionPanel.SetActive(true);
            }
        }
    }

    public bool LoadValueBool(string s)
    {
        return bool.Parse(s.Substring(s.IndexOf("_") + 1, s.Length - s.IndexOf("_") - 1));
    }
    public int LoadValueInt(string s)
    {
        return int.Parse(s.Substring(s.IndexOf("_") + 1, s.Length - s.IndexOf("_") - 1));
    }
    public string LoadValueString(string s, char Symbol = '_')
    {
        return s.Substring(s.IndexOf(Symbol) + 1, s.Length - s.IndexOf(Symbol) - 1);
    }

}
