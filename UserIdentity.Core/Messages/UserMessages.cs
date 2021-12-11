using System;
using Shared.Proto;

namespace UserIdentity.Core.Messages
{
    public class CreatedUserMessage
    {
        public CreatedUserMessage(UserDto updatedUser)
        {
            UpdatedUser = updatedUser;
        }

        public UserDto UpdatedUser { get; set; }
    }

    public class UpdatedUserDetailsMessage
    {
        public UpdatedUserDetailsMessage(UserDto updatedUser)
        {
            UpdatedUser = updatedUser;
        }

        public UserDto UpdatedUser { get; set; }
    }

    public class DeletedUserMessage
    {
        public DeletedUserMessage(Guid deletedUserId)
        {
            DeletedUserId = deletedUserId;
        }

        public Guid DeletedUserId { get; set; }
    }
}