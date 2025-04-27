using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class StringFormats
    {
        // d
        public const string DATE_FORMAT_SHORT_1 = "MM/dd/yyyy";             // 05/15/2020
        public const string DATE_FORMAT_SHORT_2 = "MM-dd-yyyy";             // 05-15-2020
        public const string DATE_FORMAT_SHORT_3 = "dd-MMM-yyyy";            // 15-May-2020
        public const string DATE_FORMAT_SHORT_4 = "yyyy-MM-dd";             // 2020-May-15
        public const string DATE_FORMAT_SHORT_5 = "MMM. dd, yyyy";          // Jun. 15, 2020
        public const string DATE_FORMAT_SHORT_7 = "MM.dd.yyyy";             // 05.15.2020
        public const string DATE_FORMAT_SHORT_8 = "MMddyyyy";               // 05152020
        public const string DATE_FORMAT_SHORT_9 = "ddMMyyyy";               // 15052020
        public const string DATE_FORMAT_SHORT_10 = "dd, yyyy";              // 15, 2020
        // D
        public const string DATE_FORMAT_LONG_1 = "dddd, MMMM dd, yyyy";     // Monday, June 15, 2020
        public const string DATE_FORMAT_LONG_2 = "MMMM dd, yyyy";           // June 15, 2020
        public const string DATE_FORMAT_LONG_3 = "MMM. dd, yyyy";           // Jun. 15, 2020
        // m, M
        public const string DATE_FORMAT_LONG_4 = "MMMM dd";                 // June 15
        // y, Y
        public const string DATE_FORMAT_LONG_5 = "MMMM yyyy";               // June 2020
        // r, R
        public const string DATE_FORMAT_LONG_6 = "ddd, dd MMM yyyy";        // Mon, 15 Jun 2020
        public const string DATE_FORMAT_LONG_7 = "MMM. dd";                 // Jun. 15
        public const string DATE_FORMAT_LONG_8 = "MMM. yyyy";               // Jun. 2020
        // t
        public const string TIME_FORMAT_SHORT_1 = "hh:mm tt";               // 06:19 PM
        public const string TIME_FORMAT_SHORT_2 = "HH:mm";                  // 18:19
        // T
        public const string TIME_FORMAT_LONG_1 = "hh:mm:ss tt";             // 06:19:32 PM
        public const string TIME_FORMAT_LONG_2 = "HH:mm:ss";                // 18:19:32

        public const string DATETIME_FORMAT_SHORT_1 = "dd-MMM-yyyy HH:mm";  // 15-Jun-2020 18:19
        public const string DATETIME_FORMAT_SHORT_2 = "MM/dd/yyyy HH:mm";   // 05/15/2020 18:19
        public const string DATETIME_FORMAT_SHORT_3 = "MMddyyyyHHmm";       // 051520201819

        public const string DATETIME_FORMAT_LONG_1 = "dd-MMM-yyyy HH:mm:ss";// 15-Jun-2020 18:19:32
        public const string DATETIME_FORMAT_LONG_2 = "dd-MM-yyyy HH:mm:ss"; // 15-05-2020 18:19:32
        public const string DATETIME_FORMAT_LONG_3 = "MM/dd/yyyy HH:mm:ss"; // 05/15/2020 18:19:32
        public const string DATETIME_FORMAT_LONG_4 = "MMddyyyyHHmmss";      // 05152020181932
    }
}
