using System;
using AspNetCore.ControllerInjection;
using ControllerInjection.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ControllerInjection.Controllers
{
    [ApiController]
    [Route("web")]
    public class WebSiteInfoController : ControllerBase
    {
        private AddressInfo _addressInfo;

        [Inject(true)]
        [InjectOrder(2)]
        private IHttpContextAccessor _accessor;
        
        [HttpGet]
        [Route("ip")]
        public IActionResult GetIP()
        {
            return Content(_addressInfo.GetAddress().ToString(), "text/plain");
        }


        [Inject]
        [InjectOrder(1)]
        private void SetServices([Inject(typeof(IAddressInfo))] AddressInfo addressInfo)
        {
            this._addressInfo = addressInfo;

            if (_accessor != null)
                throw new Exception();
        }

    }
}
