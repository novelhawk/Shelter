using System.Collections.Generic;
using Mod.Keybinds;
using Newtonsoft.Json;
using UnityEngine;
using LogType = Mod.Logging.LogType;

namespace Mod.Managers
{
    public class InputManager
    {
        private readonly ConfigManager<Dictionary<InputAction, KeyCode>> _file;
        private Dictionary<InputAction, KeyCode> _dictionary;
        private const string File = "inputs.json";
        private const int KeysNumber = 36;

        public InputManager()
        {
            _file = new ConfigManager<Dictionary<InputAction, KeyCode>>(File);
            Load();
        }

        private void Load()
        {
            Load(_file.ReadFile());

            if (_dictionary.Count < KeysNumber)
            {
                Load(_file.InvalidateCurrentFile());
                Shelter.Log("Number of actions mismatch with the number of entries in {0}", LogType.Error, File);
            }
        }

        private void Load(string json)
        {
            try
            {
                _dictionary = _file.Deserialize(json);
            }
            catch (JsonSerializationException)
            {
                Load(_file.InvalidateCurrentFile());
                Shelter.Log("File {0} was corrupted. Restoring defualt.", LogType.Error, File);
            }
        }

        public bool Save()
        {
            string obj = _file.Serialize(_dictionary);
            return _file.WriteFile(obj);
        }

        public bool IsDown(InputAction action) => Input.GetKeyDown(_dictionary[action]);
        public bool IsUp(InputAction action) => Input.GetKeyUp(_dictionary[action]);
        public bool IsKeyPressed(InputAction action) => Input.GetKey(_dictionary[action]);

        public KeyCode this[InputAction action]
        {
            get
            {
                if (_dictionary.ContainsKey(action))
                    return _dictionary[action];
                return KeyCode.None;
            }
        }
    }
}