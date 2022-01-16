namespace ClientApiGateway.Api.Resources
{
    public record ChangePasswordResource(string OldPassword, string NewPassword);
}