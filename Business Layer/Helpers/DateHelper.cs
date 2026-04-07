using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Helpers
{
    public static class DateHelper
    {
        public static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Az önce";

            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} dakika önce";

            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} saat önce";

            if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} gün önce";

            return dateTime.ToString("dd.MM.yyyy");
        }
    }
}
