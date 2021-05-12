using Nancy;
using System;
using System.IO;
using System.Reflection;

namespace LSTY.Sdtd.PatronsMod.WebApi
{
    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
