﻿using MediatR;
using Shared.Proto;

namespace DeviceManager.Api.Queries
{
    public class GetDeviceByNameQuery : IRequest<DeviceExtendedDto>
    {
        public GetDeviceByNameRequest QueryParameters { get; }

        public GetDeviceByNameQuery(GetDeviceByNameRequest queryParameters)
        {
            QueryParameters = queryParameters;
        }
    }
}