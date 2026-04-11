using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.AppUserDtos.ConfirmUserDto;
using Entity_Layer.DTOs.AppUserDtos.ForgotPasswordDtos;
using Entity_Layer.DTOs.AppUserDtos.LoginDtos;
using Entity_Layer.DTOs.AppUserDtos.ProfileDtos;
using Entity_Layer.DTOs.AppUserDtos.RegisterDtos;
using Entity_Layer.DTOs.AppUserDtos.UserListDtos;
using Entity_Layer.DTOs.JwtDtos;
using Microsoft.AspNetCore.Identity;

namespace Business_Layer.Abstract
{
    public interface IAppUserService
    {
        Task<IdentityResult> RegisterAsync(UserRegisterDto userRegisterDto);
        Task<SimpleUserDto?> LoginAsync(UserLoginDto userLoginDto);
        Task<IdentityResult> EditProfileAsync(string userName, EditProfileDto editProfileDto);
        Task<IdentityResult> ChangePasswordAsync(string userName, ChangePasswordDto changePasswordDto);
        Task<EditProfileDto> GetProfileByUserNameAsync(string userName);
        Task<bool> ConfirmEmailAsync(ConfirmUserDto confirmUserDto);
        Task ResendActivationCodeAsync(string email);
        Task<string> GetEmailByUserNameAsync(string userName);
        Task<Guid> TGetUserIdByUserNameAsync(string userName);
        Task<List<UserListDto>> GetUserListAsync();
        Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto,string scheme, string host);
    }
}
