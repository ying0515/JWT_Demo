using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace JWT_Demo.Controllers
{
    public class JWTController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public TokenModel Post(LoginModel login)
        {
            // TODO: 應固定且另外保存
            var secret = JWT_Demo.Properties.Settings.Default.jwt_key;

            // TODO: 應該是和Db比對
            if (login.account == "Tony" && login.password == "123456")
            {
                var payload = new Dictionary<string, object>()
                {
                    { "sub", "ying515.huang@msa.hinet.net" },
                    { "exp", DateTimeOffset.Now.AddSeconds(15).ToUnixTimeSeconds() }
                };

                // 大於 32 個字
                var secretTxt = secret;
                var secretKey = Encoding.UTF8.GetBytes(secretTxt);
                var token = Jose.JWT.Encode(payload, secretKey, Jose.JwsAlgorithm.HS512);

                return new TokenModel
                {
                    result = "Success",
                    token = token
                };
            }
            throw new UnauthorizedAccessException();
        }
    }

    public class LoginModel
    {
        public string account { get; set; }
        public string password { get; set; }
    }

    public class TokenModel
    {
        public string result { get; set; }
        public string token { get; set; }
    }

    public class JwtAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage response;
            // TODO: 應固定且另外保存
            var secretTxt = JWT_Demo.Properties.Settings.Default.jwt_key;
            var secretKey = Encoding.UTF8.GetBytes(secretTxt);

            if (actionContext.Request.Headers.Authorization == null ||
                actionContext.Request.Headers.Authorization.Scheme != "Bearer")
            {

                response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "未含驗證資訊或資訊錯誤！");
                actionContext.Response = response;
            }
            else
            {
                var token = actionContext.Request.Headers.Authorization.Parameter;
                // 取得Json物件
                var jsonObj = Jose.JWT.Decode<JwtAuth>(token, secretKey);

                // 撰寫你的驗證規則
                // 撰寫你的驗證規則
                // 撰寫你的驗證規則

                // 領書：驗證 sub 與 exp 時間是否超時(最快者得)

            }

            base.OnActionExecuting(actionContext);
        }
    }

    public class JwtAuth
    {
        public string sub { get; set; }
        public long exp { get; set; }
    }
}
