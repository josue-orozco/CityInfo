using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration configuration;

        // we dont use this outside of this class, so we can scope it to this namespace
        public class AuthenticateRequestBody
        {
            public string? UserName { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string userName, string firstName, string lastName, string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }

        public AuthenticationController(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // username and password will be sent via the request body for security
        // this is because servers save request URL but not the body
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticateRequestBody authenticateRequestBody)
        {
            // Step 1: Validate the username/password
            var user = ValidateUserCredentials(
                authenticateRequestBody.UserName,
                authenticateRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            // Step 2: create a token
            // currently we are using the appsettings.Developer to hold secret but this should
            // be held somewhere safe for production
            // SecretForKey: used to generate the signature
            // Issuer: entity/who creates the token so the API in this case
            // Audience: who will consume token and in this case its the API. string can be any value if preferred
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));


            var jwtSecurityToken = new JwtSecurityToken(
                configuration["Authentication:Issuer"],
                configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? userName, string? password)
        {
            // we dont have a user DB or table. If you have, check the passed-through
            // username/password against whats stored in the database.
            //
            // For demo purposes, we assume the credentials are valid

            //return a new CityInfoUser (values would normally come from your user DB/table)
            return new CityInfoUser(
                1,
                userName ?? "",
                "Kevin",
                "Jenkins",
                "Antwerp");
        }
    }
}
