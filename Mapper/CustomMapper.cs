using UGB.MVC.DTO.UsersDTO;
using UGB.MVC.Entities;

namespace UGB.MVC.Mapper
{
    public static class CustomMapper<TOutput> where TOutput : class
    {
        public static TOutput Map<TInput>(TInput input)
        {
            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserDTO))
            {
                users user = (input as users)!;
                return (new UserDTO
                {
                    id = user.id,
                    email = user.email
                } as TOutput)!;
            }

            if(typeof(TInput) == typeof(CreateUserDTO) && typeof(TOutput) == typeof(users))
            {
                CreateUserDTO dto = (input as CreateUserDTO)!;
                return (new users
                {
                    email = dto.email
                } as TOutput)!;
            }

            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserProfileDTO))
            {
                users user = (input as users)!;
                return (new UserProfileDTO
                {
                    id = user.id,
                    email = user.email,
                    firstName = user.first_name,
                    lastName = user.last_name,
                    address = user.address,
                    birthDate = user.birth_date,
                    dui = user.dui,
                    photoUrl = user.photo_path
                } as TOutput)!;
            }

            throw new NotSupportedException();
        }

        public static IEnumerable<TOutput> Map<TInput>(IEnumerable<TInput> input)
        {
            if(typeof(TInput) == typeof(users) && typeof(TOutput) == typeof(UserDTO))
            {
                return input.Select(Map);
            }

            throw new NotSupportedException();
        }

        public static List<TOutput> Map<TInput>(List<TInput> input)
        {
            return Map((IEnumerable<TInput>)input).ToList();
        }
    }
}
