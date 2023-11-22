using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DayDoc.Components.Mvc
{
    public static class ControllerExtension
    {
        private const string ControllerString = "Controller";
        /// <summary>
        /// Should be used only for Controller names.
        /// </summary>
        /// <param name="value">Controller class name ('HomeController')</param>
        /// <returns>Controller short name ('Home')</returns>
        public static string CSN(this string value)
        {
            if (value.EndsWith(ControllerString))
            {
                //Remove 'Controller' from end of value name.             
                return value.Remove(value.Length - ControllerString.Length);
            }
            throw new ApplicationException("Should be used only for Controller names.");
        }
    }
}
