﻿using MediatR;
using Shared.Proto;

namespace DeviceManager.Api.Queries
{
    public class GetAllSensorTypesQuery : IRequest<GetAllSensorTypesResponse>
    {
        public GenericGetManyRequest QueryParameter { get; }

        public GetAllSensorTypesQuery(GenericGetManyRequest queryParameter)
        {
            QueryParameter = queryParameter;
        }
    }
}