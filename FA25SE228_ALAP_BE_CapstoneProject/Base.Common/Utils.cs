using AutoMapper;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using TFU.Common.Extension;
using System.Globalization;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Base.Common
{
    public static class Utils
    {
        /// <summary>
        /// Formats the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public static string FormatCurrency(this double currency)
        {
            if (currency == 0)
                return "Miễn phí";
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            return currency.ToString("#,###", cul.NumberFormat);
        }

        /// <summary>
        /// Formats the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public static string FormatCurrency(this decimal currency, bool extends = true)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            var stringReturn = currency.ToString("#,###", cul.NumberFormat);
            if (extends) stringReturn += "đ";
            return stringReturn;
        }

        /// <summary>
        /// Gets the youtube identifier.
        /// </summary>
        /// <param name="youtubeUrl">The youtube URL.</param>
        /// <returns></returns>
        public static string GetYoutubeID(this string youtubeUrl)
        {
            if (string.IsNullOrEmpty(youtubeUrl))
                return "a4IZxku8N7w";
            return Regex.Match(youtubeUrl, "https?:\\/\\/(?:[0-9A-Z-]+\\.)?(?:youtu\\.be\\/|youtube(?:-nocookie)?\\.com\\S*[^\\w\\s-])([\\w-]{11})(?=[^\\w-]|$)(?![?=&+%\\w.-]*(?:['\"][^<>]*>|<\\/a>))[?=&+%\\w.-]*",
          RegexOptions.IgnoreCase).Groups[1].Value;

        }

        /// <summary>
        /// Converts to camelcase.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string ToCamelCase(this string str)
        {
            string lower = str.ToLower();
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (i == 0)
                    stringBuilder.Append(lower[i]);
                else
                    stringBuilder.Append(str[i]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the friendly title.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="remapToAscii">if set to <c>true</c> [remap to ASCII].</param>
        /// <param name="maxlength">The maxlength.</param>
        /// <returns></returns>
        public static string GetFriendlyTitle(string title, bool remapToAscii = true, int maxlength = 80)
        {
            if (string.IsNullOrEmpty(title))
            {
                return string.Empty;
            }

            int length = title.Length;
            bool prevdash = false;
            StringBuilder stringBuilder = new StringBuilder(length);
            char c;

            for (int i = 0; i < length; ++i)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    stringBuilder.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lower-case
                    stringBuilder.Append((char)(c | 32));
                    prevdash = false;
                }
                else if ((c == ' ') || (c == ',') || (c == '.') || (c == '/') ||
                     (c == '\\') || (c == '-') || (c == '_') || (c == '='))
                {
                    if (!prevdash && (stringBuilder.Length > 0))
                    {
                        stringBuilder.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    int previousLength = stringBuilder.Length;

                    if (remapToAscii)
                    {
                        stringBuilder.Append(RemapInternationalCharToAscii(c));
                    }
                    else
                    {
                        stringBuilder.Append(c);
                    }

                    if (previousLength != stringBuilder.Length)
                    {
                        prevdash = false;
                    }
                }

                if (i == maxlength)
                {
                    break;
                }
            }

            if (prevdash)
            {
                return stringBuilder.ToString().Substring(0, stringBuilder.Length - 1);
            }
            else
            {
                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Serializes the XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj">The object.</param>
        /// <returns></returns> 
        /// <CreatedDate>25/08/2019</CreatedDate>
        public static string SerializeXML<T>(T Obj, bool isRequireHead = false)
        {

            XmlSerializer ser;
            ser = new XmlSerializer(Obj.GetType());
            MemoryStream memStream;
            memStream = new MemoryStream();
            XmlTextWriter xmlWriter;
            xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8);
            xmlWriter.Namespaces = true;
            ser.Serialize(xmlWriter, Obj);
            xmlWriter.Close();
            memStream.Close();
            string xml;
            xml = Encoding.UTF8.GetString(memStream.GetBuffer());
            xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));

            xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
            return isRequireHead ? xml : xml.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "")
                 .Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "")
                 .Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
        }

        public static T DeserializeXML<T>(Stream fileStream)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T obj = (T)xmlSerializer.Deserialize(fileStream);
            return obj;
        }

        /// <summary>
        /// Mappings the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TDestination MappingWithIgnore<TSource, TDestination>(TSource source,
     params Expression<Func<TDestination, object>>[] selectors)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>().Ignore(selectors));
            var mapper = new Mapper(config);
            return mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Mappings the specified source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TDestination Mapping<TSource, TDestination>(TSource source)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>().IgnoreNoMap());
            var mapper = new Mapper(config);
            return mapper.Map<TDestination>(source);
        }

        /// <summary>
        /// Mappings the list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static List<TDestination> MappingList<TSource, TDestination>(List<TSource> source)
        {
            List<TDestination> destinations = new List<TDestination>();
            foreach (var item in source)
            {
                destinations.Add(Mapping<TSource, TDestination>(item));
            }
            return destinations;
        }

        /// <summary>
        /// Mappings the list.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static List<TDestination> MappingListWithIgnore<TSource, TDestination>(List<TSource> source,
        params Expression<Func<TDestination, object>>[] selector)
        {
            List<TDestination> destinations = new List<TDestination>();
            foreach (var item in source)
            {
                destinations.Add(MappingWithIgnore(item, selector));
            }
            return destinations;
        }

        /// <summary>
        /// Deserializes the iframe live stream.
        /// </summary>
        /// <param name="iframeUrl">The iframe URL.</param>
        /// <param name="pageId">The page identifier.</param>
        /// <param name="postId">The post identifier.</param>
        public static void DeserializeIframeLiveStream(string iframeUrl, out long pageId, out long postId)
        {
            pageId = 0;
            postId = 0;
            if (!string.IsNullOrEmpty(iframeUrl))
            {
                string url = System.Web.HttpUtility.UrlDecode(iframeUrl);
                // Define a regular expression for repeated words.
                Regex rx = new Regex(@"\d+",
                  RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(url);
                // Report on each match.
                if (matches.Count > 1)
                {
                    long.TryParse(matches[0].Value, out pageId);
                    long.TryParse(matches[1].Value, out postId);
                }
            }
        }

        /// <summary>
        /// Masks the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string MaskEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                string[] matches = email.Split('@');
                // Report on each match.
                if (matches.Length > 1)
                {
                    string emailName = matches[0];
                    if (emailName.Length > 1)
                        //return string.Format("{0}xxxxxx@{1}", emailName.Substring(0, 3), matches[1]);
                        return string.Format("{0}*****@{1}", emailName.Substring(0, 1), matches[1]);
                    else
                        return string.Format("{0}*****@{1}", matches[0], matches[1]);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Masks the identifier.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public static string MaskId(string Id)
        {
            if (string.IsNullOrEmpty(Id) || Id.Length <= 3)
                return string.Empty;
            return string.Format("xxxxxx{0}", Id.Substring(Id.Length - 3, 3));
        }

        /// <summary>
        /// generate mã số dự thưởng
        /// </summary>
        /// <param name="code">mã số dự thưởng</param>
        /// <param name="numberSlot">số ô quay thưởng</param>
        /// <returns></returns>
        public static string GenerateCodeWinner(string code, int numberSlot)
        {
            var output = code;
            if (!string.IsNullOrEmpty(output))
            {
                if (output.Trim().Length < numberSlot)
                {
                    output = output.PadLeft(numberSlot, '0');
                }
            }
            return output;
        }

        public static string GenerateHyperlink(string hyperlink)
        {
            var link = hyperlink;
            if (!string.IsNullOrEmpty(link))
            {
                if (!link.StartsWith("http"))
                {
                    link = "http://www." + link.Replace("www.", "");
                }
            }
            return link;
        }

        public static string GenerateAvatarFb(string facebookId)
        {
            var avatar = string.Empty;
            if (!string.IsNullOrEmpty(facebookId))
            {
                avatar = "http://graph.facebook.com/" + facebookId + "/picture?type=square";
            }
            return avatar;
        }



        //DateTime with format: dd/MM/yyyy HH:mm
        public static DateTime StringToDateTime(string datetimeStr)
        {
            var datetime = new DateTime();
            if (!string.IsNullOrEmpty(datetimeStr))
                datetime = DateTime.ParseExact(datetimeStr, Constants.FormatDateTime, new CultureInfo("vi-VN"));
            return datetime;
        }

        public static string DateTimeToString(DateTime datetime)
        {
            var datetimeStr = string.Empty;
            if (datetime != null)
                datetimeStr = datetime.ToString(Constants.FormatDateTime);
            return datetimeStr;
        }

        //FullDateTime with format: dd/MM/yyyy HH:mm:ss
        public static DateTime StringToFullDateTime(string fulldatetimeStr)
        {
            var fulldatetime = new DateTime();
            if (!string.IsNullOrEmpty(fulldatetimeStr))
                fulldatetime = DateTime.ParseExact(fulldatetimeStr, Constants.FormatFullDateTime, new CultureInfo("vi-VN"));
            return fulldatetime;
        }

        /// <summary>
        /// Convert date time to string with special fomat: dd/MM/yyyy HH:mm:ss
        /// </summary>
        /// <param name="fulldatetime"></param>
        /// <returns></returns>
        public static string FullDateTimeToString(DateTime fulldatetime)
        {
            var fulldatetimeStr = string.Empty;
            if (fulldatetime != null)
                fulldatetimeStr = fulldatetime.ToString(Constants.FormatFullDateTime);
            return fulldatetimeStr;
        }

        /// <summary>
        /// tính thời gian trước
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string TimeAgo(DateTime dt)
        {
            var span = DateTime.Now - Convert.ToDateTime(dt);
            if (span.Days > 365)
            {
                var years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format(" {0} {1} trước",
                years, "năm");
            }
            if (span.Days > 30)
            {
                var months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format(" {0} {1} trước",
                months, "tháng");
            }
            if (span.Days > 0)
                return String.Format(" {0} {1} trước",
                span.Days, "ngày");
            if (span.Hours > 0)
                return String.Format(" {0} {1} trước",
                span.Hours, "giờ");
            if (span.Minutes > 0)
                return String.Format(" {0} {1} trước",
                span.Minutes, "phút");
            if (span.Seconds > 5)
                return String.Format(" {0} giây trước", span.Seconds);
            return span.Seconds <= 5 ? "vừa xong" : string.Empty;
        }

        public static void DetectPageFacebookId(string pageUrl, out string pageIdOrName)
        {
            pageIdOrName = string.Empty;
            if (!string.IsNullOrEmpty(pageUrl))
            {
                string url = System.Web.HttpUtility.UrlDecode(pageUrl);
                var arrUrl = url.Split('/');
                if (arrUrl.Length > 2)
                {
                    var pageInfo = arrUrl[3];
                    long idPage = 0;
                    Regex rx = new Regex(@"\d+",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    MatchCollection matches = rx.Matches(url);
                    // Report on each match.
                    if (matches.Count >= 1)
                    {
                        idPage = long.Parse(matches[0].Value);
                    }
                    if (pageInfo.Contains("-" + idPage))
                    {
                        pageIdOrName = idPage.ToString();
                    }
                    else
                    {
                        pageIdOrName = pageInfo;
                    }
                }

                // Define a regular expression for repeated words.
                //        Regex rx = new Regex(@"\d+",
                //          RegexOptions.Compiled | RegexOptions.IgnoreCase);
                //        MatchCollection matches = rx.Matches(url);
                //        // Report on each match.
                //        if (matches.Count >= 1)
                //        { 
                //	pageId = matches[0].Value; 
                //}
            }
        }

        public static string GenerateAvatarName(string name, string email = "")
        {
            string avatar = string.Empty;
            if (!string.IsNullOrWhiteSpace(name))
            {
                var arr = name.Trim().Split(' ');
                if (arr != null && arr.Length > 0)
                {
                    if (arr.Length == 1)
                    {
                        avatar = arr[0].Substring(0, 1);
                    }
                    else
                    {
                        var temp1 = arr[arr.Length - 2].Substring(0, 1);
                        var temp2 = arr[arr.Length - 1].Substring(0, 1);
                        avatar = temp1 + temp2;
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(email))
            {
                avatar = email.Substring(0, 1);
            }
            return avatar;
        }


        /// <summary>
        /// Chuyển đổi chữ tiếng việt có dấu sang không dấu
        /// </summary>
        /// <returns>Trả lại một chuỗi không dấu</returns>
        public static string UnsignCharacter(string text)
        {
            var pattern = new string[7];

            pattern[0] = "a|(á|ả|à|ạ|ã|ă|ắ|ẳ|ằ|ặ|ẵ|â|ấ|ẩ|ầ|ậ|ẫ)";

            pattern[1] = "o|(ó|ỏ|ò|ọ|õ|ô|ố|ổ|ồ|ộ|ỗ|ơ|ớ|ở|ờ|ợ|ỡ)";

            pattern[2] = "e|(é|è|ẻ|ẹ|ẽ|ê|ế|ề|ể|ệ|ễ)";

            pattern[3] = "u|(ú|ù|ủ|ụ|ũ|ư|ứ|ừ|ử|ự|ữ)";

            pattern[4] = "i|(í|ì|ỉ|ị|ĩ)";

            pattern[5] = "y|(ý|ỳ|ỷ|ỵ|ỹ)";

            pattern[6] = "d|(đ|Ð|Đ)";

            for (int i = 0; i < pattern.Length; i++)
            {

                // kí tự sẽ thay thế

                char replaceChar = pattern[i][0];

                MatchCollection matchs = Regex.Matches(text, pattern[i]);

                foreach (Match m in matchs)
                {

                    text = text.Replace(m.Value[0], replaceChar);

                }

            }

            return text;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceMultiSpace(string input, string newStr = " ")
        {
            const RegexOptions options = RegexOptions.None;
            var regex = new Regex("[ ]{2,}", options);
            return regex.Replace(input, newStr);
        }


        /// <summary>
        /// Format the address
        /// </summary>
        /// <param name="street"></param>
        /// <param name="ward"></param>
        /// <param name="district"></param>
        /// <param name="province"></param>
        /// <returns></returns>
        public static string FormatAddress(string street, string ward, string district, string province)
        {
            var listAddress = new List<string>();
            if (!string.IsNullOrEmpty(street))
                listAddress.Add(street);
            if (!string.IsNullOrEmpty(ward))
                listAddress.Add(ward);
            if (!string.IsNullOrEmpty(district))
                listAddress.Add(district);
            if (!string.IsNullOrEmpty(province))
                listAddress.Add(province);
            if (listAddress.Count == 0)
                return string.Empty;
            return string.Join(", ", listAddress);
        }

        public static string DecodeVerifyEmailToken(string token, IConfiguration configuration)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = configuration["JwtSettings:Secret"]!;
            var key = Encoding.UTF8.GetBytes(secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Subject;
            }
            catch
            {
                return null;
            }
        }


        public static string GenerateOrderCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);

            return $"TXN{last9Digits}";
        }

        public static string GenerateCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);

            return last9Digits;
        }

        public static string GenerateEventCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);
            return $"EVT{last9Digits}";
        }

        public static string GenerateLinkCode()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string last9Digits = timestamp.ToString().Substring(4, 9);
            return $"EVR{last9Digits}";
        }

        public static string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static DateTime ConvertToVietnamTime(DateTime utcDateTime)
        {
            TimeZoneInfo vietNamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, vietNamTimeZone);
        }


        public static DateTime GetCurrentVNTime()
        {
            DateTime utcNow = DateTime.UtcNow;
            DateTime vnTime = ConvertToVietnamTime(utcNow);
            return vnTime;
        }

        /// <summary>
        /// Convert file path to IFormFile    
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static IFormFile ConvertToIFormFile(string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            var stream = new MemoryStream(fileBytes);

            IFormFile file = new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return file;
        }

        /// <summary>
        /// Get current date time in Vietnam timezone
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            DateTime utcNow = DateTime.UtcNow.AddHours(7);
            return utcNow;
        }

        /// <summary>
        /// Get current date time as string with format yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetCurrentDateTimeAsString(string format = "yyyy-MM-dd HH:mm")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        /// <summary>
        /// Get current date as string with format yyyy-MM-dd
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetCurrentDateAsString(string format = "yyyy-MM-dd")
        {
            DateTime vietnamTime = GetCurrentDateTime();
            return vietnamTime.ToString(format);
        }

        /// <summary>
        /// Serialize the object to json
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeObjectToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        /// <summary>
        /// Deserialize the json to object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeJsonToObject<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Generate a token for email verification
        /// </summary>
        /// <param name="_configuration"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GenerateVerifyEmailToken(IConfiguration _configuration, string email)
        {
            var claim = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),

                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (_configuration == null)
            {
                Console.WriteLine("Configuration is null");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claim,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        public static string GenerateRefreshToken(int length = 64)
        {
            var randomNumber = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }


        /// <summary>
        /// Lấy tên đăng nhập từ email (phần trước dấu @)
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static string GetUserNameFromEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return $"eventz_{Guid.NewGuid().ToString().Substring(0, 4)}@gmail.com";
            ;
            var arr = email.Trim().Split('@');
            if (arr != null && arr.Length > 0)
            {
                return arr[0];
            }
            return string.Empty;
        }


        /// <summary>
        /// Check xem có phải admin không qua Email 
        /// </summary>
        /// <param name="provideEmail"></param>
        /// <returns></returns>
        public static bool IsAdmin(string provideEmail)
        {
            return provideEmail.Equals("henry@eventz.vn", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Hàm để kiểm tra xem user có quyền thực thi 1 function trong repository không 
        /// Truyền vào email của entity và email của user đang get dữ liệu 
        /// </summary>
        /// <param name="originEmail"></param>
        /// <param name="provideEmail"></param>
        /// <returns></returns>
        public static bool HasPermisstionQuery(string originEmail, string provideEmail)
        {
            return originEmail.Equals(provideEmail, StringComparison.OrdinalIgnoreCase) || IsAdmin(provideEmail);
        }


        /// <summary>
        /// Get the first name and last name from full name
        /// 
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static (string FirstName, string LastName) SplitName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                return (string.Empty, string.Empty);

            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return (parts[0], string.Empty);

            var firstName = parts[0];
            var lastName = string.Join(' ', parts.Skip(1));

            return (firstName, lastName);
        }

        #region Private Function
        /// <summary>
        /// Remaps the international character to their equivalent ASCII characters. See
        /// http://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
        /// </summary>
        /// <param name="character">The character to remap to its ASCII equivalent.</param>
        /// <returns>The remapped character</returns>
        private static string RemapInternationalCharToAscii(char character)
        {
            string s = character.ToString().ToLowerInvariant();
            if ("àåáâäãåąā".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (character == 'ř')
            {
                return "r";
            }
            else if (character == 'ł')
            {
                return "l";
            }
            else if (character == 'đ')
            {
                return "d";
            }
            else if (character == 'ß')
            {
                return "ss";
            }
            else if (character == 'Þ')
            {
                return "th";
            }
            else if (character == 'ĥ')
            {
                return "h";
            }
            else if (character == 'ĵ')
            {
                return "j";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
