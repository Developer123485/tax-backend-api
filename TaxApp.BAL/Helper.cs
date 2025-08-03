using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;
using System.Reflection;
using static TaxApp.BAL.Models.EnumModel;
using System.Runtime.Serialization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace TaxApp.BAL
{
    public class Helper
    {
        private static IHostingEnvironment _env;
        private static IConfiguration _config;


        public static void SetEnvironmentVariable(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }
        public static string CompanyUserAutovalidatedEmailBody(string tan)
        {
            var templatePath = $"Template/26qTemplate.html";
            string body;
            //using (var sr = new StreamReader(System.Web.HttpContext.Server.MapPath(templatePath)))
            using (var sr = new StreamReader(Path.Combine(_env.WebRootPath, templatePath)))
            {
                body = sr.ReadToEnd();
            }
            return string.Format(body, tan);
        }

        public static string GetEnumMemberValueByDescription<TEnum>(string description)
        {
            var field = typeof(TEnum).GetFields()
                .FirstOrDefault(f => f.GetCustomAttribute<DescriptionAttribute>()?.Description.ToLower().Trim() == description.ToLower().Trim());

            if (field != null)
            {
                var enumMemberAttribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (enumMemberAttribute != null)
                {
                    return enumMemberAttribute.Value; // Return the EnumMember value
                }
            }

            return null;
        }

        public static string GetEnumDescriptionByEnumMemberValue<TEnum>(string enumMemberValue) where TEnum : System.Enum
        {
            if (enumMemberValue != null)
            {
                // Loop through the enum fields to find a matching EnumMember value
                foreach (var field in typeof(TEnum).GetFields())
                {
                    if (field.IsLiteral) // Only consider enum constants
                    {
                        // Get the EnumMember attribute on the enum field
                        var enumMemberAttribute = (EnumMemberAttribute)Attribute.GetCustomAttribute(field, typeof(EnumMemberAttribute));

                        if (enumMemberAttribute != null && enumMemberAttribute.Value == enumMemberValue)
                        {
                            // Get the DescriptionAttribute on the enum field
                            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                            return descriptionAttribute == null ? field.Name : descriptionAttribute.Description;
                        }
                    }
                }
            }
            else
            {
                return "";
            }
            return ""; // Handle invalid value
        }
    }
}
