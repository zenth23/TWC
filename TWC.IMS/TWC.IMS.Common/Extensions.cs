using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class Extensions
    {
        private static readonly IDictionary<Type, ICollection<PropertyInfo>> _properties = new Dictionary<Type, ICollection<PropertyInfo>>();

        public static string ToTimeAgo(this DateTime date)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int WEEK = 7 * DAY;
            const int MONTH = 4 * WEEK;
            const int YEAR = 365 * DAY;

            DateTime now = DateTime.Now;    // server time
            TimeSpan dateDiff = new TimeSpan(now.Ticks - date.Ticks);
            double dateDiffInSec = Math.Abs(dateDiff.TotalSeconds);

            if ((dateDiffInSec / MINUTE) < 1)
            {
                if (dateDiff.Seconds <= 0)
                    return "Just now";
                else
                    return dateDiff.Seconds == 1 ? "1 second ago" : $"{dateDiff.Seconds} seconds ago";
            }
            else if ((dateDiffInSec / MINUTE) < 60)
            {
                return dateDiff.Minutes == 1 ? "1 minute ago" : $"{dateDiff.Minutes} minutes ago";
            }
            else if ((dateDiffInSec / HOUR) < 24)
            {
                return dateDiff.Hours == 1 ? "1 hour ago" : $"{dateDiff.Hours} hours ago";
            }
            //else if day
            //1d ago
            else if ((dateDiffInSec / DAY) < 7)
            {
                return dateDiff.Days == 1 ? "1 day ago" : $"{dateDiff.Days} days ago";
            }
            //week
            else if ((dateDiffInSec / WEEK) < 4)
            {
                var week = dateDiff.Days / 7;

                return week == 1 ? "1 week ago" : $"{week} weeks ago";
            }
            //month
            else if ((dateDiffInSec / MONTH) < 12)
            {
                var month = dateDiff.Days / 30;

                return month == 1 ? "1 month ago" : $"{month} months ago";
            }
            //else if year
            //1y ago, 2y ago
            else if ((dateDiffInSec / YEAR) >= 1)
            {
                var year = dateDiff.Days / 365;

                return year == 1 ? "1 year ago" : $"{year} years ago";
            }
            else
            {
                return date.ToString(TWC.IMS.Common.StringFormats.DATETIME_FORMAT_SHORT_2);
            }
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> e, int startPosition, int buffer)
        {
            if (e == null || !e.Any())
                return e;
            else
                return e.Skip(startPosition).Take(buffer);
        }

        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        public static string Singularize(this string pluralText)
        {
            var svc = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            if (svc.IsPlural(pluralText))
                return svc.Singularize(pluralText);
            return pluralText;
        }

        public static string Pluralize(this string singularext)
        {
            var svc = System.Data.Entity.Design.PluralizationServices.PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            if (svc.IsSingular(singularext))
                return svc.Pluralize(singularext);
            return singularext;
        }

        public static IEnumerable<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                var objType = typeof(T);
                ICollection<PropertyInfo> properties;

                lock (_properties)
                {
                    if (!_properties.TryGetValue(objType, out properties))
                    {
                        properties = objType.GetProperties().Where(property => property.CanWrite).ToList();
                        _properties.Add(objType, properties);
                    }
                }

                var list = new List<T>(table.Rows.Count);
                foreach (var row in table.AsEnumerable().Skip(1))
                {
                    var obj = new T();

                    foreach (var prop in properties)
                    {
                        try
                        {
                            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            var safeValue = row[prop.Name] == null ? null : Convert.ChangeType(row[prop.Name], propType);

                            prop.SetValue(obj, safeValue, null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return Enumerable.Empty<T>();
            }
        }
    }
}
