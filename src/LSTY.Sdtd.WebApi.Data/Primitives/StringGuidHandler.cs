using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace LSTY.Sdtd.WebApi.Data.Primitives
{
    public class StringGuidHandler : SqlMapper.TypeHandler<string>
    {
        public override void SetValue(IDbDataParameter parameter, string value)
        {
            parameter.Value = value;
        }

        public override string Parse(object value)
        {
            return value.ToString();
        }
    }
}
