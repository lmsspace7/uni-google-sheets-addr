#if UNITY_2017_1_OR_NEWER || UNITY_BUILD
using GoogleSheet.IO;
using System.Collections.Generic;
using UGS;
using UnityEngine;

namespace UGS.IO
{
    public interface IDataLoader
    {
        public TextAsset GetOriginData(string fileName);
    }

    public class UGSWrapperReader : IFileReader
    {
        private bool _ = false;

        public IDataLoader DataLoader { get; set; }

        public string ReadData(string fileName)
        {
            string content = null;
            UGSettingObject setting = Resources.Load<UGSettingObject>("UGSettingObject");
            if (_)
            {
                content = LoadAssetFromDownloadedPath(fileName);
            }
            else
            {

                if (Application.isPlaying == false)
                {
                    content = EditorAssetLoad(fileName);
                }
                else
                {
                    content = RuntimeAssetLoad(fileName);
                }
            }

            if (setting.base64)
                content = UGS.Unused.Base64Utils.Decode(content);
            return content;
        }

        string ToUnityResourcePath(string path)
        {
            var paths = path.Split('/');
            bool link = false;
            List<string> newPath = new List<string>();
            foreach (var value in paths)
            {
                if (value == "Resources")
                {
                    link = true;
                    continue;
                }

                if (link)
                {
                    newPath.Add(value);
                }
            }

            return string.Join("/", newPath);
        }
        public string EditorAssetLoad(string fileName)
        {
            var combine = System.IO.Path.Combine(UGSettingObjectWrapper.JsonDataPath, fileName);
            combine = combine.Replace("\\", "/");
            var filePath = ToUnityResourcePath(combine);

            var textasset = Resources.Load<TextAsset>(filePath);

            if (textasset != null)
            {
                return textasset.text;
            }
            else
            {

                throw new System.Exception($"UGS File Read Failed (path = {"UGS.Data/" + fileName})");
            }
        }

        public string RuntimeAssetLoad(string fileName)
        {
            if (DataLoader == null)
            {
                Debug.LogError("데이터 로더에 값이 할당되지 않았습니다.");
                return null;
            }

            var textasset = DataLoader.GetOriginData(fileName);
            if (textasset == null)
                return null;

            return textasset.text;
        }


        public string LoadAssetFromDownloadedPath(string filename)
        {
            var path = System.IO.Path.Combine(Application.persistentDataPath, "ugs/data", filename + ".json");
            var file = new System.IO.FileInfo(path);

            if (!file.Exists)
                throw new UGS.Unused.Exceptions.DataFileNotFoundException(path + "경로에 ugs 데이터가 없습니다. 서버로부터 데이터를 받지 않았을 수 있습니다.");

            var content = System.IO.File.ReadAllText(file.FullName);
            return content;
        }
    }
}
#endif