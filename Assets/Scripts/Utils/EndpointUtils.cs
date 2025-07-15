using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Networking;

namespace Iterum.Scripts.Utils
{
    public static class EndpointUtils
    {
        private static readonly string Base = "http://localhost:5000/iterum/api";

        private static readonly string Users = $"{Base}/user";
        public static readonly string Login = $"{Users}/login";
        public static readonly string Register = $"{Users}/register";
        public static readonly string Refresh = $"{Users}/refresh";

        public static readonly string Journal = $"{Base}/journal";
        public static readonly string SaveJournal = $"{Journal}/save";
        public static readonly string GetJournals = $"{Journal}/list";
        public static string GetJournal(string name) => $"{Journal}/list/{name}";
        public static string JournalPreview(string path) => $"{Journal}/preview/{path}";
        public static string DeleteJournal(string name) => $"{Journal}/delete/{name}";

        private static readonly string Images = $"{Base}/images";
        public static readonly string UploadImage = $"{Images}/upload";
        public static readonly string GetImages = $"{Images}/list";
        public static string GetImage(string name) => $"{Images}/list/{UnityWebRequest.EscapeURL(name)}";
        public static string ImagePreview(string name) => $"{Images}/preview/{UnityWebRequest.EscapeURL(name)}";
        public static string DeleteImage(string name) => $"{Images}/delete/{name}";

        public static readonly string Maps = $"{Base}/maps";
        public static readonly string CreateMap = $"{Maps}/save";
        public static readonly string UpdateMap = $"{Maps}/update";
        public static string DeleteMap(string mapId) => $"{Maps}/delete/{mapId}";

        public static readonly string Characters = $"{Base}/characters";
        public static readonly string CreateCharacter = $"{Characters}/save";
        public static readonly string UpdateCharacter = $"{Characters}/update";
        public static string DeleteCharacter(string character) => $"{Characters}/delete/{character}";

        public static readonly string Actions = $"{Base}/actions";
        public static readonly string CreateAction = $"{Actions}/save";
        public static readonly string UpdateAction = $"{Actions}/update";
        public static string DeleteAction(string actionId) => $"{Actions}/delete/{actionId}";
    }
}
