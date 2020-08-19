using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.IO;

namespace YoutubeRPG
{
    public class XmlManager<T> //create a generic class
    {
        public Type Type;
        public XmlManager()
        {
            Type = typeof(T); //if the type isn't currently set to anything, this will set Type to whatever the generic class is set to
        }
        public T Load(string path)
        {
            T instance;
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                instance = (T)xml.Deserialize(reader);
            }
            return instance; 
        }

        public void Save(string path, object obj)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                XmlSerializer xml = new XmlSerializer(Type);
                xml.Serialize(writer, obj);
                }
        }

    }
}
