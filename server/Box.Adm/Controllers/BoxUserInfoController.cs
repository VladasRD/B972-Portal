using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace Box.Adm.Controllers
{

    /// <summary>
    /// Controller to provide user profile picture.
    /// </summary>
    public class BoxUserInfoController : Controller
    {

        private readonly Box.Security.Services.SecurityService _securityService;
        private readonly IConfiguration _configuration;

        public BoxUserInfoController(Box.Security.Services.SecurityService securityService, IConfiguration configuration)
        {                    
            _securityService = securityService;
            _configuration = configuration;
        }        

        [ResponseCache(VaryByQueryKeys = new string[] { "*" }, Duration = 2592000, Location = ResponseCacheLocation.Any)]    
        [Authorize]
        public FileResult Photo(string id)
        {
            var user = _securityService.GetUser(id, true);

            if (user == null)
                return new FileContentResult(GetColorAvatar(""), "image/png");
            
            // tries to get Gravatar
            var result = GetGravatar(user.Email);
            if(result!=null)
                return result;
            
            // returns color avatar
            var name = user.UserName;
            var clain_name = user.UserClaims.SingleOrDefault(c => c.ClaimType == "given_name");
            if (clain_name!=null) 
                name = clain_name.ClaimValue;
            return new FileContentResult(GetColorAvatar(name), "image/png");
            
        }

        #region GRAVATAR

        /// <summary>
        /// Gets user Gravatar.
        /// </summary>
        /// <param name="email">User´s email</param>
        /// <returns>The user gravatar</returns>
        private FileStreamResult GetGravatar(string email) {                        
            FileStreamResult result = null;

            try
            {
                HttpClient wc = new HttpClient();                        
                System.Net.Http.HttpResponseMessage msg = wc.GetAsync($"http://www.gravatar.com/avatar/{GravatarHashEmail(email)}.jpg?s=120&d=404").Result;
                if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    result = new FileStreamResult(msg.Content.ReadAsStreamAsync().Result, "image/jpeg");                
                }
            }
            catch (Exception) { }            
            return result;
        }

        private string GravatarHashEmail(string email)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(email));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }
    
        #endregion

        #region COLOR AVATAR

        /// <summary>
        /// Gets a color avatar based on user's initials.
        /// </summary>
        /// <param name="name">User´s name</param>
        /// <returns>The user color avatar</returns>
        public byte[] GetColorAvatar(string name) {
        
            var initials = GetInitials(name);            
            var bg = GetAvatarBG(initials);
            var font = new System.Drawing.Font(System.Drawing.FontFamily.GenericSansSerif, 22);

            System.Drawing.Bitmap img = new System.Drawing.Bitmap(60, 60);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(img);            
            g.FillRectangle(bg, 0, 0, 60, 60);

            var size = g.MeasureString(initials, font);

            g.DrawString(initials, 
                font,
                System.Drawing.Brushes.White,
                (float) 30 - (size.Width/2),
                (float) 30 - (size.Height/2));

            g.Dispose();

            return ImageToBytes(img);
        }

        private string GetInitials(string name) {
            if(string.IsNullOrEmpty(name))
                return "?";
            var parts = name.Split(' ');
            if (parts.Length==1)
                return name.Substring(0, 1).ToUpper();

            return parts.First().Substring(0, 1).ToUpper() + parts.Last().Substring(0, 1).ToUpper(); 
        }

        private string[] bgColors = {
            "#EEAD0E", "#8bbf61", "#DC143C", "#CD6889", "#8B8386", "#800080", "#8E8E38", "#7171C6",  
            "#9932CC", "#009ACD", "#00CED1", "#03A89E", "#00C78C", "#00CD66", "#66CD00", "#EEB422", 
            "#FF8C00", "#EE4000", "#388E8E" 
        };

        private System.Drawing.Brush GetAvatarBG(string initials) {
            int sum = (int) initials[0];
            if (initials.Length>1)
                sum = sum + (int) initials[1];            
            return new System.Drawing.SolidBrush(System.Drawing.ColorTranslator.FromHtml( bgColors[sum % bgColors.Length]));
        }

        private byte[] ImageToBytes(System.Drawing.Image img) {
            var imgType = System.Drawing.Imaging.ImageFormat.Png;        
            byte[] byteArray = new byte[0];
            using (var stream = new System.IO.MemoryStream()) {
                img.Save(stream, imgType);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }



        #endregion

    }
}