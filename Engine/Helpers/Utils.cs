using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Engine.Helpers
{
    public class Utils
    {
        public static string ToImageSource(byte[] byteArray)
        {            
            return byteArray == null || byteArray.Length == 0 ? string.Empty
                : "data:image;base64," + Convert.ToBase64String(byteArray);
        }
    }
}