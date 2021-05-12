using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod
{
    static class LicenseManager
    {
        public static bool Check()
        {
            if (DateTime.Now < new DateTime(2021, 6, 1))
            {
                return true;
            }

            CustomLogger.Error("Your license has expired!");
            return false;
        }
    }
}
