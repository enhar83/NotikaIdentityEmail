using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity_Layer.DTOs.CommentDtos;
using Entity_Layer.Entities;

namespace Business_Layer.Abstract
{
    public interface ICommentService:IGenericService<Comment>
    {
        Task<List<CommentListDto>> GetCommentListAsync(Guid userId);
        Task ComposeCommentAsync(ComposeCommentDto composeCommentDto);
        Task<List<CommentListForAdminDto>> GetCommentListForAdmin();
        Task UpdateCommentStatusAsync (UpdateCommentStatusDto updateCommentStatusDto);
        Task<List<CommentListForForumDto>> GetCommentListForForumAsync();
    }
}
