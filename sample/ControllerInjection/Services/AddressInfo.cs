using System.Net;
using Microsoft.AspNetCore.Http;

namespace ControllerInjection.Services
{
    public interface IAddressInfo
    {
        IPAddress GetAddress();
    }

    internal class AddressInfo : IAddressInfo
    {
        protected readonly IHttpContextAccessor _accessor;

        public AddressInfo(IHttpContextAccessor accessor)
            => _accessor = accessor;


        public IPAddress GetAddress()
        {
            return _accessor.HttpContext?.Connection.RemoteIpAddress;
        }
    }

    internal class AddressInfo2 : AddressInfo
    {
        public AddressInfo2(IHttpContextAccessor accessor) : base(accessor)
        {
        }

        public void Dummy()
        {

        }

    }
}
