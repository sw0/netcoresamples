using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthExample
{
    public static class AppConsts
    {
        public const string Issuer = "http://localhost:54037/";
        public const string Audience = Issuer;

        public const string Secret = "this_is_a_string_should_not_be_too_short_otherwise_error_occurred";
    }
}
