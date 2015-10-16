using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Kinect.Replay.Record
{
    public static class Helper
    {
        public static void SetPrivateFieldValue<T>(this object obj, string propName, T val)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            Type t = obj.GetType();
            MethodInfo fi = null;
            while (fi == null && t != null)
            {
                //fi = t.GetField(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                fi = t.GetMethod(propName);
                t = t.BaseType;
            }
            if (fi == null) throw new ArgumentOutOfRangeException("propName", string.Format("Field {0} was not found in Type {1}", propName, obj.GetType().FullName));
            

            //Ship ship = new Ship();
            //string value = "5.5";
            //PropertyInfo propertyInfo = ship.GetType().GetProperty("Latitude");
            //propertyInfo.SetValue(ship, Convert.ChangeType(value, propertyInfo.PropertyType), null);

        }
    }
}
