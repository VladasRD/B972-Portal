using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Box.Common.Web;
using SmartGeoIot.Models;
using SmartGeoIot.Extensions;
using SmartGeoIot.ViewModels;
using System.Threading.Tasks;
using Box.Security.Models;

namespace SmartGeoIot.Api
{
    [Route("api/[controller]")]
    public class SGIClientController : Controller
    {
        protected readonly SmartGeoIot.Services.SmartGeoIotService _sgiService;
        private readonly Box.Security.Services.SecurityService _securityService;
        public SGIClientController(SmartGeoIot.Services.SmartGeoIotService sgiService, Box.Security.Services.SecurityService securityService)
        {
            _sgiService = sgiService;
            _securityService = securityService;
        }
        
        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.READ, SGI-SUBCLIENT.READ")]
        public IEnumerable<Client> Get([FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] string filter = null, [FromQuery] bool? statusClient = null, [FromQuery] bool isSubClient = false)
        {
            var totalCount = new OptionalOutTotalCount();

            if(User.IsInRole("SGI-SUBCLIENT.READ") && !User.IsInRole("SGI-CLIENT.READ") && !isSubClient)
                throw new Box.Common.BoxLogicException("Usuário não tem permissão para acessar está funcionalidade.");

            var clients = _sgiService.GetClients(User, skip, top, filter, statusClient, isSubClient, totalCount);
            Request.SetListTotalCount(totalCount.Value);

            return clients;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.READ, SGI-SUBCLIENT.READ")]
        public Client Get(string id)
        {
            var client = _sgiService.GetClient(id);
            // if(!User.CanAccessClient(client.ClientUId))
            //     throw new Box.Common.BoxLogicException("Usuário não tem permissão para acessar o cliente.");

            return client;                
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.WRITE, SGI-SUBCLIENT.WRITE")]
        public void Delete(string id)
        {
            Get(id); // just to verify permission
            _sgiService.DisableClient(id);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.WRITE, SGI-SUBCLIENT.WRITE")]
        public Client Post([FromBody] Client client, [FromQuery] bool isSubClient = false)
        {
            if (client == null)
                throw new Box.Common.BoxLogicException("Cliente inválido.");
                
            client.ClientUId = Guid.NewGuid().ToString();

            if(User.IsInRole("SGI-SUBCLIENT.READ") && !User.IsInRole("SGI-CLIENT.READ") && !isSubClient)
                throw new Box.Common.BoxLogicException("Usuário não tem permissão para acessar está funcionalidade.");

            return this._sgiService.SaveClient(client, User, isSubClient);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.WRITE, SGI-SUBCLIENT.WRITE")]
        public Client Put(string id, [FromBody] Client client, [FromQuery] bool isSubClient = false)
        {
            if (client == null)
                throw new Box.Common.BoxLogicException("Cliente inválido.");

            if (id != client.ClientUId)
                throw new Box.Common.BoxLogicException("Id inválido.");

            if(User.IsInRole("SGI-SUBCLIENT.READ") && !User.IsInRole("SGI-CLIENT.READ") && !isSubClient)
                throw new Box.Common.BoxLogicException("Usuário não tem permissão para acessar está funcionalidade.");

            return this._sgiService.SaveClient(client, User, isSubClient);
        }

        [HttpPost("addUserClient/{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.WRITE, SGI-SUBCLIENT.WRITE")]
        public async Task<ApplicationUser> addUserClient(string id, [FromBody] ApplicationUser user)
        {
            // verify ir exist user
            var existUser = await _securityService.GetUserByEmail(user.Email);
            user.EmailConfirmed = true;

            if (existUser != null)
            {
                if (existUser.IsLocked)
                    throw new Box.Common.BoxLogicException("Usuário está bloqueado no sistema.");

                user.Id = existUser.Id;
                foreach(var role in user.UserRoles)
                {
                    role.UserId = user.Id;
                }
                foreach(var claim in user.UserClaims)
                {
                    claim.UserId = user.Id;
                }
                user = await _securityService.UpdateUser(user);
            }
            else
                user = await _securityService.CreateUser(user);

            // vincula o usuário ao cliente
            _sgiService.AddClientUser(id, user);
            
            await _securityService.SendDefaultUserCreatedEmail(user);
            return user;
        }

        [HttpPost("removeUserClient")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-SUBCLIENT.WRITE")]
        public void removeUserClient([FromBody] ClientUser clientUser)
        {
            // removendo vinculo do usuário com cliente
            _sgiService.RemoveClientUser(clientUser);

            // await _securityService.DeleteUser(clientUser.ApplicationUserId);

            // bloqueando o usuário, a princípio não vamos remover do sistema para termos um histórico, no futuro podemos analisar de remover
            _securityService.LockUser(clientUser.ApplicationUserId);
        }
    }
}