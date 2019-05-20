using System;
using System.IO;
using UnityEngine;

public class RecordManager 
{
    private const string _recordPath = "R.cd";

    private void _saveRecord(int score)
    {
        File.WriteAllBytes(Application.persistentDataPath + "/" + _recordPath, BitConverter.GetBytes(score));
    }

    private int _loadRecord()
    {
        string path = Application.persistentDataPath + "/" + _recordPath;
        if (!File.Exists(path)) return 0;
        return BitConverter.ToInt32(File.ReadAllBytes(path), 0);
    }

    private int _record = -1;

    public int Record
    {
        get
        {
            if (_record == -1) _record = _loadRecord();
            return _record;
        }

        set
        {
            _record = value;
            _saveRecord(_record);
        }
    }
}
