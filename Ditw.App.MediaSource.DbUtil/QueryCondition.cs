using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.App.MediaSource.DbUtil
{
    public enum ParamNameType
    {
        INT = 1,
        STRING = 2,
        DATE = 3,
        UNKNOWN = -1
    }

    public struct QueryCondition
    {
        public String ParamName
        {
            get;
            set;
        }

        public ParamNameType ParamType
        {
            get;
            set;
        }

        public Object Value
        {
            get;
            set;
        }
    }
}
