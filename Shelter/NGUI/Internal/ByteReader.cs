using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NGUI.Internal
{
    public class ByteReader
    {
        private readonly byte[] _buffer;
        private int _offset;

        public ByteReader(TextAsset asset)
        {
            this._buffer = asset.bytes;
        }

        public Dictionary<string, string> ReadDictionary()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            char[] separator = { };
            while (this.CanRead)
            {
                string str = this.ReadLine();
                if (str == null)
                {
                    return dictionary;
                }
                if (!str.StartsWith("//"))
                {
                    string[] strArray = str.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (strArray.Length == 2)
                    {
                        string str2 = strArray[0].Trim();
                        string str3 = strArray[1].Trim().Replace(@"\n", "\n");
                        dictionary[str2] = str3;
                    }
                }
            }
            return dictionary;
        }

        private string ReadLine()
        {
            int length = _buffer.Length;
            
            // Ignore every metadata character -- \r\n
            while (_offset < length)
            {
                if (_buffer[_offset] >= 32)
                    break;
                
                _offset++;
            }
            
            // Reached end of array
            if (_offset >= length)
            {
                _offset = length;
                return null;
            }
            
            // Read line
            int offset = _offset;
            while (offset < length)
            {
                // The line ends before end of array
                switch (_buffer[offset++])
                {
                    case 10: // Line Feed \n
                    case 13: // Carriage Return \r
                        _offset = offset;
                        return ByteToString(_buffer, _offset, offset - _offset - 1);
                }
            }
            
            // Line ends with end of array 
            offset++;
            _offset = offset;
            return ByteToString(_buffer, _offset, offset - _offset - 1);
        }

        private static string ByteToString(byte[] buffer, int start, int count)
        {
            return Encoding.UTF8.GetString(buffer, start, count);
        }

        private bool CanRead => _buffer != null && 
                                _offset < _buffer.Length;
    }
}

