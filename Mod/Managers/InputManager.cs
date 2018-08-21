using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using Mod.Keybinds;
using UnityEngine;

namespace Mod.Managers
{
    public class InputManager : Dictionary<InputAction, KeyCode>
    {
        private readonly ConfigManager<List<Keybind>> _file;
        private const string File = "inputs.json";
        private const int KeysNumber = (int) InputAction.__Last;

        public InputManager() : base(KeysNumber)
        {
            _file = new ConfigManager<List<Keybind>>(File);
            Load();
        }

        public void Load()
        {
            Load(_file.ReadFile());
            
            if (Count < KeysNumber)
                Load(_file.InvalidateCurrentFile());
        }

        private void Load(string json)
        {
            Clear();
            foreach (var entry in _file.Deserialize(json))
                Add(entry.Action, entry.Key);
        }

        public bool Save()
        {
            var obj = this.Select(x => new Keybind {Action = x.Key, Key = x.Value}).ToList();
            return _file.WriteFile(_file.Serialize(obj));
        }

        public bool IsKeyPressed(InputAction action)
        {
            return Input.GetKey(this[action]);
        }

        public bool IsUp(InputAction action)
        {
            return Input.GetKeyUp(this[action]);
        }
        
        public bool IsDown(InputAction action)
        {
            return Input.GetKeyDown(this[action]);
        }
    }
}