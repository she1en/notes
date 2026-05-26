using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace ConsoleApp15.Data
{
    public class DataStore<T>
    {
        private readonly string _filePath;
        private readonly JavaScriptSerializer _serializer;

        public DataStore(string fileName)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, fileName);
            _serializer = new JavaScriptSerializer();
        }

        public List<T> Load()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            var json = File.ReadAllText(_filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return _serializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        public void Save(List<T> data)
        {
            var json = _serializer.Serialize(data);
            File.WriteAllText(_filePath, json);
        }

        public string FilePath => _filePath;
    }
}
