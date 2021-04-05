using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using VirtoCommerce.Storefront.Infrastructure;
using VirtoCommerce.Storefront.Model;
using VirtoCommerce.Storefront.Model.Common;
using VirtoCommerce.Storefront.Model.Security;

namespace VirtoCommerce.Storefront.Controllers.Omninet
{
    [StorefrontApiRoute("omninet")]
    public class AuthorizationController : StorefrontControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;

        public AuthorizationController(IWorkContextAccessor workContextAccessor, IStorefrontUrlBuilder urlBuilder, IConfiguration configuration, SignInManager<User> signInManager) :
            base(workContextAccessor, urlBuilder)
        {
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            var accessToken = HttpContext.Request.Form[
                _configuration.GetSection("ExternalAuthorizationOptions")["TokenFieldName"]];

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest();
            }

            var claims = ValidateAndGetClaims(accessToken);

            var login = claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            
            var user = !string.IsNullOrWhiteSpace(login) ? await _signInManager.UserManager.FindByNameAsync(login) : null;

            if (user == null)
            {
                return Unauthorized();
            }

            await _signInManager.SignInAsync(user, true);

            WorkContext.CurrentLanguage = new Language(claims.FirstOrDefault(x => x.Type == "lang")?.Value);

            return StoreFrontRedirect("/");
        }

        private IEnumerable<Claim> ValidateAndGetClaims(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                    _configuration.GetSection("ExternalAuthorizationOptions")["SecurityKey"])),
                ValidIssuer = _configuration.GetSection("ExternalAuthorizationOptions")["Issuer"],
                ValidAudience = _configuration.GetSection("ExternalAuthorizationOptions")["Audience"],

                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = true
            };

            handler.ValidateToken(token, validationParameters, out var validToken);

            return (validToken as JwtSecurityToken)?.Claims;
        }
    }
}
