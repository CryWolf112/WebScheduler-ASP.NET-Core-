using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebScheduler.Models;

namespace WebScheduler
{
    [AttributeUsage(AttributeTargets.Class| AttributeTargets.Method)]
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(string roleType) : base(typeof(AuthorizeAction))
        {
            Arguments = new object[] {
            roleType
        };
        }

        public AuthAttribute(bool guestOnly) : base(typeof(AuthorizeCheckAction))
        {
            Arguments = new object[] {
                guestOnly
            };
        }
    }
}
