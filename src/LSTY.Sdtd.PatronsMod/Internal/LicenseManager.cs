﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSTY.Sdtd.PatronsMod.Internal
{
    static class LicenseManager
    {
        public static bool CheckPermission()
        {
            return true;

            if (DateTime.Now < new DateTime(2021, 10, 1))
            {
                return true;
            }

            CustomLogger.Error("Your license has expired!");
            return false;
        }
    }
}
