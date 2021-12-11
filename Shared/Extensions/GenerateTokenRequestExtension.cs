using System;
using Shared.Proto;

namespace Shared.Extensions
{
    public static class GenerateTokenRequestExtension
    {
        public static Guid UserId(this GenerateTokenRequest self)
        {
            return Guid.TryParse(self.UserId, out var result) ? result : Guid.Empty;
        }
    }
}