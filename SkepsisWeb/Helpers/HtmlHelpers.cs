using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SkepsisWeb.Helpers {
    public static class HtmlHelpers {
        public static MvcHtmlString MinutesToHourFormat(this HtmlHelper helper, int totalMinutes, bool includePlusSign = false) {
            string sign = "";
            if (includePlusSign) sign = "+";
            if (totalMinutes < 0) {
                sign = "-";
                totalMinutes = -totalMinutes;
            }

            if (totalMinutes == 0) {
                return MvcHtmlString.Create("0:00");
            } else {
                string hourPart = "0";
                string minPart = "00";
                string xFormat = (((double)totalMinutes) / 60.0).ToString().Replace(',', '.');
                if (xFormat.IndexOf('.') == -1) {
                    hourPart = xFormat;
                } else {
                    string[] parts = xFormat.Split('.');
                    hourPart = parts[0];
                    int leftMin = totalMinutes - (int.Parse(hourPart) * 60);
                    if (leftMin < 10) minPart = "0"; else minPart = "";
                    minPart += leftMin.ToString();
                }
                return MvcHtmlString.Create(sign + hourPart + ":" + minPart);
            }
        }

        /// Extention method. Change line breaks -> br-tag, www -> a href-links, [b] -> bold, [u] -> Underline, [i] -> Italic 
        /// Call like @Html.FormatBody()
        public static MvcHtmlString FormatBody(this HtmlHelper helper, string text, int articleID = 0) {
            if (text == null) return MvcHtmlString.Create("");
            text = ReplaceTags(text);
            if (articleID != 0) text = text.Replace("[ArticleLink]", string.Format("(<a href=\"/Article/Full/{0}\">Continues</a>)", articleID));
            return MvcHtmlString.Create(text.Trim());
        }

        public static string ReplaceTags(string text) {
            text = text.Replace(Environment.NewLine, "<br />");
            text = text.Replace("[b]", "<b>");
            text = text.Replace("[/b]", "</b>");
            text = text.Replace("[i]", "<i>");
            text = text.Replace("[/i]", "</i>");
            text = text.Replace("[u]", "<u>");
            text = text.Replace("[/u]", "</u>");
            text = text.Replace("[h]", "<b>");
            text = text.Replace("[/h]", "</b>");
            text = AddHyperlinks(text);
            text = AddImageLinks(text);
            return text;
        }

        public static string AddHyperlinks(string text) {
            if (text.Trim() == "") return "";
            int oldPos = 0;
            string newText = "";
            text = text.Trim() + " ";
            int imgFound = 0;

            while (true) {
                int newPos1 = text.ToLower().IndexOf("www.", oldPos + imgFound);
                int newPos2 = text.ToLower().IndexOf("http:", oldPos + imgFound);
                int newPos3 = text.ToLower().IndexOf("https:", oldPos + imgFound);
                if (newPos1 == -1 && newPos2 == -1 && newPos3 == -1) {
                    newText += text.Substring(oldPos, text.Length - oldPos);
                    break;
                }

                // Is www or http first
                int newPos;
                if (newPos1 == -1) newPos1 = int.MaxValue;
                if (newPos2 == -1) newPos2 = int.MaxValue;
                if (newPos3 == -1) newPos3 = int.MaxValue;
                if (newPos1 < newPos2) newPos = newPos1; else newPos = newPos2;
                if (newPos3 < newPos) newPos = newPos3;

                // Find that not [url=

                if (newPos < 5 ||
                    (text.Substring(newPos - 5, 5).ToLower() != "[url="
                    && text.Substring(newPos - 5, 5).ToLower() != "[img="
                    && text.Substring(newPos - 5, 5).ToLower() != "ideo="
                    && text.Substring(newPos - 5, 5).ToLower() != "tp://"
                    && text.Substring(newPos - 5, 5).ToLower() != "ps://")
                    ) {

                    // Find the end
                    int endPos = 0;
                    for (int i = newPos; i < text.Length; i++) {
                        if (text.Substring(i, 1) == " " || text.Substring(i, 2) == "\r\n" || text.Substring(i, 1) == ">" || text.Substring(i, 1) == "<" || text.Substring(i, 1) == "[") {
                            endPos = i;
                            break;
                        }
                    }

                    string link = text.Substring(newPos, endPos - newPos);
                    if (!link.ToLower().StartsWith("http:") && !link.ToLower().StartsWith("https:")) {
                        link = "http://" + link;
                    }
                    //linkText = link.Substring(7, link.Length - 7);

                    newText += text.Substring(oldPos, newPos - oldPos);
                    newText += string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", link, link);
                    oldPos = endPos;
                    imgFound = 0;
                } else if (text.Substring(newPos - 5, 5).ToLower() == "[url=") {

                    // Found [url=
                    // Find ]

                    string rest = text.Substring(newPos - 5, text.Length - newPos + 5);
                    string[] parts1 = rest.Split(']'); // Split [/url]
                    int eqPos = parts1[0].IndexOf('=');
                    //string[] parts2 = parts1[0].Split('=');

                    // ] or = not found
                    if (parts1.Length == 1 || eqPos == -1) {
                        newText += text.Substring(oldPos, text.Length - oldPos);
                        break;
                    }

                    string tail = rest.Substring(parts1[0].Length + 1, rest.Length - parts1[0].Length - 1);
                    string url = parts1[0].Substring(eqPos + 1, parts1[0].Length - eqPos - 1);

                    int urlEndPos = tail.ToLower().IndexOf("[/url");
                    if (urlEndPos == -1) {
                        newText += text.Substring(oldPos, text.Length - oldPos);
                        break;
                    }
                    string urlText = tail.Substring(0, urlEndPos);

                    if (!url.ToLower().StartsWith("http:") && !url.ToLower().StartsWith("https:")) url = "http://" + url;

                    newText += text.Substring(oldPos, newPos - oldPos - 5);
                    newText += string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", url, urlText);

                    oldPos = text.ToLower().IndexOf("[/url]", newPos) + 6;
                    imgFound = 0;
                } else {
                    newText += text.Substring(oldPos, newPos - oldPos);
                    oldPos = newPos;
                    imgFound = 1;
                }
            }

            return newText.Trim();
        }

        public static string AddImageLinks(string text) {
            if (text.Trim() == "") return "";

            int oldPos = 0;
            string newText = "";
            text = text.Trim() + " ";

            while (true) {
                int newPos = text.ToLower().IndexOf("[img=", oldPos);
                if (newPos == -1) {
                    newText += text.Substring(oldPos, text.Length - oldPos);
                    break;
                }

                string rest = text.Substring(newPos, text.Length - newPos);
                string[] parts1 = rest.Split(']');
                string[] parts2 = parts1[0].Split('=');

                // ] or = not found
                if (parts1.Length == 1 || parts2.Length == 1) {
                    newText += text.Substring(oldPos, text.Length - oldPos);
                    break;
                }

                string url = parts2[1];

                int extra = 0;
                if (!url.ToLower().StartsWith("http:") && !url.ToLower().StartsWith("https:")) {
                    url = "http://" + url;
                    extra = 7;
                }
                newText += text.Substring(oldPos, newPos - oldPos);
                newText += string.Format("<a href=\"{0}\"><img src=\"{0}\" style=\"border:none;max-width:620px;max-height:500px;\" alt=\"\"></a>", url);

                oldPos = newPos + 6 + url.Length - extra;
            }
            return newText.Trim();
        }

        public static MvcHtmlString GetTimeAndUserSection(this HtmlHelper helper, string creator, string userName, DateTime created) {
            string text = getFeedDateFormat(created);
            text += " ago";
            if (creator != "") {
                text += " by ";
                text += string.Format("<a href=\"/User/Info/{0}\">{1}</a>", userName, creator);
            }

            return MvcHtmlString.Create(text.Trim());
        }

        // Return in xx min or xx hour since UtcNow
        public static string GetFeedDateFormat(this HtmlHelper helper, DateTime datetime) {
            return getFeedDateFormat(datetime);
        }

        public static string GetFeedDateFormatFuture(this HtmlHelper helper, DateTime datetime) {
            string res = getFeedDateFormat(datetime, true);
            if (res == "Today") return res;
            if (datetime < DateTime.UtcNow) {
                return res + " ago";
            } else {
                return "In " + res;
            }
        }

        // Return in xx min or xx hour since UtcNow
        public static string getFeedDateFormat(DateTime datetime, bool onlyDays = false) {
            string result;

            if (onlyDays) {
                if (datetime.Day == DateTime.UtcNow.Day && datetime.Month == DateTime.UtcNow.Month && datetime.Year == DateTime.UtcNow.Year) return "Today";
            }
            int minAgo = (int)Math.Abs((DateTime.UtcNow - datetime).TotalMinutes);
            if (minAgo < 60 && !onlyDays) {
                result = minAgo.ToString() + " min";
            } else {
                if (minAgo < 120 && !onlyDays) {
                    result = "1 hour";
                } else {
                    int hours = (int)Math.Abs((DateTime.UtcNow - datetime).TotalHours);
                    if (hours < 24 && !onlyDays) {
                        result = hours.ToString() + " hours";
                    } else {
                        int days = (int)Math.Round(Math.Abs((DateTime.UtcNow - datetime).TotalDays), 0);
                        if (days <= 13) {
                            if (days == 1)
                                result = days.ToString() + " day";
                            else
                                result = days.ToString() + " days";
                        } else if (days <= 40) {
                            double weeks = Math.Round((double)days / 7, 0);
                            if (weeks == 1)
                                result = weeks.ToString() + " week";
                            else
                                result = weeks.ToString() + " weeks";
                        } else {
                            double months = Math.Round((double)days / 30, 0);
                            if (months == 1)
                                result = months.ToString() + " month";
                            else
                                result = months.ToString() + " months";
                        }
                    }
                }
            }
            return result;
        }



        public static void AddCookie(HttpContextBase httpContextBase, string cookieName, string cookieValue) {
            cookieValue = HttpUtility.UrlEncode(cookieValue);
            HttpCookie cookie = new HttpCookie(cookieName);
            cookie.Value = cookieValue;
            cookie.Expires = DateTime.UtcNow.AddYears(10);
            httpContextBase.Response.Cookies.Add(cookie);
        }

        public static void RemoveCookie(HttpContextBase httpContextBase, string cookieName) {
            if (httpContextBase.Request.Cookies.AllKeys.Contains(cookieName)) {
                HttpCookie cookie = httpContextBase.Request.Cookies[cookieName];
                cookie.Value = null;
                cookie.Expires = new DateTime(2000, 1, 1);
                httpContextBase.Response.Cookies.Add(cookie);
            }
        }

        public static string GetCookieValue(HttpContextBase httpContextBase, string cookieName) {
            if (httpContextBase.Request.Cookies.AllKeys.Contains(cookieName)) {
                string encoded = httpContextBase.Request.Cookies[cookieName].Value;
                return HttpUtility.UrlDecode(encoded);
            }
            return "";
        }
    }
}