using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models.Enums_
{
    public enum QR_Type
    {
        Profile,
        Places
    }

    public static class QR_TypeExtension
    {
        static string placesPath => "places/details_";
        public static string ToQrCode(string guid, QR_Type type)
        {

            switch(type)
            {
                case QR_Type.Profile:
                    return $"{type.ToString()}={guid}";
                case QR_Type.Places:
                    return $"https://travelorientalmindoro.ph/" + $"{placesPath}?guid={guid}";

            }
            return "";
        }

        public static QR_Type? GetType(string code)
        {
            if(code.ToLower().Contains(placesPath))
            {
                return QR_Type.Places;
            }

            var list = Enum.GetValues(typeof(QR_Type)).Cast<QR_Type>();
            foreach (var type in list)
            {
                if (code.ToLower().Contains(type.ToString().ToLower()))
                {
                    return type;
                }
            }
            return null;
        }

        public static string GetGuidFromQrCode(string code)
        {
            try
            {
                return code.Substring(code.LastIndexOf('=') + 1);
            }
            catch(Exception ex)
            {
                return code;
            }
        }
    }


}
